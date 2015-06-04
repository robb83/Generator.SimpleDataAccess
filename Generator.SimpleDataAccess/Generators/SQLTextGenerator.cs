﻿using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            if (filterColumns == null || filterColumns.Count == 0)
            {
                throw new NotSupportedException("Delete without filter.");
            }

            StringBuilder script = new StringBuilder();

            script.AppendFormat("DELETE FROM [{0}].[{1}]", table.Schema, table.Name);

            GenerateWhereClause(script, filterColumns);

            return script.ToString();
        }
        
        public static void GenerateReadBackIdentityAfterInsertToParameter(StringBuilder script, List<ColumnInfo> identityColumns)
        {
            if (identityColumns == null || identityColumns.Count == 0)
            {
                throw new ArgumentNullException("computedColumns");
            }

            if (identityColumns.Count > 1)
            {
                throw new NotSupportedException("computedColumns");
            }

            script.AppendFormat("SET {0} = SCOPE_IDENTITY();", identityColumns[0].ParameterName);
        }

        public static void GenerateReadBackIdentityAfterInsertToParameter(StringBuilder script, String target, List<ColumnInfo> identityColumns, List<ColumnInfo> computedColumns)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (String.IsNullOrWhiteSpace("target"))
            {
                throw new ArgumentNullException("target");
            }

            if (computedColumns == null || computedColumns.Count == 0)
            {
                throw new ArgumentNullException("computedColumns");
            }

            if (identityColumns == null || identityColumns.Count > 1)
            {
                throw new NotSupportedException("computedColumns");
            }

            script.Append("SELECT ");

            int b = 0;

            for (int a = 0; a < identityColumns.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("{0} = [{1}]", computedColumns[a].ParameterName, computedColumns[a].Name);
            }

            for (int a = 0; a < computedColumns.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("{0} = [{1}]", computedColumns[a].ParameterName, computedColumns[a].Name);
            }

            script.AppendFormat(" FROM {0} WHERE [{1}] = SCOPE_IDENTITY()", target, identityColumns[0].Name);
        }

        public static void GenerateReadBackComputedValuesToParameter(StringBuilder script, String target, List<ColumnInfo> key, List<ColumnInfo> computedColumns)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (String.IsNullOrWhiteSpace("target"))
            {
                throw new ArgumentNullException("target");
            }

            if (key == null || key.Count == 0)
            {
                throw new ArgumentNullException("key");
            }

            if (computedColumns == null || computedColumns.Count == 0)
            {
                throw new ArgumentNullException("computedColumns");
            }

            script.Append("SELECT ");

            for (int a = 0; a < computedColumns.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("{0} = [{1}]", computedColumns[a].ParameterName, computedColumns[a].Name);
            }

            script.AppendFormat(" FROM {0} WHERE ", target);

            for (int a = 0; a < key.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(" AND ");
                }

                script.AppendFormat("[{0}] = {1}", key[a].Name, key[a].ParameterName);
            }
        }

        public static void GenerateInsertStatement(StringBuilder script, String target, List<ColumnInfo> values)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (String.IsNullOrWhiteSpace("target"))
            {
                throw new ArgumentNullException("target");
            }

            if (values == null || values.Count == 0)
            {
                throw new ArgumentNullException("values");
            }

            script.AppendFormat("INSERT INTO {0} (", target);

            for (int a = 0; a < values.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}]", values[a].Name);
            }

            script.Append(") VALUES (");

            for (int a = 0; a < values.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("{0}", values[a].ParameterName);
            }

            script.Append(");");
        }

        public static void GenerateUpdateStatement(StringBuilder script, String target, List<ColumnInfo> key, List<ColumnInfo> values)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (String.IsNullOrWhiteSpace("target"))
            {
                throw new ArgumentNullException("target");
            }

            if (values == null || values.Count == 0)
            {
                throw new ArgumentNullException("values");
            }

            script.AppendFormat("UPDATE {0} SET ", target);

            for(int a = 0; a < values.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}] = {1}", values[a].Name, values[a].ParameterName);
            }

            if (key.Count > 0)
            {
                script.Append(" WHERE ");

                for (int a = 0; a < key.Count; ++a)
                {
                    if (a > 0)
                    {
                        script.Append(" AND ");
                    }

                    script.AppendFormat("[{0}] = {1}", key[a].Name, key[a].ParameterName);
                }
            }

            script.Append(";");
        }

        public static void GenerateMergeStatement(StringBuilder script, String target, List<ColumnInfo> key, List<ColumnInfo> editable, List<ColumnInfo> computedColumns)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }

            if (String.IsNullOrWhiteSpace("target"))
            {
                throw new ArgumentNullException("target");
            }

            if (key == null || key.Count == 0)
            {
                throw new ArgumentNullException("key");
            }

            if (editable == null || editable.Count == 0)
            {
                throw new ArgumentNullException("editable");
            }

            script.AppendFormat("MERGE {0} AS T USING (SELECT ", target);

            int b = 0;
            for (int a = 0; a < key.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.Append(key[a].ParameterName);
            }

            for (int a = 0; a < editable.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.Append(editable[a].ParameterName);
            }

            script.Append(") AS S (");

            b = 0;
            for (int a = 0; a < key.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}]", key[a].Name);
            }

            for (int a = 0; a < editable.Count; ++a, ++b)
            {
                if (b > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}]", editable[a].Name);
            }

            script.Append(") ON (");

            for (int a = 0; a < key.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(" AND ");
                }

                script.AppendFormat("S.[{0}] = T.[{0}]", key[a].Name);
            }
            
            script.Append(") WHEN MATCHED THEN UPDATE SET ");

            for (int a = 0; a < editable.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}] = S.[{0}]", editable[a].Name);
            }

            script.Append(" WHEN NOT MATCHED THEN INSERT (");

            for (int a = 0; a < editable.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("[{0}]", editable[a].Name);
            }

            script.Append(") VALUES (");

            for (int a = 0; a < editable.Count; ++a)
            {
                if (a > 0)
                {
                    script.Append(", ");
                }

                script.AppendFormat("S.[{0}]", editable[a].Name);
            }

            script.Append(")");

            if (computedColumns.Count > 0)
            {
                script.Append(" OUTPUT ");

                for (int a = 0; a < computedColumns.Count; ++a)
                {
                    if (a > 0)
                    {
                        script.Append(", ");
                    }

                    script.AppendFormat("inserted.[{0}]", computedColumns[a].Name);
                }
            }

            script.Append(";");
        }
    }
}
