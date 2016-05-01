using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;
using System.Threading;
using System.Net;
using System.IO;
using NReco.VideoConverter;

namespace RainDrop
{
    public partial class RainDrop : Form
    {
        private string link;
        private string path;
        private string finalPath;
        private FileType preferType;
        public enum FileType { MP3, AAC, OGG, AC3 };
        //private bool completed = true;
        private bool aborted = false;
        private bool errored = false;
        private bool completed = false;
        private Thread downloadThread;

        public RainDrop()
        {
            InitializeComponent();
        }
    
        public RainDrop(string link, string path, FileType prefer = FileType.MP3)
        {
            InitializeComponent();
            this.link = link;
            this.path = path;
            this.preferType = prefer;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.AllowTransparency = true;
            //this.TransparencyKey = Color.Black;
            //this.BackColor = Color.Black;
            //this.BackColor = Color.Transparent;
            //this.Opacity = 0.5;
        }

        private void RainDrop_Load(object sender, EventArgs e)
        {
            downloadThread = new Thread(new ThreadStart(download));
            downloadThread.Start();
        }

/*        private VideoInfo getVideoLink(string link) {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link,false);
            VideoInfo video;
            switch(preferType) {
                case FileType.AC3:
                    video = videoInfos.Where(info => info.VideoType == VideoType.Mp4 && info.AudioBitrate >= 192).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                    break;
                case FileType.AAC:
                    video = videoInfos.Where(info => info.AdaptiveType == AdaptiveType.Audio && info.AudioExtension.ToString().Equals(".aac", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                    break;
                case FileType.OGG:
                    video = videoInfos.Where(info => info.AdaptiveType == AdaptiveType.Audio && info.AudioExtension.ToString().Equals(".ogg", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault(); //.ogg - works
                    break;
                case FileType.MP3:
                    video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                    break;
                default:
                    //video = videoInfos.Where(info => info.CanExtractAudio && info.AudioExtension.ToString().Equals("mp3",StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                    video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                    break;
            }

            //if (video == null)
            //{
                /*if (this.preferType != FileType.AAC)
                {
                    this.preferType = FileType.AAC;
                    ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                    {
                        ((Condenser)this.MdiParent).outputBox.AppendText("Failed to find preferred file type... trying .AAC" + Environment.NewLine +
                             this.link + Environment.NewLine + Environment.NewLine, Color.Orange);
                    });
                    video = videoInfos.Where(info => info.AdaptiveType == AdaptiveType.Audio && info.AudioExtension.ToString().Equals(".aac", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                }
            if (video == null)
            {
                if (this.preferType != FileType.MP3)
                {
                    this.preferType = FileType.MP3;
                    ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                    {
                        ((Condenser)this.MdiParent).outputBox.AppendText("Failed to find preferred file type... trying .MP3" + Environment.NewLine +
                            this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                    });
                    video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                    /*if (video == null)
                    {
                        this.preferType = FileType.AC3;
                        ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                        {
                            ((Condenser)this.MdiParent).outputBox.AppendText("Failed to find preferred file type... pulling whatever I can!" + Environment.NewLine +
                                this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                        });
                        video = videoInfos.Where(info => info.VideoType == VideoType.Mp4).OrderByDescending(info => info.Resolution).FirstOrDefault();
                    }
                }
            }
            //}
            return video;
        }*/

