using System.Linq.Expressions;

namespace CleanApi.Core.Interfaces;

// We restrict this to classes so it can only be used with your Models
public interface ILookupRepository<TModel> where TModel : class
{
    Task<TModel> FindAsync(Expression<Func<TModel, bool>> predicate);
}