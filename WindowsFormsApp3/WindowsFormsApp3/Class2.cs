using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{

    /// <summary>
    /// 基础通用类的测试
    /// </summary>
     class Class2
    {
        public void Insert()
        {
            string sql = "SELECT * FROM yourTableName";

            DataTable result = DatabaseConnection.ExecuteQuery(sql);
            Console.WriteLine("瘦影响的行数"+ result);
        }
    }
}
