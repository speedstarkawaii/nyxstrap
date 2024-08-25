using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nyxblox
{
    public partial class Form1 : Form
    {
        //open source roblox launcher i made
        //its limited not really a launcher just something in freetimeeee
        //.gg/getnyx


        //basic confug
        private Timer timer;
        private int targetValue = 100;
        private int increment = 1; 
        private int currentValue = 0; 

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        //some imports we can use i can do it in C but c# is bettr
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;


        const int GWL_STYLE = -16;
        const int WS_MINIMIZEBOX = 0x20000;

        public Form1()
        {
            InitializeComponent();
            StartProgress();

            //for some reason creating events in the properties window froze vs :C
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)//moves location; theres other ways to do this but for my shadow to work i had to make it move :/
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void StartProgress()//when we start the app we want to open roblox
        {


            timer = new Timer
            {
                Interval = 8//slow
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentValue < targetValue)
            {

                currentValue += increment;//dont use while true gunabarthingie + 1 its badd
                guna2ProgressBar1.Value = Math.Min(currentValue, targetValue); 
            }
            else
            {
                timer.Stop();
                LaunchRobloxPlayer();//must be launched after progressbar otherwise the code thinks that Roblox is already open
                //ps: check the last line of code here u can see why

            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)//close roblox if user presses x
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    process.Kill();
                    Environment.Exit(-1);//close current app as well ofc
                 
                }
            }
            else
            {
                Environment.Exit(-1);
            }
        }

        private void LaunchRobloxPlayer()
        {
            string a = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string rbxx = Path.Combine(a, "Roblox");

            string f = SearchForRobloxPlayer(rbxx, "RobloxPlayerBeta.exe");//EXACT name; lots of ppl use weird directories so needs diff approach

            if (!string.IsNullOrEmpty(f))
            {
                Process.Start(f);
            }
            else
            {
                MessageBox.Show("Roblox was NOT found. You may be using a custom directory for roblox.\n\nContact @speedsterkawaii to resolve this.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SearchForRobloxPlayer(string directory, string fileName)
        {
            try
            {
                string[] files = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories);
                return files.Length > 0 ? files[0] : null; 
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (Exception ex)//handle ex in textbabel later
            {
                return null;
            }
        }

        private async void CheckForRobloxWindowAsync()//IMPORT func this will properly dispose the app
        {
            while (true)
            {
                var robloxWindow = Process.GetProcesses()//pov byfron
                    .Where(p => p.MainWindowTitle.Contains("Roblox") && IsWindowVisible(p.MainWindowHandle))
                    .FirstOrDefault();

                if (robloxWindow != null)
                    //THIS IS WHERE YOU DO THE STUFF AFTER YOUR APP CLOSES
                {
                    //RenameRobloxWindow();//bad i was gonna do it later but i realized i had to grab the window title in injector sooooo

                    string rbx = "Roblox";

                    // Find the window handle
                    IntPtr hWnd = FindWindow(null, rbx);

                    if (hWnd == IntPtr.Zero)
                    {
                        //Console.WriteLine("Window not found.");
                        return;
                    }

                    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

                    Application.Exit();//CLOSE app the moment roblox is found
                    return; 
                }

                await Task.Delay(1111);//slow delay but my cpu ded if i make it even faster
            }
        }

        //private void RenameRobloxWindow()
        //{
        //    // Get the Roblox window handle
        //    var robloxWindow = Process.GetProcesses()
        //        .FirstOrDefault(p => p.MainWindowTitle.Contains("Roblox"));

        //    if (robloxWindow != null)
        //    {
        //        // Set the title to "hi"
        //        bool result = SetWindowText(robloxWindow.MainWindowHandle, "Roblox");

        //        if (result)
        //        {
        //        }
        //        else
        //        {
        //        }
        //    }
        //    else
        //    {
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    process.Kill();

                }
            }
            else
            {
            }

            //BELOW CODE IS TECHNICALLY USELESS LOL
            var robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");

            if (robloxProcesses.Length > 0)
            {//already opened roblox. just do a warning. we can unlock a mutex of roblox but thats todo in another project
                MessageBox.Show("Another instance is already executed onto this computer.", "NYX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(-1);
            }
            else
            {
            }
            CheckForRobloxWindowAsync();
        }
    }
}
