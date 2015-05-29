using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.SimpleDataAccess.Generators
{
    public static class SQLTextGenerator
    {
        private static void GenerateWhereClause(StringBuilder script, List<ColumnInfo> filterColumns)
        {
            if (filterColumns == null || filterColumns.Count == 0)
            {
                return;
            }

            int counter = 0;
            
            ColumnInfo columnInfo = filterColumns[counter++];
            script.AppendFormat(" WHERE [{0}] = {1}", columnInfo.Name, columnInfo.ParameterName);

            for (; counter < filterColumns.Count;)
            {
                columnInfo = filterColumns[counter++];
                script.AppendFormat(" AND [{0}] = {1}", columnInfo.Name, columnInfo.ParameterName);
            }
        }

        private static void GenerateProjection(StringBuilder script, List<ColumnInfo> columns)
        {
            if (columns == null || columns.Count == 0)
            {
                return;
            }

            int counter = 0;
            
            ColumnInfo columnInfo = columns[counter++];
            script.AppendFormat("[{0}]", columnInfo.Name);

            for (; counter < columns.Count;)
            {
                columnInfo = columns[counter++];
                script.AppendFormat(", [{0}]", columnInfo.Name);
            }
        }

        public static String GenerateSelectCount(TableInfo table, List<ColumnInfo> filterColumns = null)
        {
            StringBuilder script = new StringBuilder();

            script.AppendFormat("SELECT Count(*) FROM [{0}].[{1}]", table.Schema, table.Name);

            GenerateWhereClause(script, filterColumns);

            return script.ToString();
        }

        public static String GeneratePagedSelect(TableInfo table, String firstIndexParameterName, String lastIndexParameterName, List<ColumnInfo> orderColumns, List<ColumnInfo> filterColumns = null)
        {
            // SELECT * FROM ( SELECT [Column1], [Column2], ROW_NUMBER() OVER(ORDER BY PrimaryKey) AS _ROW_NUMBER FROM [dbo].[Table]) AS T0 WHERE _ROW_NUMBER BETWEEN @FirstIndexParameterNAme AND @LastIndexParameterName ORDER BY PrimaryKey

            StringBuilder script = new StringBuilder();
            script.Append("SELECT * FROM ( SELECT ");
            
            GenerateProjection(script, table.Columns);

            script.Append(", ROW_NUMBER() OVER(ORDER BY ");
            GenerateProjection(script, orderColumns);
            script.AppendFormat(") AS _ROW_NUMBER FROM [{0}].[{1}]", table.Schema, table.Name);
            GenerateWhereClause(script, filterColumns);
            script.Append(" ) AS T0 WHERE _ROW_NUMBER BETWEEN ");
            script.Append(firstIndexParameterName);
            script.Append(" AND ");
            script.Append(lastIndexParameterName);
            script.Append(" ORDER BY ");
            GenerateProjection(script, orderColumns);

            return script.ToString();
        }

        public static String GenerateSelect(TableInfo table, List<ColumnInfo> filterColumns = null)
        {
            StringBuilder script = new StringBuilder();

            script.Append("SELECT ");

            GenerateProjection(script, table.Columns);

            script.AppendFormat(" FROM [{0}].[{1}]", table.Schema, table.Name);

            GenerateWhereClause(script, filterColumns);

            return script.ToString();
        }

        public static String GenerateDelete(TableInfo table, List<ColumnInfo> filterColumns)
        {
            StringBuilder script = new StringBuilder();

            script.AppendFormat("DELETE FROM [{0}].[{1}]", table.Schema, table.Name);

            GenerateWhereClause(script, filterColumns);

            return script.ToString();
        }

        public static String GenerateUpdate(TableInfo table, List<ColumnInfo> filterColumns)
        {
            StringBuilder script = new StringBuilder();

            script.AppendFormat("UPDATE [{0}].[{1}] SET ", table.Schema, table.Name);

            int counter = 0;
            foreach (var c in table.Columns)
            {
                if (c.IsIdentity || c.IsComputed || filterColumns.Contains(c))
                {
                    continue;
                }

                if (counter > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}] = {1}", c.Name, c.ParameterName);

                ++counter;
            }

            GenerateWhereClause(script, filterColumns);

            return script.ToString();
        }

        public static String GenerateInsert(TableInfo table)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("INSERT INTO [{0}].[{1}] (", table.Schema, table.Name);

            int counter = 0;
            foreach (var c in table.Columns)
            {
                if (c.IsIdentity || c.IsComputed)
                {
                    continue;
                }

                if (counter > 0)
                {
                    sb.Append(", ");
                }

                sb.AppendFormat("[{0}]", c.Name);

                ++counter;
            }

            sb.Append(") VALUES (");

            counter = 0;
            foreach (var c in table.Columns)
            {
                if (c.IsIdentity || c.IsComputed)
                {
                    continue;
                }

                if (counter > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(c.ParameterName);
                ++counter;
            }

            sb.Append(");");

            foreach (var c in table.Columns)
            {
                if (c.IsIdentity)
                {
                    sb.AppendFormat(" SET {0} = SCOPE_IDENTITY(); ", c.ParameterName);
                    break;
                }
            }

            return sb.ToString();
        }
    }
}
