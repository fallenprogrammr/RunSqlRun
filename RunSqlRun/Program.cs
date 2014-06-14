using Runner;
using System;
using System.IO;

namespace RunSqlRun {
    class Program {
        static void Main(string[] args) {
            //rsr v:Informix/Oracle/SqlServer/Vendor s:File/Sql
            if (args.Length < 1) {
                ShowSyntax();
            }
            else {
                var tiny = new Tiny(args);
                if (IsArgumentSyntaxValid(tiny.Arguments)) {
                    IDatabaseRunner runner = Core.LoadVendorRunner(tiny.Arguments.v);
                    if (File.Exists(tiny.Arguments.s)) {
                        runner.RunFile(tiny.Arguments.s);
                    }
                    else {
                        runner.RunSql(tiny.Arguments.s);
                    }
                }
                else {
                    ShowSyntax();
                }
            }
        }

        private static bool IsArgumentSyntaxValid(dynamic arguments) {
            if (string.IsNullOrEmpty(arguments.v) || string.IsNullOrWhiteSpace(arguments.v)) {
                return false;
            }
            if (string.IsNullOrEmpty(arguments.s) || string.IsNullOrWhiteSpace(arguments.s)) {
                return false;
            }
            return true;
        }

        static void ShowSyntax() {
            Console.WriteLine("Syntax for rsr:");
            Console.WriteLine("rsr v:<Database Vendor> s:<Sql to execute or file containing sql to execute> ");
            Console.WriteLine("Database vendors can be any of the supported / downloaded / installed dlls, example: Oracle, SqlServer, etc.");
            Console.WriteLine(@"Sql to execute is a SQL string enclosed in "".");
            Console.WriteLine(@"File containing sql can contain multiple sql statements, these will be executed one by one.");
            Console.WriteLine(@"Example: rsr v:SqlServer s:""SELECT FIRST 1 * FROM TABLE""");
            Console.WriteLine("Example: rsr v:Oracle s:C:\\Windows\\Temp\\test.sql");
        }
    }
}
