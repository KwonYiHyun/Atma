using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Dto;
using SharedData.Models;

namespace Empen.Service
{
    //public class DeckService : IDeckService
    //{
    //    private readonly MasterDbContext _masterContext;
    //    private readonly PersonDbContext _personContext;

    //    public DeckService(MasterDbContext masterContext, PersonDbContext personContext)
    //    {
    //        _masterContext = masterContext;
    //        _personContext = personContext;
    //    }

    //    public async Task<DeckInfoDto> getDeck(int personId, int slotId)
    //    {
    //        var deck = await _personContext.person_deck
    //            .AsNoTracking()
    //            .Where(d => d.person_id == personId && d.slot_id == slotId)
    //            .FirstOrDefaultAsync();

    //        if (deck == null)
    //        {
    //            return new DeckInfoDto { person_deck_id = 0 };
    //        }

    //        DeckInfoDto info = new DeckInfoDto();
    //        info.person_deck_id = deck.person_deck_id;

    //        List<int> slotIds = new List<int>() { deck.person_character_id_1, deck.person_character_id_2, deck.person_character_id_3, deck.person_character_id_4, deck.person_character_id_5 };

    //        var ids = slotIds.Where(id => id > 0).Distinct().ToList();

    //        var personCharacters = await _personContext.person_character
    //            .AsNoTracking()
    //            .Where(c => ids.Contains(c.person_character_id))
    //            .ToListAsync();

    //        var personCharactersMap = personCharacters
    //            .ToDictionary(c => c.person_character_id);

    //        var characterIds = personCharacters.Select(p => p.character_id).ToList();

    //        var masterCharacters = await _masterContext.master_character
    //            .AsNoTracking()
    //            .Where(c => characterIds.Contains(c.character_id))
    //            .ToListAsync();

    //        var masterCharactersMap = masterCharacters.ToDictionary(c => c.character_id);

    //        foreach (var id in slotIds)
    //        {
    //            if (id > 0 && personCharactersMap.TryGetValue(id, out var c))
    //            {
    //                if (masterCharactersMap.TryGetValue(c.character_id, out var masterChar))
    //                {
    //                    PersonCharacterDto dto = new PersonCharacterDto()
    //                    {
    //                        //person_character_id = c.person_character_id,
    //                        character_id = c.character_id,
    //                        character_level = c.character_level,
    //                        character_grade = c.grade,
    //                        //friendship_level = c.friendship_level,
    //                        //friendship_exp = c.friendship_exp,
    //                        //insert_date = c.insert_date,

    //                        character_name = masterChar.character_name,
    //                        //grade = masterChar.character_grade,
    //                        //character_level_max = masterChar.character_level_max,
    //                        //character_description = masterChar.character_description,
    //                        //character_comment_1 = masterChar.character_comment_1,
    //                        //character_comment_2 = masterChar.character_comment_2,
    //                        //character_comment_3 = masterChar.character_comment_3,
    //                        //character_comment_1_motion = masterChar.character_comment_1_motion,
    //                        //character_comment_2_motion = masterChar.character_comment_2_motion,
    //                        //character_comment_3_motion = masterChar.character_comment_3_motion,
    //                    };
    //                    info.characters.Add(dto);
    //                }
    //            }
    //        }

    //        return info;
    //    }

    //    public async Task setDeck(int personId, int slotId, int person_character_id_1, int person_character_id_2, int person_character_id_3, int person_character_id_4, int person_character_id_5)
    //    {
    //        List<int> inputIds = new List<int>() {
    //            person_character_id_1, person_character_id_2,
    //            person_character_id_3, person_character_id_4,
    //            person_character_id_5
    //        }.Where(id => id > 0).ToList();

    //        if (inputIds.Count > 0)
    //        {
    //            var uniqueInputIds = inputIds.Distinct().ToList();

    //            var myCharCount = await _personContext.person_character
    //                .AsNoTracking()
    //                .CountAsync(c => c.person_id == personId && uniqueInputIds.Contains(c.person_character_id));

    //            if (myCharCount != uniqueInputIds.Count)
    //            {
    //                throw new Exception("Invalid character ID found.");
    //            }
    //        }

    //        var deck = await _personContext.person_deck
    //            .FirstOrDefaultAsync(d => d.person_id == personId && d.slot_id == slotId);

    //        DateTime now = DateTime.Now;

    //        if (deck == null)
    //        {
    //            deck = new person_deck
    //            {
    //                person_id = personId,
    //                slot_id = slotId,
    //                person_character_id_1 = person_character_id_1,
    //                person_character_id_2 = person_character_id_2,
    //                person_character_id_3 = person_character_id_3,
    //                person_character_id_4 = person_character_id_4,
    //                person_character_id_5 = person_character_id_5,
    //                insert_date = now,
    //                update_date = now
    //            };
    //            _personContext.person_deck.Add(deck);
    //        }
    //        else
    //        {
    //            deck.person_character_id_1 = person_character_id_1;
    //            deck.person_character_id_2 = person_character_id_2;
    //            deck.person_character_id_3 = person_character_id_3;
    //            deck.person_character_id_4 = person_character_id_4;
    //            deck.person_character_id_5 = person_character_id_5;
    //            deck.update_date = now;
    //        }
    //        try
    //        {
    //            await _personContext.SaveChangesAsync();
    //        } catch(Exception ex)
    //        {
    //            // TODO: 로깅
    //        }
    //    }
    //}
}
