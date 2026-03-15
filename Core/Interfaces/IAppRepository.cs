namespace AppsDBLib.Core.Interfaces
{
    public interface IAppRepository : IAsyncRepository<AppModel, int>
    {
        // Custom methods specific to the App aggregate go here in the future
    }
}