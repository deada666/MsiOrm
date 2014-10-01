using System;

namespace Evil.MsiOrm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MsiTableAttribute : Attribute
    {
        public string TableName { get; set; }

        public MsiTableAttribute(string table)
        {
            TableName = table;
        }
    }
}
