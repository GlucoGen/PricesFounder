using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesChecker
{
    class SQL
    {
        readonly string DatabaseName = "GlucoBase";
        readonly string ServerName = "GlucoServer";
        string ConnectionString;
        Dictionary<string, string> Goods;

        public SQL()
        {
            Goods = new Dictionary<string, string>();
            ConnectionString = @"Data Source=.\" + ServerName + ";Initial Catalog=" + DatabaseName + ";Integrated Security=True";
        }

        public Dictionary<string,string> GetActualGoodsFromBase()
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "EXEC StructureUrlsGetGoods";
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while(reader.Read()) // построчно считываем данные
                        {                         
                            Goods.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());                         
                        }
                        connection.Close();
                        return Goods;
                    }
                    connection.Close();
                    return Goods;
                    
                }
            }
        }

        public void AddPrice(string link, string name, string category, string priceText,string city, string site)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string priceTextNew = priceText.Substring(0, priceText.IndexOf('₽'));
                decimal price = Convert.ToDecimal(priceTextNew.Replace("₽","").Trim());

                string sql = "EXEC PricesAdd ";
                sql += "'" + link + "'" + ", '" + name + "'" + ", '" + category + "'" + ","  + price + ", '" + city + "'" + ", '" + site + "'";
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand(sql, connection);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void UpdateStructureUrls(string link)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "EXEC StructureUrlsUpdate ";
                sql += "'" + link + "'" ;
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand(sql, connection);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
