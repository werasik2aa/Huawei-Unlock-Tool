using System;
using static HuaweiUnlocker.tool;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace HuaweiUnlocker
{
    public partial class FlashTool : Form
    {
        public FlashTool()
        {
            InitializeComponent();
            LOGGE.Text = "Version 4.0";
            LOG(I("SMAIN1"));
            LOG(I("SMAIN2"));
            LOG(I("SMAIN3"));
            LOG(I("Tutr"));
            foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
            {
                String[] es = a.Split('\\');
                Ld.Items.Add(PickLoader(es[es.Length - 1]));
            }
        }

        private void LOADER_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Ld.Text;
            if (Ld.Text == "") openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Programmer files (*.mbn;*.elf;*.hex)|*.mbn;*.elf;*.hex|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) Ld.Text = openFileDialog.FileName;
        }
        private void XML_PATH(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Xm.Text;
            if (Xm.Text == "") openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Sectors data files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) Xm.Text = openFileDialog.FileName;
        }

        private void Flash_Click(object sender, EventArgs e)
        {
            progr.Value = 0;
            all();
            error = false;
            if (Xm.Text.Length < 4 && !RAW.Checked)
            {
                LOG(E("ErrXML"));
                error = true;
            }
            if (Ld.Text.Length < 4)
            {
                LOG(E("ErrLdr"));
                error = true;
            }
            if (!error) if (!RAW.Checked)
                {
                    if (!FlashPartsXml(Xm.Text, Ld.Text, pather.Text))
                        LOG(E("ErrXML2")); error = true;
                }
                else
                {
                    if (!FlashPartsRaw(Ld.Text, pather.Text))
                        LOG(E("ErrBin")); error = true;
                }
            if (!error) LOG(I("Flashing") + pather.Text + L("Done"));
            else
                foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            all();
            progr.Value = 100;
        }

        private void FlashTool_Deactivate(object sender, EventArgs e)
        {
        }

        private void Xml(object sender, EventArgs e)
        {
            button2.Enabled = !AutoXml.Checked;
        }
        private void Ldr(object sender, EventArgs e)
        {
            button1.Enabled = !AutoLdr.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!RAW.Checked)
            {
                FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pather.Text = openFileDialog.SelectedPath;
                    if (AutoXml.Checked)
                    {
                        foreach (var a in Directory.GetFiles(openFileDialog.SelectedPath))
                        {
                            //LOG(a);
                            if (AutoXml.Checked && a.EndsWith(".xml")) Xm.Text = a;
                            if (AutoLdr.Checked && a.EndsWith(".mbn") || a.EndsWith(".elf") || a.EndsWith(".hex")) Ld.Text = a;
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = Xm.Text;
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Sector DUMP files (*.img;*.bin)|*.img;*.bin;*.emmc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK) pather.Text = openFileDialog.FileName;
            }
        }

        private void RAW_CheckedChanged(object sender, EventArgs e)
        {
            Xm.Visible = !RAW.Checked;
            DETECTED.Visible = !RAW.Checked;
            AutoXml.Visible = !RAW.Checked;
            button2.Visible = !RAW.Checked;
            AutoXml.Enabled = !RAW.Checked;
            button2.Enabled = !RAW.Checked;
            DETECTED.Enabled = !RAW.Checked;
            Xm.Enabled = !RAW.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            progr.Value = 0;
            all();
            error = false;
            if (Ld.Text.Length < 4)
            {
                LOG(E("ErrLdr"));
                error = true;
            }
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (!error) if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.SelectedPath+"\\DUMP.APP";
                if (!error)
                    if (!Dump(Ld.Text, pather.Text))
                    {
                        LOG("ERROR: Failed Dump All!"); error = true;
                    }
                    else ;
                if (!error) LOG(I("Dumping") + pather.Text + L("Done"));
                else
                    foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            }
            all();
            progr.Value = 100;
            getgpt = false;
        }
    }
}
