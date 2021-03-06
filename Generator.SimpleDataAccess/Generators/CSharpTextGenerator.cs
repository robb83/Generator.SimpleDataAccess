﻿using Generator.SimpleDataAccess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            code.CodeBlockBegin("public partial class {0}", tableInfo.ClassName);

            foreach (var c in tableInfo.Columns)
            {
                code.AppendLine(ToAutomaticProperty(c.PropertyName, c.FullTypeName));
            }

            code.CodeBlockEnd();
        }

        private static void GenerateColumnMapperFromSqlDataReader(CodeStringBuilder code, ColumnInfo columnInfo, int? ordinalOverride = null)
        {
            int ordinal = columnInfo.Ordinal;

            if (ordinalOverride.HasValue)
            {
                ordinal = ordinalOverride.Value;
            }

            if (columnInfo.IsNullable && columnInfo.Type.IsValueType)
            {
                code.CodeBlockBegin("if (reader.IsDBNull({0}))", ordinal);
                code.AppendLineFormat("entity.{0} = null;", columnInfo.PropertyName, columnInfo.MappingMethodName, ordinal);
                code.CodeBlockEnd();

                code.CodeBlockBegin("else");
                code.AppendLineFormat("entity.{0} = reader.{1}({2});", columnInfo.PropertyName, columnInfo.MappingMethodName, ordinal);
                code.CodeBlockEnd();
            }
            else
            {
                code.AppendLineFormat("entity.{0} = reader.{1}({2});", columnInfo.PropertyName, columnInfo.MappingMethodName, ordinal);
            }
        }

        private static void GenerateColumnMapperFromSqlParameter(CodeStringBuilder code, ColumnInfo columnInfo, bool throwIfNullAtNotNullable)
        {
            if (columnInfo.IsNullable)
            {
                code.CodeBlockBegin("if ({0}.Value == System.DBNull.Value)", columnInfo.LocalParameterVariableName);
                code.AppendLineFormat("entity.{0} = null;", columnInfo.PropertyName);
                code.CodeBlockEnd();
                
                code.CodeBlockBegin("else");
                code.AppendLineFormat("entity.{0} = ({1}){2}.Value;", columnInfo.PropertyName, columnInfo.FullTypeName ,columnInfo.LocalParameterVariableName);
                code.CodeBlockEnd();
            }
            else
            {
                if (throwIfNullAtNotNullable)
                {
                    code.CodeBlockBegin("if ({0}.Value == System.DBNull.Value)", columnInfo.LocalParameterVariableName);
                    code.Append("throw new InvalidOperationException(\"Invalid output value: " + columnInfo.LocalParameterVariableName + "\");");
                    code.CodeBlockEnd();
                }

                code.AppendLineFormat("entity.{0} = ({1}){2}.Value;", columnInfo.PropertyName, columnInfo.FullTypeName, columnInfo.LocalParameterVariableName);
            }
        }

        public static void GenerateMapping(CodeStringBuilder code, String methodName, String entityTypeName, List<ColumnInfo> columns)
        {
            code.CodeBlockBegin("public static {0} {1}(System.Data.SqlClient.SqlDataReader reader)", entityTypeName, methodName);

            code.AppendLineFormat("{0} entity = new {0}();", entityTypeName);
            
            foreach (var columnInfo in columns)
            {
                GenerateColumnMapperFromSqlDataReader(code, columnInfo);
            }
            
            code.CodeBlockEnd("return entity;");
        }

        public static void GenerateStoredProcedureMethod(CodeStringBuilder code, StoredProcedureInfo storedProcedureInfo)
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
            
            code.CodeBlockBegin("public void Execute{0}({1})", storedProcedureInfo.Name, parameters);

            code.AppendLine("BeginTransaction();");
            code.AppendLine();

            code.CodeBlockBegin("try");
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"[{0}].[{1}]\"))", storedProcedureInfo.Schema, storedProcedureInfo.Name);
            code.AppendLine("command.CommandType = System.Data.CommandType.StoredProcedure;");
            code.AppendLine();
            
            code.CodeBlockBegin("try");
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
                    parameterInfo.IsOutput ? System.Data.ParameterDirection.Output : System.Data.ParameterDirection.Input,
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
            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            // end of using
            code.CodeBlockEnd();

            // end of try
            code.AppendLine();
            code.Append("CommitTransaction();");
            code.CodeBlockEnd();
            code.CodeBlockBegin("catch");
            code.AppendLine("RollbackTransaction();");
            code.AppendLine("throw;");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
        }

        public static void GenerateSelectCountMethod(CodeStringBuilder code, String methodName, String script)
        {
            code.CodeBlockBegin("public int {0}()", methodName);
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");
            
            code.CodeBlockBegin("if (reader.Read())");
            code.Append("return reader.GetInt32(0);");
            code.CodeBlockEnd();
            
            code.CodeBlockBegin("else");
            code.Append("throw new InvalidOperationException(\"Select count failed.\");");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // method
        }

        public static void GenerateSelectPagedMethod(CodeStringBuilder code, String methodName, String entityTypeName, String mappingMethodName, String script, List<ColumnInfo> key)
        {
            if (key == null || key.Count == 0)
            {
                return;
            }

            String firstIndexParameterName = "@__FirstIndex", firstIndexLocalVariableName = "pFirstIndex", firstIndex = "firstIndex";
            String lastIndexParameterName = "@__LastIndex", lastIndexLocalVariableName = "pLastIndex", lastIndex = "lastIndex";

            code.CodeBlockBegin("public List<{1}> {0}(int {2}, int {3})", methodName, entityTypeName, firstIndex, lastIndex);

            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);

            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();
            
            GenerateSqlParameter(code, firstIndex, firstIndexLocalVariableName, firstIndexParameterName, System.Data.SqlDbType.Int, false, System.Data.ParameterDirection.Input, -1);
            code.AppendLine();

            GenerateSqlParameter(code, lastIndex, lastIndexLocalVariableName, lastIndexParameterName, System.Data.SqlDbType.Int, false, System.Data.ParameterDirection.Input, -1);
            code.AppendLine();

            code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");

            code.AppendLineFormat("List<{0}> result = new List<{0}>();", entityTypeName);

            code.CodeBlockBegin("while (reader.Read())");
            code.AppendFormat("result.Add({0}(reader));", mappingMethodName);
            code.CodeBlockEnd();

            code.Append("return result;");

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // method
        }

        public static void GenerateSelectMethod(CodeStringBuilder code, String methodName, String entityTypeName, String script, List<ColumnInfo> key, bool singleResult, String mappingMethod)
        {
            StringBuilder parameters = new StringBuilder();

            if (key != null)
            {
                for (int a = 0; a < key.Count; ++a)
                {
                    if (a > 0)
                    {
                        parameters.Append(", ");
                    }

                    parameters.AppendFormat("{0} {1}", key[a].FullTypeName, key[a].LocalVariableName);
                }
            }

            if (singleResult)
            {
                code.CodeBlockBegin("public {0} {1}({2})", entityTypeName, methodName, parameters.ToString());
            }
            else
            {
                code.CodeBlockBegin("public List<{0}> {1}({2})", entityTypeName, methodName, parameters.ToString());
            }
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            if (key != null)
            {
                foreach (ColumnInfo columnInfo in key)
                {
                    code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                    GenerateSqlParameter(code, 
                        columnInfo.LocalVariableName, 
                        columnInfo.LocalParameterVariableName, 
                        columnInfo.ParameterName, 
                        columnInfo.DbType, 
                        columnInfo.IsNullable,
                        System.Data.ParameterDirection.Input,
                        columnInfo.MaxLength);
                    code.AppendLine();
                }
            }
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");

            if (singleResult)
            {
                code.CodeBlockBegin("if (reader.Read())");
                code.AppendFormat("return {0}(reader);", mappingMethod);
                code.CodeBlockEnd();

                code.CodeBlockBegin("else");
                code.Append("return null;");
                code.CodeBlockEnd();
            }
            else
            {
                code.AppendLineFormat("List<{0}> result = new List<{0}>();", entityTypeName);

                code.CodeBlockBegin("while (reader.Read())");
                code.AppendFormat("result.Add({0}(reader));", mappingMethod);
                code.CodeBlockEnd();

                code.Append("return result;");
            }

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using
            code.CodeBlockEnd(); // method
        }
        
        private static bool IsVariableLength(System.Data.SqlDbType dbType)
        {
            switch(dbType)
            {
                case System.Data.SqlDbType.Binary:
                case System.Data.SqlDbType.Char:
                case System.Data.SqlDbType.Image:
                case System.Data.SqlDbType.NText:
                case System.Data.SqlDbType.NVarChar:
                case System.Data.SqlDbType.NChar:
                case System.Data.SqlDbType.Text:
                case System.Data.SqlDbType.VarChar:
                case System.Data.SqlDbType.VarBinary:
                    return true;
                default:
                    return false;
            }
        }

        private static void GenerateSqlParameter(CodeStringBuilder code, String valueExpression, String localParameterName, String sqlParameterName, System.Data.SqlDbType sqlDbType, bool is_nullable, System.Data.ParameterDirection parameterDirection, long max_length)
        {
            if (IsVariableLength(sqlDbType))
            {
                code.AppendLineFormat("System.Data.SqlClient.SqlParameter {0} = command.Parameters.Add(\"{1}\", System.Data.SqlDbType.{2}, {3});", localParameterName, sqlParameterName, sqlDbType.ToString(), max_length);
            }
            else
            {
                code.AppendLineFormat("System.Data.SqlClient.SqlParameter {0} = command.Parameters.Add(\"{1}\", System.Data.SqlDbType.{2});", localParameterName, sqlParameterName, sqlDbType.ToString(), max_length);
            }

            if (parameterDirection == System.Data.ParameterDirection.Output)
            {
                code.AppendLineFormat("{0}.Direction = System.Data.ParameterDirection.Output;", localParameterName);
            }
            else
            {
                if (parameterDirection == System.Data.ParameterDirection.InputOutput)
                {
                    code.AppendLineFormat("{0}.Direction = System.Data.ParameterDirection.InputOutput;", localParameterName);
                }

                if (is_nullable)
                {
                    code.CodeBlockBegin("if ({0} == null)", valueExpression);
                    code.AppendLineFormat("{0}.Value = System.DBNull.Value;", localParameterName);
                    code.CodeBlockEnd();
                    
                    code.CodeBlockBegin("else");
                    code.AppendLineFormat("{0}.Value = {1};", localParameterName, valueExpression);
                    code.CodeBlockEnd();
                }
                else
                {
                    code.AppendLineFormat("{0}.Value = {1};", localParameterName, valueExpression);
                }
            }
        }

        public static void GenerateUpsertMethod(CodeStringBuilder code, String methodName, String entityTypeName, String script, List<ColumnInfo> editable, List<ColumnInfo> computed)
        {
            code.CodeBlockBegin("public void {0}({1} entity)", methodName, entityTypeName);
            
            code.CodeBlockBegin("if (entity == null)");
            code.Append("throw new ArgumentNullException(\"entity\");");
            code.CodeBlockEnd();
            code.AppendLine();
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            foreach (ColumnInfo columnInfo in editable)
            {
                code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                GenerateSqlParameter(
                    code,
                    String.Format("entity.{0}", columnInfo.PropertyName),
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    System.Data.ParameterDirection.Input,
                    columnInfo.MaxLength);
                code.AppendLine();
            }
            
            if (computed != null && computed.Count > 0)
            {
                code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");
                
                code.CodeBlockBegin("if (reader.Read())");

                int ordinal = 0;
                foreach (ColumnInfo columnInfo in computed)
                {
                    GenerateColumnMapperFromSqlDataReader(code, columnInfo, ordinal++);
                }

                code.CodeBlockEnd();

                code.CodeBlockBegin("else");
                code.Append("throw new InvalidOperationException(\"Upsert failed.\");");
                code.CodeBlockEnd();

                code.CodeBlockEnd();
            }
            else
            {
                code.CodeBlockBegin("if (command.ExecuteNonQuery() <= 0)");
                code.Append("throw new InvalidOperationException(\"Upsert failed.\");");
                code.CodeBlockEnd();
            }
            
            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using command
            code.CodeBlockEnd(); // method
        }

        public static void GenerateUpdateMethod(CodeStringBuilder code, String methodName, String entityTypeName, String script, List<ColumnInfo> key, List<ColumnInfo> insertableColumns, List<ColumnInfo> outputColumns)
        {
            code.CodeBlockBegin("public void {0}({1} entity)", methodName, entityTypeName);
            
            code.CodeBlockBegin("if (entity == null)");
            code.Append("throw new ArgumentNullException(\"entity\");");
            code.CodeBlockEnd();
            code.AppendLine();
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            foreach (ColumnInfo columnInfo in key)
            {
                code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                GenerateSqlParameter(
                    code,
                    String.Format("entity.{0}", columnInfo.PropertyName),
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    System.Data.ParameterDirection.Input,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            foreach (ColumnInfo columnInfo in insertableColumns)
            {
                code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                GenerateSqlParameter(
                    code,
                    String.Format("entity.{0}", columnInfo.PropertyName),
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    System.Data.ParameterDirection.Input,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            if (outputColumns != null && outputColumns.Count > 0)
            {
                code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");

                code.CodeBlockBegin("if (reader.Read())");

                int ordinal = 0;
                foreach (ColumnInfo columnInfo in outputColumns)
                {
                    GenerateColumnMapperFromSqlDataReader(code, columnInfo, ordinal++);
                }

                code.CodeBlockEnd();

                code.CodeBlockBegin("else");
                code.Append("throw new InvalidOperationException(\"Insert failed.\");");
                code.CodeBlockEnd();

                code.CodeBlockEnd();
            }
            else
            {
                code.CodeBlockBegin("if (command.ExecuteNonQuery() <= 0)");
                code.Append("throw new InvalidOperationException(\"Update failed.\");");
                code.CodeBlockEnd();
            }

            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using command
            code.CodeBlockEnd(); // method
        }
        
        public static void GenerateInsertMethod(CodeStringBuilder code, String methodName, String entityTypeName, String script, List<ColumnInfo> insertableColumns, List<ColumnInfo> outputColumns)
        {
            code.CodeBlockBegin("public void {0}({1} entity)", methodName, entityTypeName);

            code.CodeBlockBegin("if (entity == null)");
            code.Append("throw new ArgumentNullException(\"entity\");");
            code.CodeBlockEnd();
            code.AppendLine();

            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            foreach (ColumnInfo columnInfo in insertableColumns)
            {
                code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                GenerateSqlParameter(
                    code,
                    String.Format("entity.{0}", columnInfo.PropertyName),
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    System.Data.ParameterDirection.Input,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            if (outputColumns != null && outputColumns.Count > 0)
            {
                code.CodeBlockBegin("using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())");

                code.CodeBlockBegin("if (reader.Read())");

                int ordinal = 0;
                foreach (ColumnInfo columnInfo in outputColumns)
                {
                    GenerateColumnMapperFromSqlDataReader(code, columnInfo, ordinal++);
                }

                code.CodeBlockEnd();

                code.CodeBlockBegin("else");
                code.Append("throw new InvalidOperationException(\"Insert failed.\");");
                code.CodeBlockEnd();

                code.CodeBlockEnd();
            }
            else
            {
                code.CodeBlockBegin("if (command.ExecuteNonQuery() <= 0)");
                code.Append("throw new InvalidOperationException(\"Insert failed.\");");
                code.CodeBlockEnd();
            }

            code.CodeBlockEnd();
            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd();
            code.CodeBlockEnd();
        }

        public static void GenerateDeleteMethod(CodeStringBuilder code, String methodName, String script, List<ColumnInfo> key)
        {
            StringBuilder parameters = new StringBuilder();

            for (int a = 0; a < key.Count; ++a)
            {
                if (a > 0)
                {
                    parameters.Append(", ");
                }

                parameters.AppendFormat("{0} {1}", key[a].FullTypeName, key[a].LocalVariableName);
            }

            code.CodeBlockBegin("public void {0}({1})", methodName, parameters.ToString());
            
            code.CodeBlockBegin("using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(\"{0}\"))", script);
            
            code.CodeBlockBegin("try");
            code.AppendLine("PopConnection(command);");
            code.AppendLine();

            foreach (ColumnInfo columnInfo in key)
            {
                code.AppendLineFormat("// Parameter settings: {0}", columnInfo.ParameterName);

                GenerateSqlParameter(
                    code,
                    columnInfo.LocalVariableName,
                    columnInfo.LocalParameterVariableName,
                    columnInfo.ParameterName,
                    columnInfo.DbType,
                    columnInfo.IsNullable,
                    System.Data.ParameterDirection.Input,
                    columnInfo.MaxLength);
                code.AppendLine();
            }

            code.Append("command.ExecuteNonQuery();");

            code.CodeBlockEnd(); // try

            code.CodeBlockBegin("finally");
            code.Append("PushConnection(command);");
            code.CodeBlockEnd();

            code.CodeBlockEnd(); // using command
            code.CodeBlockEnd(); // method
        }

    }
}
