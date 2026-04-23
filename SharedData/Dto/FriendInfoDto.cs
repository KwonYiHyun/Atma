using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class FriendInfoDto
    {
        public int person_id { get; set; }
        public string person_name { get; set; }
        public int level { get; set; }
        public string introduce { get; set; }
        public DateTime friend_date { get; set; }
        public DateTime last_login_date { get; set; }
        public int leader_person_character_id { get; set; }
        public string leader_character_name { get; set; }
        public bool each_other { get; set; }
    }
}
