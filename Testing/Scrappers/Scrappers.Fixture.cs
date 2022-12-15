using ASP.NETCoreWebApplication.Scrappers;
using Testing;

public sealed class ScrappersFixture
{
    public static ScrappersFixture Current = new ScrappersFixture();
    private static readonly AruodasLt _aruodasLt = new AruodasLt();

    private ScrappersFixture()
    {
        // Run at start
    }

    public static string GetExpectedAruodasUrl()
    {
        return "https://www.aruodas.lt/butai/?FRoomNumMin=1&FRoomNumMax=5&FPriceMin=10000&FPriceMax=300000"
               + "&FAreaOverAllMin=1&FAreaOverAllMax=160&FFloorNumMin=1&FFloorNumMax=12search_text=Senamiestis";
    }

    ~ScrappersFixture()
    {
        Dispose();
    }

    public void Dispose()
    {

        // Run at end
    }

    public static string CreateTestAruodasUrl()
    {
        return _aruodasLt.BuildUrlFromParams(
            HousingType.BuyFlat,
            1, 5,
            10000, 300000,
            1, 160,
            1, 12,
            FHouseState.None,
            "Senamiestis"
        );
    }
}