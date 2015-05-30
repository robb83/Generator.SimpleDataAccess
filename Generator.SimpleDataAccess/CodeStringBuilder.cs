using System;
using System.Text;

namespace Generator.SimpleDataAccess
{
    public class CodeStringBuilder
    {
        StringBuilder stringBuilder = new StringBuilder();
        int indent = 0;
        bool requiredIndent = true;
        bool endWithNewLine = false;

        public void AppendLine()
        {
            this.stringBuilder.AppendLine();
            this.requiredIndent = true;
            this.endWithNewLine = true;
        }

        public void Append(String chunk)
        {
            if (chunk == null) return;

            if (this.requiredIndent)
            {
                this.stringBuilder.Append("".PadLeft(this.indent));
                this.requiredIndent = false;
            }

            chunk = chunk.Replace("\n", "\n".PadRight(indent + 1));

            this.endWithNewLine = chunk.EndsWith("\n");
            this.stringBuilder.Append(chunk);
        }

        public void AppendFormat(String chunk, params Object[] args)
        {
            if (chunk == null) return;

            this.Append(String.Format(chunk, args));
        }

        public void AppendLine(String chunk)
        {
            if (chunk == null) return;

            this.Append(chunk);
            this.AppendLine();
        }

        public void AppendLineFormat(String chunk, params Object[] args)
        {
            if (chunk == null) return;

            this.Append(String.Format(chunk, args));
            this.AppendLine();
        }

        public void CodeBlockBegin(String chunk, params Object[] args)
        {
            CodeBlockBegin(String.Format(chunk, args));
        }

        public void CodeBlockBegin(String chunk = null)
        {
            if (chunk != null)
            {
                this.Append(chunk);
            }

            if (!this.endWithNewLine)
            {
                this.AppendLine();
            }

            this.Append("{");
            this.AppendLine();

            this.Push();
        }

        public void CodeBlockEnd(String chunk, params Object[] args)
        {
            CodeBlockEnd(String.Format(chunk, args));
        }

        public void CodeBlockEnd(String chunk = null)
        {
            if (chunk != null)
            {
                this.Append(chunk);
            }

            this.Pop();

            if (!this.endWithNewLine)
            {
                this.AppendLine();
            }

            this.Append("}");
            this.AppendLine();
        }

        public void Push()
        {
            this.indent += 4;
        }

        public void Pop()
        {
            this.indent -= 4;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
