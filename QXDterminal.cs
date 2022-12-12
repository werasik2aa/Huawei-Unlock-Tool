using System;
using System.Windows.Forms;
using System.IO.Ports;
using static HuaweiUnlocker.tool;
namespace HuaweiUnlocker
{
    public partial class QXDterminal : Form
    {
        public QXDterminal()
        {
            InitializeComponent();
            PCUI.Items.Clear();
            foreach (var a in SerialPort.GetPortNames())
            {
                PCUI.Items.Add(a);
                DBadapt.Items.Add(a);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PCUI.Items.Clear();
            foreach (var a in SerialPort.GetPortNames())
            {
                PCUI.Items.Add(a);
                DBadapt.Items.Add(a);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            QCDM.OpenedPort.Close();
            QCDM.OpenedPort = null;
            QCDM.opened = false;
        }

        private void CHES(object sender, EventArgs e)
        {
            button4.Enabled = QCDM.opened;
            button4.Visible = QCDM.opened;
        }
        private bool connect(string port)
        {

            if (!DBadapt.Text.StartsWith("COM"))
            {
                LOG("ERROR: SELECT DBadapter port!");
                return false;
            }
            if (!PCUI.Text.StartsWith("COM"))
            {
                LOG("ERROR: SELECT PCUI port!");
                return false;
            }
            if (QCDM.OpenedPortName != port || !QCDM.opened) {
                LOG("CONNECTING: "+port);
                if (QCDM.Open(port, 115200, Parity.None, 8, StopBits.Two, Handshake.RequestToSendXOnXOff))
                    return true;
            }
            if (!QCDM.opened)
            {
                LOG("ERROR: CAN'T CONNECT TO PORT! -> "+port);
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (connect(PCUI.Text)) {
                QCDM.SendMessge("AT$QCDMG=115200\n\r~", false, false, false);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (connect(DBadapt.Text)) {
                //QCDM.SendMessge(textBox3.Text, coder.Checked, tobytes.Checked, hex.Checked);

            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (connect(DBadapt.Text))
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    QCDM.SendFile(file.FileName, coder.Checked, hex.Checked);
                }
                //QCDM.SendMessge(tmp, coder.Checked, hex.Checked);
            }
            //NativeMethods.CreatePluginWindow(IntPtr.Zero, 1, 1);
        }
    }
}
