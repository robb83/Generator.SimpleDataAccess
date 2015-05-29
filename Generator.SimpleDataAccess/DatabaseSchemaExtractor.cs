using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Generator.SimpleDataAccess
{
    public static class DatabaseSchemaExtractor
    {
        public static DatabaseSchemaInfo GetDatabaseSchemaInfo(SqlConnection connection)
        {
            DatabaseSchemaInfo schemaInfo = new DatabaseSchemaInfo();

            schemaInfo.Tables = GetTables(connection);
            schemaInfo.StoredProcedures = GetStoredProcedures(connection);

            return schemaInfo;
        }

        public static SqlDbType SqlDbTypeFrom(String value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value");
            }

            value = value.Trim().ToLower();

            switch (value)
            {
                case "image":
                    return SqlDbType.Binary;
                case "text":
                    return SqlDbType.Text;
                case "uniqueidentifier":
                    return SqlDbType.UniqueIdentifier;
                case "date":
                    return SqlDbType.Date;
                case "time":
                    return SqlDbType.Time;
                case "datetime2":
                    return SqlDbType.DateTime2;
                case "datetimeoffset":
                    return SqlDbType.DateTimeOffset;
                case "tinyint":
                    return SqlDbType.TinyInt;
                case "smallint":
                    return SqlDbType.SmallInt;
                case "int":
                    return SqlDbType.Int;
                case "smalldatetime":
                    return SqlDbType.SmallDateTime;
                case "real":
                    return SqlDbType.Real;
                case "money":
                    return SqlDbType.Money;
                case "datetime":
                    return SqlDbType.DateTime;
                case "float":
                    return SqlDbType.Float;
                case "sql_variant":
                    return SqlDbType.Variant;
                case "ntext":
                    return SqlDbType.NText;
                case "bit":
                    return SqlDbType.Bit;
                case "decimal":
                    return SqlDbType.Decimal;
                case "numeric":
                    return SqlDbType.Decimal;
                case "smallmoney":
                    return SqlDbType.SmallMoney;
                case "bigint":
                    return SqlDbType.BigInt;
                case "varbinary":
                    return SqlDbType.VarBinary;
                case "varchar":
                    return SqlDbType.VarChar;
                case "binary":
                    return SqlDbType.VarBinary;                    
                case "char":
                    return SqlDbType.Char;
                case "timestamp":
                    return SqlDbType.Timestamp;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "nchar":
                    return SqlDbType.NChar;
                case "xml":
                    return SqlDbType.Xml;
                case "sysname":
                case "geometry":
                case "geography":
                case "hierarchyid":
                default:
                    throw new InvalidDataException(String.Format("Invalid SqlDbType: {0}", value));
            }
        }

        public static Type TypeFrom(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.BigInt:
                    return typeof(System.Int64);
                case SqlDbType.Binary:
                    return typeof(System.Byte[]);
                case SqlDbType.Bit:
                    return typeof(System.Boolean);
                case SqlDbType.Char:
                    return typeof(System.String);
                case SqlDbType.DateTime:
                    return typeof(System.DateTime);
                case SqlDbType.Decimal:
                    return typeof(System.Decimal);
                case SqlDbType.Float:
                    return typeof(System.Double);
                case SqlDbType.Image:
                    return typeof(System.Byte[]);
                case SqlDbType.Int:
                    return typeof(System.Int32);
                case SqlDbType.Money:
                    return typeof(System.Decimal);
                case SqlDbType.NChar:
                    return typeof(System.String);
                case SqlDbType.NText:
                    return typeof(System.String);
                case SqlDbType.NVarChar:
                    return typeof(System.String);
                case SqlDbType.Real:
                    return typeof(System.Single);
                case SqlDbType.UniqueIdentifier:
                    return typeof(System.Guid);
                case SqlDbType.SmallDateTime:
                    return typeof(System.DateTime);
                case SqlDbType.SmallInt:
                    return typeof(System.Int16);
                case SqlDbType.SmallMoney:
                    return typeof(System.Decimal);
                case SqlDbType.Text:
                    return typeof(System.String);
                case SqlDbType.Timestamp:
                    return typeof(System.Byte[]);
                case SqlDbType.TinyInt:
                    return typeof(System.Byte);
                case SqlDbType.VarBinary:
                    return typeof(System.Byte[]);
                case SqlDbType.VarChar:
                    return typeof(System.String);
                case SqlDbType.Xml:
                    return typeof(System.String);
                case SqlDbType.Date:
                    return typeof(System.DateTime);
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(System.DateTime);
                case SqlDbType.DateTimeOffset:
                    return typeof(System.DateTimeOffset);
                case SqlDbType.Variant:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                default:
                    throw new NotSupportedException(String.Format("Not supported SqlDbType: ", type));
            }
        }

        public static String MappingMethod(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.BigInt:
                    return "GetInt64";
                case SqlDbType.Binary:
                    return "GetBytes";
                case SqlDbType.Bit:
                    return "GetBoolean";
                case SqlDbType.Char:
                    return "GetString";
                case SqlDbType.DateTime:
                    return "GetDateTime";
                case SqlDbType.Decimal:
                    return "GetDecimal";
                case SqlDbType.Float:
                    return "GetDouble";
                case SqlDbType.Image:
                    return "GetBytes";
                case SqlDbType.Int:
                    return "GetInt32";
                case SqlDbType.Money:
                    return "GetDecimal";
                case SqlDbType.NChar:
                    return "GetString";
                case SqlDbType.NText:
                    return "GetString";
                case SqlDbType.NVarChar:
                    return "GetString";
                case SqlDbType.Real:
                    return "GetFloat";
                case SqlDbType.UniqueIdentifier:
                    return "GetGuid";
                case SqlDbType.SmallDateTime:
                    return "GetDateTime";
                case SqlDbType.SmallInt:
                    return "GetInt16";
                case SqlDbType.SmallMoney:
                    return "GetDecimal";
                case SqlDbType.Text:
                    return "GetString";
                case SqlDbType.Timestamp:
                    return "GetBytes";
                case SqlDbType.TinyInt:
                    return "GetByte";
                case SqlDbType.VarBinary:
                    return "GetBytes";
                case SqlDbType.VarChar:
                    return "GetString";
                case SqlDbType.Xml:
                    return "GetString";
                case SqlDbType.Date:
                    return "GetDateTime";
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return "GetDateTime";
                case SqlDbType.DateTimeOffset:
                    return "GetDateTimeOffset";
                case SqlDbType.Variant:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                default:
                    throw new NotSupportedException(String.Format("Not supported SqlDbType: ", type));
            }
        }

        public static String TypeFullName(Type type, bool is_nullable)
        {
            if (type.IsValueType && is_nullable)
            {
                return "System.Nullable<" + type.FullName + ">";
            }

            return type.FullName;
        }

        public static String ToPropertyName(String column_name)
        {
            //TODO:

            return column_name;
        }

        public static String ToLocalVariableName(String column_name)
        {
            String propertyName = ToPropertyName(column_name);
            return Char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }

        public static String ToParameterName(String column_name)
        {
            return "@" + ToPropertyName(column_name);
        }

        public static String ToLocalParameterName(String column_name)
        {
            return "p" + ToPropertyName(column_name);
        }

        public static String ToClassName(String table_name)
        {
            //TODO:

            return table_name;
        }

        public static List<TableInfo> GetTables(SqlConnection connection)
        {
            const String QUERY_TABLES = @"SELECT t.[object_id], t.[name], s.[name] AS [schema_name] FROM [sys].[tables] t INNER JOIN [sys].[schemas] s ON s.[schema_id] = t.[schema_id] ORDER BY t.[name]";
            const String QUERY_TABLE_COLUMNS = @"SELECT c.[name], CASE WHEN t.[is_user_defined] = 1 THEN (SELECT TOP 1 st.[name] FROM [sys].[types] st WHERE st.[user_type_id] = t.[system_type_id]) ELSE t.[name] END AS [type_name], c.[is_nullable], c.[is_identity], c.[is_computed], CASE WHEN EXISTS(SELECT 1 FROM [sys].[index_columns] ic INNER JOIN [sys].[indexes] i ON ic.index_id = i.index_id WHERE ic.[object_id] = c.[object_id] and ic.[column_id] = c.[column_id] and i.[is_primary_key] = 1) THEN 1 ELSE 0 END AS [is_primary_key], CASE WHEN EXISTS(SELECT 1 FROM [sys].[index_columns] ic INNER JOIN [sys].indexes i on ic.[index_id] = i.[index_id] WHERE ic.[object_id] = c.[object_id] and ic.[column_id] = c.[column_id] and i.[is_unique] = 1) THEN 1 ELSE 0 END AS [is_unique] from [sys].[columns] c INNER JOIN [sys].[types] t on t.[user_type_id] = c.[user_type_id] WHERE c.[object_id] = @ObjectID ORDER BY c.column_id";
            const String QUERY_TABLE_REFERENCES = @"SELECT [fk].[name], c.[name] FROM [sys].[foreign_keys] fk  INNER JOIN [sys].[foreign_key_columns] [fkc] on [fkc].[constraint_object_id] = [fk].[object_id] INNER JOIN [sys].[columns] c on c.[object_id] = [fkc].[parent_object_id] and c.[column_id] = [fkc].[parent_column_id] WHERE [fk].[parent_object_id] = @ObjectID ORDER BY [fk].[name], [fkc].[constraint_column_id]";
            const String QUERY_TABLE_INDEXES = @"SELECT i.name, c.name, i.is_unique, i.is_primary_key FROM sys.indexes i INNER JOIN sys.index_columns ic on i.object_id = ic.object_id and i.index_id = ic.index_id INNER JOIN sys.columns c on c.object_id = ic.object_id and c.column_id = ic.column_id where i.object_id = @ObjectID order by i.name, key_ordinal";
            const String QUERY_PARAMETER_OBJECTID = "@ObjectID";
            
            List<TableInfo> tables = new List<TableInfo>();

            using (SqlCommand tableCommand = new SqlCommand(QUERY_TABLES, connection))
            {
                using (SqlDataReader reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TableInfo tableInfo = new TableInfo();
                        tableInfo.ObjectID = reader.GetInt32(0);
                        tableInfo.Name = reader.GetString(1);
                        tableInfo.Schema = reader.GetString(2);
                        tableInfo.ClassName = ToClassName(tableInfo.Name);

                        tables.Add(tableInfo);
                    }
                }
            }

            foreach (var t in tables)
            {
                using (SqlCommand columnCommand = new SqlCommand(QUERY_TABLE_COLUMNS, connection))
                {
                    columnCommand.Parameters.AddWithValue(QUERY_PARAMETER_OBJECTID, t.ObjectID);

                    using (SqlDataReader reader = columnCommand.ExecuteReader())
                    {
                        int ordinal = 0;
                        while (reader.Read())
                        {
                            ColumnInfo columnInfo = new ColumnInfo();
                            columnInfo.Name = reader.GetString(0);
                            columnInfo.DbType = SqlDbTypeFrom(reader.GetString(1));
                            columnInfo.IsNullable = reader.GetBoolean(2);
                            columnInfo.IsIdentity = reader.GetBoolean(3);
                            columnInfo.IsComputed = reader.GetBoolean(4);
                            columnInfo.IsPartOfPrimaryKey = reader.GetInt32(5) > 0;
                            columnInfo.IsPartOfUniqueIndex = reader.GetInt32(6) > 0;
                            columnInfo.Type = TypeFrom(columnInfo.DbType);
                            columnInfo.MappingMethodName = MappingMethod(columnInfo.DbType);
                            columnInfo.Ordinal = ordinal;
                            columnInfo.FullTypeName = TypeFullName(columnInfo.Type, columnInfo.IsNullable);
                            columnInfo.PropertyName = ToPropertyName(columnInfo.Name);
                            columnInfo.LocalVariableName = ToLocalVariableName(columnInfo.Name);
                            columnInfo.ParameterName = ToParameterName(columnInfo.Name);
                            columnInfo.LocalParameterVariableName = ToLocalParameterName(columnInfo.Name);
                            
                            t.Columns.Add(columnInfo);

                            ordinal++;
                        }
                    }
                }

                using (SqlCommand foreignCommand = new SqlCommand(QUERY_TABLE_REFERENCES, connection))
                {
                    foreignCommand.Parameters.AddWithValue(QUERY_PARAMETER_OBJECTID, t.ObjectID);

                    using (SqlDataReader reader = foreignCommand.ExecuteReader())
                    {
                        ForeignKeyInfo lastForeignKeyInfo = null;

                        while (reader.Read())
                        {
                            String foreignKeyName = reader.GetString(0);
                            String columnName = reader.GetString(1);

                            if (lastForeignKeyInfo == null || !foreignKeyName.Equals(lastForeignKeyInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lastForeignKeyInfo = new ForeignKeyInfo();
                                lastForeignKeyInfo.Name = foreignKeyName;

                                t.ForeignKeys.Add(lastForeignKeyInfo);
                            }

                            lastForeignKeyInfo.Columns.Add(t.GetColumnByName(columnName));
                        }
                    }
                }

                using (SqlCommand indexCommand = new SqlCommand(QUERY_TABLE_INDEXES, connection))
                {
                    indexCommand.Parameters.AddWithValue(QUERY_PARAMETER_OBJECTID, t.ObjectID);

                    using (SqlDataReader reader = indexCommand.ExecuteReader())
                    {
                        IndexInfo lastIndexInfo = null;
                        while (reader.Read())
                        {
                            String indexName = reader.GetString(0);
                            String columnName = reader.GetString(1);

                            bool unique = reader.GetBoolean(2);
                            bool primarykey = reader.GetBoolean(3);

                            if (lastIndexInfo == null || !indexName.Equals(lastIndexInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lastIndexInfo = new IndexInfo();
                                lastIndexInfo.Name = indexName;
                                lastIndexInfo.IsPrimaryKey = primarykey;
                                lastIndexInfo.IsUnique = unique;

                                t.Indexes.Add(lastIndexInfo);
                            }

                            lastIndexInfo.Columns.Add(t.GetColumnByName(columnName));
                        }
                    }
                }
            }

            return tables;
        }

        public static List<StoredProcedureInfo> GetStoredProcedures(SqlConnection connection)
        {
            const String QUERY_PROCEDURES = @"SELECT p.[object_id], p.[name], s.[name] AS [schema_name], m.[definition] FROM [sys].[procedures] p INNER JOIN [sys].[schemas] s ON s.[schema_id] = p.[schema_id] LEFT JOIN [sys].[sql_modules] m ON m.[object_id] = p.[object_id] ORDER BY p.[name]";
            const String QUERY_PROCEDURE_PARAMETERS = @"SELECT p.[name], p.[is_output], t.[name] FROM [sys].[parameters] p INNER JOIN [sys].[types] t on t.[user_type_id] = p.[user_type_id] WHERE p.[object_id] = @ObjectID ORDER BY p.[parameter_id]";
            const String QUERY_PARAMETER_OBJECTID = "@ObjectID";

            List<StoredProcedureInfo> procedures = new List<StoredProcedureInfo>();

            using (SqlCommand tableCommand = new SqlCommand(QUERY_PROCEDURES, connection))
            {
                using (SqlDataReader reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StoredProcedureInfo storedProcedureInfo = new StoredProcedureInfo();
                        storedProcedureInfo.ObjectID = reader.GetInt32(0);
                        storedProcedureInfo.Name = reader.GetString(1);
                        storedProcedureInfo.Schema = reader.GetString(2);

                        String definition = reader.GetString(3);
                        if (!String.IsNullOrWhiteSpace(definition))
                        {
                            ApplyDefaultParameters(storedProcedureInfo, definition);
                        }

                        procedures.Add(storedProcedureInfo);
                    }
                }
            }

            foreach (var p in procedures)
            {
                using (SqlCommand parameterCommand = new SqlCommand(QUERY_PROCEDURE_PARAMETERS, connection))
                {
                    parameterCommand.Parameters.AddWithValue(QUERY_PARAMETER_OBJECTID, p.ObjectID);

                    using (SqlDataReader reader = parameterCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StoredProcedureParameterInfo parameterInfo = p.GetParameterByName(reader.GetString(0));
                            if (parameterInfo == null)
                            {
                                parameterInfo = new StoredProcedureParameterInfo();
                                p.Parameters.Add(parameterInfo);
                            }

                            parameterInfo.Name = reader.GetString(0).Replace("@", "");
                            parameterInfo.IsOutput = reader.GetBoolean(1);
                            parameterInfo.DbType = SqlDbTypeFrom(reader.GetString(2));
                            parameterInfo.Type = TypeFrom(parameterInfo.DbType);
                            parameterInfo.MappingMethodName = MappingMethod(parameterInfo.DbType);
                            parameterInfo.FullTypeName = TypeFullName(parameterInfo.Type, parameterInfo.HasDefault);
                            parameterInfo.LocalVariableName = ToLocalVariableName(parameterInfo.Name);
                            parameterInfo.ParameterName = ToParameterName(parameterInfo.Name);
                            parameterInfo.LocalParameterVariableName = ToLocalParameterName(parameterInfo.Name);
                        }
                    }
                }
            }

            return procedures;
        }

        private static void ApplyDefaultParameters(StoredProcedureInfo procedure, String definition)
        {
            // HACK: detecting default parameters (mssql not provide it)

            String commentPattern = "(--[^\r\n]*[\r\n])";
            String definitionPattern = "(PROCEDURE|PROC)\\s+(.*)\\sAS\\s";

            // Remove comment
            definition = Regex.Replace(definition, commentPattern, "", RegexOptions.CultureInvariant);
            definition = definition.Trim();

            // Capture parameters
            Match m = Regex.Match(definition, definitionPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);

            if (m.Success)
            {
                String group2 = m.Groups[2].Value;
                int firstAtCharacter = group2.IndexOf('@');

                if (firstAtCharacter >= 0)
                {
                    if (firstAtCharacter > 0)
                    {
                        group2 = group2.Substring(firstAtCharacter);
                    }

                    group2 = Regex.Replace(group2, "\\s+", " ").Trim();

                    String[] parameterDefinitions = group2.Split(',');

                    foreach (String parameterDefinition in parameterDefinitions)
                    {
                        String parameterDefinitionCleaned = parameterDefinition.Trim();

                        String parameterName = parameterDefinitionCleaned.Substring(0, parameterDefinitionCleaned.IndexOf(' ')).Trim();
                        bool hasDefaultValue = parameterDefinitionCleaned.Contains("=");

                        StoredProcedureParameterInfo parameterInfo = procedure.GetParameterByName(parameterName);
                        if (parameterInfo == null)
                        {
                            parameterInfo = new StoredProcedureParameterInfo();
                            parameterInfo.Name = parameterName;
                        }

                        parameterInfo.HasDefault = hasDefaultValue;
                    }
                }
            }
        }
    }
}
