/*
 * 2023年4月3日-15点39分-
 * 
 * 调用方法 -- 示例
 *     static void Main(string[] args)
    {
        ServerSocket.Socket(); //调用Socket方法
    }
 * 
 * 
 */
using System; //引入System命名空间
using System.Net; //引入System.Net命名空间
using System.Net.Sockets; //引入System.Net.Sockets命名空间
using System.Text; //引入System.Text命名空间
using System.Data.SqlClient; //引入System.Data.SqlClient命名空间
using System.Configuration;
using System.Threading.Tasks;

namespace Cnn_2023年4月20日
{
    public class ServerSocket //定义ServerSocket类
    {
        private static byte[] buffer = new byte[1024]; //定义缓冲区
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //定义服务器套接字
        private static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString); //定义数据库连接
        private static float secondFloat;
        private static float receivedFloat;
        private static float firstFloat;
        private static byte[] data = new byte[8]; //定义数据
        public static void Socket() //定义Socket方法
        {
            Console.WriteLine("Starting server..."); //输出Starting server...
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 31255)); //绑定服务器套接字
            serverSocket.Listen(5); //监听连接
            Console.WriteLine("Server started. Listening for connections..."); //输出Server started. Listening for connections...
            Task taskListen = new Task(() =>
            {
                IPAddress clientAddress = null;
                while (true) //循环
                {
                    Socket clientSocket = null;
                    try
                    {
                        clientSocket = serverSocket.Accept(); //接受客户端套接字
                        Console.WriteLine("Socket对象的哈希码是：{0}", clientSocket.GetHashCode().ToString("X"));
                        Console.WriteLine("Client connected." + DateTime.Now); //输出Client connected.
                        Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint.ToString()); // 输出客户端套接字的值
                        clientAddress = ((IPEndPoint)clientSocket.RemoteEndPoint).Address;
                        Console.WriteLine("Client IP Address: " + clientAddress);
                        clientSocket.ReceiveTimeout = 50000; //设置接收数据等待时间为5秒

                        try
                        {
                            Console.WriteLine("接收数据等待..." + DateTime.Now);
                            int receivedBytes = clientSocket.Receive(data); //接收数据
                        }
                        catch (SocketException ex) //捕获SocketException异常
                        {
                            if (ex.SocketErrorCode == SocketError.TimedOut) //如果接收数据超时
                            {
                                Console.WriteLine("接收数据超时，关闭套接字.-重新等待连接." + DateTime.Now);
                                clientSocket.Shutdown(SocketShutdown.Both); // 关闭套接字
                                clientSocket.Close();
                                continue; //重新等待连接
                            }
                        }

                    }
                    catch (SocketException ex) //捕获SocketException异常
                    {
                        Console.WriteLine("接受客户端连接时发生SocketException异常：{0}", ex.Message);
                        if (clientSocket != null)
                        {
                            clientSocket.Close();
                        }
                        continue; //重新等待连接
                    }


                    #region 老的字节转换
                    //// Determine the byte order of the system
                    //bool isLittleEndian = BitConverter.IsLittleEndian;
                    //// Convert the received 16 bytes to a float with the appropriate byte order
                    //if (isLittleEndian)
                    //{
                    //    Array.Reverse(data);
                    //    // Extract the first 4-byte data and convert it to a float
                    //    firstFloat = BitConverter.ToSingle(data, 0);

                    //    // Extract the second 4-byte data and convert it to a float
                    //    secondFloat = BitConverter.ToSingle(data, 4);
                    //    Console.WriteLine("Array.Reverse(data);.");
                    //}
                    //else
                    //{

                    //    receivedFloat = BitConverter.ToSingle(data, 0);
                    //    Console.WriteLine("receivedFloat");

                    //}

                    #endregion


                    /*
                     * 这段代码的作用是将一个长度为 16 字节的 byte 数组转换成两个 float 类型的数据。
                     * 首先，
                     * 代码通过 BitConverter.IsLittleEndian 方法确定当前系统的字节顺序是大端序还是小端序，
                     * 并将结果存储在 isLittleEndian 变量中。
                     * 如果字节顺序是小端序，
                     * 代码会先通过 Array.Reverse(data) 方法将数组中的字节顺序翻转，
                     * 再使用 BitConverter.ToSingle 方法将前 4 个字节和后 4 个字节分别转换成两个 float 类型的数据，
                     * 并将其存储在 firstFloat 和 secondFloat 变量中。如果字节顺序是大端序，
                     * 代码则直接使用 BitConverter.ToSingle 方法将整个数组转换成一个 float 类型的数据，
                     * 并将其存储在 receivedFloat 变量中。最后，代码使用 Console.WriteLine 方法输出一些调试信息。
                     */

                    // 确定系统的字节顺序
                    bool isLittleEndian = BitConverter.IsLittleEndian;
                    // 使用适当的字节顺序将接收到的 16 字节转换为 float 类型的数据
                    if (isLittleEndian) // 如果当前系统的字节顺序是小端序
                    {
                        Array.Reverse(data); // 将接收到的 16 字节的字节顺序翻转
                                             // 提取前 4 个字节的数据并将其转换为 float 类型
                        firstFloat = BitConverter.ToSingle(data, 0);
                        // 提取后 4 个字节的数据并将其转换为 float 类型
                        secondFloat = BitConverter.ToSingle(data, 4);

                        string hexString = BitConverter.ToString(data);

                        Console.WriteLine("Array.Reverse(data);."+ hexString); // 输出一些调试信息
                    }
                    else // 如果当前系统的字节顺序是大端序
                    {
                        receivedFloat = BitConverter.ToSingle(data, 0); // 直接将 16 字节的数据转换为 float 类型
                        Console.WriteLine("receivedFloat"); // 输出一些调试信息
                    }






                    Console.WriteLine("Client connected. " + firstFloat + " - " + secondFloat);
                    // Convert the float to a string
                    string humidity = firstFloat.ToString();

                    string temperature = secondFloat.ToString();

                    Console.WriteLine("Received message: " + temperature); //输出Received message: + message
                    DateTime time = DateTime.Now; // 获取当前时间
                    try
                    {
                        SqlCommand sqlCommand = new SqlCommand("INSERT INTO textTableName (TEMPERATURE, HUMIDITY, Time, ip) VALUES (@message,@message_1, @Time, @ipp)", sqlConnection); //定义SQL命令
                        sqlCommand.Parameters.AddWithValue("@message", temperature); //添加参数
                        sqlCommand.Parameters.AddWithValue("@message_1", humidity); //添加参数
                        sqlCommand.Parameters.AddWithValue("@Time", time);
                        sqlCommand.Parameters.AddWithValue("@ipp", clientAddress.ToString());
                        sqlConnection.Open(); //打开数据库连接
                        sqlCommand.ExecuteNonQuery(); //执行SQL命令
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error executing SQL command: " + ex.Message); //输出错误信息
                    }
                    finally
                    {
                        sqlConnection.Close(); //关闭数据库连接
                    }

                    //sqlConnection.Close(); //关闭数据库连接
                    clientSocket.Shutdown(SocketShutdown.Both); //关闭套接字
                    clientSocket.Close(); //关闭套接字
                    Console.WriteLine("Client disconnected."); //输出Client disconnected.
                }
            });

            taskListen.Start();
        }

        public static float HexToFloat(string hex)
        {
            int num = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            byte[] bytes = BitConverter.GetBytes(num);
            return BitConverter.ToSingle(bytes, 0);
        }
    }




}
