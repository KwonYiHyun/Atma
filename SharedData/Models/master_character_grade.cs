using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_character_grade
    {
        [Key]
        public int character_grade_id { get; set; }
        public int character_id { get; set; }
        public int grade { get; set; }
        public int critical_rate { get; set; }
        public int critical_damage { get; set; }
        public int require_count { get; set; }
        public int price_token { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public master_character character { get; set; }
    }
}
