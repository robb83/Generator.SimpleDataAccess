using System;
using System.Collections.Generic;

namespace Generator.SimpleDataAccess.Model
{
    public class IndexInfo
    {
        public String Name;
        public bool IsUnique;
        public bool IsPrimaryKey;
        public readonly List<ColumnInfo> Columns = new List<ColumnInfo>();

        public ColumnInfo GetColumnByName(String name)
        {
            foreach (ColumnInfo columnInfo in this.Columns)
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
            return this.Name;
        }
    }
}
