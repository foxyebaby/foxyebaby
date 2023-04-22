using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cnn_2023年4月20日
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerSocket.Socket();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox1)); //将控制台的输出重定向到一个名为textBox1的文本框中
        }

    }
}
