using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Request
{
    [Serializable]
    public class GuestLoginRequest
    {
        public int id;
    }

    [Serializable]
    public class GuestLoginResponse
    {
        public string accessToken;
        public string refreshToken;
        public int displayPersonId;
    }

    public class GoogleLoginRequest
    {
        public string idToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
