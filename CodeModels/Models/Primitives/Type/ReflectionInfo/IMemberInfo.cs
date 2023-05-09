using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeAnalyzation.Models.Reflection;

public interface IMemberInfo : ICustomAttributeProvider
{
    IEnumerable<CustomAttributeData> CustomAttributes { get; }
    abstract ITypeInfo DeclaringType { get; }
    abstract MemberTypes MemberType { get; }
    int MetadataToken { get; }
    Module Module { get; }
    abstract string Name { get; }
    abstract ITypeInfo ReflectedType { get; }
    bool Equals(object obj);
    IList<CustomAttributeData> GetCustomAttributesData();
    int GetHashCode();
    //static  bool operator ==(MemberInfo left, MemberInfo right);
    //static  bool operator !=(MemberInfo left, MemberInfo right);
}
