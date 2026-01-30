namespace CommonServices.Repository;

public interface IRepository<T,ID>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(ID id);
}