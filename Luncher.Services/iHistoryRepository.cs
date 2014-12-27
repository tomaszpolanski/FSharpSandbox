using Luncher.Api;
using System;

namespace Luncher.Services
{
    public interface IHistoryRepository
    {
        IObservable<PickedRestaurantType> PickedRestaurantObservable { get; }

        void Add(PickedRestaurantType restaurant);
    }
}
