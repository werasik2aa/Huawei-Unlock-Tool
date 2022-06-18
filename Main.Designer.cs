namespace HuaweiUnlocker
{
    partial class Huawei
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Huawei));
            this.gs = new System.Windows.Forms.Button();
            this.Checker = new System.Windows.Forms.Timer(this.components);
            this.UnlockTool = new System.Windows.Forms.Button();
            this.LOGGER = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.PORTER = new System.Windows.Forms.TextBox();
            this.Pg = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // gs
            // 
            this.gs.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.gs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gs.Location = new System.Drawing.Point(12, 12);
            this.gs.Name = "gs";
            this.gs.Size = new System.Drawing.Size(510, 37);
            this.gs.TabIndex = 0;
            this.gs.Text = "FlashTool";
            this.gs.UseVisualStyleBackColor = false;
            this.gs.Click += new System.EventHandler(this.gs_Click);
            // 
            // Checker
            // 
            this.Checker.Enabled = true;
            this.Checker.Tick += new System.EventHandler(this.Checker_Tick);
            // 
            // UnlockTool
            // 
            this.UnlockTool.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.UnlockTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnlockTool.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UnlockTool.Location = new System.Drawing.Point(12, 55);
            this.UnlockTool.Name = "UnlockTool";
            this.UnlockTool.Size = new System.Drawing.Size(510, 37);
            this.UnlockTool.TabIndex = 1;
            this.UnlockTool.Text = "UnlockTool";
            this.UnlockTool.UseVisualStyleBackColor = false;
            this.UnlockTool.Click += new System.EventHandler(this.UnlockTool_Click);
            // 
            // LOGGER
            // 
            this.LOGGER.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.LOGGER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LOGGER.Location = new System.Drawing.Point(12, 109);
            this.LOGGER.Multiline = true;
            this.LOGGER.Name = "LOGGER";
            this.LOGGER.ReadOnly = true;
            this.LOGGER.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LOGGER.Size = new System.Drawing.Size(902, 347);
            this.LOGGER.TabIndex = 10;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Snow;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox1.Location = new System.Drawing.Point(12, 89);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(114, 23);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Debug Log";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.debugs);
            // 
            // PORTER
            // 
            this.PORTER.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.PORTER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PORTER.Location = new System.Drawing.Point(528, 12);
            this.PORTER.Name = "PORTER";
            this.PORTER.ReadOnly = true;
            this.PORTER.Size = new System.Drawing.Size(386, 28);
            this.PORTER.TabIndex = 21;
            this.PORTER.Text = "Connect Your device";
            this.PORTER.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Pg
            // 
            this.Pg.Location = new System.Drawing.Point(528, 55);
            this.Pg.Name = "Pg";
            this.Pg.Size = new System.Drawing.Size(386, 37);
            this.Pg.TabIndex = 22;
            // 
            // Huawei
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(924, 468);
            this.Controls.Add(this.Pg);
            this.Controls.Add(this.PORTER);
            this.Controls.Add(this.LOGGER);
            this.Controls.Add(this.UnlockTool);
            this.Controls.Add(this.gs);
            this.Controls.Add(this.checkBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Huawei";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Huawei Unlock Tool v1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button gs;
        private System.Windows.Forms.Timer Checker;
        private System.Windows.Forms.Button UnlockTool;
        private System.Windows.Forms.TextBox LOGGER;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox PORTER;
        private System.Windows.Forms.ProgressBar Pg;
    }
}

