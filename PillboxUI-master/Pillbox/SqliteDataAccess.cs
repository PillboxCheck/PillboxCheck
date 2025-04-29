using Dapper;
using Microsoft.Data.Sqlite;
using Pillbox.entries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox
{
    public class SqliteDataAccess
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static List<T> LoadAll<T>(string table)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<T>($"SELECT * FROM {table}", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<T> Load<T>(string table, string feature)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<T>($"SELECT {feature} FROM {table}", new DynamicParameters());
                return output.ToList();
            }
        }

       public static void Save<T>(T data, string table, string fields ,string props)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                string savesql = $"INSERT INTO {table} ({fields}) VALUES ({props})";
                cnn.Execute(savesql, data);
            }
        }
        public static void ExecuteNonReturn<T>(T data, string sql)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                cnn.Execute(sql, data);
            }
        }

        public static void Update<T>(T data, string table, string setClause, string condition)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                string updateSql = $"UPDATE {table} SET {setClause} WHERE {condition}";
                cnn.Execute(updateSql, data);
            }
        }

        public static T FindEntry<T, V>(string table, string feature, V value)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                var result = cnn.QueryFirstOrDefault<T>(
                    $"SELECT * FROM {table} WHERE {feature} = @Value",
                    new { Value = value }
                );
                return result;
            }
        }

        public static void DeleteEntry<T, V>(string table, string feature, V value)
        {
            using (IDbConnection cnn = new SqliteConnection(LoadConnectionString()))
            {
                string deleteSql = $"DELETE FROM {table} WHERE {feature} = @Value";
                cnn.Execute(deleteSql, new { Value = value });
            }
        }


    }

}

