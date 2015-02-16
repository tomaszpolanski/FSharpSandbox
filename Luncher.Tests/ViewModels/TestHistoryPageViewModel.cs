using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FakeItEasy;
using Luncher.Api;
using Luncher.Services;
using Luncher.ViewModels;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Luncher.Tests.ViewModels
{
    [TestClass]
    public class TestHistoryPageViewModel
    {
        private readonly TestScheduler _testScheduler = new TestScheduler();
        private IHistoryRepository _historyRepository;
        private INavigator _navigator;
        private ISchedulerProvider _schedulerProvider;

        [TestInitialize]
        public void Initialize()
        {
            _historyRepository = A.Fake<IHistoryRepository>();
            _navigator = A.Fake<INavigator>();
            _schedulerProvider = A.Fake<ISchedulerProvider>();

            A.CallTo(() => _schedulerProvider.Default).Returns(_testScheduler);
        }

        private HistoryPageViewModel CreateViewModel()
        {
            return new HistoryPageViewModel(_historyRepository, _navigator, _schedulerProvider);
        }


        [TestMethod]
        public void DefaultHistoryList()
        {
            var vm = CreateViewModel();
            Assert.AreEqual(0, vm.HistoryList.Count);
        }

        [TestMethod]
        public void AddingHistoryList()
        {
            var subject = new Subject<PickedRestaurantType>();
            A.CallTo(() => _historyRepository.PickedRestaurantObservable).Returns(subject);
            var vm = CreateViewModel();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

            subject.OnNext(new PickedRestaurantType());

            _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(300).Ticks);

            Assert.AreEqual(1, vm.HistoryList.Count);
        }

        [TestMethod]
        public void AddingHistoryInReverseOrder()
        {
            var restaurants = Enumerable.Range(0, 5)
                                        .Select(number => new PickedRestaurantType { Restaurant = number.ToString(CultureInfo.InvariantCulture) })
                                        .ToList();
            A.CallTo(() => _historyRepository.PickedRestaurantObservable).Returns(restaurants.ToObservable());
            var vm = CreateViewModel();
            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);


            var reverseRestaurantNames = restaurants.Select(restaurant => restaurant.Restaurant)
                                                    .Reverse()
                                                    .ToList();

            Assert.AreEqual(reverseRestaurantNames.Count, vm.HistoryList.Count);

            var results = vm.HistoryList.Select(rest => rest.Restaurant)
                                        .Zip(reverseRestaurantNames,
                ( acctual, expected) => new { Ex = expected, Ac = acctual });

            foreach(var result in results)
            {
                Assert.AreEqual(result.Ex, result.Ac);
            }
        }

        [TestMethod]
        public void GoingBack()
        {
            var vm = CreateViewModel(); 

            vm.GoBackCommand.Execute(null);

            A.CallTo(() => _navigator.GoBack()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
