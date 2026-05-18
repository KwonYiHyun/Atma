

using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Type;

namespace Empen.Service
{
    public class ResourceService : IResourceService
    {
        private readonly PersonDbContext _personContext;

        public ResourceService(PersonDbContext personContext)
        {
            _personContext = personContext;
        }

        public async Task<bool> consumeResourceAsync(int personId, ResourceType type, int amount)
        {
            var status = await _personContext.person_status
                .FirstOrDefaultAsync(p => p.person_id == personId);

            if (status == null)
            {
                return false;
            }

            switch (type)
            {
                case ResourceType.Token:
                    if (status.token < amount)
                    {
                        return false;
                    }
                    status.token -= amount;
                    break;

                case ResourceType.Prism:
                    if (status.prism < amount)
                    {
                        return false;
                    }
                    status.prism -= amount;
                    break;

                case ResourceType.GachaResource:
                    if (status.film < amount)
                    {
                        return false;
                    }
                    status.film -= amount;
                    break;
                default:
                    return false;
            }

            status.update_date = DateTime.Now;

            return true;
        }

        public async Task addResourceAsync(int personId, ResourceType type, int amount)
        {
            var status = await _personContext.person_status
                .FirstOrDefaultAsync(p => p.person_id == personId);

            if (status == null)
            {
                return;
            }

            switch (type)
            {
                case ResourceType.Token:
                    status.token += amount;
                    break;
                case ResourceType.Prism:
                    status.prism += amount;
                    break;
                case ResourceType.GachaResource:
                    status.film += amount;
                    break;
            }

            status.update_date = DateTime.Now;
        }
    }
}
