using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Sql_A_Json
{
    class Pruebas
    {
        public void Hola()
        {
            string[] info = { "Name: Felica Walker", "Title: Mz.",
                                      "Age: 47", "Location: Paris", "Gender: F"};
            int found = 0;

            foreach (string s in info)
            {
                found = s.IndexOf(": ");
                Console.WriteLine("   {0}", s.Substring(found + 2));
            }

            //jsonString.Split("'");
            char[] delimiterChars = { ',' };
            string sss = "'ggg' : 'gfgfg', 'fff':'257', 'fff': '258', 'name': 'alex'";

            string[] a = sss.Split(delimiterChars);
            string[] b = { sss };
            //int gg = sss.IndexOf("adioas");
            string cadenaNueva = "";
            //foreach (string s in a)
            //{
            //    if (a[1]== s)
            //    {
            //         string nn = s.Substring(4, s.Length).Replace("'","");
            //        cadenaNueva +=nn;
            //    }
            //    else
            //        cadenaNueva += s +",";
            //    //int gg = sss.IndexOf("fff");
            //}
            //string neo = sss.Remove(gg + 9, 1);
            //string neo2 = neo.Remove(gg + 12, 1);
            //string ff = sss.Substring(17, 8).Replace("'", "");

            Console.WriteLine(cadenaNueva);

            var personas = new ArrayList();
            personas.Add(new Persona {Identificador=1, Name = "Juan", Email = "a@gmail.com" });
            personas.Add(new Persona {Identificador = 2, Name = "JoseManuel", Email = "c@hotmail.com" });

            string json = JsonConvert.SerializeObject(personas);

            JArray textArray = JArray.Parse(json);

            //JObject datosParaModificarEnElJson = JObject.Parse(json);
            //datosParaModificarEnElJson["TransaccionId"] = 123456;
            //JObject objeto = (JObject)datosParaModificarEnElJson["datosParaModificarEnElJson"];
            //objeto.Remove("AlgunosDatosParaEliminar");
            //json = datosParaModificarEnElJson.ToString();
        }
        class Persona
        {
            public int Identificador { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
          

        }
        public void otro()
        {

        }
    }
    
}
