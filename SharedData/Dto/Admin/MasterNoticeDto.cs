using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterNoticeDto
    {
        public int notice_id { get; set; }
        public string notice_title { get; set; }
        public string notice_content { get; set; }
        public string notice_banner_image { get; set; }
        public string notice_link { get; set; }
        public int notice_type { get; set; }
        public int show_order { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
