namespace ASP.NETCoreWebApplication.Models
{
    public class CurrencyRate
    {
        public int id { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public float rate { get; set; }

        public CurrencyRate()
        {
            
        }
    }
}