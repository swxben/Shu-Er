using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace SwxBen
{
    public interface IDataAccess
    {
        int ExecuteCommand(string sql, object parameters = null);
        IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null);
        IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new();
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
                    property.SetValue(t, resultDictionary[property.Name], null);
                }
                foreach (var field in typeof(T).GetFields().Where(f => resultDictionary.ContainsKey(f.Name)))
                {
                    field.SetValue(t, resultDictionary[field.Name]);
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

            parameter.Value = value == null ? DBNull.Value : value;

            if (value != null && value.GetType() == typeof(string))
            {
                parameter.Size = ((string)value).Length > 4000 ? -1 : 4000;
            }

            command.Parameters.Add(parameter);
        }
    }
}
