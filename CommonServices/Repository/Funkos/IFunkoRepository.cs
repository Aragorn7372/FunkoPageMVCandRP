using CommonServices.Dto;
using CommonServices.model;

namespace CommonServices.Repository.Funkos;

public interface IFunkoRepository : IRepository<Funko,long>
{
    Task<(IEnumerable<Funko> Items, int TotalCount)> GetAllAsync(FilterDto filter);    
    Task<Funko> AddAsync(Funko newFunko);
    Task<Funko?> UpdateAsync(long id, Funko newFunko);
    Task<Funko?> DeleteAsync(long id);
}