namespace ASP.NETCoreWebApplication.Models.Repositories
{
    public class RepositoryBase
    {
        protected readonly PriceWatchContext _priceWatchContext;
        
        public RepositoryBase(PriceWatchContext priceWatchContext)
        {
            _priceWatchContext = priceWatchContext;
            _priceWatchContext.Database.EnsureCreated();
        }
    }
}