using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AdoNetPractice
{
    class EntryPoint
    {
        const string SQL_SERVER_CONN_STRING = "Server=.\\SQLEXPRESS;Database=Northwind;Trusted_Connection=True;";
        static string XLSX_CONN_STRING = @"D:\Telerik\TelerikAcademy_2016-2017\Databases\04-ADO.NET\AdoNetPractice\teachers.xlsx";

        const int OLE_METAFILEPICT_START_POSITION = 78;

        public static void NumberOfCategoriesRows(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Categories", conn);

            int categoriesCount = (int)command.ExecuteScalar();
            Console.WriteLine($"Number of Categories rows: {categoriesCount}");
        }

        public static void RetrieveCategories(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand("SELECT CategoryName, Description FROM Categories", conn);

            var reader = command.ExecuteReader();

            using (reader)
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Category Name: {reader[0]}");
                    Console.WriteLine($"\tDescription: {reader[1]}");
                }
            }
        }

        public static void CategoriesAndProducts(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand("SELECT c.CategoryName, p.ProductName " +
                                                "FROM Products p " +
                                                "INNER JOIN Categories c " +
                                                "ON p.CategoryID = c.CategoryID " +
                                                "ORDER BY c.CategoryName ", conn);

            var reader = command.ExecuteReader();

            var categoryWithProducts = new Dictionary<string, List<string>>();

            using (reader)
            {
                while (reader.Read())
                {
                    var category = (string)reader["CategoryName"];
                    var product = (string)reader["ProductName"];

                    if (!categoryWithProducts.ContainsKey(category))
                    {
                        categoryWithProducts[category] = new List<string>();
                        categoryWithProducts[category].Add(product);

                    }
                    else
                    {
                        categoryWithProducts[category].Add(product);
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> kv in categoryWithProducts)
            {
                Console.WriteLine($"{kv.Key} : {string.Join(",", kv.Value)}");
                Console.WriteLine("=======================");
            }
        }

        public static void InsertProduct(SqlConnection conn, string productName, int? supplierID, int? categoryID,
            string quantityPerUnit, decimal? unitPrice, short? unitsInStock, short? unitsOnOrder, short? reorderLevel,
            bool discounter)
        {
            SqlCommand command = new SqlCommand(
                "INSERT INTO Products VALUES (@productName, @supplierID , @categoryID ,@quantityPerUnit , @unitPrice, " +
                "@unitsInStock, @unitsOnOrder, @reorderLevel, @discounter)", conn);


            command.Parameters.AddWithValue("@productName", productName);
            command.Parameters.AddWithValue("@supplierID", supplierID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@categoryID", categoryID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@quantityPerUnit", quantityPerUnit ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@unitPrice", unitPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@unitsInStock", unitsInStock ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@unitsOnOrder", unitsOnOrder ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@reorderLevel", reorderLevel ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@discounter", discounter);

            int rowsAffected = command.ExecuteNonQuery();

            Console.WriteLine("Rows affected on inserting Product: " + rowsAffected);
        }

        public static void SaveImageToHDD(SqlConnection conn)
        {
            SqlCommand command = new SqlCommand(
                "SELECT Picture FROM Categories", conn);

            var reader = command.ExecuteReader();
            
            using (reader)
            {
                int imageId = 0;
                while (reader.Read())
                {
                    var pictureBytes = (byte[])reader[0];

                    using (MemoryStream ms = new MemoryStream(pictureBytes, OLE_METAFILEPICT_START_POSITION,
                                                pictureBytes.Length - OLE_METAFILEPICT_START_POSITION))
                    {
                        var img = Image.FromStream(ms);
                        
                        img.Save($"{imageId++}.jpg", ImageFormat.Jpeg);
                    }
                }
                Console.WriteLine("Images are saved");
            }
        }

        public static void ReadFromXlsx(OleDbConnection conn)
        {
            OleDbCommand command = new OleDbCommand(
                "SELECT * FROM [teachers$]", conn);

            var reader = command.ExecuteReader();

            using (reader)
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            }
        }

        static void Main()
        {
            SqlConnection dbConSql = new SqlConnection(SQL_SERVER_CONN_STRING);
            dbConSql.Open();

            using (dbConSql)
            {
                NumberOfCategoriesRows(dbConSql);
                RetrieveCategories(dbConSql);
                CategoriesAndProducts(dbConSql);
                InsertProduct(dbConSql, "Adidas", 7, null, "4x20", null, null, null, 43, true);
                SaveImageToHDD(dbConSql);
            }

            OleDbConnection dbConXlsx = new OleDbConnection(XLSX_CONN_STRING);

            using (dbConXlsx)
            {
                ReadFromXlsx(dbConXlsx);
            }

        }
    }
}
