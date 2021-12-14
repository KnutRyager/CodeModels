#nullable disable
using System;
using System.Collections.Generic;
using Common.Reflection;

namespace TheEverythingAPI.Modelling
{
    public class Field
    {
        public int Id { get; set; }
        public int ReferenceFieldId { get; set; }
        public int ReferenceClazzdId { get; set; }
        public int ClazzId { get; set; }
        public int DataTypeId { get; set; }
        public string Name { get; set; }
        public string TypeSerialized { get; set; }
        public string DisplayName { get; set; }

        public Field ReferenceField { get; set; }
        public Clazz ReferenceClazz { get; set; }
        public Clazz Clazz { get; set; }
        public virtual IList<Field> ReferenceFields { get; set; }

        public Type Type { get; set; }
        public bool Required => ReflectionUtil.GetUnderlyingType(Type) != null;
    }
}