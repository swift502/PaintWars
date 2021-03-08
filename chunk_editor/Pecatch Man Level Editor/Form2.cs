using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pecatch_Man_Level_Editor
{
    public partial class Form2 : Form
    {
        static public string name;

        public Form2()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog(this);

            toolStripTextBox1.Text = name;
            generateText(name);
            Clipboard.SetText(textBox1.Text);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            Clipboard.SetText(textBox1.Text);
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            generateText(toolStripTextBox1.Text);
        }

        void generateText(string name)
        {
            string text = "";
            text += "bool[,] " + name + " = new bool[11,8];" + Environment.NewLine;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string value;
                    if (Form1.Scene[i, j]) value = "true";
                    else value = "false";
                    text += name + "[" + i + "," + j + "] = " + value + ";" + Environment.NewLine;
                }
            }
            textBox1.Text = text;
        }
    }
}
