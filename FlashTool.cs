using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Collections.Concurrent;

namespace HuaweiUnlocker
{
    public partial class FlashTool : Form
    {
        public FlashTool()
        {
            InitializeComponent();
            tool.LOGGBOX.Text = "Version 4.0";
            tool.LOG(tool.I("SMAIN1"));
            tool.LOG(tool.I("SMAIN2"));
            tool.LOG(tool.I("SMAIN3"));
            tool.LOG(tool.I("Tutr"));
            foreach (var a in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\qc_boot"))
            {
                String[] es = a.Split('\\');
                Ld.Items.Add(tool.PickLoader(es[es.Length - 1]));
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
            tool.progr.Value = 0;
            if (!tool.CheckDevice(Ld)) return;
            if (Xm.Text.Length < 5 && !RAW.Checked)
            {
                tool.LOG(tool.E("ErrXML"));
                return;
            }
            if (pather.Text.Length < 5 && RAW.Checked)
            {
                tool.LOG(tool.E("ErrBin"));
                return;
            }
            tool.all();
            if (!RAW.Checked)
            {
                if (!tool.FlashPartsXml(Xm.Text, Ld.Text, pather.Text))
                    tool.LOG(tool.E("ErrXML2"));
                else tool.LOG(tool.I("Flashing") + pather.Text + tool.L("Done"));
            }
            else
            {
                if (!tool.FlashPartsRaw(Ld.Text, pather.Text))
                    tool.LOG(tool.E("ErrBin2"));
                else tool.LOG(tool.I("Flashing2") + pather.Text + tool.L("Done"));
            }
            foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
            tool.all();
            tool.progr.Value = 100;
        }

        private void Xml(object sender, EventArgs e)
        {
            button2.Enabled = !AutoXml.Checked;
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
            tool.progr.Value = 0;
            if (!tool.CheckDevice(Ld)) return;
            tool.all();
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pather.Text = openFileDialog.SelectedPath + "\\DUMP.APP";
                if (!tool.Dump(Ld.Text, pather.Text))
                {
                    tool.LOG("ERROR: Failed Dump All!");
                    foreach (var process in Process.GetProcessesByName("emmcdl.exe")) { process.Kill(); break; }
                }
                else tool.LOG(tool.I("Dumping") + pather.Text + tool.L("Done"));
            }
            tool.all();
            tool.progr.Value = 100;
        }

        private void AutoLdr_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = AutoLdr.Checked;
        }

        private void AutoXml_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = AutoXml.Checked;
        }

        private void RdGPT_Click(object sender, EventArgs e)
        {
            if (!tool.CheckDevice(Ld)) return;
            RdGPT.Visible = false;
            RdGPT.Enabled = false;
            tool.LOG(tool.I("ReadGPT"));
            tool.GPTTABLE = new Dictionary<string, int[]>();
            bool gpt = tool.ReadGPT(Ld.Text);
            if (gpt)
            {
                foreach (var obj in tool.GPTTABLE) PARTLIST.Rows.Add(obj.Key, obj.Value[0], obj.Value[1]);
                PARTLIST.AutoResizeRows();
                tool.LOG(tool.I("SUCC_ReadGPT"));
            }
            else { tool.LOG(tool.I("ERR_ReadGPT")); }
            RdGPT.Visible = !gpt;
            RdGPT.Enabled = !gpt;
            tool.progr.Value = 100;
        }

        private void PARTLIST_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (PARTLIST.Rows.Count > 0)
            {
                PARTLIST.Enabled = false;
                WHAT.Enabled = true;
                WHAT.Visible = true;
                string partition = PARTLIST.CurrentRow.Cells[0].Value.ToString();
                LNG.Text = tool.L("Action") + partition;
                WHAT.Text = partition;
                tool.LOG(tool.I("sl") + partition);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WHAT.Enabled = false;
            PARTLIST.Enabled = true;
            WHAT.Visible = false;
        }

        private void ERASEevent_Click(object sender, EventArgs e)
        {
            if (!tool.CheckDevice(Ld)) return;
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(tool.L("AreY")+WHAT.Text, "WARNING: CAN CAUSE DAMAGE", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (tool.Erase(WHAT.Text, Ld.Text))
                    tool.LOG(tool.I("ErPS") + WHAT.Text);
                else
                    tool.LOG(tool.E("ErPE") + WHAT.Text);
                tool.progr.Value = 100;
            }
        }

        private void WRITEevent_Click(object sender, EventArgs e)
        {
            if (!tool.CheckDevice(Ld)) return;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (tool.Write(WHAT.Text, Ld.Text, file.FileName))
                    tool.LOG(tool.I("EwPS") + WHAT.Text);
                else
                    tool.LOG(tool.E("EwPE") + WHAT.Text);
                tool.progr.Value = 100;
            }
        }

        private void READevent_Click_1(object sender, EventArgs e)
        {
            if (!tool.CheckDevice(Ld)) return;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                int i = tool.GPTTABLE[WHAT.Text][0];
                int j = tool.GPTTABLE[WHAT.Text][1];
                if (tool.Dump(i, j, WHAT.Text, Ld.Text, folder.SelectedPath))
                    tool.LOG(tool.I("EdPS") + WHAT.Text);
                else
                    tool.LOG(tool.E("EdPE") + WHAT.Text);
                tool.progr.Value = 100;
            }
        }
    }
}
