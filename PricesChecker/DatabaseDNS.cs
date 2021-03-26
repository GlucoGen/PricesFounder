using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricesChecker
{
    class DatabaseDNS
    {
        readonly string DatabaseName = "GlucoBase";
        readonly string ServerName = "GlucoServer";
        string ConnectionString;

        public DatabaseDNS()
        {
            ConnectionString = @"Data Source=.\" + ServerName + ";Initial Catalog=" + DatabaseName + ";Integrated Security=True";
        }

        //получение списка категорий, которые не обновлялись сегодня
        public Dictionary<string,string> GetActualCategoriesFromBase()
        {
            Dictionary<string, string> Products = new Dictionary<string, string>();

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
                            Products.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());                         
                        }
                        connection.Close();
                        return Products;
                    }
                    connection.Close();
                    return Products;
                    
                }
            }
        }

        //добавление информации о товара в таблицу Prices
        public void AddPrice(string link, string name, string category, string priceText,string city, string site)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string priceTextNew = priceText.Substring(0, priceText.IndexOf('₽'));
                decimal price = Convert.ToDecimal(priceTextNew.Replace("₽","").Trim());

                name = name.Replace("'", "");

                string sql = "EXEC PricesAdd ";
                sql += "'" + link + "'" + ", '" + name + "'" + ", '" + category + "'" + ","  + price + ", '" + city + "'" + ", '" + site + "'";
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand(sql, connection);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        //Обновление даты в таблице StructureUrls
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
