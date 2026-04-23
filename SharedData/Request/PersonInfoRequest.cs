using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Request
{
    public class PersonInfoRequest
    {
        public string person_name { get; set; }
        public int leader_person_character_id { get; set; }
        public string introduce { get; set; }
    }
}
