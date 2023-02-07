using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserEvBox
{
    class Data
    {
        public static string SerialNumber { get; set; } = "";
        public static string ArticleNumber { get; set; } = "";
        public static string StationId { get; set; } = "";
        public static string ProductionDate { get; set; } = "";


        public static bool DefineProductionDate()
        {
            try
            {
                var dateNow = DateTime.Now;

                var year = dateNow.Year;
              //  var month = dateNow.Month;
                var day = dateNow.Day;

                string monthName = dateNow.ToString("MMM", CultureInfo.InvariantCulture).ToLower();

                ProductionDate = $"{day} {monthName} {year}";

                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }

    }
}
