using System;

namespace RxSandbox
{
    public interface ICategoryService
    {
        IObservable<string> GetCategoryOnce();
        IObservable<string> GetCategoryStream();
    }
}
