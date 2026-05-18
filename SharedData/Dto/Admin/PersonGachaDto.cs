using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class PersonGachaDto
    {
        public int person_gacha_id { get; set; }
        public int person_id { get; set; }
        public int gacha_id { get; set; }
        public int gacha_count { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
