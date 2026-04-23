using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class BannerInfoDto
    {
        public int banner_id;
        public int show_place_type;
        public string banner_image;
        public BannerActionType banner_action_type;
        public string banner_action_param;
        public int limited_type;
        public int limited_param;
        public DateTime start_date;
        public DateTime? end_date;
    }
}
