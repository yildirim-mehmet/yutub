using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace yutub
{
    public partial class Form1 : Form
    {
        // SADECE private deðiþkenler - KONTROL TANIMLARI YOK
        private YoutubeClient youtube;
        private List<PlaylistVideo> playlistVideos;
        private int currentTrackIndex = 0;
        private MediaFoundationReader mediaFoundationReader;
        private WaveOutEvent waveOut;
        private bool isPlaying = false;
        private bool isManualStop = false;
        private bool isLoading = false;
        private CancellationTokenSource progressCancellation;
        private CancellationTokenSource playbackCancellation;
        private readonly object playbackLock = new object();
        private string currentStreamUrl = string.Empty;
        private ConcurrentDictionary<string, string> streamUrlCache;
        private bool isDisposing = false;
        private bool minimizeToTray = true;

        public Form1()
        {
            InitializeComponent();

            // Basit bir icon oluþtur (eðer resources'ta icon yoksa)
            if (notifyIcon1.Icon == null)
            {
                CreateTrayIcon();
            }

            // Diðer ayarlar...
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 10000;

            youtube = new YoutubeClient();
            playlistVideos = new List<PlaylistVideo>();
            streamUrlCache = new ConcurrentDictionary<string, string>();
            progressCancellation = new CancellationTokenSource();
            playbackCancellation = new CancellationTokenSource();

            AttachEventHandlers();
            UpdateButtonStates();
            UpdateTrayMenu();
        }

        private void CreateTrayIcon()
        {
            try
            {
                // Basit bir bitmap oluþtur ve icon'a dönüþtür
                using (Bitmap bmp = new Bitmap(16, 16))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Blue);
                    g.FillRectangle(Brushes.White, 4, 4, 8, 8);
                    g.DrawRectangle(Pens.Black, 4, 4, 8, 8);

                    // Bitmap'ten icon oluþtur
                    IntPtr hIcon = bmp.GetHicon();
                    notifyIcon1.Icon = Icon.FromHandle(hIcon);
                    // NOT: Icon'ý dispose etmeyin, notifyIcon kendisi yönetir
                }

                notifyIcon1.Visible = true;
                notifyIcon1.Text = "YouTube Müzik Çalar";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Icon oluþturma hatasý: {ex.Message}");
            }
        }

        private void AttachEventHandlers()
        {
            // Buton event'leri
            btnPlay.Click += btnPlay_Click;
            btnPause.Click += btnPause_Click;
            btnStop.Click += btnStop_Click;
            btnNext.Click += btnNext_Click;
            btnPrevious.Click += btnPrevious_Click;
            btnForward.Click += btnForward_Click;
            btnRewind.Click += btnRewind_Click;
            btnLoadPlaylist.Click += btnLoadPlaylist_Click;

            // NotifyIcon event'leri
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;

            // Context menu event'leri
            pencereyiAçToolStripMenuItem.Click += pencereyiAçToolStripMenuItem_Click;
            çalDuraklatToolStripMenuItem.Click += çalDuraklatToolStripMenuItem_Click;
            durdurToolStripMenuItem.Click += durdurToolStripMenuItem_Click;
            sonrakiÞarkýToolStripMenuItem.Click += sonrakiÞarkýToolStripMenuItem_Click;
            öncekiÞarkýToolStripMenuItem.Click += öncekiÞarkýToolStripMenuItem_Click;
            çýkýþToolStripMenuItem.Click += çýkýþToolStripMenuItem_Click;

            // Form event'leri
            this.Resize += Form1_Resize;
            this.FormClosing += Form1_FormClosing;
        }

        private void UpdateButtonStates()
        {
            if (this.IsDisposed) return;

            bool hasPlaylist = playlistVideos != null && playlistVideos.Count > 0;
            bool isTrackPlaying = waveOut != null && waveOut.PlaybackState == PlaybackState.Playing;
            bool isTrackPaused = waveOut != null && waveOut.PlaybackState == PlaybackState.Paused;

            InvokeSafe(() =>
            {
                btnPlay.Enabled = hasPlaylist && (!isTrackPlaying || isTrackPaused);
                btnPause.Enabled = hasPlaylist && isTrackPlaying;
                btnStop.Enabled = hasPlaylist && (isTrackPlaying || isTrackPaused);
                btnNext.Enabled = hasPlaylist && playlistVideos.Count > 1;
                btnPrevious.Enabled = hasPlaylist && playlistVideos.Count > 1;
                btnForward.Enabled = hasPlaylist && (isTrackPlaying || isTrackPaused);
                btnRewind.Enabled = hasPlaylist && (isTrackPlaying || isTrackPaused);
                btnLoadPlaylist.Enabled = !isLoading;
            });
        }

        private void UpdateTrayMenu()
        {
            InvokeSafe(() =>
            {
                bool isTrackPlaying = waveOut != null && waveOut.PlaybackState == PlaybackState.Playing;
                bool isTrackPaused = waveOut != null && waveOut.PlaybackState == PlaybackState.Paused;
                bool hasPlaylist = playlistVideos != null && playlistVideos.Count > 0;

                // Çal/Duraklat menü metnini güncelle
                çalDuraklatToolStripMenuItem.Text = isTrackPlaying ? "Duraklat" : "Çal";
                çalDuraklatToolStripMenuItem.Enabled = hasPlaylist;

                // Diðer menü öðelerini güncelle
                durdurToolStripMenuItem.Enabled = hasPlaylist && (isTrackPlaying || isTrackPaused);
                sonrakiÞarkýToolStripMenuItem.Enabled = hasPlaylist && playlistVideos.Count > 1;
                öncekiÞarkýToolStripMenuItem.Enabled = hasPlaylist && playlistVideos.Count > 1;

                // NotifyIcon tooltip'ini güncelle
                string statusText = "YouTube Müzik Çalar";
                if (hasPlaylist && currentTrackIndex < playlistVideos.Count && currentTrackIndex >= 0)
                {
                    var currentTrack = playlistVideos[currentTrackIndex];
                    statusText = $"{currentTrack.Title} - {(isTrackPlaying ? "Çalýyor" : isTrackPaused ? "Duraklatýldý" : "Durduruldu")}";
                }
                notifyIcon1.Text = statusText.Length > 63 ? statusText.Substring(0, 60) + "..." : statusText;
            });
        }

        private void InvokeSafe(Action action)
        {
            try
            {
                if (this.IsDisposed || this.Disposing) return;

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (ObjectDisposedException)
            {
                // Form dispose edilmiþ, iþlem yapma
            }
        }

        #region NotifyIcon Event Handlers

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void pencereyiAçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFromTray();
        }

        private void çalDuraklatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                btnPause_Click(sender, e);
            }
            else
            {
                btnPlay_Click(sender, e);
            }
        }

        private void durdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
        }

        private void sonrakiÞarkýToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnNext_Click(sender, e);
        }

        private void öncekiÞarkýToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnPrevious_Click(sender, e);
        }

        private void çýkýþToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ShowFromTray()
        {
            InvokeSafe(() =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.BringToFront();
                this.Activate();
            });
        }

        private void HideToTray()
        {
            InvokeSafe(() =>
            {
                this.Hide();
                this.ShowInTaskbar = false;
                notifyIcon1.ShowBalloonTip(1000, "YouTube Müzik Çalar",
                    "Uygulama sistem tepsisine küçültüldü. Çift týklayarak veya sað týklayarak açabilirsiniz.",
                    ToolTipIcon.Info);
            });
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (minimizeToTray && this.WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kullanýcý X'e basarsa tray'e küçült
            if (e.CloseReason == CloseReason.UserClosing && minimizeToTray)
            {
                e.Cancel = true;
                HideToTray();
                return;
            }

            // Gerçekten kapatma
            isDisposing = true;
            progressCancellation?.Cancel();
            playbackCancellation?.Cancel();

            _ = StopPlaybackAsync(true);

            streamUrlCache?.Clear();
            progressCancellation?.Dispose();
            playbackCancellation?.Dispose();

            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
        }

        #endregion

        #region Music Player Functions

        private async void btnLoadPlaylist_Click(object sender, EventArgs e)
        {
            if (isLoading) return;
            await LoadPlaylist();
        }

        private async void btnPlay_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            try
            {
                if (playlistVideos.Count == 0)
                {
                    await LoadPlaylist();
                    if (playlistVideos.Count == 0) return;
                }

                lock (playbackLock)
                {
                    if (waveOut != null && waveOut.PlaybackState == PlaybackState.Paused)
                    {
                        waveOut.Play();
                        isPlaying = true;
                        UpdateStatus("Çalýyor (Devam)");
                        StartProgressTracking();
                    }
                    else if (waveOut == null || waveOut.PlaybackState == PlaybackState.Stopped)
                    {
                        isManualStop = false;
                        _ = Task.Run(async () => await PlayCurrentTrack());
                    }
                }

                UpdateButtonStates();
                UpdateTrayMenu();
            }
            catch (Exception ex)
            {
                HandleError("Çalma iþlemi sýrasýnda hata", ex);
            }
        }

        private async Task LoadPlaylist()
        {
            var playlistUrl = txtUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(playlistUrl))
            {
                MessageBox.Show("Lütfen bir YouTube çalma listesi URL'si girin.", "Uyarý",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isLoading) return;

            isLoading = true;
            UpdateStatus("Çalma listesi yükleniyor...");
            UpdateButtonStates();
            UpdateTrayMenu();

            try
            {
                await StopPlaybackAsync(true);
                streamUrlCache.Clear();

                youtube = new YoutubeClient();

                await Task.Run(async () =>
                {
                    var videos = await youtube.Playlists.GetVideosAsync(playlistUrl);
                    playlistVideos = videos.ToList();
                });

                UpdateStatus($"{playlistVideos.Count} þarký yüklendi");
                UpdateCurrentTrack("Çalan Þarký: Yok");

                if (playlistVideos.Count == 0)
                {
                    MessageBox.Show("Çalma listesi boþ veya eriþilemiyor.", "Uyarý",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                HandleNetworkError($"Ýnternet baðlantýsý hatasý: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                HandleError("Çalma listesi yüklenirken hata", ex);
            }
            finally
            {
                isLoading = false;
                UpdateButtonStates();
                UpdateTrayMenu();
            }
        }

        private async Task PlayCurrentTrack()
        {
            if (isDisposing) return;

            lock (playbackLock)
            {
                if (currentTrackIndex >= playlistVideos.Count || currentTrackIndex < 0)
                {
                    if (playlistVideos.Count == 0) return;
                    currentTrackIndex = 0;
                }
            }

            PlaylistVideo video;
            lock (playbackLock)
            {
                video = playlistVideos[currentTrackIndex];
            }

            UpdateCurrentTrack($"Çalan Þarký: {video.Title}");
            UpdateStatus("Þarký yükleniyor...");
            UpdateTrayMenu();

            await StopPlaybackAsync(true);

            if (isDisposing) return;

            try
            {
                var streamUrl = await GetStreamUrlAsync(video.Id);
                if (string.IsNullOrEmpty(streamUrl))
                {
                    UpdateStatus("Ses akýþý alýnamadý - Sonrakine geçiliyor");
                    await Task.Delay(1000);
                    await PlayNextTrack();
                    return;
                }

                lock (playbackLock)
                {
                    mediaFoundationReader = new MediaFoundationReader(streamUrl);
                    waveOut = new WaveOutEvent();
                    currentStreamUrl = streamUrl;

                    waveOut.PlaybackStopped += OnPlaybackStopped;
                    waveOut.Init(mediaFoundationReader);

                    waveOut.Play();
                    isPlaying = true;
                    isManualStop = false;
                }

                UpdateStatus("Çalýyor");
                UpdateTrayMenu();
                StartProgressTracking();
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                HandleNetworkError($"Þarký yüklenirken að hatasý: {httpEx.Message}");
                await PlayNextTrack();
            }
            catch (Exception ex)
            {
                HandleError("Þarký çalýnýrken hata", ex);
                await PlayNextTrack();
            }
            finally
            {
                UpdateButtonStates();
                UpdateTrayMenu();
            }
        }

        private async Task<string> GetStreamUrlAsync(string videoId)
        {
            if (streamUrlCache.TryGetValue(videoId, out var cachedUrl))
            {
                return cachedUrl;
            }

            try
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

                var audioStreamInfo = streamManifest.GetAudioOnlyStreams()
                    .OrderByDescending(s => s.Bitrate)
                    .FirstOrDefault();

                if (audioStreamInfo != null)
                {
                    var streamUrl = audioStreamInfo.Url;
                    streamUrlCache[videoId] = streamUrl;
                    return streamUrl;
                }
            }
            catch (Exception ex)
            {
                HandleError($"Stream URL alýnýrken hata (Video: {videoId})", ex);
            }

            return null;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (isDisposing) return;

            _ = Task.Run(async () =>
            {
                if (e?.Exception != null)
                {
                    HandleError("Çalma sýrasýnda hata", e.Exception);
                    await PlayNextTrack();
                    return;
                }

                if (isPlaying && !isManualStop)
                {
                    await Task.Delay(300);
                    await PlayNextTrack();
                }
            });
        }

        private void StartProgressTracking()
        {
            progressCancellation?.Cancel();
            progressCancellation = new CancellationTokenSource();

            var cancellationToken = progressCancellation.Token;

            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested && isPlaying)
                {
                    try
                    {
                        await Task.Delay(500, cancellationToken);
                        if (cancellationToken.IsCancellationRequested) break;

                        MediaFoundationReader reader;
                        WaveOutEvent player;

                        lock (playbackLock)
                        {
                            reader = mediaFoundationReader;
                            player = waveOut;
                        }

                        if (reader == null || player == null) break;

                        if (player.PlaybackState == PlaybackState.Playing || player.PlaybackState == PlaybackState.Paused)
                        {
                            var currentTime = reader.CurrentTime.TotalMilliseconds;
                            var totalTime = reader.TotalTime.TotalMilliseconds;

                            if (totalTime > 0)
                            {
                                var progress = (currentTime / totalTime) * 100;
                                InvokeSafe(() =>
                                {
                                    if (progressBar != null && !progressBar.IsDisposed)
                                    {
                                        progressBar.Value = (int)Math.Min(100, Math.Max(0, progress));
                                    }
                                });
                            }
                        }

                        if (player.PlaybackState == PlaybackState.Stopped && !isPlaying)
                        {
                            break;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Progress tracking error: {ex.Message}");
                        break;
                    }
                }
            }, cancellationToken);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            lock (playbackLock)
            {
                if (waveOut != null)
                {
                    if (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        waveOut.Pause();
                        isPlaying = false;
                        UpdateStatus("Duraklatýldý");
                    }
                    else if (waveOut.PlaybackState == PlaybackState.Paused)
                    {
                        waveOut.Play();
                        isPlaying = true;
                        UpdateStatus("Çalýyor");
                        StartProgressTracking();
                    }

                    UpdateButtonStates();
                    UpdateTrayMenu();
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isManualStop = true;
            _ = StopPlaybackAsync(true);
            UpdateStatus("Durduruldu");
            InvokeSafe(() =>
            {
                progressBar.Value = 0;
                UpdateCurrentTrack("Çalan Þarký: Yok");
            });
            UpdateButtonStates();
            UpdateTrayMenu();
        }

        private async Task StopPlaybackAsync(bool cleanUp)
        {
            isPlaying = false;
            isManualStop = true;

            progressCancellation?.Cancel();

            lock (playbackLock)
            {
                if (waveOut != null)
                {
                    waveOut.PlaybackStopped -= OnPlaybackStopped;
                    waveOut.Stop();
                    if (cleanUp)
                    {
                        waveOut.Dispose();
                        waveOut = null;
                    }
                }

                if (mediaFoundationReader != null && cleanUp)
                {
                    mediaFoundationReader.Dispose();
                    mediaFoundationReader = null;
                }

                currentStreamUrl = string.Empty;
            }

            await Task.Delay(100);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (isLoading) return;
            await PlayNextTrack();
        }

        private async Task PlayNextTrack()
        {
            isManualStop = true;

            lock (playbackLock)
            {
                if (playlistVideos.Count == 0) return;

                currentTrackIndex++;
                if (currentTrackIndex >= playlistVideos.Count)
                {
                    currentTrackIndex = 0;
                }
            }

            await StopPlaybackAsync(true);
            await Task.Delay(200);
            await PlayCurrentTrack();
        }

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (isLoading) return;
            await PlayPreviousTrack();
        }

        private async Task PlayPreviousTrack()
        {
            isManualStop = true;

            lock (playbackLock)
            {
                if (playlistVideos.Count == 0) return;

                currentTrackIndex--;
                if (currentTrackIndex < 0)
                {
                    currentTrackIndex = playlistVideos.Count - 1;
                }
            }

            await StopPlaybackAsync(true);
            await Task.Delay(200);
            await PlayCurrentTrack();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            lock (playbackLock)
            {
                if (mediaFoundationReader != null && waveOut != null && waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    var newPosition = mediaFoundationReader.CurrentTime.Add(TimeSpan.FromSeconds(10));
                    if (newPosition < mediaFoundationReader.TotalTime)
                    {
                        mediaFoundationReader.CurrentTime = newPosition;
                    }
                }
            }
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            lock (playbackLock)
            {
                if (mediaFoundationReader != null && waveOut != null && waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    var newPosition = mediaFoundationReader.CurrentTime.Subtract(TimeSpan.FromSeconds(10));
                    if (newPosition > TimeSpan.Zero)
                    {
                        mediaFoundationReader.CurrentTime = newPosition;
                    }
                    else
                    {
                        mediaFoundationReader.CurrentTime = TimeSpan.Zero;
                    }
                }
            }
        }

        private void UpdateStatus(string status)
        {
            InvokeSafe(() =>
            {
                if (lblStatus != null && !lblStatus.IsDisposed)
                {
                    lblStatus.Text = status;
                }
            });
        }

        private void UpdateCurrentTrack(string trackInfo)
        {
            InvokeSafe(() =>
            {
                if (lblCurrentTrack != null && !lblCurrentTrack.IsDisposed)
                {
                    lblCurrentTrack.Text = trackInfo;
                }
            });
        }

        private void HandleError(string context, Exception ex)
        {
            string errorMessage = $"{context}: {GetUserFriendlyErrorMessage(ex)}";

            System.Diagnostics.Debug.WriteLine($"ERROR: {errorMessage}");

            InvokeSafe(() =>
            {
                UpdateStatus($"Hata: {GetUserFriendlyErrorMessage(ex)}");

                if (ex is System.Net.Http.HttpRequestException || ex is TimeoutException)
                {
                    MessageBox.Show(errorMessage, "Baðlantý Hatasý",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
        }

        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            return ex switch
            {
                System.Net.Http.HttpRequestException => "Ýnternet baðlantýsý hatasý. Lütfen baðlantýnýzý kontrol edin.",
                TimeoutException => "Ýþlem zaman aþýmýna uðradý. Lütfen tekrar deneyin.",
                _ => ex.Message
            };
        }

        private void HandleNetworkError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"NETWORK ERROR: {message}");

            InvokeSafe(() =>
            {
                UpdateStatus("Að baðlantý hatasý");
                MessageBox.Show(message, "Að Hatasý",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
        }

        #endregion
    }
}