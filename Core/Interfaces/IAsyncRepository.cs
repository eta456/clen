using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppsDBLib.Core.Interfaces
{
    public interface IAsyncRepository<TDomainModel, TId> where TDomainModel : class
    {
        Task<TDomainModel?> GetByIdAsync(TId id);
        Task<IReadOnlyList<TDomainModel>> GetAllAsync();
        Task<TDomainModel> AddAsync(TDomainModel domainModel);
        Task UpdateAsync(TDomainModel domainModel);
        Task DeleteAsync(TDomainModel domainModel);
    }
}