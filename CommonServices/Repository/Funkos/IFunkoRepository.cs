using CommonServices.model;

namespace CommonServices.Repository.Funkos;

public interface IFunkoRepository : IRepository<Funko,long>
{
    Task<Funko> AddAsync(Funko newFunko);
    Task<Funko?> UpdateAsync(long id, Funko newFunko);
    Task<Funko?> DeleteAsync(long id);
}