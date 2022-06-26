namespace HuaweiUnlocker
{
    partial class QXDterminal
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
            this.components = new System.ComponentModel.Container();
            this.cline = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.A1 = new System.Windows.Forms.CheckBox();
            this.A2 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.Ads = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.Conns = new System.Windows.Forms.Timer(this.components);
            this.ass = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cline
            // 
            this.cline.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cline.Location = new System.Drawing.Point(12, 8);
            this.cline.Name = "cline";
            this.cline.Size = new System.Drawing.Size(590, 22);
            this.cline.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Cornsilk;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(131, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "SEND CMD";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // A1
            // 
            this.A1.AutoSize = true;
            this.A1.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.A1.ForeColor = System.Drawing.Color.Cornsilk;
            this.A1.Location = new System.Drawing.Point(12, 36);
            this.A1.Name = "A1";
            this.A1.Size = new System.Drawing.Size(58, 20);
            this.A1.TabIndex = 3;
            this.A1.Text = "NaN";
            this.A1.UseVisualStyleBackColor = true;
            // 
            // A2
            // 
            this.A2.AutoSize = true;
            this.A2.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.A2.ForeColor = System.Drawing.Color.Cornsilk;
            this.A2.Location = new System.Drawing.Point(67, 36);
            this.A2.Name = "A2";
            this.A2.Size = new System.Drawing.Size(58, 20);
            this.A2.TabIndex = 4;
            this.A2.TabStop = false;
            this.A2.Text = "NaN";
            this.A2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Cornsilk;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(13, 81);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 33);
            this.button2.TabIndex = 5;
            this.button2.Text = "Script or File";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Ads
            // 
            this.Ads.AutoSize = true;
            this.Ads.Checked = true;
            this.Ads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ads.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Ads.ForeColor = System.Drawing.Color.Cornsilk;
            this.Ads.Location = new System.Drawing.Point(120, 36);
            this.Ads.Name = "Ads";
            this.Ads.Size = new System.Drawing.Size(55, 20);
            this.Ads.TabIndex = 6;
            this.Ads.TabStop = false;
            this.Ads.Text = "+7E";
            this.Ads.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(355, 51);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(247, 24);
            this.comboBox1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.FloralWhite;
            this.label1.Location = new System.Drawing.Point(432, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "COM PORT";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Cornsilk;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(355, 81);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 33);
            this.button3.TabIndex = 9;
            this.button3.Text = "Refresh";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Cornsilk;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button4.Location = new System.Drawing.Point(502, 81);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 33);
            this.button4.TabIndex = 10;
            this.button4.Text = "Disconnect";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Conns
            // 
            this.Conns.Enabled = true;
            this.Conns.Tick += new System.EventHandler(this.CHES);
            // 
            // ass
            // 
            this.ass.AutoSize = true;
            this.ass.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ass.ForeColor = System.Drawing.Color.Cornsilk;
            this.ass.Location = new System.Drawing.Point(181, 36);
            this.ass.Name = "ass";
            this.ass.Size = new System.Drawing.Size(62, 20);
            this.ass.TabIndex = 11;
            this.ass.TabStop = false;
            this.ass.Text = "ASCI";
            this.ass.UseVisualStyleBackColor = true;
            // 
            // QXDterminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(617, 126);
            this.Controls.Add(this.ass);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.Ads);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.A2);
            this.Controls.Add(this.A1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cline);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "QXDterminal";
            this.Text = "QXDterminal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cline;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox A1;
        private System.Windows.Forms.CheckBox A2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox Ads;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer Conns;
        private System.Windows.Forms.CheckBox ass;
    }
}