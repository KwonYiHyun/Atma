using Newtonsoft.Json;
using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class ObjectDisplayDto
    {
        public ObjectType object_type;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CharacterInfoDto? character_info;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CommonItemInfoDto? common_item_info;
    }

    [Serializable]
    public class CommonItemInfoDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int amount { get; set; }

        public int item_type { get; set; }
        public int tab_type { get; set; }
        public int? item_param_1 { get; set; }
        public int? item_param_2 { get; set; }
        public DateTime? expire_date { get; set; }

        public bool usable { get; set; }
    }
}