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

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

using java.util;

namespace ebook
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer sp = null;
        const string bfilter = "Books (*.rtf)|*.rtf|PDFs (*.pdf)|*.pdf|eBooks (*.epub)|*.epub";

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

            load_doc(fn);

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

            o.Filter = bfilter;
            o.ShowDialog();

            fn = o.FileName;
            if (fn != "")
            {
                load_doc(fn);

                if (!bms.ContainsKey(fn))
                {
                    bms.Add(fn, 0);
                    listBox1.Items.Add(fn);
                }
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

        public void load_doc(string fn)
        {
            if (fn.EndsWith(".pdf"))
                richTextBox1.Text = load_pdf(fn);
            else
            if (fn.EndsWith(".rtf"))
                richTextBox1.Rtf = load_rtext(fn);
            else
            if (fn.EndsWith(".epub"))
                richTextBox1.Text = load_epub(fn);
            else
            {
                richTextBox1.Text = "Error";
                return;
            }

            if (bms.ContainsKey(fn))
                richTextBox1.SelectionStart = bms[fn];
            else
                richTextBox1.SelectionStart = 0;

            richTextBox1.ScrollToCaret();
            richTextBox1.Visible = true;
            listBox1.Visible = false;
            button1.Text = "Close";
            button3.Visible = true;
        }

        public string load_pdf(string fn)
        {
            try
            {
                PDDocument doc = PDDocument.load(fn);
                PDFTextStripper strip = new PDFTextStripper();
                string name = "";

                java.util.List pages = doc.getDocumentCatalog().getAllPages();
                java.util.Iterator iter = pages.iterator();

                PDPage page = (PDPage)iter.next();
                PDResources resources = page.getResources();
                Map images = resources.getImages();
                if (images != null)
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
                return strip.getText(doc);

            }
            catch (Exception g)
            {
                Console.WriteLine(g);
                return "";
            }
        }

        public string load_rtext(string fn)
        {
            return File.ReadAllText(fn);
        }

        public string load_epub(string fn)
        {
            // tbd
            string r = "";
            ZipFile zf = new ZipFile(fn);

            for (int i = 0; i < 100; i++)
            {
                string n = String.Format("OEBPS/text/content{0:000}.xhtml", i);

                ZipEntry z = zf.GetEntry(n);
                if (z != null)
                {
                    Console.WriteLine(n);
                    //ZipOutputStream
                    Stream s =  zf.GetInputStream(z.ZipFileIndex);
                    byte[] buff = new byte[4096];

                    while (true)
                    {
                        int nb = s.Read(buff, 0, 4096);
                        if (nb <= 0) break;
                        string t = "";
                        for (int c = 0; c < nb; c++) t += (char)buff[c];
                        r += t;
                    }

                }

            }
            return ConvertHtmlToText(r);
        }

        public static string ConvertHtmlToText(string source) {

                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating speces becuase browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                /*
                * 

               // Remove the header (prepare first by clearing attributes)
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        @"<( )*head([^>])*>", "<head>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        @"(<( )*(/)( )*head( )*>)", "</head>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        "(<head>).*(</head>)", string.Empty,
                       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                   // remove all scripts (prepare first by clearing attributes)
                   result = System.Text.RegularExpressions.Regex.Replace(result,
                            @"<( )*script([^>])*>", "<script>",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                   result = System.Text.RegularExpressions.Regex.Replace(result,
                            @"(<( )*(/)( )*script( )*>)", "</script>",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                   //result = System.Text.RegularExpressions.Regex.Replace(result, 
                   //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                   //         string.Empty, 
                   //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                   result = System.Text.RegularExpressions.Regex.Replace(result,
                            @"(<script>).*(</script>)", string.Empty,
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
 

               // remove all styles (prepare first by clearing attributes)
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        @"<( )*style([^>])*>", "<style>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        @"(<( )*(/)( )*style( )*>)", "</style>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
               result = System.Text.RegularExpressions.Regex.Replace(result,
                        "(<style>).*(</style>)", string.Empty,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    */
                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);



                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                return result;


                // Remove remaining tags like <a>, links, images,
                // comments etc - anything thats enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&nbsp;", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @">", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);


                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4. 
                // Prepare first to remove any whitespaces inbetween
                // the escaped characters and remove redundant tabs inbetween linebreaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multible tabs followind a linebreak with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for linebreaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++) {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // Thats it.
                return result;

        }
    }

}
