using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Sql_A_Json
{
    class DataApi
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime timeStamp { get; set; }
        public int duration { get; set; }
        public int @event { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public object driverName { get; set; }
        public object driverIdentification { get; set; }
        public string machineName { get; set; }
        public string value { get; set; }
    }
}
