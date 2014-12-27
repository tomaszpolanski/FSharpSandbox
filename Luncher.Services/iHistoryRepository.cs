using Luncher.Api;
using System;

namespace Luncher.Services
{
    public interface IHistoryRepository
    {
        IObservable<RestaurantType> PickedRestaurantObservable { get; }

        void Add(RestaurantType restaurant);
    }
}
