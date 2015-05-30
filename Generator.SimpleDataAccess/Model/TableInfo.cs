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

        public ColumnInfo GetFirstIdentityColumn()
        {
            foreach (var c in this.Columns)
            {
                if (c.IsIdentity)
                {
                    return c;
                }
            }

            return null;
        }

        public List<ColumnInfo> GetFirstPrimaryKey()
        {
            foreach (var i in this.Indexes)
            {
                if (i.IsPrimaryKey)
                {
                    return i.Columns;
                }
            }

            return null;
        }

        public List<ColumnInfo> GetFirstUniqueKey()
        {
            foreach (var i in this.Indexes)
            {
                if (i.IsUnique)
                {
                    return i.Columns;
                }
            }

            return null;
        }


        public bool HasComputedColumns()
        {
            foreach (var c in this.Columns)
            {
                if (c.IsComputed)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasIdentityColumn()
        {
            foreach (var c in this.Columns)
            {
                if (c.IsIdentity)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasIdentityOrPrimaryKeyOrUniqueIndex()
        {
            foreach (ColumnInfo columnInfo in this.Columns)
            {
                if (columnInfo.IsIdentity)
                {
                    return true;
                }
            }

            foreach (IndexInfo indexInfo in this.Indexes)
            {
                if (indexInfo.IsPrimaryKey || indexInfo.IsUnique)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return this.Schema + "." + this.Name;
        }
    }
}
