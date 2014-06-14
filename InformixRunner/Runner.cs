using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.Informix;
using Runner;

namespace InformixRunner {
    public class Runner : IDatabaseRunner {

        private readonly string informixConnectionString;

        public Runner(string connectionString) {
            informixConnectionString = connectionString;
        }

        public Runner(Dictionary<string, string> connectionStringParameters) {
            if (connectionStringParameters.Count < 7) {
                const string argumentExceptionMessage = "The connectionStringParameters dictionary passed has less than 7 entries, " +
                                                        "these are the minimum number of entries required to create a connection string:\n" +
                                                        "Database, Host, Server, Service, Protocol,UID and Password.";
                throw new ArgumentException(argumentExceptionMessage);
            }
            if (ConnectionStringParametersAreValid(connectionStringParameters)) {
                foreach (var connectionStringParameter in connectionStringParameters) {
                    informixConnectionString += connectionStringParameter.Key + "=" + connectionStringParameter.Value + ";";
                }
            }
            else {
                throw new ArgumentException("One or more connection string parameters specified are invalid, these are the entries required to be passed Database, Host, Server, Service, Protocol, UID and Password.");
            }
        }

        private bool ConnectionStringParametersAreValid(Dictionary<string, string> connectionStringParameters) {
            if (!connectionStringParameters.ContainsKey("Database")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Host")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Server")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Service")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("Protocol")) {
                return false;
            }
            if (!connectionStringParameters.ContainsKey("UID")) {
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

        public int RunSql(string sqlString) {
            int rowsAffected;
            using (var informixConnection = new IfxConnection(informixConnectionString)) {
                informixConnection.Open();
                using (var informixCommand = new IfxCommand(sqlString, informixConnection)) {
                    informixCommand.CommandType = CommandType.Text;
                    rowsAffected = informixCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunSql(string sqlString, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var informixConnection = new IfxConnection(informixConnectionString)) {
                informixConnection.Open();
                using (var informixCommand = new IfxCommand(sqlString, informixConnection)) {
                    informixCommand.CommandType = CommandType.Text;
                    foreach (IfxParameter param in parameters) {
                        informixCommand.Parameters.Add(param);
                    }
                    rowsAffected = informixCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        public int RunProcedure(string procedureName, IEnumerable<IDataParameter> parameters) {
            int rowsAffected;
            using (var informixConnection = new IfxConnection(informixConnectionString)) {
                informixConnection.Open();
                using (var informixCommand = new IfxCommand(procedureName, informixConnection)) {
                    informixCommand.CommandType = CommandType.StoredProcedure;
                    foreach (IfxParameter param in parameters) {
                        informixCommand.Parameters.Add(param);
                    }
                    rowsAffected = informixCommand.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }
    } 
}