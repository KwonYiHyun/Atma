using LoginServer.Models;
using System.Data;

namespace LoginServer.Repositories
{
    public interface IPersonRepository
    {
        Task<person?> getPersonByPersonIdAsync(int personId);
        Task<person?> getPersonByDisplayPersonIdAsync(string provider, int displayPersonId);
        Task<person?> getPersonByHashAsync(string provider, string hash);
        Task<int> createPersonAsync(person person, IDbTransaction? transaction = null);
        Task<bool> deletePersonAsync(int personId);
        Task<bool> existsDisplayIdAsync(int personId);
    }
}
