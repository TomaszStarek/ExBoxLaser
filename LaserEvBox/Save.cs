using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


namespace LaserEvBox
{
    public class Save
    {

        public static async Task SendLogMesTisAsync(string serial, int listIndex, string NameOfStep)
        {
            var index = listIndex + 1;
            string indexString = index.ToString();
            if (index <= 9)
                indexString = "0" + indexString;

            DateTime stop = DateTime.Now;
            wsTis.MES_TISSoapClient ws = new wsTis.MES_TISSoapClient(wsTis.MES_TISSoapClient.EndpointConfiguration.MES_TISSoap);
          //  wsTis.MES_TISSoapClient ws = new wsTis.MES_TISSoapClient(wsTis.MES_TISSoapClient.EndpointConfiguration.MES_TISSoap);

            if (ws != null)
                {
                    try
                    {

                    StringBuilder sb = new StringBuilder();
                    sb.Append($"S{serial}\n");
                    sb.Append("CEVBOX\n");
                    sb.Append($"N{System.Environment.MachineName}_{NameOfStep.Substring(NameOfStep.Length - 2)}_s{indexString}\n");
                    sb.Append($"P{NameOfStep}_s{indexString}\n");
                    sb.Append("Ooperator\n");
                    sb.Append("TP\n");

                    sb.Append("[" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append("]" + stop.ToString("yyyy-MM-dd HH:mm:ss"));

                    var res = await ws.ProcessTestDataAsync(sb.ToString(), "Generic");

                        if (res != null && res.Body.ProcessTestDataResult.ToString().ToUpper() != "PASS")
                        {
                            SaveLog(serial, listIndex, NameOfStep);
                        }
                        else
                            SaveCopyLog(serial, listIndex, NameOfStep);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        SaveLog(serial, listIndex, NameOfStep);
                    }
                    finally
                    {
                        await ws.CloseAsync();

                    }

            }
                else
                    SaveLog(serial, listIndex, NameOfStep);

            


        }

        public static void SaveLog(string serial, int listIndex, string NameOfStep)
        {
            var index = listIndex + 1;
            string indexString = index.ToString();
            if (index <= 9)
                indexString = "0" + indexString;
            try
            {
                string sciezka = "C:/tars/";      //definiowanieścieżki do której zapisywane logi
                DateTime stop = DateTime.Now;
                if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy
                if (serial != null)
                    serial = Regex.Replace(serial, @"\s+", string.Empty);

                using (StreamWriter sw = new StreamWriter("C:/tars/" + serial + "_" + $"{listIndex+1}" + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year + " " + stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
                {

                    sw.WriteLine($"S{serial}");
                    sw.WriteLine("CEVBOX");
                    sw.WriteLine($"N{System.Environment.MachineName}_{NameOfStep.Substring(NameOfStep.Length - 2)}_s{indexString}");
                    sw.WriteLine($"P{NameOfStep}_s{indexString}");
                    sw.WriteLine("Ooperator");
                    sw.WriteLine("TP");

                    sw.WriteLine("[" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("]" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                    //for (int i = 0; i > 15; i++)
                    //    result[i] = string.Empty;

                }
                SaveCopyLog(serial, listIndex, NameOfStep);
                //string sourceFile = @"C:/tars/" + serial + "_" + $"{listIndex + 1}" + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".Tars";
                //string destinationFile = @"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/" + @serial + "_" + $"{listIndex + 1}" + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".Tars";

                //if (Directory.Exists(@"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"))       //sprawdzanie czy sciezka istnieje
                //{
                //    ;
                //}
                //else
                //    System.IO.Directory.CreateDirectory(@"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"); //jeśli nie to ją tworzy

                //File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message);
            }
        }

        public static void SaveCopyLog(string serial, int listIndex, string NameOfStep)
        {
            var index = listIndex + 1;
            string indexString = index.ToString();
            if (index <= 9)
                indexString = "0" + indexString;
            try
            {
                DateTime stop = DateTime.Now;
                string sciezka = @"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/";      //definiowanieścieżki do której zapisywane logi
                
                if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy

                if (serial != null)
                    serial = Regex.Replace(serial, @"\s+", string.Empty);

                using (StreamWriter sw = new StreamWriter(sciezka + serial + "_" + $"{listIndex + 1}" + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year + " " + stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
                {

                    sw.WriteLine($"S{serial}");
                    sw.WriteLine("CEVBOX");
                    sw.WriteLine($"N{System.Environment.MachineName}_{NameOfStep.Substring(NameOfStep.Length - 2)}_s{indexString}");
                    sw.WriteLine($"P{NameOfStep}_s{indexString}");
                    sw.WriteLine("Ooperator");
                    sw.WriteLine("TP");

                    sw.WriteLine("[" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("]" + stop.ToString("yyyy-MM-dd HH:mm:ss"));

                }

            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message);
            }
        }

        public static void SaveQC2(string serial)
        {
            try
            {
                string sciezka = "C:/tars/";      //definiowanieścieżki do której zapisywane logi
                DateTime stop = DateTime.Now.AddSeconds(1);
                if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy
                if (serial != null)
                    serial = Regex.Replace(serial, @"\s+", string.Empty);
                using (StreamWriter sw = new StreamWriter("C:/tars/" + serial + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year + " " + stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
                {

                    sw.WriteLine($"S{serial}");
                    sw.WriteLine("CEVBOX");
                    sw.WriteLine($"NEVBOX_QC2_21B-1");
                    sw.WriteLine($"PQC2");
                    sw.WriteLine("Ooperator");
                    sw.WriteLine("TP");
                    sw.WriteLine("[" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("]" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                string sourceFile = @"C:/tars/" + serial + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".Tars";
                string destinationFile = @"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/" + @serial + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".Tars";

                if (Directory.Exists(@"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(@"C:/copylogi/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"); //jeśli nie to ją tworzy

                File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message);
            }
        }


        public static bool SaveLaserFile(string serial)
        {
            try
            {
                DateTime stop = DateTime.Now;
                string sciezka = @"C:/laserFiles/" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/";      //definiowanieścieżki do której zapisywane logi


                if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy
                if (serial != null)
                    serial = Regex.Replace(serial, @"\s+", string.Empty);
                using (StreamWriter sw = new StreamWriter("C:/laserFiles/" + serial + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year + " " + stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".txt"))
                {
                    sw.WriteLine(Data.ArticleNumber);
                    sw.WriteLine(Data.SerialNumber);                   
                    sw.WriteLine(Data.StationId);
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine(Data.ProductionDate);
                    sw.WriteLine(Data.SerialNumber);
                }
                string sourceFile = @"C:/laserFiles/" + serial + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".txt";
                string destinationFile = @"\\192.168.1.11\c\test2\" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/" + @serial + @"-" + @"(" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @" " + @stop.Hour + @"-" + @stop.Minute + @"-" + @stop.Second + @")" + @".txt";

                if (Directory.Exists(@"\\192.168.1.11\c\test2\" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"))       //sprawdzanie czy sciezka istnieje
                {
                    ;
                }
                else
                    System.IO.Directory.CreateDirectory(@"\\192.168.1.11\c\test2\" + @stop.Day + @"-" + @stop.Month + @"-" + @stop.Year + @"/"); //jeśli nie to ją tworzy

                File.Copy(sourceFile, destinationFile, true);

                return true;
            }
            catch (IOException iox)
            {
                MessageBox.Show(iox.Message);
                return false;
            }
        }
    }
}

