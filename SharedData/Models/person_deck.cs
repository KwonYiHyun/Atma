using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_deck
    {
        [Key]
        public int person_deck_id { get; set; }
        public int person_id { get; set; }
        public int slot_id { get; set; }
        public int person_character_id_1 { get; set; }
        public int person_character_id_2 { get; set; }
        public int person_character_id_3 { get; set; }
        public int person_character_id_4 { get; set; }
        public int person_character_id_5 { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
