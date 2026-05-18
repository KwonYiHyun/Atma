using static StackExchange.Redis.Role;

namespace Empen.Data
{
    public static class Constant
    {
        public const string LOGIN_PROVIDER_GOOGLE = "Google";
        public const string LOGIN_PROVIDER_GUEST = "Guest";

        public const string DEFAULT_USERNAME = "탐험가";
        public const string DEFAULT_INTRODUCE = "안녕하세요!";

        public const string LOGIN_BONUS_MAIL_TITLE = "로그인 보너스 보상";
        public const string LOGIN_BONUS_MAIL_DES = "{0}일차 로그인 보너스 보상입니다.";

        public const string PRODUCT_BUY_MAIL_TITLE = "상품 구매 보상";
        public const string PRODUCT_BUY_MAIL_DES = "{0} 구매 보상입니다.";

        public const string PRODUCT_PRISM_BUY_MAIL_TITLE = "프리즘 상품 구매 보상";
        public const string PRODUCT_PRISM_BUY_MAIL_DES = "{0} 구매 보상입니다.";

        public const string PRODUCT_TOKEN_BUY_MAIL_TITLE = "토큰 상품 구매 보상";
        public const string PRODUCT_TOKEN_BUY_MAIL_DES = "{0} 구매 보상입니다.";

        public const string PRODUCT_PIECE_BUY_MAIL_TITLE = "조각 상점 구매 보상";
        public const string PRODUCT_PIECE_BUY_MAIL_DES = "{0} 구매 보상입니다.";

        public const string STORY_REWARD_MAIL_TITLE = "스토리 열람 보상";
        public const string STORY_REWARD_MAIL_DES = "스토리 열람 보상입니다.";

        public const string ACHIEVEMENT_TITLE = "업적 달성: {0}";
        public const string ACHIEVEMENT_DES = "업적 달성 보상입니다.";

        public const string IMAGE_ICON_TOKEN = "icon/token.png";
        public const string IMAGE_ICON_PRISM = "icon/prism.png";
        public const string IMAGE_ICON_GACHA_RESOURCE = "icon/film.png";
        public const string IMAGE_ICON_CHARACTER = "icon/character/{0}.png";
    }
}
