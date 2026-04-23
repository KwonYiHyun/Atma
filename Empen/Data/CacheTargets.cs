using static Empen.Data.CacheKey;

namespace Empen.Data
{
    public static class CacheKey
    {
        public static class Master
        {
            public const string ItemAll = "Master:Item:All";
            public const string RewardAll = "Master:Reward:All";
            public const string BannerAll = "Master:Banner:All";
            public const string CharacterAll = "Master:Character:All";
            public const string CharacterGradeAll = "Master:CharacterGrade:All";
            public const string CharacterLevelAll = "Master:CharacterLevel:All";
            public const string LoginbonusAll = "Master:Loginbonus:All";
            public const string LoginbonusDayAll = "Master:LoginbonusDay:All";
            public const string GachaAll = "Master:Gacha:All";
            public const string GachaExec10All = "Master:GachaExec10:All";
            public const string GachaLotAll = "Master:GachaLot:All";
            public const string GachaLotGroupAll = "Master:GachaLotGroup:All";
            public const string ProductAll = "Master:Product:All";
            public const string ProductSetAll = "Master:ProductSet:All";
            public const string ProductSetPieceAll = "Master:ProductSetPiece:All";
            public const string ProductSetPrismAll = "Master:ProductSetPrism:All";
            public const string ProductSetTokenAll = "Master:ProductSetToken:All";
            public const string AchievementAll = "Master:Achievement:All";
            public const string AchievementCategoryAll = "Master:AchievementCategory:All";
            public const string NoticeAll = "Master:Notice:All";
            public const string MailAll = "Master:Mail:All";

            public static string Item(int id) => $"Master:Item:{id}";
            public static string Reward(int id) => $"Master:Reward:{id}";
            public static string Banner(int id) => $"Master:Banner:{id}";
            public static string Character(int id) => $"Master:Character:{id}";
            public static string CharacterGrade(int id) => $"Master:CharacterGrade:{id}";
            public static string CharacterLevel(int id) => $"Master:CharacterLevel:{id}";
            public static string Loginbonus(int id) => $"Master:Loginbonus:{id}";
            public static string LoginbonusDay(int id) => $"Master:LoginbonusDay:{id}";
            public static string Gacha(int id) => $"Master:Gacha:{id}";
            public static string GachaExec10(int id) => $"Master:GachaExec10:{id}";
            public static string GachaLot(int id) => $"Master:GachaLot:{id}";
            public static string GachaLotGroup(int id) => $"Master:GachaLotGroup:{id}";
            public static string Product(int id) => $"Master:Product:{id}";
            public static string ProductSet(int id) => $"Master:ProductSet:{id}";
            public static string ProductSetPiece(int id) => $"Master:ProductSetPiece:{id}";
            public static string ProductSetPrism(int id) => $"Master:ProductSetPrism:{id}";
            public static string ProductSetToken(int id) => $"Master:ProductSetToken:{id}";
            public static string Achievement(int id) => $"Master:Achievement:{id}";
            public static string AchievementCategory(int id) => $"Master:AchievementCategory:{id}";
            public static string Notice(int id) => $"Master:Notice:{id}";
            public static string Mail(int id) => $"Master:Mail:{id}";
        }

        public static class Person
        {
            public static string PersonItem(int id) => $"Person:{id}:Item";
            public static string PersonStatus(int id) => $"Person:{id}:Status";
        }

