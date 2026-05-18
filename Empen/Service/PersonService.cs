using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using ServerCore.Service;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Request;
using SharedData.Type;

namespace Empen.Service
{
    public class PersonService : IPersonService
    {
        private readonly PersonDbContext _personContext;
        private readonly ILogger<IPersonService> _logger;
        private readonly ITimeService _timeService;
        private readonly IMasterDataCacheService _masterCacheService;
        private readonly IPersonDataCacheService _personCacheService;

        public PersonService(PersonDbContext personContext, ILogger<IPersonService> logger, ITimeService timeService, IMasterDataCacheService masterCacheService, IPersonDataCacheService personCacheService)
        {
            _personContext = personContext;
            _logger = logger;
            _timeService = timeService;
            _masterCacheService = masterCacheService;
            _personCacheService = personCacheService;
        }

        public async Task<ErrorCode> createPerson(int personId, int displayPersonId, string personHash, string email)
        {
            var now = await _timeService.getNowAsync();

            try
            {
                var newStatus = new person_status
                {
                    person_id = personId,
                    display_person_id = displayPersonId,
                    person_hash = personHash,
                    email = email,
                    person_name = Constant.DEFAULT_USERNAME,
                    level = 1,
                    exp = 0,
                    token = 10000,
                    gift = 10001,
                    manual = 10002,
                    film = 10003,
                    prism = 10004,
                    character_amount_max = 50,
                    character_storage_amount_max = 50,
                    leader_person_character_id = 0,
                    introduce = Constant.DEFAULT_INTRODUCE,
                    insert_date = now,
                    update_date = now
                };

                _personContext.person_status.Add(newStatus);
                await _personContext.SaveChangesAsync();

                return ErrorCode.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CreatePerson Error");
                return ErrorCode.ServerError;
            }
        }

        public async Task<PersonInfoDto> getPersonInfo(int personId)
        {
            var info = await _personCacheService.getPersonStatusAsync(personId);

            if (info == null)
            {
                return null;
            }

            int count = await _personContext.person_mail
                .AsNoTracking()
                .Where(p => p.person_id == personId && p.is_receive == false)
                .CountAsync();

            PersonInfoDto dto = new PersonInfoDto()
            {
                display_person_id = info.display_person_id,
                email = info.email,
                insert_date = info.insert_date,
                person_name = info.person_name,
                level = info.level,
                exp = info.exp,
                token = info.token,
                gift = info.gift,
                manual = info.manual,
                film = info.film,
                prism = info.prism,
                leader_person_character_id = info.leader_person_character_id,
                introduce = info.introduce,
                is_remain_mail = count > 0
            };

            var charMap = await _masterCacheService.getCharacterMapAsync();
            if (!charMap.TryGetValue(info.leader_person_character_id, out var character))
            {
                dto.character_comment = "";
            }
            else
            {
                dto.character_comment = character.character_comment_1;
            }
            
            return dto;
        }

        public async Task<ErrorCode> setPersonInfo(int personId, PersonInfoRequest query)
        {
            try
            {
                var status = await _personContext.person_status
                    .FirstOrDefaultAsync(p => p.person_id == personId);

                if (status == null)
                {
                    return ErrorCode.PersonNotFound;
                }

                if (!string.IsNullOrEmpty(query.person_name) && status.person_name != query.person_name)
                {
                    bool isDuplicate = await _personContext.person_status
                        .AsNoTracking()
                        .AnyAsync(p => p.person_name == query.person_name);

                    if (isDuplicate)
                    {
                        return ErrorCode.DuplicateName;
                    }
                    status.person_name = query.person_name;
                }

                if (query.introduce != null)
                {
                    status.introduce = query.introduce;
                }

                status.update_date = DateTime.Now;

                await _personContext.SaveChangesAsync();
                await _personCacheService.deletePersonStatusAsync(personId);

                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetPersonInfo Error");
                return ErrorCode.ServerError;
            }
        }

        public async Task<ErrorCode> setPersonLeaderCharacter(int personId, int characterId)
        {
            DateTime now = await _timeService.getNowAsync();

            try
            {
                int affectedRows = await _personContext.person_status
                .Where(p => p.person_id == personId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.leader_person_character_id, characterId)
                    .SetProperty(p => p.update_date, now));

                if (affectedRows == 0)
                {
                    return ErrorCode.DataNotFound;
                }

                await _personCacheService.deletePersonStatusAsync(personId);

                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed setPersonLeaderCharacter PersonId {personId} CharacterId {characterId}", personId, characterId);
                return ErrorCode.TransactionFailed;
            }
        }
    }
}
