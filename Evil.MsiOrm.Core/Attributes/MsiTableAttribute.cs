using System;

namespace Evil.MsiOrm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MsiTableAttribute : Attribute
    {
        private readonly string TableName;

        public MsiTableAttribute(string table)
        {
            TableName = table;
        }
    }
}
