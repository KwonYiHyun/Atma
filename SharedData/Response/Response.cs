using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Response
{
    [Serializable]
    public class GameResponse<T>
    {
        public ErrorCode Result { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public GameResponse() { }

        public GameResponse(T res)
        {
            Result = ErrorCode.Success;
            Message = "";
            Data = res;
        }

        public GameResponse(ErrorCode error, string msg = "")
        {
            Result = error;
            Message = msg;
            Data = default;
        }
    }
}
