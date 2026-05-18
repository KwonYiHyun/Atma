using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class PersonInfoDto
    {
        public int display_person_id;
        public string email;
        public DateTime insert_date;
        public string person_name;
        public int level;
        public int exp;
        public int token;
        public int gift;
        public int manual;
        public int film;
        public int prism;
        public int leader_person_character_id;
        public string introduce;
        public int last_story_id;
        public bool is_remain_mail;
        public string character_comment;
    }
}
