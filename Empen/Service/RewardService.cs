using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using static System.Net.Mime.MediaTypeNames;

namespace Empen.Service
{
    public class RewardService : IRewardService
    {
        //private readonly MasterDbContext _masterContext;
        private readonly IMasterDataCacheService _masterDataCacheService;

        public RewardService(IMasterDataCacheService masterDataCacheService)
        {
            //_masterContext = masterContext;
            _masterDataCacheService = masterDataCacheService;
        }

        public async Task<Dictionary<int, ObjectDisplayDto>> getObjectDisplayAsync(IEnumerable<int> rewardIds)
        {
            var uniqueIds = rewardIds.Distinct().ToList();
            if (!uniqueIds.Any())
            {
                return new Dictionary<int, ObjectDisplayDto>();
            }

            //var rewards = await _masterContext.master_reward
            //    .AsNoTracking()
            //    .Where(r => uniqueIds.Contains(r.reward_id))
            //    .ToListAsync();

            var rewardMap = await _masterDataCacheService.getRewardMapAsync();
            var rewards = rewardMap
                .Values
                .Where(r => uniqueIds.Contains(r.reward_id))
                .ToList();

            var itemIds = new HashSet<int>();
            var charIds = new HashSet<int>();

            foreach (var r in rewards)
            {
                if (r.object_type == (int)ObjectType.ITEM)
                {
                    itemIds.Add(r.object_value);
                }
                else if (r.object_type == (int)ObjectType.CHARACTER)
                {
                    charIds.Add(r.object_value);
                }
            }

            var itemAllMap = await _masterDataCacheService.getItemMapAsync();
            var characterAllMap = await _masterDataCacheService.getCharacterMapAsync();

            var itemMap = itemAllMap.Values
                .Where(i => itemIds.Contains(i.item_id))
                .ToDictionary(i => i.item_id);

            var charMap = characterAllMap.Values
                .Where(c => charIds.Contains(c.character_id))
                .ToDictionary(c => c.character_id);

            //var itemMap = await _masterContext.master_item
            //    .AsNoTracking()
            //    .Where(i => itemIds.Contains(i.item_id))
            //    .ToDictionaryAsync(i => i.item_id);

            //var charMap = await _masterContext.master_character
            //    .AsNoTracking()
            //    .Where(c => charIds.Contains(c.character_id))
            //    .ToDictionaryAsync(c => c.character_id);

            var result = new Dictionary<int, ObjectDisplayDto>();

            foreach (var r in rewards)
            {
                ObjectDisplayDto dto = null;

                switch (r.object_type)
                {
                    case (int)ObjectType.ITEM:
                        if (itemMap.TryGetValue(r.object_value, out var item))
                        {
                            dto = createObjectDisplayByItem(item, r.object_amount);
                        }
                        break;

                    case (int)ObjectType.CHARACTER:
                        if (charMap.TryGetValue(r.object_value, out var character))
                        {
                            dto = createObjectDisplayByCharacter(character);
                        }
                        break;

                    case (int)ObjectType.PRISM:
                        dto = createObjectDisplayByResource(ObjectType.PRISM, "프리즘", "프리즘 입니다.", Constant.IMAGE_ICON_PRISM, r.object_amount);
                        break;

                    case (int)ObjectType.TOKEN:
                        dto = createObjectDisplayByResource(ObjectType.TOKEN, "토큰", "토큰 입니다.", Constant.IMAGE_ICON_TOKEN, r.object_amount);
                        break;

                    case (int)ObjectType.FILM:
                        dto = createObjectDisplayByResource(ObjectType.FILM, "필름", "필름 입니다.", Constant.IMAGE_ICON_GACHA_RESOURCE, r.object_amount);
                        break;
                }

                if (dto != null)
                {
                    result[r.reward_id] = dto;
                }
            }

            if (rewards.Count != result.Count)
            {
                // TODO: !! 누락된 reward가 존재합니다 !!
            }

            return result;
        }

