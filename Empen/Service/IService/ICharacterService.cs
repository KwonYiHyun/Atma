using SharedData.Dto;
using SharedData.Type;

namespace Empen.Service.IService
{
    public interface ICharacterService
    {
        Task<ICollection<PersonCharacterDto>> getMyCharacterListAsync(int personId);
        Task<(ErrorCode, CharacterDetailInfoDto)> getCharacterDetailInfoAsync(int personId, int characterId);
        Task<ICollection<int>> getAllCharacterId();
        Task<(bool, int, int)> giveCharacterAsync(int personId, int characterId);
        Task<(ErrorCode, CharacterDetailInfoDto)> characterLevelUpAsync(int personId, int characterId);
        Task<(ErrorCode, CharacterDetailInfoDto)> characterLimitBreakAsync(int personId, int characterId);
        Task<(ErrorCode, CharacterLevelupInfoDto)> getCharacterLevelupInfoAsync(int personId, int characterId);
    }
}
