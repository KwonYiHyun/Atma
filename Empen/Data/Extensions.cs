using SharedData.Dto;
using SharedData.Type;

namespace Empen.Data
{
    public static class Extensions
    {
        public static bool isUsable(this ItemType type)
        {
            return type == ItemType.CHARACTER_PIECE ||
                type == ItemType.GACHA_TICKET;
        }

        public static bool isUsable(this int type)
        {
            return isUsable((ItemType)type);
        }
    }
}
