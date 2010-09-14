using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ebook
{
    public partial class Form1 : Form
    {
        Dictionary<string,int> bms = new Dictionary<string,int>();
        string fn="";
        float sf = (float)1.0;

        public Form1()
        {
            InitializeComponent();

            this.Resize += new EventHandler(Form1_Resize);
            this.Disposed += new EventHandler(Form1_Disposed);
            richTextBox1.SelectionChanged += new EventHandler(richTextBox1_SelectionChanged);

            richTextBox1.Visible = false;
            listBox1.Visible = true;

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
                    bms.Add(f[0],n);
                    listBox1.Items.Add(f[0]);
                }
                txt += string.Format("Loaded {0} entries\n", bms.Count);
                richTextBox1.Text = txt;
            }

        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fn = listBox1.Items[listBox1.SelectedIndex].ToString();
            richTextBox1.Rtf = File.ReadAllText(fn);
            richTextBox1.SelectionStart = bms[fn];
            richTextBox1.ScrollToCaret();
            richTextBox1.Visible = true;
            listBox1.Visible = false;
            button1.Text = "Close";
        }

        void Form1_Disposed(object sender, EventArgs e)
        {
            string o =String.Format("CFG;{0};{1};{2}\n", sf, this.Height, this.Width );
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
            richTextBox1.Width = this.Width-30;
            richTextBox1.Height = this.Height - 75;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "Close")
            {
                button1.Text = "Load";
                richTextBox1.Visible = false;
                listBox1.Visible = true;
                return;
            }
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = "Books (*.rtf)|*.rtf";
            o.ShowDialog();

            try
            {
                fn = o.FileName;
                if (o.FileName != "")
                {
                    richTextBox1.Rtf = File.ReadAllText(o.FileName);
                    if (bms.ContainsKey(o.FileName))
                    {
                        richTextBox1.SelectionStart = bms[o.FileName];
                        richTextBox1.ScrollToCaret();                        
                    }
                    else
                    {
                        bms.Add(o.FileName, 0);
                    }
                }

                richTextBox1.Visible = true;
                listBox1.Visible = false;

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
            richTextBox1.SelectionStart = Convert.ToInt32(bm_lbl.Text);
            richTextBox1.ScrollToCaret();
            bms[fn] = richTextBox1.SelectionStart;
        }

        private void plus_btn_Click(object sender, EventArgs e)
        {
            sf = (float)2.0;
            richTextBox1.ZoomFactor = sf;
        }

        private void minus_btn_Click(object sender, EventArgs e)
        {
            sf = (float)1.0;
            richTextBox1.ZoomFactor = sf;
        }
    }
}
