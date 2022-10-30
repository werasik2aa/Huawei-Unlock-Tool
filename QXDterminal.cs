using System;
using System.Windows.Forms;
using System.IO.Ports;
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
            QCDM.tohex = hex.Checked;
            QCDM.encode = coder.Checked;

            if (!DBadapt.Text.StartsWith("COM"))
            {
                tool.LOG("ERROR: SELECT DBadapter port!");
                return false;
            }
            if (!PCUI.Text.StartsWith("COM"))
            {
                tool.LOG("ERROR: SELECT PCUI port!");
                return false;
            }
            if (QCDM.OpenedPortName != port || !QCDM.opened) {
                tool.LOG("CONNECTING: "+port);
                if (QCDM.Open(port, 115200, Parity.None, 8, StopBits.Two, Handshake.RequestToSendXOnXOff))
                    return true;
            }
            if (!QCDM.opened)
            {
                tool.LOG("ERROR: CAN'T CONNECT TO PORT! -> "+port);
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (connect(PCUI.Text)) {
            QCDM.SendMessge(textBox3.Text);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //WinUsb.ListDevice(false);
            if (connect(DBadapt.Text)) {
                QCDM.SendMessge(textBox3.Text);
            }
        }
    }
}
