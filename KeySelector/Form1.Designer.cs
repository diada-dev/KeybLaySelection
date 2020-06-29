namespace KeySelector
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.COMcombo = new System.Windows.Forms.ComboBox();
            this.OpenButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // COMcombo
            // 
            this.COMcombo.FormattingEnabled = true;
            this.COMcombo.Location = new System.Drawing.Point(12, 12);
            this.COMcombo.Name = "COMcombo";
            this.COMcombo.Size = new System.Drawing.Size(122, 21);
            this.COMcombo.TabIndex = 0;
            this.COMcombo.SelectedIndexChanged += new System.EventHandler(this.OnCOMChange);
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(152, 12);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(75, 23);
            this.OpenButton.TabIndex = 1;
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.COMOpen);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 257);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.COMcombo);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "KeyboardSelector";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Deactivate += new System.EventHandler(this.OnDeactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox COMcombo;
        private System.Windows.Forms.Button OpenButton;
    }
}

