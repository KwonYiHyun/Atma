using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_story_script
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int story_script_id { get; set; }
        public int story_id { get; set; }
        public int show_order { get; set; }
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
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
