namespace Empen.Service.IService
{
    public interface IPersonDataCacheService
    {
        Task<List<PersonItem>> getPersonItemAsync(int personId);
        Task deletePersonItemAsync(int personId);
        Task<PersonStatus> getPersonStatusAsync(int personId);
        Task deletePersonStatusAsync(int personId);
    }
}
