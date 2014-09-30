using Evil.MsiOrm.Core.Attributes;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Evil.MsiOrm.Core
{
    internal class MsiRowToObjectConverter
    {
        private readonly Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

        private readonly Type objectType;

        public MsiRowToObjectConverter(Type type)
        {
            this.objectType = type;
            Initialize();
        }

        private void Initialize()
        {
            var tableAttribute = objectType.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(MsiTableAttribute));
            TableName = tableAttribute.ConstructorArguments[0].Value.ToString();
            var tempProperties = objectType.GetProperties();
            foreach (var property in tempProperties)
            {
                var attribute = property.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(MsiColumnAttribute));
                if (attribute == null)
                {
                    continue;
                }

                properties.Add(attribute.ConstructorArguments[0].Value.ToString(), property);
            }
        }

        public string TableName { get; private set; }

        public object Convert(Record record)
        {
            var tempObject = Activator.CreateInstance(objectType);
            foreach (var key in properties.Keys)
            {
                var value = record[key];
                var propertyInfo = properties[key];
                propertyInfo.SetValue(tempObject, value);
            }

            return tempObject;
        }
    }
}
