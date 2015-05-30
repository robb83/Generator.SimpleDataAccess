using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator.SimpleDataAccess.Generators
{
    public class CSharpTextGenerator
    {
        public static String ToAutomaticProperty(String propertyName, String typeFullName)
        {
            return String.Format("public {0} {1} {{ get; set; }}", typeFullName, propertyName);
        }
        
        public static void GenerateModel(CodeStringBuilder code, TableInfo tableInfo)
        {
            code.AppendFormat("public partial class {0}", tableInfo.ClassName);
            code.CodeBlockBegin();

            foreach (var c in tableInfo.Columns)
            {
                code.AppendLine(ToAutomaticProperty(c.PropertyName, c.FullTypeName));
            }

            code.CodeBlockEnd();
        }

        private static void GenerateColumnMapperFromSqlDataReader(CodeStringBuilder code, ColumnInfo columnInfo)
        {
            if (columnInfo.IsNullable && columnInfo.Type.IsValueType)
            {
                code.AppendLineFormat("if (reader.IsDBNull({0}))", columnInfo.Ordinal);
                code.CodeBlockBegin();
                code.AppendLineFormat("entity.{0} = null;", columnInfo.PropertyName, columnInfo.MappingMethodName, columnInfo.Ordinal);
                code.CodeBlockEnd();
                code.AppendLine("else");
                code.CodeBlockBegin();
                code.AppendLineFormat("entity.{0} = reader.{1}({2});", columnInfo.PropertyName, columnInfo.MappingMethodName, columnInfo.Ordinal);
                code.CodeBlockEnd();
            }
            else
            {
                code.AppendLineFormat("entity.{0} = reader.{1}({2});", columnInfo.PropertyName, columnInfo.MappingMethodName, columnInfo.Ordinal);
            }
        }

        private static void GenerateColumnMapperFromSqlParameter(CodeStringBuilder code, ColumnInfo columnInfo)
        {
            if (columnInfo.IsNullable)
            {
                code.AppendLineFormat("if ({0}.Value == System.DBNull.Value)", columnInfo.LocalParameterVariableName);
                code.CodeBlockBegin();
                code.AppendLineFormat("entity.{0} = null;", columnInfo.PropertyName);
                code.CodeBlockEnd();
                code.AppendLine("else");
                code.CodeBlockBegin();
                code.AppendLineFormat("entity.{0} = ({1}){2}.Value;", columnInfo.PropertyName, columnInfo.FullTypeName ,columnInfo.LocalParameterVariableName);
                code.CodeBlockEnd();
            }
            else
            {
                code.AppendLineFormat("entity.{0} = ({1}){2}.Value;", columnInfo.PropertyName, columnInfo.FullTypeName, columnInfo.LocalParameterVariableName);
            }
        }

        public static void GenerateMapping(CodeStringBuilder code, TableInfo tableInfo)
        {
            code.AppendFormat("public static {0} Read{0}(System.Data.SqlClient.SqlDataReader reader)", tableInfo.ClassName);
            code.CodeBlockBegin();

            code.AppendLineFormat("{0} entity = new {0}();", tableInfo.ClassName);
            
            foreach (var columnInfo in tableInfo.Columns)
            {
                GenerateColumnMapperFromSqlDataReader(code, columnInfo);
            }

            code.Append("return entity;");

            code.CodeBlockEnd();
        }

        public static void GenerateStoredProcedure(CodeStringBuilder code, StoredProcedureInfo storedProcedureInfo)
        {
            String parameters = "";

            foreach (StoredProcedureParameterInfo parameterInfo in storedProcedureInfo.Parameters)
            {
                if (parameters.Length > 0)
                {
                    parameters += ", ";
                }

                if (parameterInfo.IsOutput)
                {
                    parameters += "out ";
                }

                parameters += parameterInfo.FullTypeName;
                parameters += " ";
                parameters += parameterInfo.LocalVariableName;
            }

            code.AppendFormat("public void Execute{0}({1})", storedProcedureInfo.Name, parameters);
            code.CodeBlockBegin();

            code.AppendLine("BeginTransaction();");
            code.AppendLine();


            code.Append("try");
            code.CodeBlockBegin();

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"[{0}].[{1}]\"))", storedProcedureInfo.Schema, storedProcedureInfo.Name);
            code.CodeBlockBegin();
            code.AppendLine("command.CommandType = System.Data.CommandType.StoredProcedure;");
            code.AppendLine();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            foreach (StoredProcedureParameterInfo parameterInfo in storedProcedureInfo.Parameters)
            {
                GenerateSqlParameter(code,
                    parameterInfo.LocalVariableName,
                    parameterInfo.LocalParameterVariableName,
                    parameterInfo.ParameterName,
                    parameterInfo.DbType,
                    parameterInfo.HasDefault, 
                    parameterInfo.IsOutput,
                    -1);
                code.AppendLine();
            }

            code.AppendLine("command.ExecuteNonQuery();");
            code.AppendLine();

            foreach (StoredProcedureParameterInfo parameterInfo in storedProcedureInfo.Parameters)
            {
                if (parameterInfo.IsOutput == false)
                {
                    continue;
                }

                code.AppendFormat("{0} = ({1}){2}.Value;", parameterInfo.LocalVariableName, parameterInfo.FullTypeName, parameterInfo.LocalParameterVariableName);
                code.AppendLine();
            }

            // end of try
            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            // end of using
            code.CodeBlockEnd();

            // end of try
            code.AppendLine();
            code.Append("CommitTransaction();");
            code.CodeBlockEnd();
            code.Append("catch");
            code.CodeBlockBegin();
            code.AppendLine("RollbackTransaction();");
            code.AppendLine("throw;");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
        }

        public static void GenerateCountSelect(CodeStringBuilder code, TableInfo tableInfo)
        {
            String methodName = "Select" + tableInfo.ClassName + "Count";

            code.AppendFormat("public int {1}()", tableInfo.ClassName, methodName);
            code.CodeBlockBegin();

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", SQLTextGenerator.GenerateSelectCount(tableInfo));
            code.CodeBlockBegin();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            code.Append("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");
            code.CodeBlockBegin();

            code.Append("if (reader.Read())");
            code.CodeBlockBegin();
            code.AppendFormat("return reader.GetInt32(0);", tableInfo.ClassName);
            code.CodeBlockEnd();
            code.Append("else");
            code.CodeBlockBegin();
            code.Append("throw new InvalidOperationException(\"Select count failed.\");");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }

        public static void GenerateSelectMethod(CodeStringBuilder code, TableInfo tableInfo, List<ColumnInfo> filteredColumns, bool singleResult)
        {
            String methodName = "Select" + tableInfo.ClassName;
            String parameters = "";

            if (filteredColumns != null && filteredColumns.Count > 0)
            {
                methodName += "By";

                foreach (ColumnInfo columnInfo in filteredColumns)
                {
                    methodName += columnInfo.PropertyName;

                    if (parameters.Length > 0)
                    {
                        parameters += ", ";
                    }

                    parameters += columnInfo.FullTypeName;
                    parameters += " ";
                    parameters += columnInfo.LocalVariableName;
                }
            }

            if (singleResult)
            {
                code.AppendFormat("public {0} {1}({2})", tableInfo.ClassName, methodName, parameters);
                code.CodeBlockBegin();
            }
            else
            {
                code.AppendFormat("public List<{0}> {1}({2})", tableInfo.ClassName, methodName, parameters);
                code.CodeBlockBegin();
            }

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", SQLTextGenerator.GenerateSelect(tableInfo, filteredColumns));
            code.CodeBlockBegin();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");

            if (filteredColumns != null && filteredColumns.Count > 0)
            {
                foreach (ColumnInfo columnInfo in filteredColumns)
                {
                    GenerateSqlParameter(code, 
                        columnInfo.LocalVariableName, 
                        columnInfo.LocalParameterVariableName, 
                        columnInfo.ParameterName, 
                        columnInfo.DbType, 
                        columnInfo.IsNullable, 
                        false,
                        columnInfo.MaxLength);
                    code.AppendLine();
                }
            }

            code.Append("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");
            code.CodeBlockBegin();

            if (singleResult)
            {
                code.Append("if (reader.Read())");
                code.CodeBlockBegin();
                code.AppendFormat("return Read{0}(reader);", tableInfo.ClassName);
                code.CodeBlockEnd();
                code.Append("else");
                code.CodeBlockBegin();
                code.Append("return null;");
                code.CodeBlockEnd();
            }
            else
            {
                code.AppendLineFormat("List<{0}> result = new List<{0}>();", tableInfo.ClassName);
                code.Append("while (reader.Read())");
                code.CodeBlockBegin();
                code.AppendFormat("result.Add(Read{0}(reader));", tableInfo.ClassName);
                code.CodeBlockEnd();
                code.Append("return result;");
            }

            code.CodeBlockEnd();
            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }
        
        private static bool IsVariableLength(System.Data.SqlDbType dbType)
        {
            switch(dbType)
            {
                case System.Data.SqlDbType.NText:
                case System.Data.SqlDbType.NVarChar:
                case System.Data.SqlDbType.VarChar:
                case System.Data.SqlDbType.Text:
                    return true;
                default:
                    return false;
            }
        }

        private static void GenerateSqlParameter(CodeStringBuilder code, String valueExpression, String localParameterName, String sqlParameterName, System.Data.SqlDbType sqlDbType, bool is_nullable, bool is_output, long max_length)
        {
            if (IsVariableLength(sqlDbType))
            {
                code.AppendLineFormat("System.Data.SqlClient.SqlParameter {0} = command.Parameters.Add(\"{1}\", System.Data.SqlDbType.{2}, {3});", localParameterName, sqlParameterName, sqlDbType.ToString(), max_length);
            }
            else
            {
                code.AppendLineFormat("System.Data.SqlClient.SqlParameter {0} = command.Parameters.Add(\"{1}\", System.Data.SqlDbType.{2});", localParameterName, sqlParameterName, sqlDbType.ToString(), max_length);
            }

            if (is_output)
            {
                code.AppendLineFormat("{0}.Direction = System.Data.ParameterDirection.Output;", localParameterName);
            }
            else
            {
                if (is_nullable)
                {
                    code.AppendLineFormat("if ({0} == null)", valueExpression);
                    code.CodeBlockBegin();
                    code.AppendLineFormat("{0}.Value = System.DBNull.Value;", localParameterName);
                    code.CodeBlockEnd();
                    code.Append("else");
                    code.CodeBlockBegin();
                    code.AppendLineFormat("{0}.Value = {1};", localParameterName, valueExpression);
                    code.CodeBlockEnd();
                }
                else
                {
                    code.AppendLineFormat("{0}.Value = {1};", localParameterName, valueExpression);
                }
            }
        }

        public static void GenerateUpdateMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            List<ColumnInfo> key = tableInfo.GetFirstPrimaryKey();

            code.AppendFormat("public void Update{0}({0} entity)", tableInfo.ClassName);
            code.CodeBlockBegin();

            code.Append("if (entity == null)");
            code.CodeBlockBegin();
            code.Append("throw new ArgumentNullException(\"entity\");");
            code.CodeBlockEnd();
            code.AppendLine();

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", SQLTextGenerator.GenerateUpdate(tableInfo, key));
            code.CodeBlockBegin();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");

            foreach (ColumnInfo columnInfo in tableInfo.Columns)
            {
                GenerateSqlParameter(
                    code,
                    String.Format("entity.{0}", columnInfo.PropertyName),
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    columnInfo.IsComputed,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            code.Append("if (command.ExecuteNonQuery() > 0)");
            code.CodeBlockBegin();

            foreach (var columnInfo in tableInfo.Columns)
            {
                if (columnInfo.IsComputed)
                {
                    GenerateColumnMapperFromSqlParameter(code, columnInfo);
                }
            }

            code.CodeBlockEnd();
            code.Append("else");
            code.CodeBlockBegin();
            code.Append("throw new InvalidOperationException(\"Update failed.\");");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }

        public static void GenerateInsertMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            code.AppendFormat("public void Insert{0}({0} entity)", tableInfo.ClassName);
            code.CodeBlockBegin();

            code.Append("if (entity == null)");
            code.CodeBlockBegin();
            code.Append("throw new ArgumentNullException(\"entity\");");
            code.CodeBlockEnd();
            code.AppendLine();

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", SQLTextGenerator.GenerateInsert(tableInfo));
            code.CodeBlockBegin();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");

            foreach (ColumnInfo columnInfo in tableInfo.Columns)
            {
                GenerateSqlParameter(
                    code, 
                    String.Format("entity.{0}", columnInfo.PropertyName), 
                    columnInfo.LocalParameterVariableName, 
                    columnInfo.ParameterName, 
                    columnInfo.DbType, 
                    columnInfo.IsNullable, 
                    columnInfo.IsIdentity || columnInfo.IsComputed,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            code.Append("if (command.ExecuteNonQuery() > 0)");
            code.CodeBlockBegin();

            foreach (var columnInfo in tableInfo.Columns)
            {
                if (columnInfo.IsIdentity || columnInfo.IsComputed)
                {
                    GenerateColumnMapperFromSqlParameter(code, columnInfo);
                }
            }

            code.CodeBlockEnd();
            code.Append("else");
            code.CodeBlockBegin();
            code.Append("throw new InvalidOperationException(\"Insert failed.\");");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }

        public static void GenerateDeleteMethod(CodeStringBuilder code, TableInfo tableInfo, List<ColumnInfo> filteredColumns)
        {
            String methodName = "Delete" + tableInfo.ClassName;
            String parameters = "";

            if (filteredColumns != null && filteredColumns.Count > 0)
            {
                methodName += "By";

                foreach (ColumnInfo columnInfo in filteredColumns)
                {
                    methodName += columnInfo.PropertyName;

                    if (parameters.Length > 0)
                    {
                        parameters += ", ";
                    }

                    parameters += columnInfo.FullTypeName;
                    parameters += " ";
                    parameters += columnInfo.LocalVariableName;
                }
            }

            code.AppendFormat("public void {0}({1})", methodName, parameters);
            code.CodeBlockBegin();

            code.AppendFormat("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", SQLTextGenerator.GenerateDelete(tableInfo, filteredColumns));
            code.CodeBlockBegin();

            code.Append("try");
            code.CodeBlockBegin();
            code.AppendLine("PopConnection(command);");

            if (filteredColumns != null && filteredColumns.Count > 0)
            {
                foreach (ColumnInfo columnInfo in filteredColumns)
                {
                    GenerateSqlParameter(
                        code,
                        columnInfo.LocalVariableName,
                        columnInfo.LocalParameterVariableName, 
                        columnInfo.ParameterName, 
                        columnInfo.DbType, 
                        columnInfo.IsNullable, 
                        false,
                        columnInfo.MaxLength);
                    code.AppendLine();
                }
            }

            code.Append("command.ExecuteNonQuery();");

            code.CodeBlockEnd();
            code.Append("finally");
            code.CodeBlockBegin();
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();
            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }

        public static void GenerateDatabaseAccessCode(DatabaseSchemaInfo schemaInfo, String outputFilename)
        {
            CodeStringBuilder code = new CodeStringBuilder();

            // usings
            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine();

            // namespace
            code.AppendFormat("namespace {0}", schemaInfo.Namespace);
            code.CodeBlockBegin();

            // model
            code.AppendLine("#region Models");
            code.AppendLine();
            foreach (var t in schemaInfo.Tables)
            {
                CSharpTextGenerator.GenerateModel(code, t);
                code.AppendLine();
            }
            code.AppendLine("#endregion");
            code.AppendLine();

            // class - DataAccess wrapper
            code.AppendLineFormat("public partial class {0} : IDisposable", schemaInfo.ClassName);
            code.CodeBlockBegin();

            code.AppendLine();
            code.AppendLine("private System.Data.SqlClient.SqlConnection connection;");
            code.AppendLine("private System.Data.SqlClient.SqlTransaction transaction;");
            code.AppendLine("private int transactionCounter;");
            code.AppendLine("private String connectionString;");
            code.AppendLine("private bool externalResource;");
            code.AppendLine();

            // constructor 1
            code.AppendFormat("public {0}(String connectionString)", schemaInfo.ClassName);
            code.CodeBlockBegin();
            code.AppendLine("this.externalResource = false;");
            code.AppendLine("this.connectionString = connectionString;");
            code.CodeBlockEnd();
            code.AppendLine();

            // constructor 2
            code.AppendFormat("public {0}(System.Data.SqlClient.SqlConnection connection, System.Data.SqlClient.SqlTransaction transaction)", schemaInfo.ClassName);
            code.CodeBlockBegin();
            code.AppendLine("this.externalResource = true;");
            code.AppendLine("this.connection = connection;");
            code.AppendLine("this.transaction = transaction;");
            code.CodeBlockEnd();
            code.AppendLine();

            foreach (var t in schemaInfo.Tables)
            {
                code.AppendLineFormat("#region Insert, Update, Delete, Select, Mapping - {0}", t.ToString());
                code.AppendLine();

                CSharpTextGenerator.GenerateMapping(code, t);
                code.AppendLine();

                CSharpTextGenerator.GenerateInsertMethod(code, t);
                code.AppendLine();

                CSharpTextGenerator.GenerateUpdateMethod(code, t);
                code.AppendLine();

                CSharpTextGenerator.GenerateSelectMethod(code, t, null, false);
                code.AppendLine();

                CSharpTextGenerator.GenerateCountSelect(code, t);
                code.AppendLine();

                Dictionary<String, Filter> filterColumnLists = new Dictionary<string, Filter>();
                
                foreach (var i in t.Indexes)
                {
                    String key = String.Join(":", i.Columns.OrderBy(ci => ci.Name).Select(ci => ci.Name));
                    if (!filterColumnLists.ContainsKey(key))
                    {
                        filterColumnLists.Add(key, new Filter
                        {
                            IsUnique = i.IsUnique,
                            Columns = i.Columns
                        });
                    }
                }

                foreach (var fk in t.ForeignKeys)
                {

                    String key = String.Join(":", fk.Columns.OrderBy(ci => ci.Name).Select(ci => ci.Name));
                    if (!filterColumnLists.ContainsKey(key))
                    {
                        filterColumnLists.Add(key, new Filter
                        {
                            IsUnique = false,
                            Columns = fk.Columns
                        });
                    }
                }

                foreach (KeyValuePair<String, Filter> keyValue in filterColumnLists)
                {
                    CSharpTextGenerator.GenerateSelectMethod(code, t, keyValue.Value.Columns, keyValue.Value.IsUnique);
                    code.AppendLine();

                    CSharpTextGenerator.GenerateDeleteMethod(code, t, keyValue.Value.Columns);
                    code.AppendLine();
                }

                code.AppendLine("#endregion");
                code.AppendLine();
            }

            foreach(StoredProcedureInfo storedProcedureInfo in schemaInfo.StoredProcedures)
            {
                code.AppendLine("#region Stored Procedures");
                code.AppendLine();

                GenerateStoredProcedure(code, storedProcedureInfo);

                code.AppendLine("#endregion");
                code.AppendLine();
            }

            code.Append(@"
private void PopConnection(System.Data.SqlClient.SqlCommand command)
{
    if (this.connection != null)
    {
        command.Connection = this.connection;
        command.Transaction = this.transaction;
    }
    else
    {
        command.Connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
        command.Connection.Open();
    }
}

private void PushConnection(System.Data.SqlClient.SqlCommand command)
{
    System.Data.SqlClient.SqlConnection connection = command.Connection;
    System.Data.SqlClient.SqlTransaction transaction = command.Transaction;

    command.Connection = null;
    command.Transaction = null;

    if (connection != null && this.connection != connection)
    {
        connection.Close();
    }
}

public void BeginTransaction()
{
    if (this.connection == null)
    {
        this.connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
        this.connection.Open();
    }

    if (this.transaction == null)
    {
        this.transaction = this.connection.BeginTransaction();
    }

    ++this.transactionCounter;
}

public void CommitTransaction()
{
    if (this.transaction == null || this.transactionCounter <= 0)
    {
        throw new InvalidOperationException(""currentTransaction"");
    }

    --this.transactionCounter;

    if (this.transactionCounter == 0)
    {
        this.transaction.Commit();
        this.transaction = null;
    }
}

public void RollbackTransaction()
{
    if (this.transaction == null || this.transactionCounter <= 0)
    {
        throw new InvalidOperationException(""currentTransaction"");
    }

    this.transactionCounter = 0;
    this.transaction.Rollback();
    this.transaction = null;
}

public void Dispose()
{
    if (this.externalResource)
    {
        return;
    }

    try
    {
        if (this.transaction != null)
        {
            this.transaction.Rollback();
            this.transaction = null;
            this.transactionCounter = 0;
        }
    }
    finally
    {
        if (this.connection != null)
        {
            this.connection.Close();
            this.connection = null;
        }
    }
}
");

            code.CodeBlockEnd();
            code.CodeBlockEnd();

            File.WriteAllText(outputFilename, code.ToString());
        }

        class Filter
        {
            public bool IsUnique;
            public List<ColumnInfo> Columns;
        }
    }
}
