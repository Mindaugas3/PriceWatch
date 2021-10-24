namespace ASP.NETCoreWebApplication.Models
{
    public class HousingObject
    {
        private readonly int price;
        private readonly int area;
        private readonly string destinationUrl;
        private readonly string imgUrl;

        public HousingObject(int price, int area, string destinationUrl, string imgUrl)
        {
            this.price = price;
            this.area = area;
            this.destinationUrl = destinationUrl;
            this.imgUrl = imgUrl;
        }
    }
}