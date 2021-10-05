﻿
using System;

namespace CarPostClient
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.notifyIconWork = new System.Windows.Forms.NotifyIcon(this.components);
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // notifyIconWork
            // 
            this.notifyIconWork.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconWork.Icon")));
            this.notifyIconWork.Text = "Идёт передача данных";
            this.notifyIconWork.Visible = true;
            this.notifyIconWork.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconWork_MouseDoubleClick);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(800, 450);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.WordWrap = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxLog);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.Text = "CarPostClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconWork;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
    }
}

