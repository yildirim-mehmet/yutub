namespace yutub
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnPlay = new Button();
            btnPause = new Button();
            btnStop = new Button();
            btnNext = new Button();
            btnPrevious = new Button();
            btnForward = new Button();
            btnRewind = new Button();
            txtUrl = new TextBox();
            lblUrl = new Label();
            lblStatus = new Label();
            lblCurrentTrack = new Label();
            progressBar = new ProgressBar();
            btnLoadPlaylist = new Button();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            pencereyiAçToolStripMenuItem = new ToolStripMenuItem();
            çalDuraklatToolStripMenuItem = new ToolStripMenuItem();
            durdurToolStripMenuItem = new ToolStripMenuItem();
            sonrakiŞarkıToolStripMenuItem = new ToolStripMenuItem();
            öncekiŞarkıToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            çıkışToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnPlay
            // 
            btnPlay.Location = new Point(105, 64);
            btnPlay.Margin = new Padding(4, 3, 4, 3);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(88, 27);
            btnPlay.TabIndex = 0;
            btnPlay.Text = "Çal";
            btnPlay.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            btnPause.Location = new Point(199, 64);
            btnPause.Margin = new Padding(4, 3, 4, 3);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(88, 27);
            btnPause.TabIndex = 1;
            btnPause.Text = "Duraklat";
            btnPause.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(294, 64);
            btnStop.Margin = new Padding(4, 3, 4, 3);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(88, 27);
            btnStop.TabIndex = 2;
            btnStop.Text = "Durdur";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(389, 64);
            btnNext.Margin = new Padding(4, 3, 4, 3);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(88, 27);
            btnNext.TabIndex = 3;
            btnNext.Text = "Sonraki";
            btnNext.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            btnPrevious.Location = new Point(483, 64);
            btnPrevious.Margin = new Padding(4, 3, 4, 3);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(88, 27);
            btnPrevious.TabIndex = 4;
            btnPrevious.Text = "Önceki";
            btnPrevious.UseVisualStyleBackColor = true;
            // 
            // btnForward
            // 
            btnForward.Location = new Point(577, 64);
            btnForward.Margin = new Padding(4, 3, 4, 3);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(88, 27);
            btnForward.TabIndex = 5;
            btnForward.Text = "İleri Sar >>";
            btnForward.UseVisualStyleBackColor = true;
            // 
            // btnRewind
            // 
            btnRewind.Location = new Point(11, 64);
            btnRewind.Margin = new Padding(4, 3, 4, 3);
            btnRewind.Name = "btnRewind";
            btnRewind.Size = new Size(88, 27);
            btnRewind.TabIndex = 6;
            btnRewind.Text = "<< Geri Sar";
            btnRewind.UseVisualStyleBackColor = true;
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(105, 7);
            txtUrl.Margin = new Padding(4, 3, 4, 3);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(466, 23);
            txtUrl.TabIndex = 7;
            txtUrl.Text = "https://www.youtube.com/playlist?list=OLAK5uy_nq89mp71tlDjZfclnbvZGDQ1H4OPtjOas";
            // 
            // lblUrl
            // 
            lblUrl.AutoSize = true;
            lblUrl.Location = new Point(7, 10);
            lblUrl.Margin = new Padding(4, 0, 4, 0);
            lblUrl.Name = "lblUrl";
            lblUrl.Size = new Size(79, 15);
            lblUrl.TabIndex = 8;
            lblUrl.Text = "Çalma Listesi:";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(7, 110);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(47, 15);
            lblStatus.TabIndex = 9;
            lblStatus.Text = "Durum:";
            // 
            // lblCurrentTrack
            // 
            lblCurrentTrack.AutoSize = true;
            lblCurrentTrack.Location = new Point(7, 145);
            lblCurrentTrack.Margin = new Padding(4, 0, 4, 0);
            lblCurrentTrack.Name = "lblCurrentTrack";
            lblCurrentTrack.Size = new Size(90, 15);
            lblCurrentTrack.TabIndex = 10;
            lblCurrentTrack.Text = "Çalan Şarkı: Yok";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(11, 180);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(654, 27);
            progressBar.TabIndex = 11;
            // 
            // btnLoadPlaylist
            // 
            btnLoadPlaylist.Location = new Point(579, 4);
            btnLoadPlaylist.Margin = new Padding(4, 3, 4, 3);
            btnLoadPlaylist.Name = "btnLoadPlaylist";
            btnLoadPlaylist.Size = new Size(88, 27);
            btnLoadPlaylist.TabIndex = 12;
            btnLoadPlaylist.Text = "Listeyi Yükle";
            btnLoadPlaylist.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "YouTube Müzik Çalar";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.BalloonTipTitle = "YouTube Müzik Çalar";
            this.notifyIcon1.BalloonTipText = "Uygulama çalışıyor";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { pencereyiAçToolStripMenuItem, çalDuraklatToolStripMenuItem, durdurToolStripMenuItem, sonrakiŞarkıToolStripMenuItem, öncekiŞarkıToolStripMenuItem, toolStripSeparator1, çıkışToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(143, 142);
            // 
            // pencereyiAçToolStripMenuItem
            // 
            pencereyiAçToolStripMenuItem.Name = "pencereyiAçToolStripMenuItem";
            pencereyiAçToolStripMenuItem.Size = new Size(142, 22);
            pencereyiAçToolStripMenuItem.Text = "Pencereyi Aç";
            // 
            // çalDuraklatToolStripMenuItem
            // 
            çalDuraklatToolStripMenuItem.Name = "çalDuraklatToolStripMenuItem";
            çalDuraklatToolStripMenuItem.Size = new Size(142, 22);
            çalDuraklatToolStripMenuItem.Text = "Çal/Duraklat";
            // 
            // durdurToolStripMenuItem
            // 
            durdurToolStripMenuItem.Name = "durdurToolStripMenuItem";
            durdurToolStripMenuItem.Size = new Size(142, 22);
            durdurToolStripMenuItem.Text = "Durdur";
            // 
            // sonrakiŞarkıToolStripMenuItem
            // 
            sonrakiŞarkıToolStripMenuItem.Name = "sonrakiŞarkıToolStripMenuItem";
            sonrakiŞarkıToolStripMenuItem.Size = new Size(142, 22);
            sonrakiŞarkıToolStripMenuItem.Text = "Sonraki Şarkı";
            // 
            // öncekiŞarkıToolStripMenuItem
            // 
            öncekiŞarkıToolStripMenuItem.Name = "öncekiŞarkıToolStripMenuItem";
            öncekiŞarkıToolStripMenuItem.Size = new Size(142, 22);
            öncekiŞarkıToolStripMenuItem.Text = "Önceki Şarkı";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(139, 6);
            // 
            // çıkışToolStripMenuItem
            // 
            çıkışToolStripMenuItem.Name = "çıkışToolStripMenuItem";
            çıkışToolStripMenuItem.Size = new Size(142, 22);
            çıkışToolStripMenuItem.Text = "Çıkış";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(670, 214);
            Controls.Add(btnLoadPlaylist);
            Controls.Add(progressBar);
            Controls.Add(lblCurrentTrack);
            Controls.Add(lblStatus);
            Controls.Add(lblUrl);
            Controls.Add(txtUrl);
            Controls.Add(btnRewind);
            Controls.Add(btnForward);
            Controls.Add(btnPrevious);
            Controls.Add(btnNext);
            Controls.Add(btnStop);
            Controls.Add(btnPause);
            Controls.Add(btnPlay);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "YouTube Müzik Çalar";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblCurrentTrack;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnLoadPlaylist;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pencereyiAçToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem çalDuraklatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem durdurToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sonrakiŞarkıToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem öncekiŞarkıToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem çıkışToolStripMenuItem;
    }
}