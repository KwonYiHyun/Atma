using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class StoryGroupDto
    {
        public int story_group_id { get; set; }
        public string group_name { get; set; }
        public int story_type { get; set; }
        public string main_character { get; set; }
    }
}
