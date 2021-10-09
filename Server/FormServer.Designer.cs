
namespace Server
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxPostsData = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelPostsStartStop = new System.Windows.Forms.Label();
            this.buttonPostsStartStop = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBoxCarPosts = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelCarPostsStartStop = new System.Windows.Forms.Label();
            this.buttonCarPostsStartStop = new System.Windows.Forms.Button();
            this.backgroundWorkerPosts = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerCarPosts = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 528);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 500);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Посты мониторинга";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxPostsData);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 38);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 459);
            this.panel2.TabIndex = 5;
            // 
            // textBoxPostsData
            // 
            this.textBoxPostsData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPostsData.Location = new System.Drawing.Point(0, 0);
            this.textBoxPostsData.Multiline = true;
            this.textBoxPostsData.Name = "textBoxPostsData";
            this.textBoxPostsData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPostsData.Size = new System.Drawing.Size(786, 459);
            this.textBoxPostsData.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelPostsStartStop);
            this.panel1.Controls.Add(this.buttonPostsStartStop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 35);
            this.panel1.TabIndex = 4;
            // 
            // labelPostsStartStop
            // 
            this.labelPostsStartStop.AutoSize = true;
            this.labelPostsStartStop.Location = new System.Drawing.Point(96, 4);
            this.labelPostsStartStop.Name = "labelPostsStartStop";
            this.labelPostsStartStop.Size = new System.Drawing.Size(56, 15);
            this.labelPostsStartStop.TabIndex = 1;
            this.labelPostsStartStop.Text = "Работает";
            // 
            // buttonPostsStartStop
            // 
            this.buttonPostsStartStop.Location = new System.Drawing.Point(6, 4);
            this.buttonPostsStartStop.Name = "buttonPostsStartStop";
            this.buttonPostsStartStop.Size = new System.Drawing.Size(84, 23);
            this.buttonPostsStartStop.TabIndex = 0;
            this.buttonPostsStartStop.Text = "Остановить";
            this.buttonPostsStartStop.UseVisualStyleBackColor = true;
            this.buttonPostsStartStop.Click += new System.EventHandler(this.buttonPostsStartStop_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel4);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 500);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Автомобильные посты";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.textBoxCarPosts);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 38);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(786, 459);
            this.panel4.TabIndex = 1;
            // 
            // textBoxCarPosts
            // 
            this.textBoxCarPosts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCarPosts.Location = new System.Drawing.Point(0, 0);
            this.textBoxCarPosts.Multiline = true;
            this.textBoxCarPosts.Name = "textBoxCarPosts";
            this.textBoxCarPosts.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCarPosts.Size = new System.Drawing.Size(786, 459);
            this.textBoxCarPosts.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelCarPostsStartStop);
            this.panel3.Controls.Add(this.buttonCarPostsStartStop);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(786, 35);
            this.panel3.TabIndex = 0;
            // 
            // labelCarPostsStartStop
            // 
            this.labelCarPostsStartStop.AutoSize = true;
            this.labelCarPostsStartStop.Location = new System.Drawing.Point(95, 3);
            this.labelCarPostsStartStop.Name = "labelCarPostsStartStop";
            this.labelCarPostsStartStop.Size = new System.Drawing.Size(56, 15);
            this.labelCarPostsStartStop.TabIndex = 3;
            this.labelCarPostsStartStop.Text = "Работает";
            // 
            // buttonCarPostsStartStop
            // 
            this.buttonCarPostsStartStop.Location = new System.Drawing.Point(5, 3);
            this.buttonCarPostsStartStop.Name = "buttonCarPostsStartStop";
            this.buttonCarPostsStartStop.Size = new System.Drawing.Size(84, 23);
            this.buttonCarPostsStartStop.TabIndex = 2;
            this.buttonCarPostsStartStop.Text = "Остановить";
            this.buttonCarPostsStartStop.UseVisualStyleBackColor = true;
            this.buttonCarPostsStartStop.Click += new System.EventHandler(this.buttonCarPostsStartStop_Click);
            // 
            // backgroundWorkerPosts
            // 
            this.backgroundWorkerPosts.WorkerSupportsCancellation = true;
            this.backgroundWorkerPosts.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPosts_DoWork);
            this.backgroundWorkerPosts.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPosts_RunWorkerCompleted);
            // 
            // backgroundWorkerCarPosts
            // 
            this.backgroundWorkerCarPosts.WorkerSupportsCancellation = true;
            this.backgroundWorkerCarPosts.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCarPosts_DoWork);
            this.backgroundWorkerCarPosts.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerCarPosts_RunWorkerCompleted);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 528);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMain";
            this.Text = "SmartEcoA";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPosts;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxPostsData;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelPostsStartStop;
        private System.Windows.Forms.Button buttonPostsStartStop;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelCarPostsStartStop;
        private System.Windows.Forms.Button buttonCarPostsStartStop;
        private System.ComponentModel.BackgroundWorker backgroundWorkerCarPosts;
        private System.Windows.Forms.TextBox textBoxCarPosts;
    }
}

