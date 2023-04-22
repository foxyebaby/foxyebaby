using System; // 基础命名空间
using System.Windows.Forms; // Windows Forms相关命名空间
using System.Data.SqlClient; // SQL Server数据库相关命名空间
using System.Windows.Forms.DataVisualization.Charting; // chart控件相关命名空间
using System.Threading;
using System.IO;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {


        //  Aa`111111
        //192.168.3.129, 1433
        // 123.56.44.177
        // 在类的成员变量中定义连接对象
        private SqlConnection connection = new SqlConnection("Data Source = 8.8.8.8; Initial Catalog = MISDB; User ID = sa; Password=888888;");

        private bool isRunning = true;
        private Thread thread;
        delegate void UpdateLabelDelegate(double temperature, double humidity, DateTime time);

        public Form1()
        {
            InitializeComponent(); // 初始化窗体控件

        }

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {

            // 创建名为 Temperature 的新数据系列
            Series series = new Series("Temperature");
            // 创建名为 Humidity 的新数据系列
            Series series_1 = new Series("Humidity");
            // 将 Temperature 数据系列的图表类型设置为 Spline
            series.ChartType = SeriesChartType.Spline;
            // 将 Humidity 数据系列的图表类型设置为 Spline
            series_1.ChartType = SeriesChartType.Spline;
            // 将新的数据系列添加到 Chart 控件上
            chart1.Series.Add(series);
            // 将新的数据系列添加到 Chart 控件上
            chart1.Series.Add(series_1);

            chart1.Series[0].BorderWidth = 2; // 将第一个 Series 曲线的线条宽度设置为 3
            chart1.Series[1].BorderWidth = 2; // 将第一个 Series 曲线的线条宽度设置为 3


            chart1.ChartAreas[0].BackColor = Color.FromArgb(240, 240, 240); // 设置背景色为淡灰色
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            //chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray; // 设置 X 轴主网格线颜色为灰色
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot; // 设置 X 轴主网格线为虚线
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1; // 设置 X 轴主网格线宽度为 1
                                                                //chart1.ChartAreas[0].AxisX.MajorGrid.LineOpacity = 100; // 设置 X 轴主网格线透明度为 100%
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray; // 设置 X 轴主网格线颜色为灰色
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot; // 设置 X 轴主网格线为虚线
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1; // 设置 X 轴主网格线宽度为 1

            chart1.Series["Temperature"].MarkerStep = 18; // 设置温度数据点的显示间隔为2
            chart1.Series["Humidity"].MarkerStep = 18; // 设置湿度数据点的显示间隔为2

            ////是否启用智能标签，可以避免标签重叠。
            //chart1.Series["Temperature"].SmartLabelStyle.Enabled = true;

            // 设置X轴显示单位为天
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
            // 设置X轴标签的格式为“天:小时”
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = @"dd\:hh";
            Console.SetOut(new TextBoxWriter(textBox1)); //将控制台的输出重定向到一个名为textBox1的文本框中
            Console.WriteLine("程序启动");
            timer1.Interval = 30000;
            timer1.Start();

        }
        #endregion

        #region Form1_FormClosing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 确认关闭操作
            DialogResult result = MessageBox.Show("确定要关闭程序吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true; // 取消关闭操作
                return;
            }


            // 关闭定时器
            timer1.Stop();
            timer1.Dispose();

            // 关闭数据库连接
            connection.Close();
            connection.Dispose();
            System.Environment.Exit(0);
        }

        #endregion

        #region timer1_Tick
        private void timer1_Tick(object sender, EventArgs e)
        {

            double temperature = 0;
            double humidity = 0;
            DateTime time = DateTime.Now; // 初始化时间变量
            try
            {
                connection.Open(); // 打开数据库连接 
                SqlCommand command = new SqlCommand("SELECT TOP 1 Temperature, Humidity,Time  FROM textTableName ORDER BY WDId DESC;", connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    temperature = (double)reader["Temperature"];
                    humidity = (double)reader["Humidity"];
                    time = (DateTime)reader["Time"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL server 连接失败：" + DateTime.Now+ ex.Message);

            }
            finally
            {
                connection.Close(); // 关闭数据库连接
            }
            UpdateLabel(temperature, humidity, time); // 调用委托方法更新标签控件

        }

        private void UpdateLabel(double temperature, double humidity, DateTime time)
        {
            if (this.InvokeRequired)
            {
                UpdateLabelDelegate d = new UpdateLabelDelegate(UpdateLabel);
                this.Invoke(d, new object[] { temperature, humidity });
            }
            else
            {
                //label1.Text = temperature.ToString("0.0") + "℃"; // 显示温度值
                //label2.Text = humidity.ToString("0.0") + "%RH"; // 显示湿度值
                label1.Text =  temperature.ToString() + " ℃   " + humidity.ToString() + " %RH" + Environment.NewLine + time.ToString();
                Console.WriteLine("数据更新完成: "+DateTime.Now);
            }
        }
        #endregion







        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("开始读取数据");

            thread = new Thread(RefreshChart); // 实例化一个线程，调用DoWork方法

            thread.Start(); // 启动线程


            //RefreshChart();
        }

        /// <summary>
        /// 启动一个线程---来刷新数据
        /// </summary>
        private void StartRefreshThread()
        {
            // 创建一个新的线程
            Thread thread = new Thread(() =>
            {
                // 定时刷新 Chart 控件
                while (true)
                {
                    // 刷新 Chart 控件的数据
                    chart1.Invoke(new Action(() =>
                    {
                        RefreshChart();
                    }));
                    // 等待一段时间
                    Thread.Sleep(1000);
                }
            });
            // 启动线程
            thread.Start();
        }




        private void RefreshChart()
        {
            while (true)
            {

                if (connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("connection.State:" + ConnectionState.Open);
                    // 数据库连接已经打开

                    Conn_text_a();

                    return;
                }
                else
                {
                    // 数据库连接没打开，重新连接

                    if (connection.State == ConnectionState.Closed)
                    {
                        Console.WriteLine("connection.State:" + ConnectionState.Closed);
                        isRunning = true;
                        connection.Close();
                        Conn_text();
                        Thread.Sleep(1500);

                    }
                    else
                    {

                        Console.WriteLine("connection.State:" + connection.State + " : " + DateTime.Now);

                        Thread.Sleep(1500);

                    }

                }

            }

        }
        private void RefreshChart_2()
        {
            while (true)
            {

                if (connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("connection.State:" + ConnectionState.Open);
                    // 数据库连接已经打开


                    // 查询温度数据
                    string query = "select TOP 1 * from textTableName ORDER BY WDId DESC;"; // 查询语句
                    SqlCommand command = new SqlCommand(query, connection); // 创建执行命令对象
                    SqlDataReader reader = command.ExecuteReader(); // 执行查询命令
                    if (reader.Read()) // 循环读取查询结果
                    {                                              // 添加温度数据到chart中
                        double value = (double)reader["Temperature"];  // 获取温度数据
                        double value_1 = (double)reader["Humidity"];  // 获取湿度数据
                        DateTime time = (DateTime)reader["Time"];
                        // 使用 BeginInvoke 方法将访问 UI 控件的代码委托给创建它的线程执行
                        label1.BeginInvoke(new Action(() =>
                        {
                            label1.Text = "[ " + value.ToString() + " ]  [ " + value_1.ToString() + " ]" + Environment.NewLine + time.ToString();
                        }));
                    }
                    Console.WriteLine("更新OK:" + DateTime.Now);
                    // 关闭数据库连接
                    reader.Close(); // 关闭查询结果读取器

                    connection.Close(); // 关闭数据库连接
                    thread.Abort();
                    return;
                }
                else
                {
                    // 数据库连接没打开，重新连接

                    if (connection.State == ConnectionState.Closed)
                    {
                        Console.WriteLine("connection.State:" + ConnectionState.Closed);
                        isRunning = true;
                        connection.Close();
                        Conn_text();
                        Thread.Sleep(1500);

                    }
                    else
                    {

                        Console.WriteLine("connection.State:" + connection.State + " : " + DateTime.Now);

                        Thread.Sleep(1500);

                    }

                }

            }

        }



        #region 提示 输出到 TextBox----已全注释
        /// -----------------------------提示 输出到 TextBox -------------------------------------

        //public class TextBoxWriter : TextWriter
        //{
        //    private TextBox textBox;
        //    public TextBoxWriter(TextBox textBox)
        //    {
        //        this.textBox = textBox;
        //    }
        //    public override void WriteLine(string value)
        //    {
        //        // 在 TextBox 中显示输出
        //        textBox.AppendText(value + Environment.NewLine);
        //    }
        //    public override Encoding Encoding
        //    {
        //        get { return Encoding.UTF8; }
        //    }
        //}


        //public class TextBoxWriter : TextWriter
        //{
        //    private TextBox textBox;
        //    private Action<string> writeAction; // 委托
        //    public TextBoxWriter(TextBox textBox, Action<string> writeAction = null)
        //    {
        //        this.textBox = textBox;
        //        this.writeAction = writeAction;
        //    }
        //    public override void WriteLine(string value)
        //    {
        //        // 在 TextBox 中显示输出
        //        if (textBox.InvokeRequired)
        //        {
        //            textBox.Invoke(new Action<string>(UpdateTextBox), value);
        //        }
        //        else
        //        {
        //            textBox.Text = value;
        //        }
        //    }
        //    private void UpdateTextBox(string value)
        //    {
        //        textBox.Text = value;
        //    }
        //    public override Encoding Encoding
        //    {
        //        get { return Encoding.UTF8; }
        //    }
        //}
        #endregion


        //public class TextBoxWriter : TextWriter
        //{
        //    private TextBox textBox;
        //    private Action<string> writeAction; // 委托
        //    public TextBoxWriter(TextBox textBox, Action<string> writeAction = null)
        //    {
        //        this.textBox = textBox;
        //        this.writeAction = writeAction;
        //    }
        //    public override void WriteLine(string value)
        //    {
        //        // 在 TextBox 中显示输出
        //        if (textBox.InvokeRequired)
        //        {
        //            textBox.Invoke(new Action<string>(UpdateTextBox), value);
        //        }
        //        else
        //        {
        //            textBox.AppendText(Environment.NewLine + value);
        //        }
        //        // 调用委托
        //        if (writeAction != null)
        //        {
        //            writeAction(value);
        //        }
        //    }
        //    private void UpdateTextBox(string value)
        //    {
        //        textBox.AppendText(Environment.NewLine + value);
        //    }
        //    public override Encoding Encoding
        //    {
        //        get { return Encoding.UTF8; }
        //    }
        //}

        #region Conn_text()

        private Thread dbThread; // 定义线程变量
        private void Conn_text() // 定义连接方法
        {
            while (isRunning) // 循环判断是否正在运行
            {
                Console.WriteLine("dbThread:" + dbThread); // 输出线程变量
                if (dbThread != null && dbThread.IsAlive) // 如果线程变量不为空且线程正在运行
                {
                    Console.WriteLine("dbThread;" + dbThread); // 输出线程变量
                    dbThread.Abort(); // 终止线程
                    dbThread = null; // 将线程变量置为空
                }
                Console.WriteLine("执行--Conn_text();"); // 输出执行提示
                dbThread = new Thread(ConnectDatabase); // 创建新的线程实例
                Console.WriteLine("线程启动--ConnectDatabase()"); // 输出线程启动提示
                dbThread.Start(); // 启动线程
                isRunning = false; // 将运行状态标记为false
            }
        }
        private void ConnectDatabase()  // 定义连接数据库方法
        {
            Console.WriteLine("~ConnectDatabase~--已执行"); // 输出方法执行提示
            bool isOpen = false; // 定义连接状态
            while (!isOpen) // 循环判断是否连接成功
            {
                try
                {
                    connection.Close();
                    Console.WriteLine("当前- conn：" + connection); // 输出当前连接状态
                    Console.WriteLine("准备开启数据库连接"); // 输出连接提示
                    connection.Open(); // 打开数据库连接
                    isOpen = true; // 将连接状态标记为true
                    Console.WriteLine("数据库已打开"); // 输出连接成功提示
                }
                catch
                {
                    Console.WriteLine("数据库打开失败，5秒后将重新打开..." + DateTime.Now); // 输出连接失败提示
                    connection.Close(); // 关闭数据库连接
                    Thread.Sleep(5000); // 线程休眠5秒
                }
            }
            Console.WriteLine("连接OK~~!!!!!!"); // 输出连接成功提示
        }
        #endregion


        #region Conn_text_a()
        int cnn_ddsj = 0; // 定义一个整型变量cnn_ddsj，并初始化为0
        private void Conn_text_a() // 定义方法Conn_text_a
        {
            Console.WriteLine("Conn_text_a = : " + connection); // 输出连接对象connection
            string query = "SELECT * FROM textTableName"; // 定义查询语句
            SqlCommand command = new SqlCommand(query, connection); // 创建执行命令对象command，使用查询语句和连接对象作为参数
                                                                    //string query = "SELECT TOP 1 * FROM yourTableName ORDER BY WDId DESC"; // 查询语句
                                                                    //SqlCommand command = new SqlCommand(query, connection); // 创建执行命令对象
            SqlDataReader reader = null; // 定义SqlDataReader对象reader，并初始化为null
            if (connection.State == ConnectionState.Open) // 判断连接状态是否打开
            {
                Console.WriteLine("connection.State:" + ConnectionState.Open); // 输出连接状态
                try
                {
                    reader = command.ExecuteReader(); // 执行查询命令，返回一个SqlDataReader对象
                }
                catch (Exception)
                {
                    Console.WriteLine("查询失败--数据库可能未打开" + DateTime.Now); // 查询失败时输出提示信息
                }
                Console.WriteLine("reader=:" + reader); // 输出SqlDataReader对象

                if (chart1.InvokeRequired) // 判断chart1控件是否跨线程访问
                {
                    chart1.Invoke(new MethodInvoker(delegate // 使用Invoke方法，将委托方法添加到主线程队列中执行
                    {

                        chart1.Series["Temperature"].Points.Clear(); // 清空图表的温度数据
                        chart1.Series["Humidity"].Points.Clear(); // 清空图表的湿度数据
                        Console.WriteLine("清除之前的数据------" + DateTime.Now); // 输出清除数据的时间
                        DateTime lastTime = DateTime.MinValue;  // 定义DateTime对象lastTime，并初始化为最小时间
                        while (reader.Read()) // 循环读取查询结果
                        {
                            double value = (double)reader["Temperature"];  // 获取温度数据
                            double value_1 = (double)reader["Humidity"];  // 获取湿度数据
                            DateTime time = (DateTime)reader["Time"]; // 获取时间数据
                            if (time.Hour != lastTime.Hour)  // 判断是否是同一个小时的数据
                            {

                                DataPoint point = chart1.Series["Temperature"].Points.Add(value); // 添加温度数据点
                                DataPoint point_1 = chart1.Series["Humidity"].Points.Add(value_1); // 添加湿度数据点

                                point.XValue = time.ToOADate(); // 设置温度数据点的X轴时间值
                                point_1.XValue = time.ToOADate(); // 设置湿度数据点的X轴时间值
                                point.YValues[0] = value; // 设置温度数据点的Y轴数值
                                point_1.YValues[0] = value_1; // 设置湿度数据点的Y轴数值
                                                              // 设置数据点的标签-----如果不想要显示 标签 就是曲线上的数值 -注释掉下面的2行代码
                                                              //point.Label = value.ToString(); // 设置温度数据点的标签
                                                              //point_1.Label = value_1.ToString(); // 设置湿度数据点的标签

                                for (int i = 0; i < chart1.Series["Temperature"].Points.Count; i++)
                                {
                                    if (i % 6 == 0) // 每隔两个数据点设置一个标签
                                    {
                                        chart1.Series["Temperature"].Points[i].Label = chart1.Series["Temperature"].Points[i].YValues[0].ToString("0.0");
                                        chart1.Series["Humidity"].Points[i].Label = chart1.Series["Humidity"].Points[i].YValues[0].ToString("0.0");
                                    }
                                    else
                                    {
                                        chart1.Series["Temperature"].Points[i].Label = "";
                                        chart1.Series["Humidity"].Points[i].Label = "";
                                    }
                                }
                                lastTime = time; // 更新lastTime的值为当前时间
                                //chart1.Series["Temperature"].ChartType = SeriesChartType.Spline; // 设置温度曲线的类型为样条曲线
                                //chart1.Series["Humidity"].ChartType = SeriesChartType.Spline; // 设置湿度曲线的类型为样条曲线
                            }
                        }

                        reader.Close(); // 关闭SqlDataReader对象
                        connection.Close(); // 关闭数据库连接
                        Console.WriteLine("数据读取显示完成:" + DateTime.Now); // 输出读取数据完成的时间

                    }));

                }
            }
            else
            {
                cnn_ddsj++; // 将cnn_ddsj的值加1
                if (cnn_ddsj >= 100) // 判断cnn_ddsj是否大于等于100
                {
                    Console.WriteLine("-------- if (cnn_ddsj >= 100)--------"); // 输出连接成功提示
                }
            }
        }




        #endregion

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
