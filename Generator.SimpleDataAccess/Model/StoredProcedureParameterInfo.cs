using System;

namespace Generator.SimpleDataAccess.Model
{
    public class StoredProcedureParameterInfo
    {
        public String Name;
        public Type Type;
        public System.Data.SqlDbType DbType;
        public bool IsOutput;
        public bool HasDefault;
        public String MappingMethodName;
        public String FullTypeName;
        public String ParameterName;
        public String LocalVariableName;

        public override string ToString()
        {
            return this.Name;
        }
    }
}
