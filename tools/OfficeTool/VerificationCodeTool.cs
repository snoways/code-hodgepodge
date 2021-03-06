﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using mshtml;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;

namespace OfficeTool
{
    public partial class VerificationCodeTool : Form
    {

        Dictionary<string, string> _result;

        public VerificationCodeTool()
        {
            InitializeComponent();
            webBrowser1.Navigate(new Uri(ConfigurationManager.AppSettings["CheckinUrl"]));
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var id = ConfigurationManager.AppSettings["ImgId"];

            var img = webBrowser1.Document.GetElementById(id);
            img.InvokeMember("click");   

            HTMLDocument html = (HTMLDocument)this.webBrowser1.Document.DomDocument;
            IHTMLControlElement img2 = (IHTMLControlElement)webBrowser1.Document.Images[id].DomElement;
            IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)html.body).createControlRange();

            range.add(img2);
            range.execCommand("Copy", false, null);

            if (Clipboard.ContainsImage())
            {
                pictureBox1.Image = Clipboard.GetImage();

            }
            else
            {
                MessageBox.Show("获取验证码失败");
            }

            Clipboard.Clear();

            pictureBox2.Image = VerificationCode.ProcessBmp(new Bitmap(pictureBox1.Image));
            textBox1.Text = VerificationCode.Spot(new Bitmap(pictureBox1.Image), 4);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var list = VerificationCode.SegmentBmp(new Bitmap(pictureBox2.Image));
            if (!string.IsNullOrEmpty(textBox1.Text) && list != null)
            {
                var arr = textBox1.Text.ToCharArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    VerificationCode.WriteDB(arr[i].ToString(), list[i]);
                }
            }
        }

    }
}

