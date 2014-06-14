using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.DataAccess.Client;
using Runner;

namespace OracleRunner {
    public class Runner : IDatabaseRunner {

        private readonly string oracleConnectionString;
        public Runner(string connectionString) {
            oracleConnectionString = connectionString;
        }

        public Runner(Dictionary<string,string> connectionStringParameters) {
            if (connectionStringParameters.Count < 3) {
                const string argumentExceptionMessage = "The connectionStringParameters dictionary passed has less than 4 entries, " +
                                                        "these are the minimum number of entries required to create a connection string:\n" +
                                                        "Data Source, User Id and Password.";
                throw new ArgumentException(argumentExceptionMessage);
            }
            if (ConnectionStringParametersAreValid(connectionStringParameters)) {
                foreach (var connectionStringParameter in connectionStringParameters) {
                    oracleConnectionString += connectionStringParameter.Key + "=" + connectionStringParameter.Value + ";";
                }
            }
            else {
                throw new ArgumentException("One or more connection string parameters specified are invalid, these are the entries required to be passed Data Source, User Id and Password.");
            }
        }

        private bool ConnectionStringParametersAreValid(Dictionary<string, string> connectionStringParameters) {
            if (!connectionStringParameters.ContainsKey("Data Source")) {
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
            using (var oracleConnection = new OracleConnection()) {
                oracleConnection.ConnectionString = oracleConnectionString;
                oracleConnection.Open();
                using (var oracleCommand = new OracleCommand()) {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = sql;
                    oracleCommand.CommandType = CommandType.Text;
                    rowsAffected = oracleCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunSql(string sql, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var oracleConnection = new OracleConnection()) {
                oracleConnection.ConnectionString = oracleConnectionString;
                oracleConnection.Open();
                using (var oracleCommand = new OracleCommand()) {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = sql;
                    oracleCommand.CommandType = CommandType.Text;
                    foreach (OracleParameter param in parameters) {
                        oracleCommand.Parameters.Add(param);
                    }
                    rowsAffected = oracleCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }


        public int RunProcedure(string procedure, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var oracleConnection = new OracleConnection()) {
                oracleConnection.ConnectionString = oracleConnectionString;
                oracleConnection.Open();
                using (var oracleCommand = new OracleCommand()) {
                    oracleCommand.Connection = oracleConnection;
                    oracleCommand.CommandText = procedure;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    foreach (OracleParameter param in parameters) {
                        oracleCommand.Parameters.Add(param);
                    }
                    rowsAffected = oracleCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }
    }
}
