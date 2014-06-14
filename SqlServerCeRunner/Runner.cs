using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Linq;
using Runner;

namespace SqlServerCeRunner {
    public class Runner : IDatabaseRunner {
        private string sqlServerCeConnectionString;

        public Runner() {
            //default constructor, required for example to instantiate from a command-line application.
        }
        public Runner(string ceConnectionString) {
            sqlServerCeConnectionString = ceConnectionString;
        }

        public Runner(Dictionary<string, string> connectionStringParameters) {
            if (connectionStringParameters.Count < 2) {
                const string argumentExceptionMessage = "The connectionStringParameters dictionary passed has less than 4 entries, " +
                                                        "these are the minimum number of entries required to create a connection string:\n" +
                                                        "Data Source and Password.";
                throw new ArgumentException(argumentExceptionMessage);
            }
            if (ConnectionStringParametersAreValid(connectionStringParameters)) {
                foreach (var connectionStringParameter in connectionStringParameters) {
                    sqlServerCeConnectionString += connectionStringParameter.Key + "=" + connectionStringParameter.Value + ";";
                }
            }
            else {
                throw new ArgumentException("One or more connection string parameters specified are invalid, these are the entries required to be passed Server, Database, User Id and Password.");
            }
        }

        private bool ConnectionStringParametersAreValid(Dictionary<string, string> connectionStringParameters) {
            if (!connectionStringParameters.ContainsKey("Data Source")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Password")) {
                return false;
            }
            return true;
        }

        public int RunFile(string filePath) {
            var sqlQueries = Core.GetSqlQueriesFromFile(filePath);
            return sqlQueries.ToList().Sum(query => RunSql(query));
        }

        public int RunSql(string sql) {
            var rowsAffected = 0;
            if (sqlServerCeConnectionString != null) {
                if (!(string.IsNullOrEmpty(sql) || sql.Trim() == string.Empty)) {
                    using (var sqlServerConnection = new SqlCeConnection(sqlServerCeConnectionString)) {
                        sqlServerConnection.Open();
                        using (var sqlServerCommand = new SqlCeCommand(sql, sqlServerConnection)) {
                            rowsAffected = sqlServerCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            else {
                if (sql.StartsWith("create database", true, CultureInfo.InvariantCulture)) {
                    var databaseFileName = GetDatabaseFileNameFromSql(sql);
                    var databasePassword = GetDatabasePasswordFromSql(sql);
                    var connString = "Data Source=" + databaseFileName + ";" +
                        (databasePassword == null ? "" : "Password=" + databasePassword);
                    using (var engine = new SqlCeEngine(connString)) {
                        if (File.Exists(databaseFileName.Replace("'", ""))) {
                            File.Delete(databaseFileName.Replace("'", ""));
                        }
                        engine.CreateDatabase();
                        sqlServerCeConnectionString = connString;
                    }
                }
                else {
                    throw new ApplicationException("The input file does not start with the 'CREATE DATABASE' statement, rsr cannot evaluate the database name to connect to as there is no connection string by the name of 'SqlServerCe' defined.");
                }
            }

            return rowsAffected;
        }

        private static string GetDatabasePasswordFromSql(string sql) {
            return sql.ToLower().Contains("databasepassword") ? sql.Split(' ')[4].Replace(";", "") : null;
        }

        private static string GetDatabaseFileNameFromSql(string sql) {
            return sql.Split(' ')[2].Replace("'", "\"");
        }

        public int RunSql(string sql, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var sqlServerConnection = new SqlCeConnection(sqlServerCeConnectionString)) {
                sqlServerConnection.Open();
                using (var sqlServerCommand = new SqlCeCommand(sql, sqlServerConnection)) {
                    foreach (SqlCeParameter param in parameters) {
                        sqlServerCommand.Parameters.Add(param);
                    }
                    rowsAffected = sqlServerCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunProcedure(string procedure, IEnumerable<IDataParameter> parameters) {
            throw new ApplicationException("Stored procedures are not supported in Sql Server Compact Edition.");
        }
    }
}
