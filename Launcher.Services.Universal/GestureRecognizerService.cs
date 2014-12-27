using System;
using Windows.UI.Xaml;
using System.Reactive.Linq;
using Windows.UI.Xaml.Input;
using Luncher.Services;
using System.Reactive.Subjects;

namespace Launcher.Services.Universal
{
    public class GestureRecognizerService : IGestureRecognizerService
    {
        private Subject<UIElement> _elementSubject = new Subject<UIElement>();

        public IObservable<SwipeType> SwipeObservable { get; private set; }

        public GestureRecognizerService()
        {
            SwipeObservable = DefineSwipe(_elementSubject);
        }

        public void SetUiElement(UIElement element)
        {
            _elementSubject.OnNext(element);
        }

        private static IObservable<SwipeType> DefineSwipe(IObservable<UIElement> elementOb)
        {
            return elementOb.DistinctUntilChanged().Select(element => element != null ?
                                               GetGestureObservable(element) :
                                               Observable.Never<SwipeType>())
                            .Switch()
                            .Publish()
                            .RefCount();
        }

        private static IObservable<SwipeType> GetGestureObservable( UIElement element)
        {
            var completedOb = Observable.FromEventPattern<ManipulationDeltaEventHandler, ManipulationDeltaRoutedEventArgs>
                (h => element.ManipulationDelta += h,
                 h => element.ManipulationDelta -= h)
                .Select(arg => arg.EventArgs);
            return Observable.FromEventPattern<ManipulationStartedEventHandler, ManipulationStartedRoutedEventArgs>
                (h => element.ManipulationStarted += h,
                 h => element.ManipulationStarted -= h)
                .Select(arg => arg.EventArgs.Position)
                .Select(start => completedOb.Select(end => new { Delta = end.Position.X - start.X, Arg = end })
                                            .Where(delta => Math.Abs(delta.Delta) >= 200)
                                            .Do(delta => delta.Arg.Complete())
                                            .Select(delta => delta.Delta > 0 ? SwipeType.Right : SwipeType.Left))
                .Switch();

        }


    }
}
