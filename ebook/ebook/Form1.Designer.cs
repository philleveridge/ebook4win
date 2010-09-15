namespace ebook
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bm_lbl = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.plus_btn = new System.Windows.Forms.Button();
            this.minus_btn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(10, 36);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(433, 217);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(52, 27);
            this.button1.TabIndex = 1;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(177, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bookmark: ";
            // 
            // bm_lbl
            // 
            this.bm_lbl.AutoSize = true;
            this.bm_lbl.Location = new System.Drawing.Point(238, 12);
            this.bm_lbl.Name = "bm_lbl";
            this.bm_lbl.Size = new System.Drawing.Size(0, 13);
            this.bm_lbl.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(68, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 27);
            this.button2.TabIndex = 4;
            this.button2.Text = "BM";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // plus_btn
            // 
            this.plus_btn.Location = new System.Drawing.Point(114, 12);
            this.plus_btn.Name = "plus_btn";
            this.plus_btn.Size = new System.Drawing.Size(21, 24);
            this.plus_btn.TabIndex = 5;
            this.plus_btn.Text = "+";
            this.plus_btn.UseVisualStyleBackColor = true;
            this.plus_btn.Click += new System.EventHandler(this.plus_btn_Click);
            // 
            // minus_btn
            // 
            this.minus_btn.Location = new System.Drawing.Point(132, 12);
            this.minus_btn.Name = "minus_btn";
            this.minus_btn.Size = new System.Drawing.Size(21, 24);
            this.minus_btn.TabIndex = 6;
            this.minus_btn.Text = "-";
            this.minus_btn.UseVisualStyleBackColor = true;
            this.minus_btn.Click += new System.EventHandler(this.minus_btn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(10, 36);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(433, 199);
            this.listBox1.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(380, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(63, 26);
            this.button3.TabIndex = 8;
            this.button3.Text = "Speak";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(78, 71);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(276, 138);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 264);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.minus_btn);
            this.Controls.Add(this.plus_btn);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bm_lbl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Simple eBook Reader";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label bm_lbl;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button plus_btn;
        private System.Windows.Forms.Button minus_btn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

