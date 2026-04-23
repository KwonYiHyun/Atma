using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class AchievementDto
    {
        public int achievement_id;
        public AchievementCategory achievement_category_id;
        public string achievement_title;
        public string achievement_description;
        public AchievementType achievement_type;

        public int achievement_param_1;
        public int achievement_param_2;
        public int achievement_param_3;

        public int current_value;
        public int target_value;

        public bool is_clear;
        public bool is_taken;
        public ObjectDisplayDto reward_1;
        public ObjectDisplayDto? reward_2;
        public ObjectDisplayDto? reward_3;

        public long remaining_seconds;
    }

    [Serializable]
    public class AchievementSuccessInfo
    {
        public string title { get; set; }
        public int targetValue { get; set; }
    }
}
