using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using System.Speech.Synthesis;

using org.pdfbox;
using org.pdfbox.pdmodel;
using org.pdfbox.util;
using org.pdfbox.pdmodel.graphics.xobject;

using java.util;

namespace ebook
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer sp = null;

        Dictionary<string, int> bms = new Dictionary<string, int>();
        string fn = "";
        float sf = (float)1.0;
        int delay = 850;

        public Form1()
        {
            InitializeComponent();

            sp = new SpeechSynthesizer();

            this.Resize += new EventHandler(Form1_Resize);
            this.Disposed += new EventHandler(Form1_Disposed);
            richTextBox1.SelectionChanged += new EventHandler(richTextBox1_SelectionChanged);

            richTextBox1.Visible = false;
            listBox1.Visible = true;
            button3.Visible = false;

            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);


            if (File.Exists("ebook.ini"))
            {
                string txt = "";
                string[] r = File.ReadAllLines("ebook.ini");
                foreach (string l in r)
                {
                    txt += l + "\n";
                    string[] f = l.Split(';'); // Filename:bookmark
                    int n = 0;

                    if (f[0] == "CFG")
                    {
                        sf = (float)Convert.ToDouble(f[1]);
                        this.Height = Convert.ToInt32(f[2]);
                        this.Width = Convert.ToInt32(f[3]);
                        continue;
                    }

                    try
                    {
                        n = Convert.ToInt32(f[1]);
                    }
                    catch
                    {
                    }
                    if (f[0] != "")
                    {
                        bms.Add(f[0], n);
                        listBox1.Items.Add(f[0]);
                    }
                }
                txt += string.Format("Loaded {0} entries\n", bms.Count);
                richTextBox1.Text = txt;
            }

        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;

            fn = listBox1.Items[listBox1.SelectedIndex].ToString();
            if (fn.EndsWith(".pdf"))
            {
                try
                {
                    PDDocument doc = PDDocument.load(fn);
                    PDFTextStripper strip = new PDFTextStripper();
                    string name="";

                    java.util.List pages = doc.getDocumentCatalog().getAllPages();
                    java.util.Iterator iter = pages.iterator();

                    PDPage page = (PDPage)iter.next();
                    PDResources resources = page.getResources();
                    Map images = resources.getImages();
                    if( images != null )
                    {
                        Iterator imageIter = images.keySet().iterator();
                        while (imageIter.hasNext())
                        {
                            name = (String)imageIter.next();
                            PDXObjectImage image = (PDXObjectImage)(images.get(name));
                            Console.WriteLine("Writing image:" + name);
                            image.write2file(name);
                        }
                    }

                    //pictureBox1.Image = Image.FromFile(name+".jpg");
                    //pictureBox1.Visible = true;
                    richTextBox1.Text = strip.getText(doc);

                }
                catch (Exception g)
                {
                    Console.WriteLine(g);
                }

            }
            else
            {
                richTextBox1.Rtf = File.ReadAllText(fn);
            }
            richTextBox1.SelectionStart = bms[fn];
            richTextBox1.ScrollToCaret();
            richTextBox1.Visible = true;
            listBox1.Visible = false;
            button1.Text = "Close";
            button3.Visible = true;
        }

        void Form1_Disposed(object sender, EventArgs e)
        {
            string o = String.Format("CFG;{0};{1};{2}\n", sf, this.Height, this.Width);
            foreach (string k in bms.Keys)
            {
                o += String.Format("{0};{1}\n", k, bms[k]);
            }

            File.WriteAllText("ebook.ini", o);
            Console.WriteLine("BMs=" + o);
        }

        void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            // book mark
            bm_lbl.Text = richTextBox1.SelectionStart.ToString();
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            richTextBox1.Width = this.Width - 30;
            richTextBox1.Height = this.Height - 75;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "Close")
            {
                button1.Text = "Load";
                richTextBox1.Visible = false;
                listBox1.Visible = true;
                button3.Visible = false;
                return;
            }
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = "Books (*.rtf)|*.rtf|PDFs (*.pdf)|*.pdf";
            o.ShowDialog();

            try
            {
                fn = o.FileName;
                if (fn != "")
                {
                    if (fn.EndsWith(".pdf"))
                    {
                        try
                        {
                            org.pdfbox.pdmodel.PDDocument doc = org.pdfbox.pdmodel.PDDocument.load(fn);
                            org.pdfbox.util.PDFTextStripper strip = new org.pdfbox.util.PDFTextStripper();

                            richTextBox1.Text = strip.getText(doc);
                        }
                        catch (Exception g)
                        {
                            Console.WriteLine(g);
                        }
                    }
                    else
                    {
                        richTextBox1.Rtf = File.ReadAllText(fn);
                    }

                    if (bms.ContainsKey(o.FileName))
                    {
                        richTextBox1.SelectionStart = bms[fn];
                        richTextBox1.ScrollToCaret();
                    }
                    else
                    {
                        bms.Add(fn, 0);
                        listBox1.Items.Add(fn);
                    }
                }

                richTextBox1.Visible = true;
                listBox1.Visible = false;
                button3.Visible = true;
                button1.Text = "Close";

            }
            catch
            {
                richTextBox1.Text = "Error - try again";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // goto bookmark
            try
            {
                richTextBox1.SelectionStart = Convert.ToInt32(bm_lbl.Text);
                richTextBox1.ScrollToCaret();
                bms[fn] = richTextBox1.SelectionStart;
            }
            catch
            {
            }
        }

        private void plus_btn_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Stop")
            {
                delay = delay - delay / 10;
            }
            else
            {
                sf = (float)2.0;
                richTextBox1.ZoomFactor = sf;
            }
        }

        private void minus_btn_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Stop")
            {
                delay = delay + delay / 10;
            }
            else
            {
                sf = (float)1.0;
                richTextBox1.ZoomFactor = sf;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Stop")
            {
                button3.Text = "Speak";
                return;
            }


            button3.Text = "Stop";
            int ss = richTextBox1.SelectionStart;
            int sl = 0;

            while (button3.Text == "Stop")
            {

                ss = richTextBox1.SelectionStart;
                sl = 0;

                richTextBox1.ShowSelectionMargin = true;

                string t = richTextBox1.Text.Substring(ss, 300);

                int cr;
                while ((cr = t.IndexOf("\n")) > 0)
                {
                    if (t[cr - 1] == '-')
                    {
                        t = t.Substring(0, cr-1) + t.Substring(cr + 1);
                    }
                    else
                    {
                        t = t.Substring(0, cr) + " " + t.Substring(cr + 1);
                    }
                }

                int p = t.IndexOf('.');
                int q = t.IndexOf('?');

                if (p > 2 && (t.Substring(p - 2, 3) == "Mr." || t.Substring(p - 2, 3) == "Dr." ))
                    p = t.IndexOf('.', p + 1);

                if (q >= 0 && q < p) p = q;

                if (p >= 0)
                {
                    t = t.Substring(0, p + 1);
                    sl = p + 1;
                }
                else
                {
                    sl = 300;
                }

                richTextBox1.Select(0, ss);
                richTextBox1.SelectionColor = Color.Blue;

                richTextBox1.Select(ss, sl);
                richTextBox1.SelectionColor = Color.Red;
                richTextBox1.Update();
                richTextBox1.Show();
                Application.DoEvents();



                Console.WriteLine("[" + t + "]");

                if (t.Trim() == "" || t.Trim() == "." || t.Trim() == "?")
                {
                }
                else
                {
                    sp.Speak(t);
                    System.Threading.Thread.Sleep(delay);
                }

                richTextBox1.SelectionStart = ss + sl + 1;
                richTextBox1.SelectionLength = 0;
                richTextBox1.ScrollToCaret();

                bm_lbl.Text = richTextBox1.SelectionStart.ToString();                 // book mark


                Application.DoEvents();
            }
            ss = richTextBox1.SelectionStart;
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionStart = ss;
        }
    }
}
