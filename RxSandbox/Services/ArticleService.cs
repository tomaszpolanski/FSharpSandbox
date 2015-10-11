using System;
using System.Linq;
using System.Reactive.Linq;

namespace RxSandbox
{
    public class ArticleService : IArticleService
    {
        public IObservable<string> GetArticleOnce(string category)
            => Observable.Return($"{category} Something");

        public IObservable<string> GetArticleStream(string category)
            => Observable.Interval(TimeSpan.FromSeconds(1))
                         .Select(it => $"{category} {it.ToString()}");
    }
}
