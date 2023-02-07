using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace LaserEvBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _this = this;
             

            NetworkCredential myCred = new NetworkCredential(
                "admin", "solferino5", "");



            //var ujh =    Process.Start("CMDKEY", @"/add:""\\192.168.1.11"" /user:""admin"" /pass:""solferino5""");
            //ujh.WaitForExit();
            //watch();

            var directory = @"\192.168.1.11\c\ ";
            var username = "admin ";
            var password = "solferino5";

            //      net use f: \\DEVSRV\c$ / u:domain\\test_user passwordhere

            var command = "net use \\\\192.168.1.11\\c /u:admin solferino5 /persistent:yes";
                ExecuteCommand(command, 5000);

            //using (new NetworkConnection("\\\\192.168.1.11\\", myCred))
            //{
            //    watch();
            //}                net use \\192.168.1.11\c /u:admin solferino5 /persistent:yes
            watch();
        }

        public static int ExecuteCommand(string command, int timeout)
        {
            try
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = "C:\\",
                };

                var process = Process.Start(processInfo);
                process.WaitForExit(timeout);
                var exitCode = process.ExitCode;
                process.Close();
                return exitCode;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"Brak połączenia z komputerem od lasera!");
                return 6669;
            }

        }

        FileSystemWatcher watcher;
        private static MainWindow _this;

        private static void MoveFiles(string sourceDir, string targetDir)
        {
            IEnumerable<FileInfo> files = Directory.GetFiles(sourceDir).Select(f => new FileInfo(f));
            foreach (var file in files)
            {
                File.Move(file.FullName, System.IO.Path.Combine(targetDir, file.Name));
            }
        }


        private void watch()
        {

            watcher = new FileSystemWatcher(@"\\192.168.1.11\c\test\");
            //watcher = new FileSystemWatcher();
            //watcher.Path = "C:\\filewatcher";
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            //                   //    | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //watcher.Filter = "*.*";
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.EnableRaisingEvents = true;

            

            //* Assign event handler. 
        //    watcher.Changed += new FileSystemEventHandler(watcher_Created);
            watcher.Created += OnCreated;

            //* Start watching. 
            watcher.EnableRaisingEvents = true;
        }
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (Stream stream = new FileStream(e.FullPath, FileMode.Open))
                {
                    // File/Stream manipulating code here
                }
            }
            catch
            {
                Thread.Sleep(500);
                //check here why it failed and ask user to retry if the file is in use.
            }
            try
            {
                    MoveFiles(@"\\192.168.1.11\c\test", @"C:\tars\");
                //           File.Move(e.FullPath, @"D:\watcher\" + e.Name);
                if(Data.SerialNumber.Length > 1)
                    MainWindow._this.PartDone(e.Name);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //* Something went wrong. You can do additional proceesing here, like fire-up new thread for retry move procedure.
            }
        }

        private void PartDone(string? sn)
        {
            Dispatcher.Invoke(new Action(() => labelStatusInfo.Content = $"Proces zakończony pomyślnie!\n{sn}"));
            Dispatcher.Invoke(new Action(() => labelStatusInfo.Background = System.Windows.Media.Brushes.LawnGreen));
            Dispatcher.Invoke(new Action(() => textBox.Text = String.Empty));
            Dispatcher.Invoke(new Action(() => textBox.IsEnabled = true));
        }


        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            
            TextBox textBox = (TextBox)sender;
            if (e.Key == Key.Return)
            {
                //  if (CheckHistoryMes.CheckSerialNumberByCheckpointEPS(textBox.Text, "evbox", CheckpointToCheck).ToUpper().Equals("TRUE"))
                if (textBox.Text.Length > 5)
                {
                    
                    textBox.Text = Regex.Replace(textBox.Text, @"\s+", string.Empty);
                    Data.SerialNumber = textBox.Text;


                    if(Data.DefineProductionDate() && CheckHistoryMes.GetBoardData(Data.SerialNumber, "evbox") && CheckHistoryMes.GetMeasuredData(Data.SerialNumber, "evbox"))
                    {
                        if (Save.SaveLaserFile(Data.SerialNumber))
                        {
                            Dispatcher.Invoke(new Action(() => labelStatusInfo.Content = $"Stworzono plik z danymi\nCzekam na plik od lasera!"));
                            Dispatcher.Invoke(new Action(() => labelStatusInfo.Background = System.Windows.Media.Brushes.Yellow));
                            Dispatcher.Invoke(new Action(() => textBox.IsEnabled = false));
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() => labelStatusInfo.Content = "Błąd tworzenia pliku!"));
                            Dispatcher.Invoke(new Action(() => labelStatusInfo.Background = System.Windows.Media.Brushes.Orange));
                            Dispatcher.Invoke(new Action(() => textBox.Text = String.Empty));
                        }

                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => labelStatusInfo.Content = "Nie udało się pobrać danych z mes!"));
                        Dispatcher.Invoke(new Action(() => labelStatusInfo.Background = System.Windows.Media.Brushes.DarkMagenta));
                        Dispatcher.Invoke(new Action(() => textBox.Text = String.Empty));
                    }

                }
                else
                {
                    Dispatcher.Invoke(new Action(() => labelStatusInfo.Content = "Podaj poprawny barkod!"));
                    Dispatcher.Invoke(new Action(() => labelStatusInfo.Background = System.Windows.Media.Brushes.IndianRed));
                    Dispatcher.Invoke(new Action(() => textBox.Text = String.Empty));
                }
            }
        }

        //private void OnChanged(object source, FileSystemEventArgs e)
        //{
        //    try
        //    {
        //        //Provider Source Folder Path
        //        string SourceFolder = @"C:\filewatcher";
        //        //Provide Destination Folder path
        //        string DestinationFolder = @"D:\watcher\";

        //        var files = new DirectoryInfo(SourceFolder).GetFiles("*.*");

        //        //Loop throught files and Move to destination folder
        //        foreach (FileInfo file in files)
        //        {

        //            file.MoveTo(DestinationFolder + file.Name);

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        MessageBox.Show("");
        //    }
        //    //Copies file to another directory.
        //}
    }
}
