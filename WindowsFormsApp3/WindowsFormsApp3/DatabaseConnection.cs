/*	<connectionStrings>
		<add name="MyConnectionString" connectionString="Data Source=192.168.3.129,1433;Initial Catalog=MISDB;User ID=sa;Password=123456;Initial Catalog=MISDB;Integrated Security=True" />
	</connectionStrings>
 * 
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;



namespace WindowsFormsApp3
{

    public class DatabaseConnection
    {
        //读取配置文件

        private static SqlConnection connection;
        private static string connectionString= ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString();
        // 构造函数，传入数据库服务器地址、数据库名、用户名和密码等信息
        public DatabaseConnection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ToString();
            // 创建 SqlConnection 对象
            SqlConnection connection = new SqlConnection(connectionString);
        }
        // 打开数据库连接
        public static void Connect()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
            }
        }
        // 关闭数据库连接
        public static void Disconnect()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // 如果连接没有关闭，就关闭连接
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        // 执行查询语句，并返回查询结果
        public static DataTable ExecuteQuery(string sql)
        {
            // 打开数据库连接


        
            Connect();
            SqlConnection connection = new SqlConnection(connectionString);
            // 创建 DataTable 对象
            var dataTable = new DataTable();
            // 创建 SqlCommand 对象，并设置 SQL 语句和连接对象
            var command = new SqlCommand(sql, connection);
            // 创建 SqlDataAdapter 对象，并设置 SqlCommand 对象
            var dataAdapter = new SqlDataAdapter(command);
            // 执行查询，并将结果填充到 DataTable 对象中
            dataAdapter.Fill(dataTable);
            // 关闭数据库连接
            Disconnect();
            // 返回查询结果
            return dataTable;
        }
        // 执行非查询语句，如插入、更新和删除等操作
        public static void ExecuteNonQuery(string sql)
        {
            // 打开数据库连接
            Connect();
            // 创建 SqlCommand 对象，并设置 SQL 语句和连接对象
            var command = new SqlCommand(sql, connection);
            // 执行非查询语句
            command.ExecuteNonQuery();
            // 关闭数据库连接
            Disconnect();
        }
    }
}
