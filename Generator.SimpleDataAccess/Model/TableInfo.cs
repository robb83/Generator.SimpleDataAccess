using System;
using System.Collections.Generic;

namespace Generator.SimpleDataAccess.Model
{
    public class TableInfo
    {
        public int ObjectID;
        public String Name;
        public String Schema;
        public List<ColumnInfo> Columns = new List<ColumnInfo>();
        public List<ForeignKeyInfo> ForeignKeys = new List<ForeignKeyInfo>();
        public List<IndexInfo> Indexes = new List<IndexInfo>();
        public bool HasIndentity;
        public String ClassName;

        public ColumnInfo GetColumnByName(String name)
        {
            foreach(ColumnInfo columnInfo in this.Columns)
            {
                if (columnInfo.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return columnInfo;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return this.Schema + "." + this.Name;
        }
    }
}
