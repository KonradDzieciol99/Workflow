using Socjal.API.Models;

namespace Socjal.API.Repositories
{
    public interface IUserRepositorySingleton
    {
        Task<bool> AddUser(User user);
    }
}
