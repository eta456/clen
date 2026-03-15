using AppsDBLib.Core.Interfaces;
using AppsDBLib.Core.Models;
using AppsDBLib.Infrastructure.Contexts;
using AppsDBLib.Infrastructure.Entities;

namespace AppsDBLib.Infrastructure.Repositories
{
    internal class AppRepository : RepositoryBase<AppEntity, AppModel, int>, IAppRepository
    {
        public AppRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        // Implementation of any custom methods defined in IAppRepository goes here
    }
}