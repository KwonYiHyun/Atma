using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SharedData.Models;

namespace Empen.Service
{
    public interface IMasterDataCacheService
    {
        Task<Dictionary<int, master_item>> getItemMapAsync();
        Task<Dictionary<int, master_reward>> getRewardMapAsync();
        Task<Dictionary<int, master_banner>> getBannerMapAsync();
        Task<Dictionary<int, master_character>> getCharacterMapAsync();
        Task<Dictionary<(int, int), master_character_level>> getCharacterLevelMapAsync();
        Task<Dictionary<(int, int), master_character_grade>> getCharacterGradeMapAsync();
        Task<Dictionary<int, master_daily_login_bonus>> getLoginbonusMapAsync();
        Task<Dictionary<int, master_daily_login_bonus_day>> getLoginbonusDayMapAsync();
        Task<Dictionary<int, master_gacha>> getGachaMapAsync();
        Task<Dictionary<int, master_gacha_exec_10>> getGachaExec10MapAsync();
        Task<Dictionary<int, master_gacha_lot>> getGachaLotMapAsync();
        Task<Dictionary<int, master_gacha_lot_group>> getGachaLotGroupMapAsync();
        Task<Dictionary<int, master_product>> getProductMapAsync();
        Task<Dictionary<int, master_product_set>> getProductSetMapAsync();
        Task<Dictionary<int, master_product_set_piece>> getProductSetPieceMapAsync();
        Task<Dictionary<int, master_product_set_prism>> getProductSetPrismMapAsync();
        Task<Dictionary<int, master_product_set_token>> getProductSetTokenMapAsync();
        Task<Dictionary<int, master_achievement>> getAchievementMapAsync();
        Task<Dictionary<int, master_achievement_category>> getAchievementCategoryMapAsync();
        Task<Dictionary<int, master_notice>> getNoticeMapAsync();
        Task<Dictionary<int, master_mail>> getMailMapAsync();
    }
}