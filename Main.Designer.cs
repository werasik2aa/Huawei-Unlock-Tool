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
            this.SuspendLayout();
            // 
            // gs
            // 
            this.gs.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.gs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gs.Location = new System.Drawing.Point(12, 12);
            this.gs.Name = "gs";
            this.gs.Size = new System.Drawing.Size(902, 37);
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
            this.UnlockTool.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.UnlockTool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnlockTool.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UnlockTool.Location = new System.Drawing.Point(12, 55);
            this.UnlockTool.Name = "UnlockTool";
            this.UnlockTool.Size = new System.Drawing.Size(902, 37);
            this.UnlockTool.TabIndex = 1;
            this.UnlockTool.Text = "UnlockTool";
            this.UnlockTool.UseVisualStyleBackColor = false;
            this.UnlockTool.Click += new System.EventHandler(this.UnlockTool_Click);
            // 
            // Huawei
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(924, 107);
            this.Controls.Add(this.UnlockTool);
            this.Controls.Add(this.gs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Huawei";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Huawei Unlock Tool v1";
            this.Deactivate += new System.EventHandler(this.Huawei_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button gs;
        private System.Windows.Forms.Timer Checker;
        private System.Windows.Forms.Button UnlockTool;
    }
}

