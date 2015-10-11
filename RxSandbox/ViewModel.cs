using System.Linq;
using Utilities.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Reactive.Subjects;

namespace RxSandbox
{
    public class ViewModel
    {
        public ReadonlyReactiveProperty<string> Article { get; private set; }

        public ReactiveProperty<string> Category { get; private set; } = new ReactiveProperty<string>();

        public ICommand ButtonCommand { get; } 

        private readonly Subject<int> _test = new Subject<int>();

        private readonly IArticleService _articleService = new ArticleService();

        private readonly ICategoryService _categoryService = new CategoryService();

        

        public ViewModel(IArticleService articleService,
           ICategoryService categoryService)
        {
            _articleService = articleService;
            _categoryService = categoryService;

            ButtonCommand = new SimpleCommand(() => _test.OnNext(1));

            //Article = _categoryService.GetCategoryStream()
            //                        .Select(it => _articleService.GetArticleOnce(it))
            //                        .Switch()
            //                        .ToReadonlyReactiveProperty();

            //Article = Category
            //            .Select(it => _articleService.GetArticleStream(it))
            //            .Switch()
            //            .ToReadonlyReactiveProperty();

            Article = _test.Select(it => _articleService.GetArticleStream(it.ToString()))
                        .Switch()
                        .ToReadonlyReactiveProperty();
        }


    }
}
