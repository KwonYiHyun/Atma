using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterCharacterDto
    {
        public int character_id { get; set; }
        public int show_order { get; set; }
        public string character_name { get; set; }
        public int character_level_max { get; set; }
        public int character_grade_max { get; set; }
        public int character_grade { get; set; }
        public int character_critical_rate { get; set; }
        public int character_critical_damage { get; set; }
        public int piece_item_id { get; set; }
        public int piece_amount_duplicate { get; set; }
        public string character_description { get; set; }
        public string character_comment_1 { get; set; }
        public string character_comment_2 { get; set; }
        public string character_comment_3 { get; set; }
        public string character_comment_1_motion { get; set; }
        public string character_comment_2_motion { get; set; }
        public string character_comment_3_motion { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
