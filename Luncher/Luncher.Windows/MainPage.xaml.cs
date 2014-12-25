using Windows.UI.Xaml.Controls;
using System.Linq;
using System.IO;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Luncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var test = new Luncher.Api.Test.LuncherApi();
            var list = test.GetRestaurants("1,2,3,4,5,6");
            var res = test.ImHungry(list).Take(10).ToList();

            if (res != null)
            {

            }
        }
    }
}
