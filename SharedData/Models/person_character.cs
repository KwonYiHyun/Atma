using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_character
    {
        [Key]
        public int person_character_id { get; set; }
        public int person_id { get; set; }
        public int character_id { get; set; }
        public int character_level { get; set; }
        public int grade { get; set; }
        public int friendship_level { get; set; }
        public int friendship_exp { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
