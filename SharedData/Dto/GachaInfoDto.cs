using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class GachaInfoDto
    {
        public int gacha_id;
        public string gacha_name;
        public int gacha_type;
        public int gacha_consume_value;
        public string gacha_image;
        public string gacha_detail_image;
        public string gacha_detail_html;
        public int gacha_point;
        public DateTime current_date;
        public DateTime start_date;
        public DateTime? end_date;
    }
}
