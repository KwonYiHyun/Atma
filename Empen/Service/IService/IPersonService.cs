using SharedData.Dto;
using SharedData.Request;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface IPersonService
    {
        Task<PersonInfoDto> getPersonInfo(int personId);
        Task<ErrorCode> setPersonInfo(int personId, PersonInfoRequest query);
        Task<ErrorCode> setPersonLeaderCharacter(int personId, int characterId);
        Task<ErrorCode> createPerson(int personId, int displayPersonId, string personHash, string email);
    }
}
