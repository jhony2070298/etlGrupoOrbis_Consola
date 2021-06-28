using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Sql_A_Json
{
    class Exportaciones
    {
        public class ResultExportData
        {
            public bool ValidResponse { get; set; }
            public List<string> Message { get; set; }
            public int Code { get; set; }
            public string Response { get; set; }
        }

        public class ExportData
        {
            public string Nit { get; set; }
            public string NumeroExportacion { get; set; }
            public string FechaEstimadaLLegadaPuertoAeropuerto { get; set; }
            public string Observaciones { get; set; }
            public List<Container> Container { get; set; }
        }
        public class Container
        {
            public int Identificador { get; set; }
            public string NumeroContenedorVin { get; set; }
            public string Sello { get; set; }
            public string Peso { get; set; }
            public string Volumen { get; set; }
            public string CantidadPiezas { get; set; }
            public string Placa { get; set; }
            public string FechaSalidaBodega { get; set; }
            public string FechaLlegadaPuerto { get; set; }
        }
        public IRestResponse ApiExportaciones(string token,string session)
        {
            var json = QueryJson();
            var url = "https://pruebassoul.magnum.com.co/centralizer/api/LandExport/SaveLandExportData?security.token=" + session + "&security.lrp=2&security.tipoConsulta=2";

            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public string  QueryJson()
        {
            var queryWithForJson = "SELECT TOP 1 EO.Nit, EO.NumeroExportacion, EO.FechaEstLLegadaPuertoAeropuerto AS FechaEstimadaLLegadaPuertoAeropuerto," +
                                            "Contenedor.Identificador, Contenedor.ContenedorVin AS NumeroContenedorVin, Contenedor.Peso, Contenedor.Volumen,Contenedor.CantidadPiezas," +
                                            "PV.F_CARGUE_FIN_REAL AS FechaSalidaBodega, PV.F_DESCARGUE_INICIO_REAL AS FechaLlegadaPuerto," +
                                            "M.PlacaVehiculo AS Placa, DR.SellosCliente AS Sello" +
                                    " FROM[dbo].[ExportacionesOrbis] EO" +
                                    " INNER JOIN[dbo].[InfoExportacionesOrbis]Contenedor ON Contenedor.NumeroExportacion = EO.NumeroExportacion" +
                                    " INNER JOIN[dbo].[PedidosVenta] PV ON PV.PedidoCliente = EO.NumeroExportacion" +
                                    " INNER JOIN[dbo].[EntregasTransporte] ET ON ET.NumeroEntrega = PV.NumeroEntregaActual" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[Manifiestos] M on M.NumeroManifiesto = ET.NumeroTransporte" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[Remesas] R ON R.NumeroManifiesto = M.NumeroManifiesto" +
                                        " AND R.CodigoCliente COLLATE SQL_Latin1_General_CP1_CI_AS = PV.CodigoCliente COLLATE SQL_Latin1_General_CP1_CI_AS" +
                                        " AND R.IdentificacionCliente COLLATE SQL_Latin1_General_CP1_CI_AS = PV.NitCliente COLLATE SQL_Latin1_General_CP1_CI_AS" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[DetallesRemesas] DR ON DR.NumeroRemesa = R.NumeroRemesa" +
                                    " WHERE EO.SINCRONIZADO = 'P'"+
                                    " FOR JSON AUTO";
            using (var conn = new SqlConnection("Server=172.28.4.27;Initial Catalog = IndicadoresServicioDesarrollo;User Id=trafico;Password=trafico;"))
            {
                using (var cmd = new SqlCommand(queryWithForJson, conn))
                {
                    conn.Open();
                    var jsonResult = new StringBuilder();
                    var reader = cmd.ExecuteReader();
                    var dataQuery = new StringBuilder();
                    string json;
                    if (!reader.HasRows)
                    {
                        jsonResult.Append("[]");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            dataQuery = jsonResult.Append(reader.GetValue(0).ToString());
                        }
                    }
                    string  jsonString = dataQuery.ToString().Replace("\"PV\":[{", "").Replace("\"M\":[{", "").Replace("\"DR\":[{", "").Replace("}]}]}]}]", "").Replace("\"Contenedor\":[{", "");
                    if (jsonString != "")
                    {
                        List<ExportData> listExport = JsonConvert.DeserializeObject<List<ExportData>>(jsonString);
                        List<Container> listContainer = JsonConvert.DeserializeObject<List<Container>>(jsonString);
                        Container containerData = new Container();
                        ExportData exportData = new ExportData();
                        foreach (Container container in listContainer)
                        {
                            DateTime fechaSalidaBodega = Convert.ToDateTime(container.FechaSalidaBodega);
                            DateTime fechaLlegadaPuerto = Convert.ToDateTime(container.FechaLlegadaPuerto);
                            var myContainer = new Container
                            {
                                Identificador = container.Identificador,
                                NumeroContenedorVin = container.NumeroContenedorVin,
                                Peso = container.Peso,
                                Volumen = container.Volumen,
                                CantidadPiezas = container.CantidadPiezas,
                                FechaSalidaBodega = fechaSalidaBodega.ToString("d"),
                                FechaLlegadaPuerto = fechaLlegadaPuerto.ToString("d"),
                                Placa = container.Placa,
                                Sello = container.Sello
                            };
                            containerData = myContainer;
                        }
                        foreach (ExportData export in listExport)
                        {
                            if (export.Observaciones == null)
                            {
                                export.Observaciones = "";
                            }
                            if (export.FechaEstimadaLLegadaPuertoAeropuerto == null)
                            {
                                export.FechaEstimadaLLegadaPuertoAeropuerto = containerData.FechaLlegadaPuerto;
                            }

                            var myExport = new ExportData
                            {
                                Nit = export.Nit,
                                NumeroExportacion = export.NumeroExportacion,
                                FechaEstimadaLLegadaPuertoAeropuerto = export.FechaEstimadaLLegadaPuertoAeropuerto,
                                Observaciones = export.Observaciones,
                                Container = new List<Container>
                                {
                                    new Container
                                    {
                                        Identificador = containerData.Identificador,
                                        NumeroContenedorVin= containerData.NumeroContenedorVin,
                                        Sello = containerData.Sello,
                                        Peso = containerData.Peso,
                                        Volumen = containerData.Volumen,
                                        CantidadPiezas = containerData.CantidadPiezas,
                                        Placa = containerData.Placa,
                                        FechaSalidaBodega = containerData.FechaSalidaBodega,
                                        FechaLlegadaPuerto = containerData.FechaLlegadaPuerto
                                    }
                                }
                            };
                            exportData = myExport; 
                        }
                        json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
                    }   
                    else
                    {
                        json = "";
                    }
                    conn.Close();
                    return json;
                }
            }
        }
        public void updateTable(string NumeroExportacion)
        {
            DateTime FechaSincronizacion = DateTime.Now;
            string Sincronizado = "S";
            using (SqlConnection conn = new SqlConnection("Server=172.28.4.27;Initial Catalog = IndicadoresServicioDesarrollo;User Id=trafico;Password=trafico;"))
            {
                conn.Open();
                string querystr = "UPDATE [dbo].[ExportacionesOrbis] SET [Sincronizado]=@p_Sincronizado, [FechaSincronizacion]=@p_FechaSincronizacion WHERE [NumeroExportacion]=@p_NumeroExportacion";
                using (var cmd = new SqlCommand(querystr, conn))
                {
                    cmd.Parameters.AddWithValue("@p_Sincronizado", Sincronizado);
                    cmd.Parameters.AddWithValue("@p_NumeroExportacion", NumeroExportacion);
                    cmd.Parameters.AddWithValue("@p_FechaSincronizacion", FechaSincronizacion);
                    int ret = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public string GetNumeroExportacion()
        {
            SqlConnection conn = new SqlConnection("Server=172.28.4.27;Initial Catalog = IndicadoresServicioDesarrollo;User Id=trafico;Password=trafico;");
            conn.Open();
            string query = "SELECT TOP 1  EO.NumeroExportacion" +     
                                    " FROM [dbo].[ExportacionesOrbis] EO" +
                                    " INNER JOIN[dbo].[InfoExportacionesOrbis]Contenedor ON Contenedor.NumeroExportacion = EO.NumeroExportacion" +
                                    " INNER JOIN[dbo].[PedidosVenta] PV ON PV.PedidoCliente = EO.NumeroExportacion" +
                                    " INNER JOIN[dbo].[EntregasTransporte] ET ON ET.NumeroEntrega = PV.NumeroEntregaActual" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[Manifiestos] M on M.NumeroManifiesto = ET.NumeroTransporte" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[Remesas] R ON R.NumeroManifiesto = M.NumeroManifiesto" +
                                        " AND R.CodigoCliente COLLATE SQL_Latin1_General_CP1_CI_AS = PV.CodigoCliente COLLATE SQL_Latin1_General_CP1_CI_AS" +
                                        " AND R.IdentificacionCliente COLLATE SQL_Latin1_General_CP1_CI_AS = PV.NitCliente COLLATE SQL_Latin1_General_CP1_CI_AS" +
                                    " INNER JOIN[TraficoVehicularDesarrollo].[dbo].[DetallesRemesas] DR ON DR.NumeroRemesa = R.NumeroRemesa" +
                                    " WHERE EO.SINCRONIZADO = 'P'";
            SqlCommand command = new SqlCommand(query, conn);
            var jsonResult = new StringBuilder();
            var dataQuery = new StringBuilder();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    dataQuery = jsonResult.Append(reader.GetValue(0).ToString());      
                }
            }
            string numeroExportacion = dataQuery.ToString().Replace("{", "").Replace("}", "");
            conn.Close();
            
            return numeroExportacion;
        }
        public void InsertTable(string responseValid,string message,int code,string numeroExportacion,string nombreServicio)
        {
            using (SqlConnection conn = new SqlConnection("Server=172.28.4.27;Initial Catalog = IndicadoresServicioDesarrollo;User Id=trafico;Password=trafico;"))
            {  
                string querystr = "INSERT INTO [dbo].[RespuestaServiciosOrbis] (RespuestaValida, Mensaje, Codigo, NumeroExportacion, NombreServicioWeb)" +
                                    " VALUES (@RespuestaValida, @Mensaje, @Codigo, @NumeroExportacion, @NombreServicioWeb)";
                using (var cmd = new SqlCommand(querystr, conn))
                {
                    cmd.Parameters.AddWithValue("@RespuestaValida", responseValid);
                    cmd.Parameters.AddWithValue("@Mensaje", message);
                    cmd.Parameters.AddWithValue("@Codigo", code);
                    cmd.Parameters.AddWithValue("@NumeroExportacion", numeroExportacion);
                    cmd.Parameters.AddWithValue("@NombreServicioWeb", nombreServicio);
                    try
                    {
                        conn.Open();
                        int recordsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        // error here
                    }
                    finally
                    {
                        conn.Close();
                    }
                }             
            }
        }
    }
}