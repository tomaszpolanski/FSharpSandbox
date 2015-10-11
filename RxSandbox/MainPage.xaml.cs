using System.Reactive.Linq;
using System;


namespace RxSandbox
{
    public sealed partial class MainPage 
    {

        public MainPage()
        {
            InitializeComponent();

            var categoryS = new CategoryService();

            categoryS.GetCategoryOnce()
                .Subscribe(cat => SubscribeField.Text = cat);

            var vm = new ViewModel(new ArticleService(), categoryS);

            DataContext = vm;
        }
    }
}