        public static class Local
        {
            public const string ItemMap = "Local:Master:Item:Map";
            public const string RewardMap = "Local:Master:Reward:Map";
            public const string BannerMap = "Local:Master:Banner:Map";
            public const string CharMap = "Local:Master:Char:Map";
            public const string CharGradeMap = "Local:Master:CharGrade:Map";
            public const string CharLevelMap = "Local:Master:CharLevel:Map";
            public const string LoginbonusMap = "Local:Master:Loginbonus:Map";
            public const string LoginbonusDayMap = "Local:Master:LoginbonusDay:Map";
            public const string GachaMap = "Local:Master:Gacha:Map";
            public const string GachaExec10Map = "Local:Master:GachaExec10:Map";
            public const string GachaLotMap = "Local:Master:GachaLot:Map";
            public const string GachaLotGroupMap = "Local:Master:GachaLotGroup:Map";
            public const string ProductMap = "Local:Master:Product:Map";
            public const string ProductSetMap = "Local:Master:ProductSet:Map";
            public const string ProductSetPieceMap = "Local:Master:ProductSetPiece:Map";
            public const string ProductSetPrismMap = "Local:Master:ProductSetPrism:Map";
            public const string ProductSetTokenMap = "Local:Master:ProductSetToken:Map";
            public const string AchievementMap = "Local:Master:Achievement:Map";
            public const string AchievementCategoryMap = "Local:Master:AchievementCategory:Map";
            public const string NoticeMap = "Local:Master:Notice:Map";
            public const string MailMap = "Local:Master:Mail:Map";
        }

        public static class Channel
        {
            public const string Master = "System:MasterData:Update";
        }
    }


    public static class CacheTargets
    {
        public const string MAINTENANCE_KEY = "server:maintenance";

        public static readonly Dictionary<string, string> Map = new()
        {
            { "전체 (All)", "ALL" },
            { "아이템 (Item)", "ITEM" },
            { "리워드 (Reward)", "REWARD" },
            { "배너 (Banner)", "BANNER" },
            { "캐릭터 (Character)", "CHARACTER" },
            { "캐릭터 등급 (CharacterGrade)", "CHARACTER_GRADE" },
            { "캐릭터 레벨 (CharacterLevel)", "CHARACTER_LEVEL" },
            { "로그인보너스 (LoginBonus)", "LOGINBONUS" },
            { "로그인보너스 (LoginBonusDay)", "LOGINBONUS_DAY" },
            { "가차 (Gacha)", "GACHA" },
            { "가차 (GachaExec10)", "GACHA_EXEC_10" },
            { "가차 (GachaLot)", "GACHA_LOT" },
            { "가차 (GachaLotGroup)", "GACHA_LOT_GROUP" },
            { "상점 (Product)", "PRODUCT" },
            { "상점 (ProductSet)", "PRODUCT_SET" },
            { "상점 (ProductSetPiece)", "PRODUCT_SET_PIECE" },
            { "상점 (ProductSetPrism)", "PRODUCT_SET_PRISM" },
            { "상점 (ProductSetToken)", "PRODUCT_SET_TOKEN" },
            { "업적 (Achievement)", "ACHIEVEMENT" },
            { "업적 (AchievementCategory)", "ACHIEVEMENT_CATEGORY" },
            { "공지 (Notice)", "NOTICE" },
            { "메일 (Mail)", "MAIL" }
        };

        public static string? getRedisKey(string targetCode)
        {
            return targetCode switch
            {
                "ITEM" => Master.ItemAll,
                "REWARD" => Master.RewardAll,
                "BANNER" => Master.BannerAll,
                "CHARACTER" => Master.CharacterAll,
                "CHARACTER_GRADE" => Master.CharacterGradeAll,
                "CHARACTER_LEVEL" => Master.CharacterLevelAll,
                "LOGINBONUS" => Master.LoginbonusAll,
                "LOGINBONUS_DAY" => Master.LoginbonusDayAll,
                "GACHA" => Master.GachaAll,
                "GACHA_EXEC_10" => Master.GachaExec10All,
                "GACHA_LOT" => Master.GachaLotAll,
                "GACHA_LOT_GROUP" => Master.GachaLotGroupAll,
                "PRODUCT" => Master.ProductAll,
                "PRODUCT_SET" => Master.ProductSetAll,
                "PRODUCT_SET_PIECE" => Master.ProductSetPieceAll,
                "PRODUCT_SET_PRISM" => Master.ProductSetPrismAll,
                "PRODUCT_SET_TOKEN" => Master.ProductSetTokenAll,
                "ACHIEVEMENT" => Master.AchievementAll,
                "ACHIEVEMENT_CATEGORY" => Master.AchievementCategoryAll,
                "NOTICE" => Master.NoticeAll,
                "MAIL" => Master.MailAll,
                _ => null
            };
        }
    }
}
