using System;

namespace RxSandbox
{
    public interface IArticleService
    {
        IObservable<string> GetArticleOnce(string category);
        IObservable<string> GetArticleStream(string category);
    }
}
