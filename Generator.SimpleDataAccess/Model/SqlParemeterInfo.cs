using System;

namespace Generator.SimpleDataAccess.Model
{
    public class SqlParemeterWithMappingInfo
    {
        public System.Data.SqlDbType DbType;
        public bool IsOutput;
        public bool IsNullable;
        public long MaxLength;
        public String MappingMethodName;
        public String FullTypeName;
        public String ParameterName;
        public String LocalVariableName;
        public String LocalParameterVariableName;
    }
}
