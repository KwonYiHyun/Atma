using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Request
{
    [Serializable]
    public class GachaPlayRequest
    {
        public GachaPlayRequest(int gachaId, int execCount) { this.gachaId = gachaId; this.execCount = execCount; }
        public int gachaId;
        public int execCount;
    }
}
