using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Type
{
    public enum ServerType
    {
        LoginServer = 0,
        GameServer
    }

    public enum ErrorCode
    {
        // 성공
        Success = 0,

        // 공통
        InvalidRequest,
        Unauthorized,
        ServerError,
        DataNotFound,
        TransactionFailed,
        DbUpdateConcurrencyException,
        DuplicationId,
        ServerMaintenance,

        // 가챠
        GachaNotFound,
        NotEnoughFlim,
        InvalidExecCount,
        
        // 상점
        ProductNotFound,
        NotEnoughCurrency,
        LimitExceeded,
        NotInSalesPeriod,

        // 사용자 정보
        PersonNotFound,
        DuplicateName,

        // 친구
        FriendNotFound,
        MyselfError,
        DuplicationFriend,

        // 아이템 사용
        ItemNotFound,
        NotEnoughAmount,
        Expired,
        UnknownType,
        EffectFailed,

        // 캐릭터 상세
        CharacterNotFound,
        LevelNotFound,
        GradeNotFound,

        // 업적
        AchievementNotFound,
        NotConditionMet,
        AlreadyReceived,

        // 한계돌파
        NotEnoughPiece,
        NotEnoughToken,
        AlreadyMaxGrade,

        // 레벨업
        NotEnoughMaterial,
        AlreadyMaxLevel,
        FailedUseItem
    }

    public enum ResourceType
    {
        Token,
        Prism,
        GachaResource,
        ActivePoint
    }

    public enum ItemType
    {
        GACHA_TICKET = 1,
        GACHA_POINT,
        CHARACTER_PIECE,
        CHARACTER_REINFORCEMENT_MATERIAL
    }

    public enum ItemTabType
    {
        ITEM = 1,               // 아이템
        MATERIAL,               // 강화소재
        PIECE                   // 피스 (한돌)
    }

    public enum ObjectType
    {
        ITEM = 1,
        CHARACTER,
        PRISM,
        TOKEN,
        FILM,
        ETC
    }

    public enum BannerActionType
    {
        GACHA = 1,
        PRODUCT,
        COMBAT
    }

    public enum GachaType
    {
        START = 1,  // 스타트 뽑기
        BASIC,      // 프리즘 사용하는 뽑기
        TICKET      // 티켓 뽑기
    }

    public enum BannerShowType
    {
        MAIN = 1,

    }

    public enum AchievementType
    {
        STORY_CLEAR = 1,
        LOGIN_COUNT,
        STAGE_CLEAR,
        GACHA_PLAY
    }

    public enum AchievementCategory
    {
        DAILY = 1,
        WEEKLY,
        SPECIAL
    }

    public enum PriceType
    {
        KRW,
        PRISM,
        TOKEN,
        PIECE
    }
}
