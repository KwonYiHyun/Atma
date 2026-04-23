using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using System;
using System.Data;

namespace Empen.Service
{
    public class CharacterService : ICharacterService
    {
        private readonly PersonDbContext _personContext;
        private readonly IItemService _itemService;
        private readonly IRewardService _rewardService;
        private readonly ITimeService _timeService;
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly ILogger<ICharacterService> _logger;
        private readonly IPersonDataCacheService _personDataCacheService;

        public CharacterService(PersonDbContext personContext, IItemService itemService, IRewardService rewardService, ITimeService timeService, IMasterDataCacheService masterCacheService, ILogger<ICharacterService> logger, IPersonDataCacheService personDataCacheService)
        {
            _personContext = personContext;
            _itemService = itemService;
            _rewardService = rewardService;
            _timeService = timeService;
            _masterCacheService = masterCacheService;
            _logger = logger;
            _personDataCacheService = personDataCacheService;
        }

        public async Task<ICollection<int>> getAllCharacterId()
        {
            var charMap = await _masterCacheService.getCharacterMapAsync();

            return charMap.Values.OrderBy(c => c.show_order).Select(c => c.character_id).ToList();
        }

        public async Task<(ErrorCode, CharacterDetailInfoDto)> getCharacterDetailInfoAsync(int personId, int characterId)
        {
            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(characterId, out var character))
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            var currentCharacter = await _personContext.person_character
                .AsNoTracking()
                .Where(c => c.character_id == characterId && c.person_id == personId)
                .FirstOrDefaultAsync();

            if (currentCharacter == null)
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            var itemMap = await _masterCacheService.getItemMapAsync();
            itemMap.TryGetValue(character.piece_item_id, out var pieceItem);

            var personItem = await _personContext.person_item
                .AsNoTracking()
                .Where(p => p.person_id == personId && p.item_id == pieceItem.item_id)
                .FirstOrDefaultAsync();

            int amount = 0;
            if (personItem != null)
            {
                amount = personItem.amount;
            }

            var piece = _rewardService.createObjectDisplayByItem(pieceItem, amount);

            var levelMap = await _masterCacheService.getCharacterLevelMapAsync();
            if (!levelMap.TryGetValue((characterId, currentCharacter.character_level), out var characterStat))
            {
                return (ErrorCode.LevelNotFound, null);
            }

            var gradeMap = await _masterCacheService.getCharacterGradeMapAsync();
            if (!gradeMap.TryGetValue((characterId, currentCharacter.grade), out var characterGrade))
            {
                return (ErrorCode.GradeNotFound, null);
            }

            int next_critical_rate = 0;
            int next_critical_damage = 0;
            int piece_require_amount = 0;
            int price_token = 0;
            bool is_max_level = currentCharacter.character_level >= character.character_level_max;
            bool is_max_grade = currentCharacter.grade >= character.character_grade_max;

            if (!is_max_grade)
            {
                // 다음 등급 조회도 Map에서 바로 찾음
                if (gradeMap.TryGetValue((characterId, currentCharacter.grade + 1), out var nextGrade))
                {
                    next_critical_rate = nextGrade.critical_rate;
                    next_critical_damage = nextGrade.critical_damage;
                    piece_require_amount = nextGrade.require_count;
                    price_token = nextGrade.price_token;
                }
            }

