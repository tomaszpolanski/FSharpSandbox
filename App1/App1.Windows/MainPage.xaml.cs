using System.Threading;
using Windows.UI.Xaml.Controls;
using Portable;
using System;
using System.Linq;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var a = new Test1.AsyncTest();
            var eeee = Test();
            var list = eeee.Take(5).ToList();
            Test();
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(2200));
            try
            {
                Texxt.Text = await a.StartMyAsyncAsTask(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Texxt.Text = "Canceled";
            }
        }

        private IEnumerable<int> Test()
        {
            int start = 1;
            while (true)
            {
                yield return start;
                start *= 2;
            }

        }

        private int Bad()
        {
            int i;    
            int length;
            var array = new int[] { 1, 2, 3 };
            int sum = 0;  

            length = array.Length;  
            for (i = 0; i < length; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        private int Good()
        {
            var array = new int[] { 1, 2, 3 };
            return array.Sum(x => x);
        }


    }
}
