using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class PersonCharacterDto
    {
        // person_character
        public int character_id { get; set; }
        public int character_level { get; set; }
        public int atk { get; set; }
        public int def { get; set; }
        public int hp { get; set; }
        public int stance { get; set; }
        public int character_grade { get; set; }

        // master_character
        public string character_name { get; set; }
    }
}
