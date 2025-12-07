using LernApp.Models;

namespace LernApp.Data.Repository;

public interface ILernplanRepository
{
    Task<List<LernEinheit>> GetAllAsync();
    Task AddAsync(LernEinheit einheit);
    Task UpdateAsync(LernEinheit einheit);
    Task DeleteAsync(int id);
}
