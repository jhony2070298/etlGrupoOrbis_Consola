using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Sql_A_Json
{
    class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        public IRestResponse ApiToken()
        {
            var client = new RestClient("https://pruebassoul.magnum.com.co/centralizer/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", "administracion@tdm.com.co");
            request.AddParameter("password", "Integracion0rbis*");
            IRestResponse response = client.Execute(request);
            return response; 
        }
    }
}
