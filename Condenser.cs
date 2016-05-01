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
using System.IO;
using System.Reflection;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace RainDrop
{
    public partial class Condenser : Form
    {
        private Point placeDrop = new Point(13, 240);
        private HelpForm f = new HelpForm();
        private bool isCondensing = false;
        private int maxRaindrops;
        private int currentRaindrops = 0;
        private bool secret = false;
        public Condenser()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.AllowTransparency = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = Properties.Settings.Default.LastPath;
            this.preferAC3.Checked = Properties.Settings.Default.PreferAC3;
            this.preferAAC.Checked = Properties.Settings.Default.PreferACC;
            this.preferOGG.Checked = Properties.Settings.Default.PreferOGG;
            this.preferMP3.Checked = Properties.Settings.Default.PreferMP3;
            this.maxRaindrops = Properties.Settings.Default.MaxRaindrops;
            this.comboBox1.Text = maxRaindrops.ToString();
            //MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            //MessageBox.Show(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase));
            /*new Thread(new ThreadStart(delegate
            {
                try
                {
                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                    ffMpeg.ConvertProgress += (thesender, args) => this.textBox1.Text = args.Processed.ToString();
                    ffMpeg.ConvertMedia(@"C:\Users\Sean\Music\Nero - Promises.mp4", @"C:\Users\Sean\Music\Nero - Promises.mp4", Format.aiff);
                }
                catch (Exception err) { MessageBox.Show(err.ToString()); }
            })).Start();*/
        }

        /*void ffMpeg_ConvertProgress(object sender, ConvertProgressEventArgs e)
        {
            this.textBox1.Text = e.Processed.ToString();
        }*/

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                this.textBox1.SelectAll();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void organizeChildren(bool reverseActivate)
        {
            if (currentRaindrops == 0)
                return;
            /*if (this.Width < 1000)
                this.Width = 1000;
            int height = 340;
            int count = 0;
            for(int x = 0; x < this.MdiChildren.Length; x++)
            {
                if (count >= 5 && count <= 20)
                    height += 30;
                count++;
            }
            if(this.Height < height)
                this.Height = height;
            */
            int yoffset = 5;
            int xoffset = 505;
            foreach (Form f in this.MdiChildren)
            {
                f.WindowState = FormWindowState.Normal;
                f.Location = new Point(xoffset, yoffset);
                if(!reverseActivate)
                    f.Activate();
                //offset += 86;
                yoffset += 70;
                if (yoffset > 700)
                {
                    yoffset = 5;
                    xoffset += 471;
                }
                //offset += 30;
            }
            if(reverseActivate)
                foreach (Form f in this.MdiChildren.Reverse())
                    f.Activate();
        }

        /*private void spreadChildren() //spread the forms out so you can see each one... it looks dumb
        {
            if (this.Width < 1000)
                this.Width = 1000;
            int height = this.Height;
            if (height < 340)
                height = 340;
            int count = 0;
            for (int x = 0; x < this.MdiChildren.Length; x++)
            {
                if (count > 3 && count < 10)
                    height += 90;
                count++;
            }
            this.Height = height;
            int offset = 5;
            foreach (Form f in this.MdiChildren)
            {
                f.WindowState = FormWindowState.Normal;
                f.Location = new Point(505, offset);
                f.Activate();
                offset += 86;
                //offset += 30;
            }
        }*/

        private void minimizeChildren()
        {
            if (currentRaindrops == 0)
                return;
            foreach (Form f in this.MdiChildren)
                f.WindowState = FormWindowState.Minimized;
        }

        private void adjustHeight()
        {
            if (this.maxRaindrops <= 5)
                this.Height = 450;
            else
                this.Height = 800;

            if (this.maxRaindrops > 10)
                this.Width = 1500;
            else
                this.Width = 1000;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox2.Text.Equals("") || !Directory.Exists(this.textBox2.Text))
            {
                MessageBox.Show("Please place a RainBucket", "RainBucket Required");
                return;
            }
            this.button1.Enabled = false;
            this.textBox1.ReadOnly = true;
            string b1t = this.button1.Text;
            //this.button1.Text = "Condensing Raindrops...";
            //Color tb1 = this.textBox1.ForeColor;
            //this.textBox1.ForeColor = Color.Yellow;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.IsMdiContainer = true;
            //this.Text = "RainTube - Raining...";
            this.Text = "RainDrop - Raining...";

            /*if(this.Width < 1000)
                this.Width = 1000;
            int height = 340;
            int count = 0;
            foreach (string s in this.textBox1.Lines)
            {
                if (!s.Equals("") && count > 5 && count <= maxRaindrops)
                    height += 30;
                    //height += 90;
                count++;
            }
            if(this.Height < height)
                this.Height = height;*/

            //this.WindowState = FormWindowState.Maximized;
            typeof(MdiClient).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this.Controls.OfType<MdiClient>().First(), true, null);
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient)
                {
                    c.BackColor = Color.Black;
                    c.BackgroundImage = this.BackgroundImage;
                    c.Resize += c_Resize;
                }
            }
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    this.isCondensing = true;
                    //int offset = 200;
                    //int offset = 5;
                    int fileCount = 0;
                    foreach (string line in this.textBox1.Lines)
                    {
                        if (!this.isCondensing)
                            return;
                        if (line.Equals(""))
                            continue;
                        fileCount++;
                        string link = line;
                        if (!link.Contains("http://") && !link.Contains("https://"))
                        {
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://google.com/search?btnI=1&q=" + WebUtility.UrlEncode(link + " site:youtube.com"));
                            //req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
                            req.UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                            req.Proxy = null;
                            //req.Credentials = System.Net.CredentialCache.DefaultCredentials;
                            //req.AllowAutoRedirect = true;
                            //WebUtility.UrlEncode(link + " site:youtube.com");
                            //MessageBox.Show("http://google.com/search?btnI=1&q=" + link + " site:youtube.com").ToString());
                            HttpWebResponse myResp = (HttpWebResponse)req.GetResponse();
                            string found = myResp.ResponseUri.ToString();
                            myResp.Close();
                            myResp.Dispose();
                            if (found.Contains("/watch") || found.Contains("/playlist"))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    outputBox.AppendText("Searched for: " + link +
                                        Environment.NewLine + "Found Link: " + found.ToString() + Environment.NewLine + Environment.NewLine, Color.Lime);
                                });
                                link = found.ToString();
                            }
                            else
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    outputBox.AppendText("Failed to find: " + line +
                                        Environment.NewLine + Environment.NewLine + Environment.NewLine, Color.Red);
                                });
                                continue;
                            }
                        }
                        if (link.Contains("/playlist?list="))
                        {
                            this.Invoke((MethodInvoker)delegate { 
                                outputBox.AppendText("Pulling Playlist Links...",Color.Yellow);
                                //this.Size = new Size(499, 436);
                            });
                            string theplaylistLinks = "";
                            using (WebClient wc = new WebClient())
                            {
                                //MessageBox.Show("http://gdata.youtube.com/feeds/api/playlists/" + link.Substring(link.IndexOf("playlist?list="), link.Length - link.IndexOf("playlist?list=")).Replace("playlist?list=","") + "?max-results=50&v=2&alt=jsonc");
                                //string dlstring = "http://gdata.youtube.com/feeds/api/playlists/" + link.Substring(link.IndexOf("playlist?list="), link.Length - link.IndexOf("playlist?list=")).Replace("playlist?list=", "") + "?max-results=50&v=2&alt=jsonc";
                                //string dl = wc.DownloadString(link);
                                //string pattern = @"/watch\?.+\&amp;index=";
                                wc.UseDefaultCredentials = true;
                                string pattern = @"/watch\?v=...........";
                                for (int startIndex = 1; startIndex <= 200; startIndex += 50)
                                {
                                    string dlstring = "http://gdata.youtube.com/feeds/api/playlists/" + link.Substring(link.IndexOf("playlist?list=") + ("playlist?list=").Length) + "?v=2&alt=jsonc&max-results=50&start-index=" + startIndex;
                                    //outputBox.AppendText("Parsing [" + dlstring + "]..." + Environment.NewLine, Color.Yellow);
                                    string dl = wc.DownloadString(dlstring);
                                    string last = "";
                                    bool foundMatch = false;
                                    foreach (Match m in Regex.Matches(dl, pattern))
                                    {
                                        foundMatch = true;
                                        if (!last.Equals(m.Value))
                                        {
                                            //outputBox.AppendText("http://youtube.com" + m.Value + Environment.NewLine);
                                            theplaylistLinks += "http://youtube.com" + m.Value + Environment.NewLine;
                                            last = m.Value;
                                        }
                                    }
                                    if (!foundMatch)
                                        break;
                                }
                                this.Invoke((MethodInvoker)delegate
                                {
                                    outputBox.AppendText("Done!" + Environment.NewLine, Color.LightGreen);
                                    //this.Size = new Size(499, 436);
                                });
                            }
                            outputBox.AppendText(Environment.NewLine);
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.IsMdiContainer = false;
                                this.Size = new Size(499, 436);
                                //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                                this.Text = "RainDrop - Condensing the Cloud";
                                //this.WindowState = FormWindowState.Normal;
                                this.textBox1.ReadOnly = false;
                                //this.button1.Text = b1t;
                                this.button1.Enabled = true;
                                this.textBox1.Focus();
                                this.isCondensing = false;
                                this.textBox1.Text = theplaylistLinks;
                            });
                            return;
                            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"~(?:http|https|)(?::\/\/|)(?:www.|)(?:youtu\.be\/|youtube\.com(?:\/embed\/|\/v\/|\/watch\?v=|\/ytscreeningroom\?v=|\/feeds\/api\/videos\/|\/user\S*[^\w\-\s]|\S*[^\w\-\s]))([\w\-]{11})[a-z0-9;:@#?&%=+\/\$_.-]*~i");
                            //Match match = Regex.Match(dl, @"watch\?.*\&");
                            //outputBox.AppendText(match.Value + Environment.NewLine);
                            //Match nmatch;
                            //do
                            //{
                                //nmatch = match.NextMatch();
                                //outputBox.AppendText(nmatch.Value + Environment.NewLine);
                            //}
                            //while (nmatch.Success);
                            //while ((nmatch = match.NextMatch()) != null)
                            //{
                                //outputBox.AppendText(nmatch + Environment.NewLine);
                            //}
                            //foreach (string match in regex.Matches(dl))
                            //{
                                //outputBox.AppendText(match + Environment.NewLine);
                                //MessageBox.Show(match);
                            //}
                            //Stream stream = wc.OpenRead("http://gdata.youtube.com/feeds/api/playlists/" + link.Substring(link.IndexOf("playlist?list="), link.Length - link.IndexOf("playlist?list=")).Replace("playlist?list=", "") + "?max-results=50&v=2&alt=jsonc");
                            //StreamReader reader = new StreamReader(stream);
                            //reader.Read();
                                
                            //string json = "";
                            //List<object> json = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(reader.Read().ToString());
                            //foreach (string item in json)
                            //{
                                //this.Invoke((MethodInvoker)delegate { outputBox.AppendText(item); });
                            //}
                            //MessageBox.Show("Done!");
                            //throw (new Exception("Done Pulling Playlist Links!"));
                            //return;
                            //continue;
                            
                        }
                        if (!this.isCondensing)
                            return;
                        if (currentRaindrops == 0)
                            this.Invoke((MethodInvoker)delegate { this.adjustHeight(); });
                        RainDrop t;
                        this.currentRaindrops++;
                        if(this.preferAC3.Checked)
                            t = new RainDrop(link.Replace("https://", "http://"), this.textBox2.Text, RainDrop.FileType.AC3);
                        else if(this.preferAAC.Checked)
                            t = new RainDrop(link.Replace("https://", "http://"), this.textBox2.Text, RainDrop.FileType.AAC);
                        else if(this.preferOGG.Checked)
                            t = new RainDrop(link.Replace("https://", "http://"), this.textBox2.Text, RainDrop.FileType.OGG);
                        else if(this.preferMP3.Checked)
                            t = new RainDrop(link.Replace("https://", "http://"), this.textBox2.Text, RainDrop.FileType.MP3);
                        else
                            t = new RainDrop(link.Replace("https://", "http://"), this.textBox2.Text, RainDrop.FileType.MP3);
                        this.Invoke((MethodInvoker)delegate
                        {
                            t.MdiParent = this;
                            t.FormClosed += t_FormClosed;
                            t.Show();
                            //t.Location = new Point(5, offset);
                            //t.Location = new Point(505, offset);
                            //offset += 86;
                            //offset += 30;
                            organizeChildren(false);
                            //this.Invoke((MethodInvoker)delegate { this.Height += 30; });
                        });
                        /*for (int x = 0; x < 1; x++) //Sleep for 5 seconds between raindrops - break early if it succeeds/fails
                        {
                            if ((!t.Text.Equals("Condensing...") || t.Text.Contains("Failed to Condense [")) && x >= 3)
                                break;
                            Thread.Sleep(1000);
                        }*/
                        Thread.Sleep(250); //Throttle a little between raindrops so we don't go crazy and use all the cpu
                        while (this.currentRaindrops >= this.maxRaindrops)
                            Thread.Sleep(1000);
                    }
                    if (fileCount == 0)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.IsMdiContainer = false;
                            this.Size = new Size(499, 436);
                            //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                            this.Text = "RainDrop - Condensing the Cloud";
                            //this.WindowState = FormWindowState.Normal;
                            MessageBox.Show("No valid RainDrops found...", "Paste Links First Noob :D");
                            this.textBox1.ReadOnly = false;
                            //this.button1.Text = b1t;
                            this.button1.Enabled = true;
                            this.textBox1.Focus();
                            this.isCondensing = false;
                        });
                        return;
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        bool allDone = true;
                        foreach (Form f in this.MdiChildren)
                        {
                            if (!f.IsDisposed)
                            {
                                allDone = false;
                                break;
                            }
                        }
                        if (allDone)
                        {
                            //if (this.WindowState == FormWindowState.Maximized)
                            this.IsMdiContainer = false;
                            //this.WindowState = FormWindowState.Normal;
                            this.Size = new Size(499, 436);
                            //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                            this.Text = "RainDrop - Condensing the Cloud";
                            this.textBox1.ReadOnly = false;
                            this.button1.Enabled = true;
                            this.textBox1.Focus();
                            this.isCondensing = false;
                        }
                        else
                        {
                            this.Text = "RainDrop - Capturing Raindrops...";
                            this.textBox1.ReadOnly = false;
                            this.button1.Enabled = true;
                            this.textBox1.Focus();
                            this.isCondensing = false;
                        }
                    });
                }
                catch (Exception) { 
                    //MessageBox.Show(err.ToString());
                    this.isCondensing = false;
                    this.Invoke((MethodInvoker)delegate
                    {
                        /*this.outputBox.AppendText("Error while Condensing Raindrops - Finishing Current Raindrops: " + Environment.NewLine + err.Message.ToString() + Environment.NewLine + Environment.NewLine,Color.OrangeRed);
                        this.Text = "RainDrop - Capturing Raindrops...";
                        this.textBox1.ReadOnly = false;
                        this.button1.Enabled = true;
                        this.textBox1.Focus();
                        this.isCondensing = false;*/
                        //if (this.WindowState == FormWindowState.Maximized)
                        this.IsMdiContainer = false;
                        //this.WindowState = FormWindowState.Normal;
                        this.Size = new Size(499, 436);
                        //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                        this.Text = "RainDrop - Condensing the Cloud";
                        this.textBox1.ReadOnly = false;
                        this.button1.Enabled = true;
                        this.textBox1.Focus();
                        this.isCondensing = false;
                    });
                }
                //this.textBox1.ForeColor = tb1;
            })).Start();
        }

        void t_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((RainDrop)sender).Dispose();
            this.currentRaindrops--;
            if (isCondensing)
                return;
            bool allDone = true;
            foreach (Form f in this.MdiChildren)
            {
                if (!f.IsDisposed)
                {
                    allDone = false;
                    break;
                }
            }
            if (allDone)
            {
                //if (this.WindowState == FormWindowState.Maximized)
                this.IsMdiContainer = false;
                //this.WindowState = FormWindowState.Normal;
                this.Size = new Size(499, 436);
                //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                this.Text = "RainDrop - Condensing the Cloud";
            }
        }

        void c_Resize(object sender, EventArgs e)
        {
            //this.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            DialogResult res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.textBox2.Text = d.SelectedPath + "\\";
            }
        }

        private void RainTube_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                try
                {
                    f.Close();
                }
                catch { }
            }
            if (isCondensing)
            {
                isCondensing = false;
                e.Cancel = true;
                this.Invoke((MethodInvoker)delegate
                {
                    this.IsMdiContainer = false;
                    this.Size = new Size(499, 436);
                    //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    this.Text = "RainDrop - Condensing the Cloud";
                    //this.WindowState = FormWindowState.Normal;
                    this.textBox1.ReadOnly = false;
                    //this.button1.Text = b1t;
                    this.button1.Enabled = true;
                    this.textBox1.Focus();
                    this.isCondensing = false;
                });
                /*try
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Thread.Sleep(5000);
                        DirectoryInfo di = new DirectoryInfo(textBox2.Text.ToString());
                        //FileInfo[] files = di.GetFiles("*.raindrop").Where(p => p.Extension == ".msi").ToArray();
                        foreach (FileInfo file in di.GetFiles("*.raindrop").Where(p => p.Extension == ".msi").ToArray())
                        {
                            try
                            {
                                //file.Attributes
                                File.Delete(file.FullName);
                                MessageBox.Show(file.FullName);
                            }
                            catch { }
                        }
                    })).Start();
                }
                catch { }*/
            }
            else
            {
                Properties.Settings.Default.LastPath = this.textBox2.Text;
                Properties.Settings.Default.PreferAC3 = this.preferAC3.Checked;
                Properties.Settings.Default.PreferACC = this.preferAAC.Checked;
                Properties.Settings.Default.PreferOGG = this.preferOGG.Checked;
                Properties.Settings.Default.PreferMP3 = this.preferMP3.Checked;
                try
                {
                    Properties.Settings.Default.MaxRaindrops = int.Parse(this.comboBox1.Text.ToString());
                }
                catch { }
                Properties.Settings.Default.Save();
                try
                {
                    //File.Delete(System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "ffmpeg.exe");
                    File.Delete(Directory.GetCurrentDirectory() + "\\ffmpeg.exe");
                }
                catch { }
                Environment.Exit(0);
            }
        }

        private void RainTube_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            organizeChildren(false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            minimizeChildren();
        }

        //private void button5_Click(object sender, EventArgs e)
        //{
            //spreadChildren();
        //}

        private void button5_Click_1(object sender, EventArgs e)
        {
            organizeChildren(true);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            f.ShowDialog();
        }

        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            if(!Directory.Exists(this.textBox2.Text))
                return;
            string cmdText = this.textBox2.Text;
            System.Diagnostics.Process.Start("explorer", cmdText);
            this.textBox2.DeselectAll();
        }

        //AAC
        private void preferHQ_CheckedChanged(object sender, EventArgs e)
        {
            if (this.preferAAC.Checked)
            {
                this.preferOGG.Checked = false;
                this.preferOGG.ForeColor = Color.Gray;
                this.preferMP3.Checked = false;
                this.preferMP3.ForeColor = Color.Gray;
                this.preferAAC.ForeColor = Color.Orange;
                this.preferAC3.Checked = false;
                this.preferAC3.ForeColor = Color.Gray;
            }
            else
            {
                this.preferAAC.ForeColor = Color.Gray;
            }
        }
        //OGG
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.preferOGG.Checked)
            {
                this.preferAAC.Checked = false;
                this.preferAAC.ForeColor = Color.Gray;
                this.preferMP3.Checked = false;
                this.preferMP3.ForeColor = Color.Gray;
                this.preferOGG.ForeColor = Color.Orange;
                this.preferAC3.Checked = false;
                this.preferAC3.ForeColor = Color.Gray;
            }
            else
            {
                this.preferOGG.ForeColor = Color.Gray;
            }
        }
        //MP3
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.preferMP3.Checked)
            {
                this.preferAAC.Checked = false;
                this.preferAAC.ForeColor = Color.Gray;
                this.preferOGG.Checked = false;
                this.preferOGG.ForeColor = Color.Gray;
                this.preferMP3.ForeColor = Color.Orange;
                this.preferAC3.Checked = false;
                this.preferAC3.ForeColor = Color.Gray;
            }
            else
            {
                this.preferMP3.ForeColor = Color.Gray;
            }
        }

        private void outputBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                //Prefer chrome plz
                System.Diagnostics.Process.Start("chrome", e.LinkText);
            }
            catch
            {
                try
                {
                    System.Diagnostics.Process.Start(e.LinkText);
                }
                catch { }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.maxRaindrops = int.Parse(comboBox1.Text.ToString());
                if (this.isCondensing)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (this.maxRaindrops <= 5)
                            this.Height = 450;
                        else
                            this.Height = 800;

                        if (this.maxRaindrops > 10)
                            this.Width = 1500;
                        else
                            this.Width = 1000;
                    });
                }
            }
            catch { /*this should never happen!*/ }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (this.secret)
                return;
            this.secret = true;
            this.pictureBox1.BringToFront();
            this.pictureBox1.Show();
            new Thread(new ThreadStart(delegate {
                this.Invoke((MethodInvoker)delegate { this.pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); this.pictureBox1.Refresh(); });
                Thread.Sleep(250);
                this.Invoke((MethodInvoker)delegate { this.pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); this.pictureBox1.Refresh(); });
                Thread.Sleep(250);
                this.Invoke((MethodInvoker)delegate { this.pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); this.pictureBox1.Refresh(); });
                Thread.Sleep(250);
                this.Invoke((MethodInvoker)delegate { this.pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); this.pictureBox1.Refresh(); });
                Thread.Sleep(250);
                this.Invoke((MethodInvoker)delegate { pictureBox1.Hide(); secret = false; });
            })).Start();
        }

        private void preferAC3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.preferAC3.Checked)
            {
                this.preferAAC.Checked = false;
                this.preferAAC.ForeColor = Color.Gray;
                this.preferOGG.Checked = false;
                this.preferOGG.ForeColor = Color.Gray;
                this.preferMP3.Checked = false;
                this.preferMP3.ForeColor = Color.Gray;
                this.preferAC3.ForeColor = Color.Orange;
            }
            else
            {
                this.preferAC3.ForeColor = Color.Gray;
            }
        }
    }
}