            CharacterDetailInfoDto dto = new CharacterDetailInfoDto()
            {
                character_id = character.character_id,
                character_name = character.character_name,
                character_level_max = character.character_level_max,
                character_grade_max = character.character_grade_max,
                character_grade = character.character_grade,
                character_critical_rate = character.character_critical_rate,
                character_critical_damage = character.character_critical_damage,
                piece_item = piece,
                piece_require_amount = piece_require_amount,
                current_piece_amount = amount,
                price_token = price_token,
                character_description = character.character_description,
                character_comment_1 = character.character_comment_1,
                character_comment_2 = character.character_comment_2,
                character_comment_3 = character.character_comment_3,
                character_comment_1_motion = character.character_comment_1_motion,
                character_comment_2_motion = character.character_comment_2_motion,
                character_comment_3_motion = character.character_comment_3_motion,

                current_level = characterStat.level,
                current_grade = characterGrade.grade,
                current_atk = characterStat.atk,
                current_def = characterStat.def,
                current_hp = characterStat.hp,
                current_stance = characterStat.stance,
                current_critical_rate = characterGrade.critical_rate,
                current_critical_damage = characterGrade.critical_damage,

                next_critical_rate = next_critical_rate,
                next_critical_damage = next_critical_damage,

                is_max_level = is_max_level,
                is_max_grade = is_max_grade
            };

            return (ErrorCode.Success, dto);
        }

        public async Task<(ErrorCode, CharacterLevelupInfoDto)> getCharacterLevelupInfoAsync(int personId, int characterId)
        {
            int level = await _personContext.person_character
                .AsNoTracking()
                .Where(p => p.person_id == personId && p.character_id == characterId)
                .Select(p => p.character_level)
                .FirstOrDefaultAsync();

            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(characterId, out var characterMaster))
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            if (level >= characterMaster.character_level_max)
            {
                return (ErrorCode.AlreadyMaxLevel, null);
            }

            int targetLevel = level + 1;

            var levelMap = await _masterCacheService.getCharacterLevelMapAsync();
            if (!levelMap.TryGetValue((characterId, targetLevel), out var resource))
            {
                return (ErrorCode.LevelNotFound, null);
            }

            List<int> requreItems = new List<int>();
            if (resource.resource_item_id_1 > 0)
            {
                requreItems.Add(resource.resource_item_id_1);
            }
            if (resource.resource_item_id_2 > 0)
            {
                requreItems.Add(resource.resource_item_id_2);
            }
            if (resource.resource_item_id_3 > 0)
            {
                requreItems.Add(resource.resource_item_id_3);
            }

            var userItems = await _personContext.person_item
                .AsNoTracking()
                .Where(i => requreItems.Contains(i.item_id) && i.person_id == personId)
                .ToDictionaryAsync(i => i.item_id, i => i.amount);

            var itemMap = await _masterCacheService.getItemMapAsync();

            master_item GetItem(int id) => itemMap.TryGetValue(id, out var item) ? item : null;

            if (!levelMap.TryGetValue((characterId, level), out var current))
            {
                return (ErrorCode.LevelNotFound, null);
            }

            CharacterLevelupInfoDto dto = new CharacterLevelupInfoDto
            {
                resource_item_1 = _rewardService.createObjectDisplayByItem(GetItem(resource.resource_item_id_1), resource.item_1_amount),
                resource_item_2 = _rewardService.createObjectDisplayByItem(GetItem(resource.resource_item_id_2), resource.item_2_amount),
                resource_item_3 = _rewardService.createObjectDisplayByItem(GetItem(resource.resource_item_id_3), resource.item_3_amount),

                item_1_amount = userItems.GetValueOrDefault(resource.resource_item_id_1, 0),
                item_2_amount = userItems.GetValueOrDefault(resource.resource_item_id_2, 0),
                item_3_amount = userItems.GetValueOrDefault(resource.resource_item_id_3, 0),

                item_1_require = resource.item_1_amount,
                item_2_require = resource.item_2_amount,
                item_3_require = resource.item_3_amount,

                current_atk = current.atk,
                current_def = current.def,
                current_hp = current.hp,
                current_stance = current.stance,

                next_atk = resource.atk,
                next_def = resource.def,
                next_hp = resource.hp,
                next_stance = resource.stance
            };

