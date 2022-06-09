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
            this.button1 = new System.Windows.Forms.Button();
            this.ISA = new System.Windows.Forms.CheckBox();
            this.Loader_text = new System.Windows.Forms.Label();
            this.Loaders = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DETECTED = new System.Windows.Forms.Label();
            this.DEVICER = new System.Windows.Forms.ComboBox();
            this.PORTER = new System.Windows.Forms.TextBox();
            this.REPEAT = new System.Windows.Forms.Timer(this.components);
            this.DEBUGER = new System.Windows.Forms.CheckBox();
            this.LOGGER = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Black", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.button1.Location = new System.Drawing.Point(12, 458);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(902, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "Unlock";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ISA
            // 
            this.ISA.AutoSize = true;
            this.ISA.Checked = true;
            this.ISA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ISA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ISA.Font = new System.Drawing.Font("Arial", 13.2F, System.Drawing.FontStyle.Bold);
            this.ISA.ForeColor = System.Drawing.Color.MintCream;
            this.ISA.Location = new System.Drawing.Point(60, 69);
            this.ISA.Name = "ISA";
            this.ISA.Size = new System.Drawing.Size(166, 30);
            this.ISA.TabIndex = 1;
            this.ISA.Text = "Automatically";
            this.ISA.UseVisualStyleBackColor = true;
            // 
            // Loader_text
            // 
            this.Loader_text.AutoSize = true;
            this.Loader_text.Cursor = System.Windows.Forms.Cursors.Default;
            this.Loader_text.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Loader_text.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Loader_text.ForeColor = System.Drawing.Color.MintCream;
            this.Loader_text.Location = new System.Drawing.Point(74, 12);
            this.Loader_text.Name = "Loader_text";
            this.Loader_text.Size = new System.Drawing.Size(133, 24);
            this.Loader_text.TabIndex = 3;
            this.Loader_text.Text = "Select loader";
            // 
            // Loaders
            // 
            this.Loaders.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.Loaders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Loaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Loaders.Font = new System.Drawing.Font("Arial", 9.8F, System.Drawing.FontStyle.Bold);
            this.Loaders.FormattingEnabled = true;
            this.Loaders.Location = new System.Drawing.Point(12, 39);
            this.Loaders.Name = "Loaders";
            this.Loaders.Size = new System.Drawing.Size(286, 27);
            this.Loaders.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.MintCream;
            this.label1.Location = new System.Drawing.Point(692, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select DEVICE";
            // 
            // DETECTED
            // 
            this.DETECTED.AutoSize = true;
            this.DETECTED.Cursor = System.Windows.Forms.Cursors.Default;
            this.DETECTED.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DETECTED.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DETECTED.ForeColor = System.Drawing.Color.MintCream;
            this.DETECTED.Location = new System.Drawing.Point(363, 9);
            this.DETECTED.Name = "DETECTED";
            this.DETECTED.Size = new System.Drawing.Size(197, 24);
            this.DETECTED.TabIndex = 7;
            this.DETECTED.Text = "CONNECTED PORT";
            // 
            // DEVICER
            // 
            this.DEVICER.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.DEVICER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEVICER.Font = new System.Drawing.Font("Arial", 9.8F, System.Drawing.FontStyle.Bold);
            this.DEVICER.Location = new System.Drawing.Point(628, 40);
            this.DEVICER.Name = "DEVICER";
            this.DEVICER.Size = new System.Drawing.Size(286, 27);
            this.DEVICER.TabIndex = 8;
            this.DEVICER.Text = "ATU-L31";
            // 
            // PORTER
            // 
            this.PORTER.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.PORTER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PORTER.Location = new System.Drawing.Point(345, 39);
            this.PORTER.Name = "PORTER";
            this.PORTER.ReadOnly = true;
            this.PORTER.Size = new System.Drawing.Size(238, 28);
            this.PORTER.TabIndex = 2;
            this.PORTER.Text = "Connect Your device";
            this.PORTER.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // REPEAT
            // 
            this.REPEAT.Tick += new System.EventHandler(this.Search_PORT);
            // 
            // DEBUGER
            // 
            this.DEBUGER.AutoSize = true;
            this.DEBUGER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DEBUGER.Font = new System.Drawing.Font("Arial", 13.2F, System.Drawing.FontStyle.Bold);
            this.DEBUGER.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.DEBUGER.Location = new System.Drawing.Point(744, 69);
            this.DEBUGER.Name = "DEBUGER";
            this.DEBUGER.Size = new System.Drawing.Size(163, 30);
            this.DEBUGER.TabIndex = 10;
            this.DEBUGER.Text = "DEBUG LOG";
            this.DEBUGER.UseVisualStyleBackColor = true;
            // 
            // LOGGER
            // 
            this.LOGGER.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.LOGGER.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LOGGER.Location = new System.Drawing.Point(12, 105);
            this.LOGGER.Multiline = true;
            this.LOGGER.Name = "LOGGER";
            this.LOGGER.ReadOnly = true;
            this.LOGGER.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LOGGER.Size = new System.Drawing.Size(902, 347);
            this.LOGGER.TabIndex = 9;
            // 
            // Huawei
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(926, 513);
            this.Controls.Add(this.LOGGER);
            this.Controls.Add(this.DEBUGER);
            this.Controls.Add(this.DEVICER);
            this.Controls.Add(this.DETECTED);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Loaders);
            this.Controls.Add(this.Loader_text);
            this.Controls.Add(this.PORTER);
            this.Controls.Add(this.ISA);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Huawei";
            this.Text = "Huawei Unlock Tool v1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox ISA;
        private System.Windows.Forms.Label Loader_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DETECTED;
        private System.Windows.Forms.Timer REPEAT;
        private System.Windows.Forms.CheckBox DEBUGER;
        private System.Windows.Forms.ComboBox Loaders;
        private System.Windows.Forms.ComboBox DEVICER;
        private System.Windows.Forms.TextBox PORTER;
        private System.Windows.Forms.TextBox LOGGER;
    }
}

