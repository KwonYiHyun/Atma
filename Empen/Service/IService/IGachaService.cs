using Microsoft.AspNetCore.Mvc;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IGachaService
    {
        Task<ICollection<GachaInfoDto>> getAllGachaAsync();
        Task<(ErrorCode, GachaInfoDto)> getGachaByIdAsync(int id);
        Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGacha(int personId, int gachaId, int execCount);
        Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGachaExecAsync(int personId, int gachaId);
        Task<(ErrorCode, ICollection<GachaPlayInfoDto>)> playGachaExec10Async(int personId, int gachaId);
        Task<bool> canGacha(int personId, int gachaId, int execCount);
    }
}
