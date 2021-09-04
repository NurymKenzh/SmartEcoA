
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
            this.backgroundWorkerDividePostDatas = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxGetPostsData = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.textBoxDividePostsDatas = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.backgroundWorkerGetPostsData = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorkerDividePostDatas
            // 
            this.backgroundWorkerDividePostDatas.WorkerSupportsCancellation = true;
            this.backgroundWorkerDividePostDatas.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDividePostDatas_DoWork);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabControl2);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 422);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Посты мониторинга";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(786, 416);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textBoxGetPostsData);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(778, 388);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Получение";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBoxGetPostsData
            // 
            this.textBoxGetPostsData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxGetPostsData.Location = new System.Drawing.Point(3, 3);
            this.textBoxGetPostsData.Multiline = true;
            this.textBoxGetPostsData.Name = "textBoxGetPostsData";
            this.textBoxGetPostsData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxGetPostsData.Size = new System.Drawing.Size(772, 382);
            this.textBoxGetPostsData.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.textBoxDividePostsDatas);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(778, 388);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Разделение";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // textBoxDividePostsDatas
            // 
            this.textBoxDividePostsDatas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDividePostsDatas.Location = new System.Drawing.Point(3, 3);
            this.textBoxDividePostsDatas.Multiline = true;
            this.textBoxDividePostsDatas.Name = "textBoxDividePostsDatas";
            this.textBoxDividePostsDatas.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDividePostsDatas.Size = new System.Drawing.Size(772, 382);
            this.textBoxDividePostsDatas.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(778, 388);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Усреднение";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 422);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Автомобильные посты";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // backgroundWorkerGetPostsData
            // 
            this.backgroundWorkerGetPostsData.WorkerSupportsCancellation = true;
            this.backgroundWorkerGetPostsData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerGetPostsData_DoWork);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMain";
            this.Text = "SmartEcoA";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorkerDividePostDatas;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox textBoxDividePostsDatas;
        private System.ComponentModel.BackgroundWorker backgroundWorkerGetPostsData;
        private System.Windows.Forms.TextBox textBoxGetPostsData;
    }
}

