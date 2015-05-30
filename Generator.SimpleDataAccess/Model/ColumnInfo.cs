using System;

namespace Generator.SimpleDataAccess.Model
{
    public class ColumnInfo
    {
        public String Name;
        public System.Data.SqlDbType DbType;
        public bool IsIdentity;
        public bool IsNullable;
        public bool IsComputed;
        public bool IsPartOfPrimaryKey;
        public bool IsPartOfUniqueIndex;
        public long MaxLength;
        public int Ordinal;
        public Type Type;
        public String MappingMethodName;
        public String FullTypeName;
        public String PropertyName;
        public String ParameterName;
        public String LocalVariableName;
        public String LocalParameterVariableName;

        public override string ToString()
        {
            return this.Name;
        }
    }
}
