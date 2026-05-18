using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class GachaResultDto
    {
        public ErrorCode result;
        public List<GachaPlayInfoDto> results;

        public GachaResultDto()
        {
            results = new List<GachaPlayInfoDto>();
        }
    }

    [Serializable]
    public class GachaPlayInfoDto
    {
        public ObjectDisplayDto obj { get; set; }
        public bool is_new { get; set; }
    }
}
