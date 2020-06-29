using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeySelector
{
    public partial class Form1 : Form {

        private const UInt32 KLF_SETFORPROCESS = 0x00000100;
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint KLF_ACTIVATE = 1;

        private const int Gr = 4;

        public delegate void SetK(int b);
        public SetK _setk;

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        static SerialPort _serialPort;

        private List<InputLanguage> KeybItems = new List<InputLanguage>();
        private List<string> Keyblang = new List<string>();

        private ComboBox[] Keybs = new ComboBox[Gr];
        private Button[] Bts = new Button[Gr];
        private int[] Setkey = new int[Gr];
        private Boolean NeedSaveConfig;
        private Boolean ComOpenYet;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        public Form1() {
            int x;

            InitializeComponent();

            ComOpenYet = false;
            OpenButton.Text = "Открыть";
            _setk = new SetK(SetKMethod);

            // Create the NotifyIcon.
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = this.Icon;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            //notifyIcon1.ContextMenu = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Смена раскладки клавиатуры";
            notifyIcon1.Visible = false;

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);

            // Задаем массивы элементов управления 
            InputLanguageCollection installedLangs = InputLanguage.InstalledInputLanguages;
            foreach (InputLanguage lang in installedLangs) {
                KeybItems.Add(lang);
                Keyblang.Add(lang.LayoutName);
            }

            this.Width = 36 + 141 * Gr;
            // Комбобоксы
            for (x = 0; x < Gr; x++) {
                ComboBox newcombo = new ComboBox{
                    Location = new Point(12 + x * 141, 40),
                    Size = new Size(width: 135, height: 21),
                    Visible = true,
                    Parent = this
                };

                newcombo.SelectedIndexChanged += new EventHandler(keyb_onchange);
                newcombo.Items.AddRange(Keyblang.ToArray());
                Keybs[x] = newcombo;
            }
            // Кнопки
            for (x = 0; x < Gr; x++) {
                Button newbut = new Button{
                    Location = new Point(12 + x * 141, 70),
                    Size = new Size(width: 135, height: 21),
                    Visible = true,
                    Parent = this
                };

                newbut.Click += new EventHandler(but_onclick);
                Bts[x] = newbut;
            }
            // Настройки
            x = 0;
            string opencomst = "";
            if (System.IO.File.Exists("config.ini")) {
                StreamReader sr = new StreamReader("config.ini");
                string line;
                
                opencomst = sr.ReadLine();

                int i = 0;
                while (((line = sr.ReadLine()) != null) && (x < Gr)) {
                    i = Convert.ToInt32(line);
                    if ((i < KeybItems.Count)&&(i >= 0)) {
                        //Setkey[x] = i;
                        Keybs[x].SelectedIndex = i;
                    } else {
                        Setkey[x] = -1;
                    };

                    x++;
                }

                sr.Close();
            }

            for (; x < Gr; x++) {
                Setkey[x] = -1;
            }

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            int cc = -1;
            foreach (string s in SerialPort.GetPortNames()) {
                COMcombo.Items.Add(s);
                if (opencomst == s) cc = COMcombo.Items.Count;
            }

            if (cc != -1) {
                COMcombo.SelectedItem = COMcombo.Items[cc - 1];
                COMOpen(null, null);
            }

            if (!System.IO.File.Exists("config.ini")) NeedSaveConfig = true;
            else NeedSaveConfig = false;

        }

        private void but_onclick(object sender, EventArgs e) {
            int index = 0;
            while (!sender.Equals(Bts[index])) index++;

            if (Setkey[index]!=-1) {
                //InputLanguage.CurrentInputLanguage = KeybItems[Setkey[index]];
                string layoutName = KeybItems[Setkey[index]].Culture.LCID.ToString("x8");
                string Klid = new StringBuilder(layoutName).ToString();
                PostMessage(GetForegroundWindow(), WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(Klid, KLF_ACTIVATE));
            };
        }

        private void keyb_onchange(object sender, EventArgs e) {
            int index = 0;
            while (!sender.Equals(Keybs[index])) index++;
            Setkey[index] = Keybs[index].SelectedIndex;
            NeedSaveConfig = true;
        }
        private void OnCOMChange(object sender, EventArgs e) {
            NeedSaveConfig = true;
        } 

        private void COMOpen(object sender, EventArgs e) {
            if (!ComOpenYet) {
                _serialPort = new SerialPort {
                    PortName = COMcombo.SelectedItem.ToString(),
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.Two,
                    Parity = Parity.None,
                    ReadTimeout = 5000
                };

                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

                ComOpenYet = true;
                OpenButton.Text = "Закрыть";

            } else {
                _serialPort.Close();
                _serialPort.Dispose();

                ComOpenYet = false;
                OpenButton.Text = "Открыть";
            }
        }

        private void SerialPort_DataReceived(object sender, EventArgs e) {
            int buferSize = _serialPort.BytesToRead;
            for (int i = 0; i < buferSize; ++i) {
                //  читаем по одному байту
                int bt = _serialPort.ReadByte();
                //  если встретили начало кадра (0xFF) - начинаем запись в _bufer
                if (bt == 170) {
                    int bt1 = _serialPort.ReadByte();

                    Invoke(_setk, new Object[] {(bt1 - 1)});

                    //_serialPort.DiscardInBuffer();
                    return;
                }
            }

            _serialPort.DiscardInBuffer();
        }

        public void SetKMethod(int b) {
            if ((b < Gr)&&(Setkey[b] != -1)) {
                //InputLanguage.CurrentInputLanguage = KeybItems[Setkey[b]];
                string layoutName = KeybItems[Setkey[b]].Culture.LCID.ToString("x8");
                string Klid = new StringBuilder(layoutName).ToString();
                PostMessage(GetForegroundWindow(), WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(Klid, KLF_ACTIVATE));
            };
        }
        private void notifyIcon1_DoubleClick(object Sender, EventArgs e) {
            // Show the form when the user double clicks on the notify icon.
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;

                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            
                // Activate the form.
                this.Activate();
        }

        private void OnLoad(object sender, EventArgs e) {
            notifyIcon1.Visible = true;
            //Minimize form
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void OnDeactivate(object sender, EventArgs e) {
            if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized) {
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }

        }
        private void OnClosing(object sender, FormClosingEventArgs e) {
            if (ComOpenYet) {
                _serialPort.Close();
                _serialPort.Dispose();
            }

            if (!NeedSaveConfig) return;
            StreamWriter sw = new StreamWriter("config.ini");

            sw.WriteLine(COMcombo.SelectedItem.ToString());

            for (int x = 0; x < Gr; x++) {
                sw.WriteLine(Setkey[x].ToString());
            }

            sw.Close();
        }
    }
}
