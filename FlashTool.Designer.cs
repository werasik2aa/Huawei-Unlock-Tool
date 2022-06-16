namespace HuaweiUnlocker
{
    partial class FlashTool
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.LOGS = new System.Windows.Forms.TextBox();
            this.Flash = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.PORTER = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.pather = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DETECTED = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Xm = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Ld = new System.Windows.Forms.ComboBox();
            this.AutoXml = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AutoLdr = new System.Windows.Forms.CheckBox();
            this.debs = new System.Windows.Forms.CheckBox();
            this.RAW = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LOGS
            // 
            this.LOGS.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.LOGS.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LOGS.Location = new System.Drawing.Point(12, 12);
            this.LOGS.Multiline = true;
            this.LOGS.Name = "LOGS";
            this.LOGS.ReadOnly = true;
            this.LOGS.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LOGS.Size = new System.Drawing.Size(474, 488);
            this.LOGS.TabIndex = 10;
            // 
            // Flash
            // 
            this.Flash.BackColor = System.Drawing.Color.AliceBlue;
            this.Flash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Flash.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Flash.Location = new System.Drawing.Point(492, 461);
            this.Flash.Name = "Flash";
            this.Flash.Size = new System.Drawing.Size(466, 39);
            this.Flash.TabIndex = 11;
            this.Flash.Text = "Flash Firmware";
            this.Flash.UseVisualStyleBackColor = false;
            this.Flash.Click += new System.EventHandler(this.Flash_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.MintCream;
            this.label2.Location = new System.Drawing.Point(626, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 24);
            this.label2.TabIndex = 21;
            this.label2.Text = "CONNECTED PORT";
            // 
            // PORTER
            // 
            this.PORTER.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.PORTER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PORTER.Location = new System.Drawing.Point(492, 36);
            this.PORTER.Name = "PORTER";
            this.PORTER.ReadOnly = true;
            this.PORTER.Size = new System.Drawing.Size(466, 28);
            this.PORTER.TabIndex = 20;
            this.PORTER.Text = "Connect Your device";
            this.PORTER.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.AliceBlue;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Location = new System.Drawing.Point(877, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(81, 29);
            this.button3.TabIndex = 23;
            this.button3.Text = "Select";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pather
            // 
            this.pather.Location = new System.Drawing.Point(492, 70);
            this.pather.Name = "pather";
            this.pather.Size = new System.Drawing.Size(379, 22);
            this.pather.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.MintCream;
            this.label3.Location = new System.Drawing.Point(492, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 24);
            this.label3.TabIndex = 24;
            this.label3.Text = "Path To Firmware";
            // 
            // DETECTED
            // 
            this.DETECTED.AutoSize = true;
            this.DETECTED.Cursor = System.Windows.Forms.Cursors.Default;
            this.DETECTED.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DETECTED.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DETECTED.ForeColor = System.Drawing.Color.MintCream;
            this.DETECTED.Location = new System.Drawing.Point(3, 8);
            this.DETECTED.Name = "DETECTED";
            this.DETECTED.Size = new System.Drawing.Size(217, 24);
            this.DETECTED.TabIndex = 14;
            this.DETECTED.Text = "RAWPROGRAM0.XML";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.MintCream;
            this.label1.Location = new System.Drawing.Point(3, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 24);
            this.label1.TabIndex = 15;
            this.label1.Text = "MSM LOADER.MBN";
            // 
            // Xm
            // 
            this.Xm.Location = new System.Drawing.Point(3, 35);
            this.Xm.Name = "Xm";
            this.Xm.Size = new System.Drawing.Size(379, 22);
            this.Xm.TabIndex = 16;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.AliceBlue;
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(388, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 29);
            this.button1.TabIndex = 18;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.LOADER_PATH);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.AliceBlue;
            this.button2.Enabled = false;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(388, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 29);
            this.button2.TabIndex = 19;
            this.button2.Text = "Select";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.XML_PATH);
            // 
            // Ld
            // 
            this.Ld.FormattingEnabled = true;
            this.Ld.Location = new System.Drawing.Point(3, 99);
            this.Ld.Name = "Ld";
            this.Ld.Size = new System.Drawing.Size(379, 24);
            this.Ld.TabIndex = 20;
            // 
            // AutoXml
            // 
            this.AutoXml.AutoSize = true;
            this.AutoXml.Checked = true;
            this.AutoXml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoXml.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AutoXml.Location = new System.Drawing.Point(388, 6);
            this.AutoXml.Name = "AutoXml";
            this.AutoXml.Size = new System.Drawing.Size(68, 23);
            this.AutoXml.TabIndex = 26;
            this.AutoXml.Text = "Auto";
            this.AutoXml.UseVisualStyleBackColor = true;
            this.AutoXml.CheckStateChanged += new System.EventHandler(this.Xml);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AutoLdr);
            this.panel1.Controls.Add(this.AutoXml);
            this.panel1.Controls.Add(this.Ld);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.Xm);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.DETECTED);
            this.panel1.Location = new System.Drawing.Point(489, 330);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(478, 131);
            this.panel1.TabIndex = 25;
            // 
            // AutoLdr
            // 
            this.AutoLdr.AutoSize = true;
            this.AutoLdr.Checked = true;
            this.AutoLdr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoLdr.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AutoLdr.Location = new System.Drawing.Point(388, 66);
            this.AutoLdr.Name = "AutoLdr";
            this.AutoLdr.Size = new System.Drawing.Size(68, 23);
            this.AutoLdr.TabIndex = 27;
            this.AutoLdr.Text = "Auto";
            this.AutoLdr.UseVisualStyleBackColor = true;
            this.AutoLdr.CheckStateChanged += new System.EventHandler(this.Ldr);
            // 
            // debs
            // 
            this.debs.AutoSize = true;
            this.debs.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.debs.Location = new System.Drawing.Point(877, 101);
            this.debs.Name = "debs";
            this.debs.Size = new System.Drawing.Size(76, 20);
            this.debs.TabIndex = 26;
            this.debs.Text = "Debug";
            this.debs.UseVisualStyleBackColor = true;
            // 
            // RAW
            // 
            this.RAW.AutoSize = true;
            this.RAW.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RAW.Location = new System.Drawing.Point(717, 97);
            this.RAW.Name = "RAW";
            this.RAW.Size = new System.Drawing.Size(106, 22);
            this.RAW.TabIndex = 27;
            this.RAW.Text = "Raw Image";
            this.RAW.UseVisualStyleBackColor = true;
            this.RAW.CheckedChanged += new System.EventHandler(this.RAW_CheckedChanged);
            // 
            // FlashTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(970, 512);
            this.Controls.Add(this.RAW);
            this.Controls.Add(this.debs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.pather);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PORTER);
            this.Controls.Add(this.Flash);
            this.Controls.Add(this.LOGS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FlashTool";
            this.Text = "FlashTool";
            this.Deactivate += new System.EventHandler(this.FlashTool_Deactivate);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LOGS;
        private System.Windows.Forms.Button Flash;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PORTER;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox pather;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label DETECTED;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Xm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox Ld;
        private System.Windows.Forms.CheckBox AutoXml;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox AutoLdr;
        private System.Windows.Forms.CheckBox debs;
        private System.Windows.Forms.CheckBox RAW;
    }
}