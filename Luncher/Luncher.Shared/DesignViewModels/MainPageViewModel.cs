namespace Luncher.DesignViewModels
{
    public class MainPageViewModel
    {
        public object RestaurantText
        {
            get { return new { Value = "How about The Bird?" }; }
        }

        public object PickedRestaurantText
        {
            get { return new { Value = "Lety's go to The Bird" }; }
        }
    }
}
