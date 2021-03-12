using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesFounder
{
    class SQL
    {
        readonly string DatabaseName = "GlucoBase";
        readonly string ServerName = "GlucoServer";
        string ConnectionString;

        public SQL()
        {

            ConnectionString= @"Data Source=.\"+ ServerName + ";Initial Catalog="+ DatabaseName + ";Integrated Security=True";
        }
        public bool InsertStructureUrlToBase(Dictionary<string,string> data)
        {
           
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                // Создаем объект DataAdapter
           
                foreach (KeyValuePair<string, string> kvp in data)
                {
                    string sql = "EXEC StructureUrlsAdd  ";
                    sql += "'" + kvp.Value + "'" + ", '" + kvp.Key + "'";
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    SqlCommand command = new SqlCommand(sql, connection);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
            }
            return true;
        }
    }
}
