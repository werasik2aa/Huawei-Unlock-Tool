namespace HuaweiUnlocker
{
    partial class Unlock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Unlock));
            this.button1 = new System.Windows.Forms.Button();
            this.ISA = new System.Windows.Forms.CheckBox();
            this.Loader_text = new System.Windows.Forms.Label();
            this.Loaders = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DEVICER = new System.Windows.Forms.ComboBox();
            this.UnlockFrp = new System.Windows.Forms.Button();
            this.Erasda = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.Color.Snow;
            this.button1.Location = new System.Drawing.Point(12, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(391, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "Unlock";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ISA
            // 
            this.ISA.AutoSize = true;
            this.ISA.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ISA.Checked = true;
            this.ISA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ISA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ISA.Font = new System.Drawing.Font("Arial", 13.2F, System.Drawing.FontStyle.Bold);
            this.ISA.ForeColor = System.Drawing.Color.Cornsilk;
            this.ISA.Location = new System.Drawing.Point(325, 0);
            this.ISA.Name = "ISA";
            this.ISA.Size = new System.Drawing.Size(78, 30);
            this.ISA.TabIndex = 1;
            this.ISA.Text = "Auto";
            this.ISA.UseVisualStyleBackColor = false;
            this.ISA.CheckedChanged += new System.EventHandler(this.ISAS);
            // 
            // Loader_text
            // 
            this.Loader_text.AutoSize = true;
            this.Loader_text.Cursor = System.Windows.Forms.Cursors.Default;
            this.Loader_text.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Loader_text.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Loader_text.ForeColor = System.Drawing.Color.MintCream;
            this.Loader_text.Location = new System.Drawing.Point(12, 4);
            this.Loader_text.Name = "Loader_text";
            this.Loader_text.Size = new System.Drawing.Size(133, 24);
            this.Loader_text.TabIndex = 3;
            this.Loader_text.Text = "Select loader";
            // 
            // Loaders
            // 
            this.Loaders.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.Loaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Loaders.Font = new System.Drawing.Font("Arial", 9.8F, System.Drawing.FontStyle.Bold);
            this.Loaders.FormattingEnabled = true;
            this.Loaders.Location = new System.Drawing.Point(12, 31);
            this.Loaders.Name = "Loaders";
            this.Loaders.Size = new System.Drawing.Size(391, 27);
            this.Loaders.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Cornsilk;
            this.label1.Location = new System.Drawing.Point(647, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select DEVICE";
            // 
            // DEVICER
            // 
            this.DEVICER.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.DEVICER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEVICER.Font = new System.Drawing.Font("Arial", 9.8F, System.Drawing.FontStyle.Bold);
            this.DEVICER.Location = new System.Drawing.Point(409, 31);
            this.DEVICER.Name = "DEVICER";
            this.DEVICER.Size = new System.Drawing.Size(384, 27);
            this.DEVICER.TabIndex = 8;
            this.DEVICER.Text = "ATU-L31";
            // 
            // UnlockFrp
            // 
            this.UnlockFrp.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.UnlockFrp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnlockFrp.Font = new System.Drawing.Font("Arial Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UnlockFrp.ForeColor = System.Drawing.Color.Snow;
            this.UnlockFrp.Location = new System.Drawing.Point(409, 64);
            this.UnlockFrp.Name = "UnlockFrp";
            this.UnlockFrp.Size = new System.Drawing.Size(384, 43);
            this.UnlockFrp.TabIndex = 9;
            this.UnlockFrp.Text = "Unlock Frp";
            this.UnlockFrp.UseVisualStyleBackColor = false;
            this.UnlockFrp.Click += new System.EventHandler(this.UnlockFrp_Click);
            // 
            // Erasda
            // 
            this.Erasda.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Erasda.Enabled = false;
            this.Erasda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Erasda.Font = new System.Drawing.Font("Arial Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Erasda.ForeColor = System.Drawing.Color.Snow;
            this.Erasda.Location = new System.Drawing.Point(12, 114);
            this.Erasda.Name = "Erasda";
            this.Erasda.Size = new System.Drawing.Size(391, 43);
            this.Erasda.TabIndex = 10;
            this.Erasda.Text = "Format Userdata";
            this.Erasda.UseVisualStyleBackColor = false;
            this.Erasda.Visible = false;
            this.Erasda.Click += new System.EventHandler(this.Erasda_Click);
            // 
            // Unlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(817, 117);
            this.Controls.Add(this.Erasda);
            this.Controls.Add(this.UnlockFrp);
            this.Controls.Add(this.DEVICER);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Loaders);
            this.Controls.Add(this.Loader_text);
            this.Controls.Add(this.ISA);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Unlock";
            this.Text = "Huawei Unlock Tool v1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox ISA;
        private System.Windows.Forms.Label Loader_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Loaders;
        private System.Windows.Forms.ComboBox DEVICER;
        private System.Windows.Forms.Button UnlockFrp;
        private System.Windows.Forms.Button Erasda;
    }
}

