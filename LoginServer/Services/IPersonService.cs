using LoginServer.Models;

namespace LoginServer.Services
{
    public interface IPersonService
    {
        Task<person?> registerPersonAsync(string loginProvider, string personHash, string email);
        //Task<bool> checkExistsDisplayPersonIdAsync(int displayPersonId);
        //Task<bool> checkExistsPersonIdAsync(int personId);
        Task<person?> findByPersonIdAsync(int personId);
        Task<person?> findByDisplayPersonIdAsync(string provider, int personId);
        Task<person?> findByHashAsync(string provider, string hash);
    }
}
