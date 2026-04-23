using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_friend
    {
        [Key]
        public int person_friend_id { get; set; }
        public int person_id { get; set; }
        public int friend_person_id { get; set; }
        public int type { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
