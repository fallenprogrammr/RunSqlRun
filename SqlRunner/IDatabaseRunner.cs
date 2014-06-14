using System.Collections.Generic;
using System.Data;

namespace Runner {
    public interface IDatabaseRunner {
        int RunFile(string filePath);
        int RunSql(string sqlString);
        int RunSql(string sqlString, IEnumerable<IDataParameter> parameters);
        int RunProcedure(string procedureName, IEnumerable<IDataParameter> parameters);
    }
}
