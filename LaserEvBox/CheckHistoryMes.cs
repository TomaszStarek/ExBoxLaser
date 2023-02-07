using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaserEvBox
{
    public static class CheckHistoryMes
    {
        private enum MesResult
        {
            Pass,
            STEP_MISS,
            FAIL,
            CONNECTION_FAIL
        }

        public static string CheckSerialNumberByCheckpointEPS(string SerialTxt, string client, string step)
        {
            using (MESwebservice.BoardsSoapClient wsMES = new MESwebservice.BoardsSoapClient(MESwebservice.BoardsSoapClient.EndpointConfiguration.BoardsSoap12))
            {
                try
                {
                    return wsMES.CheckSerialNumberByCheckpointEPS(@client, @step, SerialTxt);
                }
                catch
                {
                    return "Błąd połączenia";
                }
               
            }
        }

        public static bool GetBoardData(string SerialTxt, string client)
        {
            using (MESwebservice.BoardsSoapClient wsMES = new MESwebservice.BoardsSoapClient(MESwebservice.BoardsSoapClient.EndpointConfiguration.BoardsSoap12))
            {
                try
                {
                    var boardData = wsMES.GetBoardData(@client, SerialTxt);

                    boardData[1] = boardData[1].Remove(0, 7);

                    Data.ArticleNumber = boardData[1];
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool GetMeasuredData(string SerialTxt, string client)
        {
            using (MESwebservice.BoardsSoapClient wsMES = new MESwebservice.BoardsSoapClient(MESwebservice.BoardsSoapClient.EndpointConfiguration.BoardsSoap12))
            {
                try
                {
                    var boardData = wsMES.GetMeasuredData(@client, SerialTxt, "MES_StationID", "");

                    boardData[0] = boardData[0].Remove(0, 14);

                    Data.StationId = boardData[0];
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        //public static int CheckTestResultFVT(string SerialTxt)
        //{
        //    using (wsTis.MES_TISSoapClient ws = new wsTis.MES_TISSoapClient(wsTis.MES_TISSoapClient.EndpointConfiguration.MES_TISSoap))
        //    {
        //        try
        //        {
        //            var res = ws.GetLastTestResult(SerialTxt, @"TRILLIANT", "", "FVT");

        //            if (res.Contains("<TestStatus>P</TestStatus>"))
        //                return (int)MesResult.Pass;
        //            else if (res.Contains("<TestStatus>F</TestStatus>"))
        //                return (int)MesResult.FAIL;
        //            else
        //                return (int)MesResult.STEP_MISS;
        //        }
        //        catch (Exception ex)
        //        {
        //            return (int)MesResult.CONNECTION_FAIL;
        //        }

        //    }
        //}

        public static double CheckHistory(string SerialTxt)
        {
            using (wsTis.MES_TISSoapClient ws = new wsTis.MES_TISSoapClient(wsTis.MES_TISSoapClient.EndpointConfiguration.MES_TISSoap))
            {
                try
                {
                    var res = ws.GetTestHistory(SerialTxt, "evbox","");

                    var resd = res.Split('\n');

                    double minutesDiff = 0;

                    foreach (string line in resd)
                    {
                        if (line.Contains("<StartTime>"))
                        {
                            try
                            {
                                DateTime ts = DateTime.Parse(line.Substring(15, 25));
                                minutesDiff = (DateTime.Now - ts).TotalMinutes;

                                //   MessageBox.Show($"{line} --> {ts.ToString("c")}");

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("{0}: Bad Format", line);
                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("{0}: Overflow", line);
                            }
                        }
                    }


                    return minutesDiff;



                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }








        public static double CheckTackTime(string SerialTxt)
        {
            using (wsTis.MES_TISSoapClient ws = new wsTis.MES_TISSoapClient(wsTis.MES_TISSoapClient.EndpointConfiguration.MES_TISSoap))
            {
                try
                {
                    var res = ws.GetLastTestResult(SerialTxt, @"evbox", "", "DISPLAY_ASSEMBLY");

                    var resd = res.Split('\n');

                    double minutesDiff = 0;

                    foreach (string line in resd)
                    {
                        if (line.Contains("<StartTime>"))
                        {
                            try
                            {
                                DateTime ts = DateTime.Parse(line.Substring(15, 25));
                                minutesDiff = (DateTime.Now - ts).TotalMinutes;

                             //   MessageBox.Show($"{line} --> {ts.ToString("c")}");

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("{0}: Bad Format", line);
                            }
                            catch (OverflowException)
                            {
                                MessageBox.Show("{0}: Overflow", line);
                            }
                        }
                    }


                        return minutesDiff;



                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

    }
}
