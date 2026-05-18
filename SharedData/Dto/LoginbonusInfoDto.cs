using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class LoginbonusInfoDto
    {
        public int daily_login_bonus_id;
        public string title_text;
        public string back_image;
        public int current_day;
        public List<LoginbonusDayDto> bonus_days;
    }

    [Serializable]
    public class LoginbonusDayDto
    {
        public int daily_login_bonus_day_id;
        public int total_login_count;
        public ObjectDisplayDto reward_1;
    }
}
