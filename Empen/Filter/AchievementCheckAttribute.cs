using Microsoft.AspNetCore.Mvc;

namespace Empen.Filter
{
    public class AchievementCheckAttribute : TypeFilterAttribute
    {
        public AchievementCheckAttribute() : base(typeof(AchievementCheckFilter))
        {
            
        }
    }
}
