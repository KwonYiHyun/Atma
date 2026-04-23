using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface ILoginbonusService
    {
        Task<ICollection<LoginbonusInfoDto>> getAllLoginbonusAsync();
        Task<ICollection<LoginbonusInfoDto>> getActiveLoginbonusAsync();
        Task<ICollection<LoginbonusInfoDto>> getAllLoginbonusByPersonIdAsync(int personId);
    }
}
