using Luncher.Api;
using Luncher.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Reactive;

namespace Launcher.Services.Universal
{
    public class HistoryRepository : IHistoryRepository
    {
        private const string CacheFileName = "HistoryRepository";

        private readonly ICacheService _cacheService;
        private readonly Subject<RestaurantType> _addedDataSubject = new Subject<RestaurantType>();
        private readonly IConnectableObservable<RestaurantType> _dataObservable;
        private readonly Collection<RestaurantType> _pickedRestaurant = new Collection<RestaurantType>();
        private readonly IDisposable _pickRestaurantSubscription;
        private readonly IDisposable _updatedSubscription;

        public IObservable<RestaurantType> PickedRestaurantObservable { get { return _dataObservable; } }

        public HistoryRepository(ICacheService cacheService)
        {
            _cacheService = cacheService;
            _dataObservable = DefineShareDataObservable(cacheService)
                .Concat(_addedDataSubject)
                .Replay();
            _pickRestaurantSubscription = _dataObservable.Connect();
            _updatedSubscription = PickedRestaurantObservable.Subscribe(AddPickedRestaurant);
        }

        public void Dispose()
        {
            _pickRestaurantSubscription.Dispose();
            _updatedSubscription.Dispose();
        }

        private void AddPickedRestaurant(RestaurantType data)
        {
            _pickedRestaurant.Add(data);
        }

        private static IObservable<RestaurantType> DefineShareDataObservable(ICacheService service)
        {
            return Observable.FromAsync(_ => GetShareDataAsync(service, CancellationToken.None))
                             .WhereIsNotNull()
                             .Select(shareDataList => shareDataList.ToObservable())
                             .Switch()
                             .Publish()
                             .RefCount();
        }

        private static async Task<ICollection<RestaurantType>> GetShareDataAsync(ICacheService service, CancellationToken token)
        {
            try
            {
                // Retrieve the items from the cache
                var data = await service.GetDataAsync<ICollection<RestaurantType>>(CacheFileName, token);
                return data;
            }
            catch (FileNotFoundException)
            {
            }

            return null;

        }

        public void Add(RestaurantType shareData)
        {
            _addedDataSubject.OnNext(shareData);
            _cacheService.SaveDataAsync(CacheFileName, _pickedRestaurant, CancellationToken.None);
        }
    }
}
