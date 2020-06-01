namespace CameraCapture
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCamIndex = new System.Windows.Forms.ComboBox();
            this.camImageBox = new Emgu.CV.UI.ImageBox();
            this.facesNumLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fpsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.camImageBox)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(466, 27);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(121, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(466, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select Camera";
            // 
            // cbCamIndex
            // 
            this.cbCamIndex.FormattingEnabled = true;
            this.cbCamIndex.Items.AddRange(new object[] {
            "0",
            "1"});
            this.cbCamIndex.Location = new System.Drawing.Point(466, 69);
            this.cbCamIndex.Name = "cbCamIndex";
            this.cbCamIndex.Size = new System.Drawing.Size(121, 21);
            this.cbCamIndex.TabIndex = 3;
            this.cbCamIndex.SelectedIndexChanged += new System.EventHandler(this.cbCamIndex_SelectedIndexChanged);
            // 
            // camImageBox
            // 
            this.camImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.camImageBox.Location = new System.Drawing.Point(12, 27);
            this.camImageBox.Name = "camImageBox";
            this.camImageBox.Size = new System.Drawing.Size(448, 309);
            this.camImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.camImageBox.TabIndex = 2;
            this.camImageBox.TabStop = false;
            // 
            // facesNumLabel
            // 
            this.facesNumLabel.AutoSize = true;
            this.facesNumLabel.Location = new System.Drawing.Point(466, 102);
            this.facesNumLabel.Name = "facesNumLabel";
            this.facesNumLabel.Size = new System.Drawing.Size(68, 13);
            this.facesNumLabel.TabIndex = 5;
            this.facesNumLabel.Text = "Faces: None";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(597, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(466, 126);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(59, 13);
            this.fpsLabel.TabIndex = 7;
            this.fpsLabel.Text = "FPS: None";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 358);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.facesNumLabel);
            this.Controls.Add(this.camImageBox);
            this.Controls.Add(this.cbCamIndex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.camImageBox)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCamIndex;
        private Emgu.CV.UI.ImageBox camImageBox;
        private System.Windows.Forms.Label facesNumLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Label fpsLabel;
    }
}