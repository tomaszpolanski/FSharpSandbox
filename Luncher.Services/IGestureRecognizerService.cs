using System;
using System.Reactive;

namespace Luncher.Services
{
    public enum SwipeType
    {
        Left,
        Right
    }
    public interface IGestureRecognizerService
    {

        IObservable<SwipeType> SwipeObservable { get; }
    }
}
