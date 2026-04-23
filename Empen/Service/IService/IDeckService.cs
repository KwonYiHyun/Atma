using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface IDeckService
    {
        Task setDeck(int personId, int slotId, int person_character_id_1, int person_character_id_2, int person_character_id_3, int person_character_id_4, int person_character_id_5);
        Task<DeckInfoDto> getDeck(int personId, int slotId);
    }
}
