using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Console_Sql_A_Json.Exportaciones;

namespace Console_Sql_A_Json
{
    class Program
    {
        static void Main(string[] args)
        {
            string NumeroExportacion = "";
            string message = string.Empty;
            Token token = new Token();
            var response = token.ApiToken();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Token deserializedJson = JsonConvert.DeserializeObject<Token>(response.Content);
                string tokenResponse = deserializedJson.access_token;
                Authentication authentication = new Authentication();
                var responseAuthenticacion = authentication.ApiSession(tokenResponse);
                if (responseAuthenticacion.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Authentication deserializedAuthentication = JsonConvert.DeserializeObject<Authentication>(responseAuthenticacion.Content);
                    string Session = deserializedAuthentication.Session;
                    Exportaciones exportaciones = new Exportaciones();
                    int code;
                    int contador = 0;
                    bool flag = true;
                    while (flag == true || contador == 3)
                    {
                        var responseExportaciones = exportaciones.ApiExportaciones(tokenResponse, Session);
                        if (responseExportaciones.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Exportaciones.ResultExportData deserializedExportacion = JsonConvert.DeserializeObject<Exportaciones.ResultExportData>(responseExportaciones.Content);
                            bool validResponse = deserializedExportacion.ValidResponse;
                            if (validResponse == true)
                            {
                                NumeroExportacion = exportaciones.GetNumeroExportacion();
                                exportaciones.updateTable(NumeroExportacion);
                                message = deserializedExportacion.Message[0];
                                code = deserializedExportacion.Code;
                                exportaciones.InsertTable(validResponse.ToString(),message,code,NumeroExportacion,"SaveLandExportData");
                            }
                            else
                            {
                                NumeroExportacion = exportaciones.GetNumeroExportacion();
                                message = deserializedExportacion.Message[0];
                                code = deserializedExportacion.Code;
                                exportaciones.InsertTable(validResponse.ToString(), message, code, NumeroExportacion, "SaveLandExportData");
                            }
                            contador = contador + 1;
                            flag = true;
                        }
                        if (responseExportaciones.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            message = "salió mal";
                        }
                        if (responseExportaciones.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            message = "json vacio";
                            flag = false;
                        }
                    }
                }
            }
        }
    }
}
