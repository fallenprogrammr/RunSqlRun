using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Runner;

namespace SqlServerRunner {
    public class Runner : IDatabaseRunner {
        private readonly string sqlServerConnectionString;
        public Runner(string connectionString) {
            sqlServerConnectionString = connectionString;
        }

        public Runner(Dictionary<string, string> connectionStringParameters) {
            if (connectionStringParameters.Count < 4) {
                const string argumentExceptionMessage = "The connectionStringParameters dictionary passed has less than 4 entries, " +
                                                        "these are the minimum number of entries required to create a connection string:\n" +
                                                        "Server, Database, User Id and Password.";
                throw new ArgumentException(argumentExceptionMessage);
            }
            if (ConnectionStringParametersAreValid(connectionStringParameters)) {
                foreach (var connectionStringParameter in connectionStringParameters) {
                    sqlServerConnectionString += connectionStringParameter.Key + "=" + connectionStringParameter.Value + ";";
                }
            }
            else {
                throw new ArgumentException("One or more connection string parameters specified are invalid, these are the entries required to be passed Server, Database, User Id and Password.");
            }
        }

        private bool ConnectionStringParametersAreValid(Dictionary<string, string> connectionStringParameters) {
            if (!connectionStringParameters.ContainsKey("Server")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Database")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("User Id")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Password")) {
                return false;
            }
            return true;
        }

        public int RunFile(string filePath) {
            var sqlQueries = Core.GetSqlQueriesFromFile(filePath);
            return sqlQueries.Sum(query => RunSql(query));
        }

        public int RunSql(string sql) {
            int rowsAffected;
            using (var sqlServerConnection = new SqlConnection(sqlServerConnectionString)) {
                sqlServerConnection.Open();
                using (var sqlServerCommand = new SqlCommand(sql, sqlServerConnection)) {
                    sqlServerCommand.CommandType = CommandType.Text;
                    rowsAffected = sqlServerCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunSql(string sql, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var sqlServerConnection = new SqlConnection(sqlServerConnectionString)) {
                sqlServerConnection.Open();
                using (var sqlServerCommand = new SqlCommand(sql, sqlServerConnection)) {
                    sqlServerCommand.CommandType = CommandType.Text;
                    foreach (SqlParameter param in parameters) {
                        sqlServerCommand.Parameters.Add(param);
                    }
                    rowsAffected = sqlServerCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunProcedure(string procedure, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var sqlServerConnection = new SqlConnection(sqlServerConnectionString)) {
                sqlServerConnection.Open();
                using (var sqlServerCommand = new SqlCommand(procedure, sqlServerConnection)) {
                    sqlServerCommand.CommandType = CommandType.StoredProcedure;
                    foreach (SqlParameter param in parameters) {
                        sqlServerCommand.Parameters.Add(param);
                    }
                    rowsAffected = sqlServerCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }
    }
}