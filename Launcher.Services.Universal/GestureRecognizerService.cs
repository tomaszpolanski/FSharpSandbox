using System;
using Windows.UI.Xaml;
using System.Reactive.Linq;
using Windows.UI.Xaml.Input;
using Utilities.Reactive;
using System.Reactive;
using Luncher.Services;
using System.Reactive.Subjects;

namespace Launcher.Services.Universal
{
    public class GestureRecognizerService : IGestureRecognizerService
    {
        private Subject<UIElement> _elementSubject = new Subject<UIElement>();

        public IObservable<Unit> SwipeObservable { get; private set; }

        public GestureRecognizerService()
        {
            SwipeObservable = DefineSwipe(_elementSubject);
        }

        public void SetUiElement(UIElement element)
        {
            _elementSubject.OnNext(element);
        }

        private static IObservable<Unit> DefineSwipe(IObservable<UIElement> elementOb)
        {
            return elementOb.Select(element => element != null ?
                                               GetGestureObservable(element) :
                                               Observable.Never<Unit>())
                            .Switch();
        }

        private static IObservable<Unit> GetGestureObservable( UIElement element)
        {
            var completedOb = Observable.FromEventPattern<ManipulationDeltaEventHandler, ManipulationDeltaRoutedEventArgs>
                (h => element.ManipulationDelta += h,
                 h => element.ManipulationDelta -= h)
                .Select(arg => arg.EventArgs);
            return Observable.FromEventPattern<ManipulationStartedEventHandler, ManipulationStartedRoutedEventArgs>
                (h => element.ManipulationStarted += h,
                 h => element.ManipulationStarted -= h)
                .Select(arg => arg.EventArgs.Position)
                .Do(_ => System.Diagnostics.Debug.WriteLine("Start " + DateTime.Now))
                .Select(start => completedOb.Select(end => new { Delta = end.Position.X - start.X, Arg = end })
                                            .Where(delta => delta.Delta >= 800)
                                            .Do(delta => delta.Arg.Complete())
                                            .SelectUnit())
                .Switch();

        }


    }
}
