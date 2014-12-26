using System;
using System.Reactive;

namespace Luncher.Services
{
    public interface IGestureRecognizerService
    {
        IObservable<Unit> SwipeObservable { get; }
    }
}
