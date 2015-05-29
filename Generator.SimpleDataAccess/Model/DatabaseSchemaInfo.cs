using System;
using System.Collections.Generic;

namespace Generator.SimpleDataAccess.Model
{
    public class DatabaseSchemaInfo
    {
        public String Namespace;
        public String ClassName;
        public List<TableInfo> Tables;
        public List<StoredProcedureInfo> StoredProcedures;
    }
}
