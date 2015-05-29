using System;
using System.Collections.Generic;

namespace Generator.SimpleDataAccess.Model
{
    public class StoredProcedureInfo
    {
        public int ObjectID;
        public String Schema;
        public String Name;
        public List<StoredProcedureParameterInfo> Parameters = new List<StoredProcedureParameterInfo>();

        public StoredProcedureParameterInfo GetParameterByName(String name)
        {
            foreach (StoredProcedureParameterInfo parameterInfo in this.Parameters)
            {
                if (parameterInfo.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return parameterInfo;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return this.Schema + "." + this.Name;
        }
    }
}
