using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Generator.SimpleDataAccess.Generators
{
    public class SimpleDataAccessGenerator
    {
        public static void GenerateUpsertMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);
            String methodName = String.Format("Upsert{0}", entityTypeName);
            List<ColumnInfo> editableColumns = tableInfo.GetEditableColumns();
            List<ColumnInfo> computedColumns = tableInfo.GetComputedColumns();
            List<ColumnInfo> identityColumns = tableInfo.GetIdentityColumns();
            List<ColumnInfo> key = tableInfo.GetKey();

            List<ColumnInfo> outputColumns = new List<ColumnInfo>();
            outputColumns.AddRange(identityColumns);
            outputColumns.AddRange(computedColumns);

            List<ColumnInfo> inputColumns = new List<ColumnInfo>();
            inputColumns.AddRange(editableColumns);
            inputColumns.AddRange(identityColumns);

            if (editableColumns.Count == 0)
            {
                return;
            }

            if (key.SequenceEqual(editableColumns))
            {
                return;
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateMergeStatement(script, target, key, editableColumns, outputColumns);

            // generate code for script
            CSharpTextGenerator.GenerateUpsertMethod(code, methodName, entityTypeName, script.ToString(), inputColumns, outputColumns);
        }

        public static void GenerateUpdateMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);
            String methodName = String.Format("Update{0}", entityTypeName);
            List<ColumnInfo> editableColumns = tableInfo.GetEditableColumns();
            List<ColumnInfo> computedColumns = tableInfo.GetComputedColumns();
            List<ColumnInfo> key = tableInfo.GetKey();

            if (editableColumns.Count == 0)
            {
                return;
            }

            if (key.SequenceEqual(editableColumns))
            {
                return;
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateUpdateStatement(script, target, key, editableColumns, computedColumns);

            // generate code for script
            CSharpTextGenerator.GenerateUpdateMethod(code, methodName, entityTypeName, script.ToString(), key, editableColumns, computedColumns);
        }

        public static void GenerateInsertMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);
            String methodName = String.Format("Insert{0}", entityTypeName);            
            List<ColumnInfo> editableColumns = tableInfo.GetEditableColumns();
            List<ColumnInfo> computedColumns = tableInfo.GetComputedColumns();
            List<ColumnInfo> identityColumns = tableInfo.GetIdentityColumns();
            List<ColumnInfo> key = tableInfo.GetKey();

            List<ColumnInfo> readBackColumns = new List<ColumnInfo>();
            readBackColumns.AddRange(identityColumns);
            readBackColumns.AddRange(computedColumns);

            if (editableColumns.Count == 0)
            {
                return;
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateInsertStatement(script, target, editableColumns, readBackColumns);
            
            // generate code for script
            CSharpTextGenerator.GenerateInsertMethod(code, methodName, entityTypeName, script.ToString(), editableColumns, readBackColumns);
        }

        public static void GenerateDeleteMethod(CodeStringBuilder code, TableInfo tableInfo, List<ColumnInfo> key)
        {
            if (key == null || key.Count == 0)
            {
                return;
            }

            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);

            StringBuilder methodName = new StringBuilder();
            methodName.Append("Delete");
            methodName.Append(entityTypeName);
            methodName.Append("By");
            foreach(var k in key)
            {
                methodName.Append(k.PropertyName);
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateDeleteStatement(script, target, key);

            // generate code for script
            CSharpTextGenerator.GenerateDeleteMethod(code, methodName.ToString(), script.ToString(), key);
        }

        public static void GenerateSelectMethod(CodeStringBuilder code, TableInfo tableInfo, List<ColumnInfo> key, bool singleResult, String mappingMethodName)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);

            StringBuilder methodName = new StringBuilder();
            methodName.Append("Select");
            methodName.Append(entityTypeName);
            if (key != null && key.Count > 0)
            {
                methodName.Append("By");
                foreach (var k in key)
                {
                    methodName.Append(k.PropertyName);
                }
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateSelectStatement(script, target, tableInfo.Columns ,key);

            // generate code for script
            CSharpTextGenerator.GenerateSelectMethod(code, methodName.ToString(), entityTypeName, script.ToString(), key, singleResult, mappingMethodName);
        }

        public static void GenerateSelectPagedMethod(CodeStringBuilder code, TableInfo tableInfo, String mappingMethodName)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);
            List<ColumnInfo> key = tableInfo.GetKey();

            StringBuilder methodName = new StringBuilder();
            methodName.Append("Select");
            methodName.Append(entityTypeName);
            methodName.Append("Paged");

            if (key == null || key.Count == 0)
            {
                return;
            }

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateSelectStatementPaged(script, target, tableInfo.Columns, "@__FirstIndex", "@__LastIndex", key, null);

            // generate code for script
            CSharpTextGenerator.GenerateSelectPagedMethod(code, methodName.ToString(), entityTypeName, mappingMethodName, script.ToString(), key);
        }

        public static void GenerateSelectCountMethod(CodeStringBuilder code, TableInfo tableInfo)
        {
            String entityTypeName = tableInfo.ClassName;
            String target = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.Name);

            StringBuilder methodName = new StringBuilder();
            methodName.Append("Select");
            methodName.Append(entityTypeName);
            methodName.Append("Count");

            // generate sql script
            StringBuilder script = new StringBuilder();
            SQLTextGenerator.GenerateSelectStatementCount(script, target, null);

            // generate code for script
            CSharpTextGenerator.GenerateSelectCountMethod(code, methodName.ToString(), script.ToString());
        }

        public static void GenerateDatabaseAccessCode(DatabaseSchemaInfo schemaInfo, String outputFilename)
        {
            CodeStringBuilder code = new CodeStringBuilder();

            // usings
            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine();

            // namespace
            code.CodeBlockBegin("namespace {0}", schemaInfo.Namespace);

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
            code.CodeBlockBegin("public partial class {0} : IDisposable", schemaInfo.ClassName);

            // variable
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
                String entityTypeName = t.ClassName;
                String mappingMethodName = "Read" + entityTypeName;

                code.AppendLineFormat("#region Upsert, Insert, Update, Delete, Select, Mapping - {0}", t.ToString());
                code.AppendLine();

                CSharpTextGenerator.GenerateMapping(code, mappingMethodName, entityTypeName, t.Columns);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateUpsertMethod(code, t);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateInsertMethod(code, t);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateUpdateMethod(code, t);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateSelectMethod(code, t, null, false, mappingMethodName);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateSelectCountMethod(code, t);
                code.AppendLine();

                SimpleDataAccessGenerator.GenerateSelectPagedMethod(code, t, mappingMethodName);
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
                    SimpleDataAccessGenerator.GenerateSelectMethod(code, t, keyValue.Value.Columns, keyValue.Value.IsUnique, mappingMethodName);
                    code.AppendLine();

                    SimpleDataAccessGenerator.GenerateDeleteMethod(code, t, keyValue.Value.Columns);
                    code.AppendLine();
                }

                code.AppendLine("#endregion");
                code.AppendLine();
            }

            foreach (StoredProcedureInfo storedProcedureInfo in schemaInfo.StoredProcedures)
            {
                code.AppendLine("#region Stored Procedures");
                code.AppendLine();

                CSharpTextGenerator.GenerateStoredProcedureMethod(code, storedProcedureInfo);

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

public void BeginTransaction(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified)
{
    if (this.connection == null)
    {
        this.connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
        this.connection.Open();
    }
        
    if (this.transaction == null)
    {
        this.transaction = this.connection.BeginTransaction(isolationLevel);
    }
    else
    {
        if (this.transaction.IsolationLevel != isolationLevel)
        {
            throw new InvalidOperationException(""Transaction isolation level mismatch."");
        }
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