        public async Task<List<ObjectDisplayDto>> getObjectDisplayListByContentAsync(Dictionary<int, int> itemAmounts, List<int> characterIds, int prism, int token, int gacha)
        {
            var result = new List<ObjectDisplayDto>();

            // 재화
            if (token > 0)
            {
                result.Add(createObjectDisplayByResource(ObjectType.TOKEN, "토큰", "토큰 입니다.", Constant.IMAGE_ICON_TOKEN, token));
            }
            if (prism > 0)
            {
                result.Add(createObjectDisplayByResource(ObjectType.PRISM, "프리즘", "프리즘 입니다.", Constant.IMAGE_ICON_PRISM, prism));
            }
            if (gacha > 0)
            {
                result.Add(createObjectDisplayByResource(ObjectType.FILM, "필름", "필름 입니다.", Constant.IMAGE_ICON_GACHA_RESOURCE, gacha));
            }

            // 아이템, 캐릭터
            Dictionary<int, master_item> masterItemMap = new Dictionary<int, master_item>();
            if (itemAmounts.Count > 0)
            {
                var masterItemAll = await _masterDataCacheService.getItemMapAsync();

                masterItemMap = masterItemAll
                    .Values
                    .Where(i => itemAmounts.Keys.Contains(i.item_id))
                    .ToDictionary(i => i.item_id);
            }

            Dictionary<int, master_character> masterCharMap = new Dictionary<int, master_character>();
            if (characterIds.Count > 0)
            {
                var masterCharacterAll = await _masterDataCacheService.getCharacterMapAsync();

                masterCharMap = masterCharacterAll
                    .Values
                    .Where(c => characterIds.Contains(c.character_id))
                    .ToDictionary(c => c.character_id);
            }

            // DTO
            foreach (var item in itemAmounts)
            {
                if (masterItemMap.TryGetValue(item.Key, out var masterItem))
                {
                    result.Add(createObjectDisplayByItem(masterItem, item.Value));
                }
            }

            foreach (var charId in characterIds)
            {
                if (masterCharMap.TryGetValue(charId, out var masterChar))
                {
                    result.Add(createObjectDisplayByCharacter(masterChar));
                }
            }

            return result;
        }

        public async Task<ObjectDisplayDto?> getObjectDisplayOneAsync(int rewardId)
        {
            var map = await getObjectDisplayAsync(new List<int> { rewardId });

            if (map.TryGetValue(rewardId, out var dto))
            {
                return dto;
            }

            return null;
        }
        
        public ObjectDisplayDto createObjectDisplayByItem(master_item item, int amount)
        {
            return new ObjectDisplayDto
            {
                object_type = ObjectType.ITEM,
                character_info = null,
                common_item_info = new CommonItemInfoDto
                {
                    id = item.item_id,
                    name = item.item_name,
                    description = item.item_description,
                    image = item.item_image_name,
                    amount = amount,

                    item_type = item.item_type,
                    tab_type = item.tab_type,
                    item_param_1 = item.item_param_1,
                    item_param_2 = item.item_param_2,
                    expire_date = null,

                    usable = item.item_type.isUsable(),
                }
            };
        }

        public ObjectDisplayDto createObjectDisplayByCharacter(master_character charData)
        {
            return new ObjectDisplayDto
            {
                object_type = ObjectType.CHARACTER,
                common_item_info = null,
                character_info = new CharacterInfoDto
                {
                    character_id = charData.character_id,
                    character_name = charData.character_name,
                    character_level_max = charData.character_level_max,
                    character_grade_max = charData.character_grade_max,
                    character_grade = charData.character_grade,
                    //character_critical_rate = charData.character_critical_rate,
                    //character_critical_damage = charData.character_critical_damage,
                    //character_description = charData.character_description,
                    //character_comment_1 = charData.character_comment_1,
                    //character_comment_2 = charData.character_comment_2,
                    //character_comment_3 = charData.character_comment_3,
                    //character_comment_1_motion = charData.character_comment_1_motion,
                    //character_comment_2_motion = charData.character_comment_2_motion,
                    //character_comment_3_motion = charData.character_comment_3_motion,
                    //character_category_id_1 = charData.character_category_id_1,
                    //character_category_id_2 = charData.character_category_id_2,
                    //character_category_id_3 = charData.character_category_id_3,
                    //collection_no = charData.collection_no
                }
            };
        }

        private ObjectDisplayDto createObjectDisplayByResource(ObjectType type, string name, string desc, string image, int amount)
        {
            return new ObjectDisplayDto
            {
                object_type = type,
                character_info = null,
                common_item_info = new CommonItemInfoDto
                {
                    id = 0,
                    name = name,
                    description = desc,
                    image = image,
                    amount = amount,

                    // X
                    item_type = 0,
                    tab_type = 0,
                    item_param_1 = null,
                    item_param_2 = null,
                    expire_date = null,

                    usable = false
                }
            };
        }
    }
}
