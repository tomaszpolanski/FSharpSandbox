using System;
using System.Reactive.Linq;

namespace RxSandbox
{
    public class CategoryService : ICategoryService
    {
        public IObservable<string> GetCategoryOnce()
        {
            return Observable.Return("news");
        }

        public IObservable<string> GetCategoryStream()
        {
            return Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(it => it % 2 == 0
                                 ? "digital life"
                                 : "sports");   
        }
    }
}
