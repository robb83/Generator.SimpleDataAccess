﻿using Generator.SimpleDataAccess.Model;
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

        public static String GenerateSelectPaged(TableInfo table, String firstIndexParameterName, String lastIndexParameterName, List<ColumnInfo> orderColumns, List<ColumnInfo> filterColumns = null)
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

            script.Append(";");
            GenerateReRead(script, table, true, filterColumns);

            return script.ToString();
        }

        public static String GenerateInsert(TableInfo table)
        {
            StringBuilder script = new StringBuilder();

            script.AppendFormat("INSERT INTO [{0}].[{1}] (", table.Schema, table.Name);

            int counter = 0;
            foreach (var c in table.Columns)
            {
                if (c.IsIdentity || c.IsComputed)
                {
                    continue;
                }

                if (counter > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}]", c.Name);

                ++counter;
            }

            script.Append(") VALUES (");

            counter = 0;
            foreach (var c in table.Columns)
            {
                if (c.IsIdentity || c.IsComputed)
                {
                    continue;
                }

                if (counter > 0)
                {
                    script.Append(", ");
                }

                script.Append(c.ParameterName);
                ++counter;
            }

            script.Append(");");

            GenerateReRead(script, table, false);

            return script.ToString();
        }

        public static String GenerateMerge(TableInfo tableInfo, List<ColumnInfo> key)
        {
            if (key == null || key.Count == 0)
            {
                throw new NotSupportedException("No key");
            }

            StringBuilder script = new StringBuilder();

            script.AppendFormat("MERGE [{0}].[{1}] AS T USING (SELECT ", tableInfo.Schema, tableInfo.Name);

            if (tableInfo.Columns.Count > 0)
            {
                int counter = 0;
                script.Append(tableInfo.Columns[counter++].ParameterName);

                while (counter < tableInfo.Columns.Count)
                {
                    script.Append(", ");
                    script.Append(tableInfo.Columns[counter++].ParameterName);
                }
            }

            script.Append(") AS S (");

            if (tableInfo.Columns.Count > 0)
            {
                int counter = 0;
                script.Append(tableInfo.Columns[counter++].Name);

                while (counter < tableInfo.Columns.Count)
                {
                    script.AppendFormat(", [{0}]", tableInfo.Columns[counter++].Name);
                }
            }

            script.Append(") ON (");

            if (key.Count > 0)
            {
                int counter = 0;
                script.AppendFormat("S.[{0}] = T.[{0}]", key[counter++].Name);

                while (counter < key.Count)
                {
                    script.AppendFormat(" AND S.[{0}] = T.[{0}]", key[counter++].Name);
                }
            }

            script.Append(")");

            if (tableInfo.Columns.Count > 0)
            {
                StringBuilder subScript = new StringBuilder();

                foreach (ColumnInfo columnInfo in tableInfo.Columns)
                {
                    if (columnInfo.IsComputed || columnInfo.IsIdentity || key.Contains(columnInfo))
                    {
                        continue;
                    }

                    if (subScript.Length > 0)
                    {
                        subScript.Append(", ");
                    }
                    subScript.AppendFormat("[{0}] = S.[{0}]", columnInfo.Name);
                }

                script.AppendFormat(" WHEN MATCHED THEN UPDATE SET {0}", subScript.ToString());
            }

            if (tableInfo.Columns.Count > 0)
            {
                StringBuilder subScript1 = new StringBuilder();
                StringBuilder subScript2 = new StringBuilder();

                foreach (ColumnInfo columnInfo in tableInfo.Columns)
                {
                    if (columnInfo.IsComputed || columnInfo.IsIdentity)
                    {
                        continue;
                    }

                    if (subScript1.Length > 0)
                    {
                        subScript1.Append(", ");
                    }

                    if (subScript2.Length > 0)
                    {
                        subScript2.Append(", ");
                    }

                    subScript1.AppendFormat("[{0}]", columnInfo.Name);
                    subScript2.AppendFormat("S.[{0}]", columnInfo.Name);
                }
                
                script.AppendFormat(" WHEN NOT MATCHED THEN INSERT ({0}) VALUES ({1})", subScript1.ToString(), subScript2.ToString());
            }

            if (tableInfo.HasComputedColumns() || tableInfo.HasIdentityColumn())
            {
                StringBuilder subScript = new StringBuilder();
                
                foreach (ColumnInfo columnInfo in tableInfo.Columns)
                {
                    if (!columnInfo.IsComputed && !columnInfo.IsIdentity)
                    {
                        continue;
                    }
                    
                    if (subScript.Length > 0)
                    {
                        subScript.Append(", ");
                    }
                    subScript.AppendFormat("inserted.[{0}]", columnInfo.Name);
                }

                script.AppendFormat(" OUTPUT {0}", subScript.ToString());
            }
            script.Append(";");

            return script.ToString();
        }

        private static void GenerateReRead(StringBuilder script, TableInfo tableInfo, bool update, List<ColumnInfo> keys = null)
        {
            bool hasIdentity = tableInfo.HasIdentityColumn();
            bool hasComputedColumns = tableInfo.HasComputedColumns();
            bool readRequired = (!update && hasIdentity) || hasComputedColumns;

            if (readRequired)
            {
                // compute key
                ColumnInfo identityColumn = null;

                if (keys == null)
                {
                    identityColumn = tableInfo.GetFirstIdentityColumn();

                    if (identityColumn == null)
                    {
                        if (keys == null)
                        {
                            keys = tableInfo.GetFirstPrimaryKey();
                        }

                        if (keys == null)
                        {
                            keys = tableInfo.GetFirstUniqueKey();
                        }

                        if (keys == null || keys.Count == 0)
                        {
                            throw new InvalidProgramException("No keys found");
                        }
                    }
                }

                if (!hasComputedColumns)
                {
                    if (update)
                    {
                        throw new InvalidProgramException("Update cannot reread identity column value.");
                    }
                    else
                    {
                        script.AppendFormat(" SET {0} = SCOPE_IDENTITY();", identityColumn.ParameterName);
                    }
                }
                else
                {
                    StringBuilder subquery = new StringBuilder();
                    
                    foreach (var columnInfo in tableInfo.Columns)
                    {
                        if ((!update && columnInfo.IsIdentity) || columnInfo.IsComputed)
                        {
                            if (subquery.Length > 0)
                            {
                                subquery.Append(", ");
                            }

                            subquery.AppendFormat("{0} = [{1}]", columnInfo.ParameterName, columnInfo.Name);
                        }
                    }

                    if (keys != null)
                    {
                        StringBuilder keySelector = new StringBuilder();
                        GenerateWhereClause(keySelector, keys);

                        script.AppendFormat(" SELECT {0} FROM [{1}].[{2}] {3}", subquery.ToString(), tableInfo.Schema, tableInfo.Name, keySelector.ToString());
                    }
                    else if (identityColumn != null)
                    {
                        if (update)
                        {
                            script.AppendFormat(" SELECT {0} FROM [{1}].[{2}] WHERE [{3}] = {4}", subquery.ToString(), tableInfo.Schema, tableInfo.Name, identityColumn.Name, identityColumn.ParameterName);
                        }
                        else
                        {
                            script.AppendFormat(" SELECT {0} FROM [{1}].[{2}] WHERE [{3}] = SCOPE_IDENTITY()", subquery.ToString(), tableInfo.Schema, tableInfo.Name, identityColumn.Name);
                        }
                    }
                    else
                    {
                        throw new InvalidProgramException("Something wrong.");
                    }
                }
            }
        }
    }
}
