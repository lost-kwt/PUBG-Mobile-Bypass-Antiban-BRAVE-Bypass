using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using KeyAuth;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace BraveBypass
{
    public partial class bypass1 : Form
    {
        private const string MutexIdentifier = "YourApplicationMutex";
        private readonly string[] reversingToolProcessNames = { "OllyDbg", "ida_32bit", "ida_64bit", "cheatengine-x86_64-SSE4-AVX2.exe", "Wireshark" };
        private bool dragging = false;
        private Point startPoint = new Point(0, 0);

        public static api KeyAuthApp = new api(
        name: "BGMI BYPASS",
        ownerid: "5J0eFrdWq9",
        secret: "00caa6078fd967bea73eb81c3cc2bae408b800956e8d23e3b0b2921bc9aa754b",
        version: "1.0" );

        private Timer reversingToolCheckTimer;

        public bypass1()
        {
            InitializeComponent();

            reversingToolCheckTimer = new Timer();
            reversingToolCheckTimer.Interval = 5000; // Check every 5 seconds 
            reversingToolCheckTimer.Tick += ReversingToolCheckTimer_Tick;
            reversingToolCheckTimer.Start();

            if (DetectReversingTools())
            {
                Environment.Exit(0);
            }
        }

        private void ReversingToolCheckTimer_Tick(object sender, EventArgs e)
        {
            if (DetectReversingTools())
            {
                Environment.Exit(0);
            }
        }

        private bool DetectReversingTools()
        {
            foreach (var processName in reversingToolProcessNames)
            {
                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void bypass1_Load(object sender, EventArgs e)
        {
            this.MouseDown += FormMouseDown;
            this.MouseMove += FormMouseMove;
            this.MouseUp += FormMouseUp;
            KeyAuthApp.init();
            CheckBraveKey();
        }

        private void CheckBraveKey()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Brave.txt");

            try
            {
                if (File.Exists(filePath))
                {
                    string savedKey = File.ReadAllText(filePath);

                    Task.Run(() => VerifyKey(savedKey));
                }
                else
                {
                    Application.Exit();
                }
            }
            catch
            {
                Application.Exit();
            }
        }

        private void VerifyKey(string key)
        {
            try
            {
                Task.Run(() => KeyAuthApp.license(key));

                if (KeyAuthApp.response.success)
                {
                    Invoke(new Action(() => { this.Show(); }));
                    /*statusv.Text = "Welcome to Brave Bypass";*/
                }
                else
                {
                    Application.Exit();
                }
            }
            catch
            {
                Application.Exit();
            }
        }

        private void FormMouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
        }

        private void FormMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - startPoint.X, p.Y - startPoint.Y);
            }
        }

        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            GameLibFunction("Global", "libmarsxlog.so");
        }

        private void SaveFile(string filePath, byte[] content)
        {
            try
            {
                File.WriteAllBytes(filePath, content);
                //Console.WriteLine($"File saved to: {filePath}");
            }
            catch
            {
                statusv.Text = "Error";
                throw; 
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            GameLibFunction("TWN", "libmarsxlog.so");
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            GameLibFunction("BGMI", "libBrave++.so"); // your lib 
            GameLibFunction("BGMI", "libhdmpve.so"); // loader lib
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            GameLibFunction("VNG", "libmarsxlog.so");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await AnimateTextChange("Starting Emulator...", statusv);
            await Task.Delay(1000);

            await Task.Run(() => StartAndroidEmulator());
        }

        private async Task AnimateTextChange(string newText, Label label)
        {
            for (int i = 0; i < newText.Length; i++)
            {
                label.Text = newText.Substring(0, i + 1);
                await Task.Delay(100);
            }
        }

        private void StartAndroidEmulator()
        {
            string registryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Tencent\MobileGamePC\UI";
            string valueName = "InstallPath";
            string defaultPath = "";

            string start = Registry.GetValue(registryKey, valueName, defaultPath)?.ToString();

            if (!string.IsNullOrEmpty(start))
            {
                string executablePath = Path.Combine(start, "AndroidEmulatorEx.exe");

                if (!IsProcessRunning("AndroidEmulatorEx"))
                {
                    Process.Start(executablePath, "-vm 100");
                }
                else
                {
                    Invoke((MethodInvoker)(() => statusv.Text = "Emulator is Already Running..."));
                    statusv.Refresh();
                }
            }
            else
            {
                Console.WriteLine("Error Contact Seller!");
            }
        }

        static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                 Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/Brave.lic"));
                 Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/libmarsxlog.so"));
             //   Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/libmarsxlog.so")); // add for bgmi
             //   Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/libmarsxlog.so")); // add for bgmi
                string filePath = Path.Combine("C:\\", "libmarsxlog.so");
            //    string filePath1 = Path.Combine("C:\\", "libmarsxlog.so"); // add for bgmi too

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                KillProcessesByName("AppMarket");
                KillProcessesByName("AppMarket.exe");
                KillProcessesByName("AndroidEmulatorEx.exe");
                KillProcessesByName("AndroidEmulatorEx");
                KillProcessesByName("AndroidEmulatorEn.exe");
                KillProcessesByName("AndroidEmulatorEn");
                KillProcessesByName("appmarket.exe");
                KillProcessesByName("appmarket");
                KillProcessesByName("androidemulator");
                KillProcessesByName("androidemulator.exe");
                KillProcessesByName("aow_exe.exe");
                KillProcessesByName("QMEmulatorService.exe");
                KillProcessesByName("RuntimeBroker.exe");
                KillProcessesByName("adb.exe");
                KillProcessesByName("GameLoader.exe");
                KillProcessesByName("TSettingCenter.exe");
                KillProcessesByName("syzs_dl_svr.exe");

                statusv.Text = "SAFE EXIT DONE!";
            }
            catch (Exception ex)
            {
                statusv.Text = $"SAFE EXIT FAILED: {ex.Message}";
            }
        }

        private void KillProcessesByName(string processName)
        {
            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit(); 
                }
                catch
                {
                 //   Console.WriteLine($"Failed to kill process {processName}: {ex.Message}");
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
            Task.Delay(100).Wait();
            string filePath = Path.Combine("C:\\", "libmarsxlog.so");


            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
                Application.Exit();
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            button2.Enabled = false;
           

            if (radioButton1.Checked)  // GL
            {
                
                await AnimateTextChange("Starting Global!...", statusv);
                await Task.Run(() => CommandLine("adb kill-server"));
                await Task.Run(() => CommandLine("adb start-server"));
                await Task.Run(() => CommandLine("adb shell am kill com.tencent.ig"));
                await Task.Run(() => CommandLine("adb shell am force-stop com.tencent.ig"));


                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/.backups"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/BGMI"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/MidasOversea"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.json"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/LobbyBubble"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Lobby"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Login"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.sav"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashrecord"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashSight"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_databases"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_flutter"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_geolocation"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_textures"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_webview"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/code_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/files/*"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/no_backup"));
                await Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/Brave.lic"));


                await PushFileToEmulatorSystem("C:\\libmarsxlog.so", "data/app/com.tencent.ig-1/lib/arm/");

                await Task.Run(() => CommandLine("adb shell am start -n com.tencent.ig/com.epicgames.ue4.SplashActivity filter"));

                Task.Delay(2000).Wait();


                string filePath = Path.Combine("C:\\", "libmarsxlog.so");


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await AnimateTextChange("Bypass Done! Enjoy Global...", statusv);
                    
                }
                button2.Enabled = true;

            }
            else if (radioButton2.Checked) // TWN
            {
                await AnimateTextChange("Starting Taiwan!...", statusv);

                await Task.Run(() => CommandLine("adb kill-server"));
                await Task.Run(() => CommandLine("adb start-server"));
                await Task.Run(() => CommandLine("adb shell am kill com.tencent.ig"));
                await Task.Run(() => CommandLine("adb shell am force-stop com.tencent.ig"));


                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/.backups"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/BGMI"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/MidasOversea"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.json"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/LobbyBubble"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Lobby"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Login"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.sav"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashrecord"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashSight"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_databases"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_flutter"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_geolocation"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_textures"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_webview"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/code_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/files/*"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/no_backup"));
                await Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/Brave.lic"));


                await PushFileToEmulatorSystem("C:\\libmarsxlog.so", "data/app/com.tencent.ig-1/lib/arm/");

                await Task.Run(() => CommandLine("adb shell am start -n com.tencent.ig/com.epicgames.ue4.SplashActivity filter"));

                Task.Delay(2000).Wait();


                string filePath = Path.Combine("C:\\", "libmarsxlog.so");


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await AnimateTextChange("Bypass Done! Enjoy Taiwan...", statusv);
                   
                }

                button2.Enabled = true;
            }
            else if (radioButton3.Checked) // BGMI
            {

                await AnimateTextChange("Starting BGMI!...", statusv);
                
                await Task.Run(() => CommandLine("adb kill-server"));
                await Task.Run(() => CommandLine("adb start-server"));
                await Task.Run(() => CommandLine("adb shell am kill com.pubg.imobile"));
                await Task.Run(() => CommandLine("adb shell am force-stop com.pubg.imobile"));

                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/.backups"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/BGMI"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/MidasOversea"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.pubg.imobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.json"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.pubg.imobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/LobbyBubble"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.pubg.imobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Lobby"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.pubg.imobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Login"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.pubg.imobile/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.sav"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/datacom.pubg.imobile/app_crashrecord"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_crashSight"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_databases"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_flutter"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_geolocation"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_textures"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/app_webview"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/code_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/files/*"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.pubg.imobile/no_backup"));
                await Task.Run(() => CommandLine("adb shell rm -rf data/app/com.pubg.imobile-1/lib/arm/Brave.lic"));


                await PushFileToEmulatorSystem("C:\\libmarsxlog.so", "data/app/com.pubg.imobile-1/lib/arm/");

                await Task.Run(() => CommandLine("adb shell am start -n com.pubg.imobile/com.epicgames.ue4.SplashActivity filter"));

                Task.Delay(2000).Wait();


                string filePath = Path.Combine("C:\\", "libmarsxlog.so"); // add bgmi lib


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await AnimateTextChange("Bypass Done! Enjoy BGMI...", statusv);
                    
                }
                button2.Enabled = true;

            } 
            else if (radioButton4.Checked) // VNG
            {

                await AnimateTextChange("Starting VNG!...", statusv);
               
                await Task.Run(() => CommandLine("adb kill-server"));
                await Task.Run(() => CommandLine("adb start-server"));
                await Task.Run(() => CommandLine("adb shell am kill com.tencent.ig"));
                await Task.Run(() => CommandLine("adb shell am force-stop com.tencent.ig"));

                // package name will be different 

                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/.backups"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/BGMI"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/MidasOversea"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.json"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/LobbyBubble"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Lobby"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Login"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.sav"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashrecord"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashSight"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_databases"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_flutter"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_geolocation"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_textures"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_webview"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/code_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/files/*"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/no_backup"));
                await Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/Brave.lic"));


                await PushFileToEmulatorSystem("C:\\libmarsxlog.so", "data/app/com.tencent.ig-1/lib/arm/");

                await Task.Run(() => CommandLine("adb shell am start -n com.tencent.ig/com.epicgames.ue4.SplashActivity filter"));

                Task.Delay(2000).Wait();


                string filePath = Path.Combine("C:\\", "libmarsxlog.so");


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await AnimateTextChange("Bypass Done! Enjoy VNG...", statusv);
                   
                }

                button2.Enabled = true;
            }

            else if (radioButton5.Checked) // kr
            {
                await AnimateTextChange("Starting KR!...", statusv);
               
                // package name will be different 

                await Task.Run(() => CommandLine("adb kill-server"));
                await Task.Run(() => CommandLine("adb start-server"));
                await Task.Run(() => CommandLine("adb shell am kill com.tencent.ig"));
                await Task.Run(() => CommandLine("adb shell am force-stop com.tencent.ig"));


                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/.backups"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/BGMI"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/media/0/MidasOversea"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.json"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/LobbyBubble"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Lobby"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/Login"));
                await Task.Run(() => CommandLine("adb shell rm -rf /storage/emulated/0/Android/data/com.tencent.ig/files/UE4Game/ShadowTrackerExtra/ShadowTrackerExtra/Saved/SaveGames/*.sav"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashrecord"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_crashSight"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_databases"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_flutter"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_geolocation"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_textures"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/app_webview"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/code_cache"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/files/*"));
                await Task.Run(() => CommandLine("adb shell rm -rf /data/data/com.tencent.ig/no_backup"));
                await Task.Run(() => CommandLine("adb shell rm -rf data/app/com.tencent.ig-1/lib/arm/Brave.lic"));


                await PushFileToEmulatorSystem("C:\\libmarsxlog.so", "data/app/com.tencent.ig-1/lib/arm/");

                await Task.Run(() => CommandLine("adb shell am start -n com.tencent.ig/com.epicgames.ue4.SplashActivity filter"));

                Task.Delay(2000).Wait();


                string filePath = Path.Combine("C:\\", "libmarsxlog.so");


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await AnimateTextChange("Bypass Done! Enjoy KR...", statusv);
                   
                }

                button2.Enabled = true;

            }

        }
        
        private async Task PushFileToEmulatorSystem(string localFilePath, string targetDirectory)
        {
            await Task.Run(() =>
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Brave.lic");
                if (File.Exists(localFilePath))
                {
                    CommandLine($"adb push \"{localFilePath}\" \"{targetDirectory}\"");
                    CommandLine($"adb push \"{filePath}\" \"{targetDirectory}\"");
                }
                else
                {
                    statusv.Text = "Error Contact Seller";
                }
            });
        }

        private void CommandLine(string arg)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe"),
                Arguments = $"/c {arg}"
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private async void button4_Click(object sender, EventArgs e) // reset guest
        {
            string filePath = Path.Combine("C:\\", "greset.bat");

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"Existing file deleted: {filePath}");
                }

                byte[] result1 = await Task.Run(() => KeyAuthApp.download("153682")); // greset.bat

                await Task.Run(() => SaveFile(filePath, result1));

                await Task.Run(() => statusv.Text = "Resetting Guest...");

                await Task.Run(() => RunFile(filePath));

              
                await Task.Run(() => statusv.Text = "Guest reset done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                
                await Task.Run(() => statusv.Text = $"Error: {ex.Message}");
            }
        }


        private void RunFile(string filePath)
        {
            try
            {
                // Start the process to run the saved file
                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running file: {ex.Message}");
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            GameLibFunction("KR", "libmarsxlog.so");
        }

        private async void GameLibFunction(string selection, string libraryName)
        {
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                radioButton4.Enabled = false;
                radioButton5.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;

                try
                {
                    await AnimateTextChange($"You Selected {selection}...", statusv);
                    

                    string filePath = Path.Combine("C:\\", libraryName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    if (selection == "BGMI")
                    {
                        byte[] result1 = await Task.Run(() => KeyAuthApp.download("685552")); // for bgmi
                        byte[] result2 = await Task.Run(() => KeyAuthApp.download("685552")); // for bgmi
                        await Task.Run(() => SaveFile(filePath, result1));
                        await Task.Run(() => SaveFile(filePath, result2));
                        await AnimateTextChange("Start Game Now...", statusv);
                    }

                    else // Non-BGMI versions ! ....
                    {
                        byte[] result = await Task.Run(() => KeyAuthApp.download("685552"));
                        await AnimateTextChange("Start Game Now...", statusv);
                    }
                }
                catch (Exception ex)
                {
                    statusv.Text = $"Error Contact Seller: {ex.Message}";
                }
                finally
                {
                    radioButton1.Enabled = true;
                    radioButton2.Enabled = true;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    radioButton5.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}