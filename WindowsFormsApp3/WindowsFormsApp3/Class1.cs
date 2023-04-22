using System;
using System.IO.Ports; // 导入串口通信相关的命名空间
using System.Windows.Forms; // 导入窗体相关的命名空间
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SerialPortDemo
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort; // 声明一个串口对象
        public Form1()
        {
            InitializeComponent(); // 初始化窗体组件
            serialPort = new SerialPort(); // 创建串口对象
            serialPort.PortName = "COM1"; // 设置串口号
            serialPort.BaudRate = 9600; // 设置波特率
            serialPort.Parity = Parity.None; // 设置校验位
            serialPort.DataBits = 8; // 设置数据位
            serialPort.StopBits = StopBits.One; // 设置停止位
            serialPort.DataReceived += SerialPort_DataReceived; // 注册数据接收事件
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting(); // 读取串口缓冲区中的数据
            // 在文本框中显示数据，需要使用委托跨线程调用控件
            this.Invoke(new Action(() => { textBox1.AppendText(data); }));
        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort.Open(); // 打开串口
                buttonOpen.Enabled = false; // 禁用打开按钮
                buttonClose.Enabled = true; // 启用关闭按钮
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开串口失败：" + ex.Message);
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            serialPort.Close(); // 关闭串口
            buttonOpen.Enabled = true; // 启用打开按钮
            buttonClose.Enabled = false; // 禁用关闭按钮
        }

 
        
    }

}