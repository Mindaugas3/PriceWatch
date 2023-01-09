namespace ASP.NETCoreWebApplication.Scrappers
{
    // NOTE i am not using an enum. This way its easier to play with string constants.
    public struct HousingType
    {
        public const string RentFlat = "RENT_FLAT";
        public const string BuyFlat = "BUY_FLAT";
        public const string RentHouse = "RENT_HOUSE";
        public const string BuyHouse = "BUY_HOUSE";

        public static string[] Values()
        {
            var values = new[] { RentFlat, BuyFlat, RentHouse, BuyHouse };
            return values;
        }
    }

    public struct DataSources
    {
        public const string AruodasLt = "Aruodas.lt";
        public const string AlioLt = "Alio.lt";

        public static string[] Values()
        {
            var values = new[] {AlioLt, AruodasLt};
            return values;
        }
    }
    
    public struct SortingType {
        public const string PriceAsc = "PriceAsc";
        public const string PriceDesc = "PriceDesc";
        public const string LocationAsc = "LocationAsc";
        public const string LocationDesc = "LocationDesc";
        
        public static string[] Values()
        {
            var values = new[] {PriceAsc, PriceDesc, LocationAsc, LocationDesc};
            return values;
        }
    }
}
