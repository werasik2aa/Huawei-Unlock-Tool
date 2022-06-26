using System;
using System.Windows.Forms;
using HuaweiUnlocker;
using System.Threading;
using System.IO;
using System.IO.Ports;
namespace HuaweiUnlocker
{
    public partial class QXDterminal : Form
    {
        public QXDterminal()
        {
            InitializeComponent();
            QCDM.CL = A2;
            QCDM.CR = A1;
            QCDM.Ads = Ads;
            QCDM.ass = ass;
            comboBox1.Items.Clear();
            foreach (var a in SerialPort.GetPortNames()) comboBox1.Items.Add(a);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!QCDM.opened || (QCDM.opened && QCDM.OpenedPortName != comboBox1.Text)) if (connect("COM13"))
                    tool.LOG("Connected To port");
                else
                {
                    tool.LOG("ERROR: Cann't connect to port");
                    return;
                }
            
            QCDM.SendMessge(cline.Text);
            cline.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!QCDM.opened || (QCDM.opened && QCDM.OpenedPortName != comboBox1.Text)) if (connect(comboBox1.Text))
                    tool.LOG("Connected To port");
                else
                {
                    tool.LOG("ERROR: Cann't connect to port");
                    return;
                }
            if (!QCDM.opened) return;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "HEX LINES text file (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.EndsWith(".txt"))
                {
                    StreamReader reader = new StreamReader(openFileDialog.FileName);
                    string line = reader.ReadLine();
                    while ((line = reader.ReadLine()) != null) QCDM.SendMessge(line);
                }
                else
                    QCDM.SendMessge(File.ReadAllBytes(openFileDialog.FileName));
            }
            cline.Text = "";
        }
        private bool connect(string port)
        {
            if (!comboBox1.Text.StartsWith("COM")) { tool.LOG("Select COM PORT. First"); return false; }
            if (QCDM.opened) { QCDM.OpenedPort.Close(); QCDM.opened = false; }
            if (QCDM.Open(comboBox1.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One, System.IO.Ports.Handshake.RequestToSendXOnXOff))
            { QCDM.OpenedPortName = comboBox1.Text; return true; }
            else
                return false;
        }
        private byte[] file(string path)
        {
            return File.ReadAllBytes(path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (var a in SerialPort.GetPortNames()) comboBox1.Items.Add(a);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            QCDM.OpenedPort.Close();
            QCDM.opened = false;
        }

        private void CHES(object sender, EventArgs e)
        {
            button4.Enabled = QCDM.opened;
            button4.Visible = QCDM.opened;
        }
    }
}
