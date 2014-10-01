using System;

namespace Evil.MsiOrm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MsiColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public MsiColumnAttribute(string column)
        {
            ColumnName = column;
        }
    }
}