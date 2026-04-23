using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class StoryDto
    {
        public int story_id { get; set; }
        public int story_group_id { get; set; }
        public string story_name { get; set; }
        public int first_reward_id_1 { get; set; }
        public int first_reward_id_2 { get; set; }
        public int first_reward_id_3 { get; set; }
    }
}
