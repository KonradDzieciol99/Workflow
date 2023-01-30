using Socjal.API.Entity;

namespace Socjal.API.Repositories
{
    public interface IUserRepositorySingleton
    {
        Task<bool> AddUser(User user);
    }
}
