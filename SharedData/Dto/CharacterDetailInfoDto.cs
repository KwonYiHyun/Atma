using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class LimitBreakResultDto
    {
        public ErrorCode result;
        public CharacterDetailInfoDto info;
    }

    [Serializable]
    public class LevelUpResultDto
    {
        public ErrorCode result;
        public CharacterDetailInfoDto info;
    }

    [Serializable]
    public class CharacterDetailInfoDto
    {
        public int character_id { get; set; }
        public string character_name { get; set; }
        public int character_level_max { get; set; }
        public int character_grade_max { get; set; }
        public int character_grade { get; set; }
        public int character_critical_rate { get; set; }
        public int character_critical_damage { get; set; }
        public ObjectDisplayDto piece_item { get; set; }
        public int piece_require_amount { get; set; }
        public int current_piece_amount { get; set; }
        public int price_token { get; set; }
        public string character_description { get; set; }
        public string character_comment_1 { get; set; }
        public string character_comment_2 { get; set; }
        public string character_comment_3 { get; set; }
        public string character_comment_1_motion { get; set; }
        public string character_comment_2_motion { get; set; }
        public string character_comment_3_motion { get; set; }

        public int current_level { get; set; }
        public int current_grade { get; set; }
        public int current_atk { get; set; }
        public int current_def { get; set; }
        public int current_hp { get; set; }
        public int current_stance { get; set; }
        public int current_critical_rate { get; set; }
        public int current_critical_damage { get; set; }

        public int next_critical_rate { get; set; }
        public int next_critical_damage { get; set; }

        public bool is_max_level { get; set; }
        public bool is_max_grade { get; set; }
    }
}
