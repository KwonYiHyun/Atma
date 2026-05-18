using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class StoryScriptDto
    {
        public int story_script_id { get; set; }
        public int story_id { get; set; }
        public string story_background_image { get; set; }
        public int background_effect { get; set; }
        public int pos_1_char { get; set; }
        public int pos_1_char_motion { get; set; }
        public int pos_2_char { get; set; }
        public int pos_2_char_motion { get; set; }
        public string story_text { get; set; }
        public string answer_1 { get; set; }
        public string answer_2 { get; set; }
        public string answer_3 { get; set; }
        public int answer_1_story_id { get; set; }
        public int answer_2_story_id { get; set; }
        public int answer_3_story_id { get; set; }
    }
}
