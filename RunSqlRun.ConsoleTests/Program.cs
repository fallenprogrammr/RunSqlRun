using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RunSqlRun.ConsoleTests {
    class Program {
        static void Main(string[] args) {
            RunInformixTest();
            RunSqlServerTest();
            RunOracleTest();

        }

        private static void RunOracleTest() {
            var oracleRunner = new OracleRunner.Runner(ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString);
            Console.WriteLine(oracleRunner.RunSql("select sys_context('userenv','db_name') from dual"));
        }

        private static void RunSqlServerTest() {
            var sqlServerRunner = new SqlServerRunner.Runner(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString);
            Console.WriteLine(sqlServerRunner.RunSql("Select top 1 * from spt_monitor"));
        }

        private static void RunInformixTest() {
            var ifxRunner = new InformixRunner.Runner(ConfigurationManager.ConnectionStrings["Informix"].ConnectionString);
            Console.WriteLine(ifxRunner.RunSql("Select first 1 * from systables"));
        }
    }
}
