using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class DeckInfoDto
    {
        public int person_deck_id { get; set; }
        public List<PersonCharacterDto> characters { get; set; }
    }
}
