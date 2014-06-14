using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Runner {
    public static class Core {
        public static IEnumerable<string> GetSqlQueriesFromFile(string filePath) {
            if (!File.Exists(filePath)) {
                throw new ArgumentException("'" + filePath + "' was not found.");
            }
            var sql = File.ReadAllText(filePath);
            return sql.Split(';');
        }
        public static IDatabaseRunner LoadVendorRunner(string vendor, string assemblypath = null) {
            IDatabaseRunner vendorRunner=null;
            if (vendor == null) {
                throw new ArgumentException("Please provide the vendor name to load the runner.");
            }
            var vendorRunnerPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "runners\\" + vendor + "runner.dll");
            if (!string.IsNullOrEmpty(assemblypath) && !File.Exists(assemblypath)) {
                throw new ArgumentException(assemblypath + " cannot be found.");
            }
            var connectionString = ConfigurationManager.ConnectionStrings[vendor];
            if (assemblypath == null) {
                vendorRunner = GetVendorRunner(vendor, vendorRunnerPath, connectionString);
            }
            else {
                try {
                    vendorRunner = GetVendorRunner(vendor, assemblypath, connectionString);
                }
                catch (Exception ex) {
                    Console.WriteLine("There was an error loading the vendor dll (" + assemblypath + "), attempting to load the " + vendor + "runner.dll from an alternate location." + Environment.NewLine + "Error message: " + ex.Message);
                }
                if (vendorRunner == null) {
                    vendorRunner = GetVendorRunner(vendor, vendorRunnerPath, connectionString);
                }
            }
            
            return vendorRunner;
        }

        private static IDatabaseRunner GetVendorRunner(string vendor, string assemblypath,ConnectionStringSettings connectionString) {
            IDatabaseRunner vendorRunner = null;
            var runnerAssembly = Assembly.LoadFile(assemblypath);
            var databaseRunnerType = runnerAssembly.GetType(vendor + "Runner.Runner");
            if (connectionString==null) {
                vendorRunner = Activator.CreateInstance(databaseRunnerType) as IDatabaseRunner;
            }
            else {
                vendorRunner = Activator.CreateInstance(databaseRunnerType, connectionString.ConnectionString) as IDatabaseRunner;
            }
            return vendorRunner;
        }
    }
}