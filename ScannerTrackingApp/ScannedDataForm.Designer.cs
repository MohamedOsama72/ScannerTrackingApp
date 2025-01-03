
namespace ScannerTrackingApp
{
    partial class ScannedDataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstDataDisplay = new System.Windows.Forms.ListBox();
            this.txtResponseBody = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lstDataDisplay
            // 
            this.lstDataDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstDataDisplay.FormattingEnabled = true;
            this.lstDataDisplay.ItemHeight = 25;
            this.lstDataDisplay.Location = new System.Drawing.Point(2, 16);
            this.lstDataDisplay.Name = "lstDataDisplay";
            this.lstDataDisplay.Size = new System.Drawing.Size(460, 354);
            this.lstDataDisplay.TabIndex = 0;
            // 
            // txtResponseBody
            // 
            this.txtResponseBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.txtResponseBody.Location = new System.Drawing.Point(468, 16);
            this.txtResponseBody.Multiline = true;
            this.txtResponseBody.Name = "txtResponseBody";
            this.txtResponseBody.ReadOnly = true;
            this.txtResponseBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResponseBody.Size = new System.Drawing.Size(320, 354);
            this.txtResponseBody.TabIndex = 1;
            // 
            // ScannedDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lstDataDisplay);
            this.Controls.Add(this.txtResponseBody);
            this.Name = "ScannedDataForm";
            this.Text = "ScannedDataForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstDataDisplay;
        private System.Windows.Forms.TextBox txtResponseBody;
    }
}