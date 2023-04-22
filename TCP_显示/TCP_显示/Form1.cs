using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_显示
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            Console.WriteLine("正在连接本机的端口：");
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation connection in connections)
            {
                if (connection.State == TcpState.Established)
                {
                    Console.WriteLine("{0}:{1} <--> {2}:{3}", connection.LocalEndPoint.Address, connection.LocalEndPoint.Port, connection.RemoteEndPoint.Address, connection.RemoteEndPoint.Port + " - " + DateTime.Now);
                }
            }
            Console.WriteLine("--------------------------------");
            Console.ReadLine();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox1)); //将控制台的输出重定向到一个名为textBox1的文本框中
        }
    }
}
