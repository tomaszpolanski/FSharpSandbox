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
        private readonly Subject<PickedRestaurantType> _addedDataSubject = new Subject<PickedRestaurantType>();
        private readonly IConnectableObservable<PickedRestaurantType> _dataObservable;
        private readonly Collection<PickedRestaurantType> _pickedRestaurant = new Collection<PickedRestaurantType>();
        private readonly IDisposable _pickRestaurantSubscription;
        private readonly IDisposable _updatedSubscription;

        public IObservable<PickedRestaurantType> PickedRestaurantObservable { get { return _dataObservable; } }

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

        private void AddPickedRestaurant(PickedRestaurantType data)
        {
            _pickedRestaurant.Add(data);
        }

        private static IObservable<PickedRestaurantType> DefineShareDataObservable(ICacheService service)
        {
            return Observable.FromAsync(_ => GetShareDataAsync(service, CancellationToken.None))
                             .WhereIsNotNull()
                             .Select(shareDataList => shareDataList.ToObservable())
                             .Switch()
                             .Publish()
                             .RefCount();
        }

        private static async Task<IEnumerable<PickedRestaurantType>> GetShareDataAsync(ICacheService service, CancellationToken token)
        {
            try
            {
                // Retrieve the items from the cache
                var data = await service.GetDataAsync<ICollection<PickedRestaurantType>>(CacheFileName, token);
                return data;
            }
            catch (FileNotFoundException)
            {
            }

            return null;

        }

        public void Add(PickedRestaurantType pickedRestaurant)
        {
            _addedDataSubject.OnNext(pickedRestaurant);
            _cacheService.SaveDataAsync(CacheFileName, _pickedRestaurant, CancellationToken.None);
        }
    }
}
