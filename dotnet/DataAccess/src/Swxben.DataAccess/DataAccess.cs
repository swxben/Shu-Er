using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace SwxBen
{
    public interface IDataAccess
    {
        int ExecuteCommand(string sql, object parameters = null);
        IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null);
        IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new();
        void Insert<T>(T value);
        void Update<T>(T value, string id);
        void DropTable(string tableName);
    }

    public class DataAccess : IDataAccess
    {
        string _connectionString = "";

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int ExecuteCommand(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = GetCommand(sql, connection, parameters);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = GetCommand(sql, connection, parameters);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new ExpandoObject();
                        var resultDictionary = result as IDictionary<string, object>;

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            resultDictionary.Add(reader.GetName(i), DBNull.Value.Equals(reader[i]) ? null : reader[i]);
                        }

                        yield return result;
                    }
                }
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new()
        {
            return ExecuteQuery(sql, parameters).Select(result =>
            {
                var resultDictionary = result as IDictionary<string, object>;
                var t = new T();

                foreach (var property in typeof(T).GetProperties().Where(p => resultDictionary.ContainsKey(p.Name)))
                {
                    var value = resultDictionary[property.Name];
                    if (value != null && value is string && property.PropertyType.IsEnum)
                    {
                        value = Enum.Parse(property.PropertyType, value as string);
                    }

                    property.SetValue(t, value, null);
                }
                foreach (var field in typeof(T).GetFields().Where(f => resultDictionary.ContainsKey(f.Name)))
                {
                    var value = resultDictionary[field.Name];
                    if (value != null && value is string && field.FieldType.IsEnum)
                    {
                        value = Enum.Parse(field.FieldType, value as string);
                    }

                    field.SetValue(t, value);
                }

                return t;
            });
        }

        private static SqlCommand GetCommand(string sql, SqlConnection connection, object parameters)
        {
            var command = new SqlCommand();

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetProperties())
                {
                    var value = property.GetValue(parameters, null);
                    AddParameterValueToCommand(command, property.Name, value);
                }
                foreach (var field in parameters.GetType().GetFields())
                {
                    var value = field.GetValue(parameters);
                    AddParameterValueToCommand(command, field.Name, value);
                }
            }

            return command;
        }

        private static void AddParameterValueToCommand(SqlCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", name);

            if (value != null && value.GetType().IsEnum) value = value.ToString();

            parameter.Value = value == null ? DBNull.Value : value;

            if (value != null && value.GetType() == typeof(string))
            {
                parameter.Size = ((string)value).Length > 4000 ? -1 : 4000;
            }

            command.Parameters.Add(parameter);
        }

        public void Insert<T>(T value)
        {
            ExecuteCommand(GetInsertSqlFor<T>(), value);
        }

        public void Update<T>(T value, string id)
        {
            ExecuteCommand(GetUpdateSqlFor<T>(id), value);
        }

        public static string GetInsertSqlFor<T>()
        {
            var fieldsSql = new StringBuilder();
            var valuesSql = new StringBuilder();
            foreach (var field in GetAllFieldNames<T>())
            {
                if (!string.IsNullOrEmpty(fieldsSql.ToString()))
                {
                    fieldsSql.Append(", ");
                    valuesSql.Append(", ");
                }
                fieldsSql.Append(field);
                valuesSql.Append("@").Append(field);
            }

            return string.Format(
                "INSERT INTO {0}s({1}) VALUES({2})",
                typeof(T).Name,
                fieldsSql,
                valuesSql);
        }

        public static string GetUpdateSqlFor<T>(string id)
        {
            var set = new StringBuilder();

            foreach (var field in GetAllFieldNames<T>())
            {
                if (field.ToUpper() == id.ToUpper()) continue;
                if (!string.IsNullOrEmpty(set.ToString())) set.Append(", ");
                set.AppendFormat("{0} = @{0}", field);
            }

            return string.Format(
                "UPDATE {0}s SET {1} WHERE {2} = @{2}",
                typeof(T).Name,
                set,
                id);
        }

        private static IEnumerable<string> GetAllFieldNames<T>()
        {
            return
                typeof(T).GetFields().Select(f => f.Name)
                .Concat(typeof(T).GetProperties().Select(p => p.Name));
        }

        public void DropTable(string tableName)
        {
            var sql = string.Format(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U'))
    DROP TABLE {0}", tableName);

            ExecuteCommand(sql);
        }
    }
}
