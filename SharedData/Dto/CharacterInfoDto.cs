using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class CharacterInfoDto
    {
        public int character_id { get; set; }
        public string character_name { get; set; }
        public int character_level_max { get; set; }
        public int character_grade_max { get; set; }
        public int character_grade { get; set; }
        //public int character_critical_rate { get; set; }
        //public int character_critical_damage { get; set; }
        //public ObjectDisplayDto piece_item { get; set; }
        //public string character_description { get; set; }
        //public string character_comment_1 { get; set; }
        //public string character_comment_2 { get; set; }
        //public string character_comment_3 { get; set; }
        //public string character_comment_1_motion { get; set; }
        //public string character_comment_2_motion { get; set; }
        //public string character_comment_3_motion { get; set; }
        //public int character_category_id_1 { get; set; }
        //public int character_category_id_2 { get; set; }
        //public int character_category_id_3 { get; set; }
        //public int collection_no { get; set; }
    }
}
