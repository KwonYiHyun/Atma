using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class PersonStatusDto
    {
        public int person_status_id { get; set; }
        public int person_id { get; set; }
        public int display_person_id { get; set; }
        public string person_hash { get; set; }
        public string email { get; set; }
        public string? person_name { get; set; }
        public int level { get; set; }
        public int exp { get; set; }
        public int token { get; set; }
        public int gift { get; set; }
        public int manual { get; set; }
        public int flim { get; set; }
        public int prism { get; set; }
        public int character_amount_max { get; set; }
        public int character_storage_amount_max { get; set; }
        public int leader_person_character_id { get; set; }
        public string? introduce { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