        private void download()
        {
            try
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link,false);
                VideoInfo video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                switch (this.preferType)
                {
                    case FileType.AC3:
                        video = videoInfos.Where(info => info.VideoType == VideoType.Mp4 && info.AudioBitrate >= 192).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                        break;
                    case FileType.AAC:
                        video = videoInfos.Where(info => info.AdaptiveType == AdaptiveType.Audio && info.AudioExtension.ToString().Equals(".aac", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                        break;
                    case FileType.OGG:
                        video = videoInfos.Where(info => info.AdaptiveType == AdaptiveType.Audio && info.AudioExtension.ToString().Equals(".ogg", StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(info => info.AudioBitrate).FirstOrDefault(); //.ogg - works
                        break;
                    case FileType.MP3:
                        video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                        break;
                    default:
                        video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                        break;
                }

                if (video == null)
                {
                    if (this.preferType != FileType.MP3)
                    {
                        this.preferType = FileType.MP3;
                        ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                        {
                            ((Condenser)this.MdiParent).outputBox.AppendText("Failed to find preferred file type... trying .MP3" + Environment.NewLine +
                                this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                        });
                        video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).FirstOrDefault();
                        /*if (video == null)
                        {
                            this.preferType = FileType.AC3;
                            ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                            {
                                ((Condenser)this.MdiParent).outputBox.AppendText("Failed to find preferred file type... pulling whatever I can!" + Environment.NewLine +
                                    this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                            });
                            video = videoInfos.Where(info => info.VideoType == VideoType.Mp4).OrderByDescending(info => info.Resolution).FirstOrDefault();
                        }*/
                    }
                }

                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }
                var invalidChars = System.IO.Path.GetInvalidFileNameChars();
                var validFileName = new String(video.Title.Where(x => !invalidChars.Contains(x)).ToArray());

                if (this.preferType != FileType.MP3)
                {
                    var audioDownloader = new VideoDownloader(video, path + validFileName + "." + video.AudioType.ToString().ToLower());
                    this.finalPath = path + validFileName + "." + video.AudioType.ToString().ToLower();
                    if (this.preferType == FileType.AC3)
                    {
                        audioDownloader = new VideoDownloader(video, path + validFileName + ".raindrop");// + video.AudioType.ToString().ToLower());
                        this.finalPath = path + validFileName + ".raindrop";
                        if (File.Exists(path + validFileName + ".ac3"))
                        {
                            ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                            {
                                ((Condenser)this.MdiParent).outputBox.AppendText(validFileName + " is already in the bucket... skipping." + Environment.NewLine +
                                     this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                            });
                            this.completed = true;
                            this.Invoke((MethodInvoker)delegate { this.Close(); /*this.completed = true;*/ });
                            return;
                        }
                    }
                    
                    if (File.Exists(this.finalPath))
                    {
                        ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                        {
                            ((Condenser)this.MdiParent).outputBox.AppendText(validFileName + " is already in the bucket... skipping." + Environment.NewLine +
                                 this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                        });
                        this.completed = true;
                        this.Invoke((MethodInvoker)delegate { this.Close(); /*this.completed = true;*/ });
                        return;
                    }
                    //var audioDownloader = new VideoDownloader(video, path + validFileName + "." + video.AudioExtension);
                    //var audioDownloader = new VideoDownloader(video, path + validFileName + video.VideoExtension);
                    this.Invoke((MethodInvoker)delegate { this.Text = validFileName; });
                    //w.DownloadFile(video.DownloadUrl, path + validFileName + video.AudioExtension);
                    audioDownloader.DownloadProgressChanged += (thesender, args) => this.Invoke((MethodInvoker) delegate { this.progressBar1.Value = (int)(args.ProgressPercentage);});/* * 0.85*/
                    //audioDownloader.AudioExtractionProgressChanged += (thesender, args) => this.progressBar1.Value = (int)((args.ProgressPercentage * 0.15) + 85);
                    audioDownloader.Execute();
                    if (preferType == FileType.AC3)
                    {
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        this.Invoke((MethodInvoker)delegate { this.Text = "[Converting...] " + this.Text; });
                        //ffMpeg.ConvertProgress += (o, args) => this.Text = String.Format("Converting: {0} / {1}", args.Processed.ToString("mm:ss"), args.TotalDuration.ToString("mm:ss"));
                        ffMpeg.ConvertMedia(path + validFileName + ".raindrop", path + validFileName + ".ac3", "ac3");
                        try
                        {
                            File.Delete(path + validFileName + ".raindrop");
                        }
                        catch { }
                    }
                    this.completed = true;
                    ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                    {
                        ((Condenser)this.MdiParent).outputBox.AppendText(validFileName + " completed successfully." + Environment.NewLine +
                             this.link + Environment.NewLine + Environment.NewLine, Color.Lime);

                    });
                }
                else
                {
                    var audioDownloader = new AudioDownloader(video, path + validFileName + video.AudioExtension);
                    this.finalPath = path + validFileName + "." + video.AudioExtension;
                    if (File.Exists(this.finalPath))
                    {
                        ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                        {
                            ((Condenser)this.MdiParent).outputBox.AppendText(validFileName + " is already in the bucket... skipping." + Environment.NewLine +
                                 this.link + Environment.NewLine + Environment.NewLine, Color.Orange);

                        });
                        this.completed = true;
                        this.Invoke((MethodInvoker)delegate { this.Close(); /*this.completed = true;*/ });
                        return;
                    }
                    this.Invoke((MethodInvoker)delegate { this.Text = validFileName; });
                    audioDownloader.DownloadProgressChanged += (thesender, args) => this.progressBar1.Value = (int)(args.ProgressPercentage * 0.85);
                    audioDownloader.AudioExtractionProgressChanged += (thesender, args) => this.progressBar1.Value = (int)((args.ProgressPercentage * 0.15) + 85);
                    audioDownloader.Execute();
                    this.completed = true;
                    ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                    {
                        ((Condenser)this.MdiParent).outputBox.AppendText(validFileName + " completed successfully." + Environment.NewLine +
                             this.link + Environment.NewLine + Environment.NewLine,Color.Lime);

                    });
                }
                this.Invoke((MethodInvoker)delegate { this.Close(); /*this.completed = true;*/ });
            }
            catch(Exception err)
            {
                if (!this.aborted)
                {
                    try
                    {
                        this.errored = true;
                        //if (!(err is InvalidOperationException))
                        //{
                            ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                            {
                                ((Condenser)this.MdiParent).outputBox.AppendText(this.link + " failed to condense." + Environment.NewLine +
                                    err.Message.ToString() + Environment.NewLine + Environment.NewLine, Color.Red);
                                /*((Condenser)this.MdiParent).outputBox.AppendText(this.link + " failed to condense." + Environment.NewLine +
                                    err.ToString() + Environment.NewLine + Environment.NewLine, Color.Red);*/
                            });
                        //}
                        for (int x = 3; x > 0; x--)
                        {
                            string evaporate = "Failed to Condense [" + this.link + "]" + " Evaporating in " + x + " second";
                            evaporate += (x == 1) ? "" : "s";
                            evaporate += "...";
                            this.Invoke((MethodInvoker)delegate { this.Text = evaporate; });
                            Thread.Sleep(1000);
                        }
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Close();
                        });
                    }
                    catch { }
                }
            }
        }

        private void RainDrop_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (preferType != FileType.MP3 && !this.completed)
            {
                try
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Thread.Sleep(4000);
                        try
                        {
                            if (finalPath != null)
                                System.IO.File.Delete(finalPath);
                        }
                        catch { }
                    })).Start();
                }
                catch (Exception) { /*MessageBox.Show("Failed to delete: " + ugh.Message.ToString());*/ }
            }
        }

        private void RainDrop_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                this.aborted = true;
            try
            {
                if (this.aborted)
                {
                    if (!this.errored && !this.completed)
                    {
                        try
                        {
                            downloadThread.Abort();
                        }
                        catch { }
                        ((Condenser)this.MdiParent).Invoke((MethodInvoker)delegate
                        {
                            ((Condenser)this.MdiParent).outputBox.AppendText(this.link + " was aborted." + Environment.NewLine + Environment.NewLine, Color.Orange);
                        });
                    }
                    
                }
            }
            catch { }
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }
    }
}
