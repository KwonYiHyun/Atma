using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IResourceService
    {
        Task<bool> consumeResourceAsync(int personId, ResourceType type, int amount);
        Task addResourceAsync(int personId, ResourceType type, int amount);
    }
}
