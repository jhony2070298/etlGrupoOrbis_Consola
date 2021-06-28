using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Sql_A_Json
{
    class Authentication
    {
        public string Session { get; set; }
        public bool ValidResponse { get; set; }
        public List<string> Message { get; set; }
        public int Code { get; set; }
        public string Response { get; set; }

        public IRestResponse ApiSession(string token)
        {
            var client = new RestClient("https://pruebassoul.magnum.com.co/centralizer/api/LandExport/Authentication");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer "+token);
            request.AddParameter("application/json", "{\r\n  \"User\": \"hector.jaramillo@tdm.com.co\",\r\n  \"Password\": \"Integracion0rbis*\",\r\n  \"Lrp\": 2\r\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
            
        }
    }
}
