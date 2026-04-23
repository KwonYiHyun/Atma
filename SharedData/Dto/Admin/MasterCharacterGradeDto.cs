using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterCharacterGradeDto
    {
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
    }
}
