using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_character_level
    {
        [Key]
        public int character_level_id { get; set; }
        public int character_id { get; set; }
        public int level { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int hp { get; set; }
        public int stance { get; set; }
        public int resource_item_id_1 { get; set; }
        public int resource_item_id_2 { get; set; }
        public int resource_item_id_3 { get; set; }
        public int item_1_amount { get; set; }
        public int item_2_amount { get; set; }
        public int item_3_amount { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public master_character character { get; set; }
    }
}
