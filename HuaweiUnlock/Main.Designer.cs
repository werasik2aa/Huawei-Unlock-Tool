namespace HuaweiUnlocker
{
    partial class Huawei
    {
                                private System.ComponentModel.IContainer components = null;

                                        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LOGGER = new System.Windows.Forms.TextBox();
            this.DBB = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // LOGGER
            // 
            this.LOGGER.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.LOGGER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LOGGER.Location = new System.Drawing.Point(9, 11);
            this.LOGGER.Margin = new System.Windows.Forms.Padding(2);
            this.LOGGER.Multiline = true;
            this.LOGGER.Name = "LOGGER";
            this.LOGGER.ReadOnly = true;
            this.LOGGER.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LOGGER.Size = new System.Drawing.Size(678, 361);
            this.LOGGER.TabIndex = 10;
            // 
            // DBB
            // 
            this.DBB.AutoSize = true;
            this.DBB.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.DBB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DBB.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DBB.Location = new System.Drawing.Point(539, 12);
            this.DBB.Margin = new System.Windows.Forms.Padding(2);
            this.DBB.Name = "DBB";
            this.DBB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DBB.Size = new System.Drawing.Size(100, 20);
            this.DBB.TabIndex = 11;
            this.DBB.Text = "Debug Log";
            this.DBB.UseVisualStyleBackColor = false;
            this.DBB.CheckedChanged += new System.EventHandler(this.debugs);
            this.DBB.CheckStateChanged += new System.EventHandler(this.debugs);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Huawei
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(693, 380);
            this.Controls.Add(this.DBB);
            this.Controls.Add(this.LOGGER);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Huawei";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Huawei Unlock Tool v1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox LOGGER;
        private System.Windows.Forms.CheckBox DBB;
        private System.Windows.Forms.Timer timer1;
    }
}

