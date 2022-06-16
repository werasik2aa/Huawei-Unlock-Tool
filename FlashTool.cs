using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HuaweiUnlocker
{
    public partial class FlashTool : Form
    {
        private tool a = new tool();
        public FlashTool()
        {
            InitializeComponent();
            tool.LOGGE = LOGS;
            tool.PORTER = PORTER;
            tool.LOGGE.Text = "Version 2.0 beta";
            tool.LOG("INFO: Qualcom Flash Tool (c)");
            tool.LOG("INFO: This tool can Flash Firmware");
            tool.LOG("INFO: Select Files");
            tool.LOG("INFO: Connect device via EDL (9008 mode)");
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
            Flash.Enabled = false;
            tool.error = false;
            tool.debug = debs.Checked;
            if (Xm.Text == "")
            {
                tool.LOG("ERROR: Please Select (Rawprogram0.xml)!");
                tool.error = true;
                return;
            }
            if (Ld.Text == "")
            {
                tool.LOG("ERROR: Please Select Loader! (LOADER.ELF | LOADER.MBN | LOADER.HEX)!");
                tool.error = true;
                return;
            }
            if (!RAW.Checked)
                if (!tool.FlashPartsXml(Xm.Text, Ld.Text, pather.Text))
                {
                    tool.LOG("ERROR: WRONG RAWPROGRAM0? OR DEVICE DISCONNECTED!"); tool.error = true;
                }
                else ;
            else
                if (!tool.FlashPartsRaw(Ld.Text, pather.Text))
                {
                    tool.LOG("ERROR: WRONG RAWPROGRAM0? OR DEVICE DISCONNECTED!"); tool.error = true;
                }
            if (!tool.error) tool.LOG("INFO: FLASHING " + pather.Text + " DONE");
            Flash.Enabled = true;
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
                            //tool.LOG(a);
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
                if (Xm.Text == "") openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Sector DUMP files (*.img;*.bin)|*.img;*.bin;*.emmc|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK) Xm.Text = openFileDialog.FileName;
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
    }
}
