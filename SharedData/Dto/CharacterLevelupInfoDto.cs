using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class CharacterLevelupInfoDto
    {
        public ObjectDisplayDto resource_item_1 { get; set; }
        public ObjectDisplayDto resource_item_2 { get; set; }
        public ObjectDisplayDto resource_item_3 { get; set; }

        public int item_1_amount { get; set; }
        public int item_2_amount { get; set; }
        public int item_3_amount { get; set; }

        public int item_1_require { get; set; }
        public int item_2_require { get; set; }
        public int item_3_require { get; set; }

        public int current_atk { get; set; }
        public int current_def { get; set; }
        public int current_hp { get; set; }
        public int current_stance { get; set; }

        public int next_atk { get; set; }
        public int next_def { get; set; }
        public int next_hp { get; set; }
        public int next_stance { get; set; }
    }
}
