using System;

namespace Evil.MsiOrm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MsiColumnAttribute : Attribute
    {
        public readonly string ColumnName;

        public MsiColumnAttribute(string column)
        {
            ColumnName = column;
        }
    }
}