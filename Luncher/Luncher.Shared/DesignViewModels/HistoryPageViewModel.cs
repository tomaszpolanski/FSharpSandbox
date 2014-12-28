using System;
using System.Collections.Generic;

namespace Luncher.DesignViewModels
{
    public class HistoryPageViewModel
    {
        public IEnumerable<object> HistoryList
        {
            get
            {
                yield return new { Restaurant = "The Bird", Date = new DateTime(2014, 11, 24) };
                yield return new { Restaurant = "HAXXXE", Date = DateTime.Now };
                yield return new { Restaurant = "The Bird", Date = DateTime.Now };
                yield return new { Restaurant = "The Bird", Date = DateTime.Now };
                yield return new { Restaurant = "The Bird", Date = DateTime.Now };
            }
        }


    }
}