            return (ErrorCode.Success, dto);
        }

        public async Task<ICollection<PersonCharacterDto>> getMyCharacterListAsync(int personId)
        {
            var myCharacters = await _personContext.person_character
                .AsNoTracking()
                .Where(c => c.person_id == personId)
                .OrderByDescending(c => c.insert_date)
                .ToListAsync();

            if (myCharacters.Count == 0)
            {
                return new List<PersonCharacterDto>();
            }

            var charMap = await _masterCacheService.getCharacterMapAsync();
            var levelMap = await _masterCacheService.getCharacterLevelMapAsync();

            List<PersonCharacterDto> resultList = new List<PersonCharacterDto>();

            foreach (var myChar in myCharacters)
            {
                if (!charMap.TryGetValue(myChar.character_id, out var masterInfo))
                {
                    continue;
                }

                if (!levelMap.TryGetValue((myChar.character_id, myChar.character_level), out var levelStat))
                {
                    // 해당 레벨의 데이터가 누락된 경우
                    levelStat = new master_character_level
                    {
                        atk = 0,
                        def = 0,
                        hp = 0,
                        stance = 0
                    };
                }

                resultList.Add(new PersonCharacterDto
                {
                    character_id = myChar.character_id,
                    character_level = myChar.character_level,
                    character_grade = myChar.grade,
                    character_name = masterInfo.character_name,
                    atk = levelStat.atk,
                    def = levelStat.def,
                    hp = levelStat.hp,
                    stance = levelStat.stance
                });
            }

            return resultList;
        }

        public async Task<(bool, int, int)> giveCharacterAsync(int personId, int characterId)
        {
            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(characterId, out var masterChar))
            {
                return (false, 0, 0);
            }

            DateTime now = await _timeService.getNowAsync();

            bool hasCharacter = _personContext.person_character.Local
                .Any(p => p.person_id == personId && p.character_id == characterId);

            if (!hasCharacter)
            {
                hasCharacter = await _personContext.person_character
                    .AnyAsync(p => p.person_id == personId && p.character_id == characterId);
            }

            if (hasCharacter)
            {
                if (masterChar.piece_item_id > 0)
                {
                    int amount = masterChar.piece_amount_duplicate;
                    if (amount <= 0)
                    {
                        amount = 10;
                    }
                    return (false, masterChar.piece_item_id, amount);
                }
                return (false, 0, 0);
            }
            else
            {
                var newChar = new person_character
                {
                    person_id = personId,
                    character_id = characterId,
                    character_level = 1,
                    grade = masterChar.character_grade,
                    friendship_level = 1,
                    friendship_exp = 0,
                    insert_date = now,
                    update_date = now
                };

                _personContext.person_character.Add(newChar);

                return (true, 0, 0);
            }
        }

        public async Task<(ErrorCode, CharacterDetailInfoDto)> characterLevelUpAsync(int personId, int characterId)
        {
            var myCharacter = await _personContext.person_character
                .Where(p => p.person_id == personId && p.character_id == characterId)
                .FirstOrDefaultAsync();

            if (myCharacter == null)
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(characterId, out var master))
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            if (myCharacter.character_level >= master.character_level_max)
            {
                return (ErrorCode.AlreadyMaxLevel, null);
            }

            var levelMap = await _masterCacheService.getCharacterLevelMapAsync();
            int targetLevel = myCharacter.character_level + 1;

            if (!levelMap.TryGetValue((characterId, targetLevel), out var nextLevel))
            {
                return (ErrorCode.LevelNotFound, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    List<(int, int)> requireItems = new List<(int, int)>();
                    if (nextLevel.resource_item_id_1 > 0)
                    {
                        requireItems.Add((nextLevel.resource_item_id_1, nextLevel.item_1_amount));
                    }
                    if (nextLevel.resource_item_id_2 > 0)
                    {
                        requireItems.Add((nextLevel.resource_item_id_2, nextLevel.item_2_amount));
                    }
                    if (nextLevel.resource_item_id_3 > 0)
                    {
                        requireItems.Add((nextLevel.resource_item_id_3, nextLevel.item_3_amount));
                    }

                    var Ids = requireItems.Select(r => r.Item1).ToList();

                    var userItems = await _personContext.person_item
                        .AsNoTracking()
                        .Where(i => i.person_id == personId && Ids.Contains(i.item_id))
                        .ToDictionaryAsync(c => c.item_id, c => c.amount);

                    foreach (var req in requireItems)
                    {
                        if (userItems.GetValueOrDefault(req.Item1, 0) < req.Item2)
                        {
                            return (ErrorCode.NotEnoughMaterial, null);
                        }
                    }

                    foreach (var req in requireItems)
                    {
                        ErrorCode error = await _itemService.useItemAsync(personId, req.Item1, req.Item2);
                        if (error != ErrorCode.Success)
                        {
                            return (ErrorCode.FailedUseItem, null);
                        }
                    }
                    myCharacter.character_level = targetLevel;

                    DateTime now = await _timeService.getNowAsync();

                    person_levelup levelup = new person_levelup
                    {
                        person_id = personId,
                        character_id = characterId,
                        level = targetLevel,
                        insert_date = now,
                        update_date = now
                    };

                    _personContext.person_levelup.Add(levelup);

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Exception from characterLevelUpAsync!");
                    return (ErrorCode.TransactionFailed, null);
                }
            }

            var (errorCode, info) = await getCharacterDetailInfoAsync(personId, characterId);
            if (errorCode != ErrorCode.Success)
            {
                return (errorCode, info);
            }

            return (ErrorCode.Success, info);
        }

        public async Task<(ErrorCode, CharacterDetailInfoDto)> characterLimitBreakAsync(int personId, int characterId)
        {
            var myCharacter = await _personContext.person_character
                .Where(p => p.person_id == personId && p.character_id == characterId)
                .FirstOrDefaultAsync();

            if (myCharacter == null)
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(characterId, out var characterMaster))
            {
                return (ErrorCode.CharacterNotFound, null);
            }

            if (myCharacter.grade >= characterMaster.character_grade_max)
            {
                return (ErrorCode.AlreadyMaxGrade, null);
            }

            int targetGrade = myCharacter.grade + 1;

            var gradeMap = await _masterCacheService.getCharacterGradeMapAsync();
            if (!gradeMap.TryGetValue((characterId, targetGrade), out var nextGrade))
            {
                return (ErrorCode.GradeNotFound, null);
            }

            int price_token = nextGrade.price_token;
            int current_token = await _personContext.person_status
                .AsNoTracking()
                .Where(p => p.person_id == personId)
                .Select(p => p.token)
                .FirstOrDefaultAsync();

            if (current_token < price_token)
            {
                return (ErrorCode.NotEnoughToken, null);
            }

            using (var transaction = await _personContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (characterMaster.piece_item_id <= 0)
                    {
                        return (ErrorCode.ItemNotFound, null);
                    }

                    var userPieceItem = await _personContext.person_item
                        .FirstOrDefaultAsync(i => i.person_id == personId && i.item_id == characterMaster.piece_item_id);

                    int amount = userPieceItem?.amount ?? 0;

                    if (amount < nextGrade.require_count)
                    {
                        return (ErrorCode.NotEnoughPiece, null);
                    }

                    var now = await _timeService.getNowAsync();

                    person_limitbreak limitbreak = new person_limitbreak
                    {
                        person_id = personId,
                        character_id = characterId,
                        grade = nextGrade.grade,
                        insert_date = now,
                        update_date = now
                    };

                    _personContext.person_limitbreak.Add(limitbreak);

                    await _itemService.useItemAsync(personId, characterMaster.piece_item_id, nextGrade.require_count);
                    myCharacter.grade = targetGrade;

                    await _personContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _personDataCacheService.deletePersonItemAsync(personId);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogInformation(ex, "Exception from CharacterService!");
                    return (ErrorCode.TransactionFailed, null);
                }
            }

            var (errorCode, info) = await getCharacterDetailInfoAsync(personId, characterId);
            if (errorCode != ErrorCode.Success)
            {
                return (errorCode, null);
            }

            return (ErrorCode.Success, info);
        }
    }
}
