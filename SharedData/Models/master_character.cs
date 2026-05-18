using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_character
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
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
        public ICollection<master_gacha_lot> gacha_lots { get; set; }
        public ICollection<master_character_level> character_levels { get; set; }
        public ICollection<master_character_grade> character_grades { get; set; }
    }
}
