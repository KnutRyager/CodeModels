using System;
using System.Linq;
using System.Reflection;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.DataStructures;
using Common.Reflection;

namespace CodeModels.Models;

public abstract record TypeFromReflection(Type ReflectedType, string TypeName, bool Required = true, bool IsMulti = false)
    : AbstractType(TypeName, ReflectedType.GetGenericArguments().Select(x => Create(x)).ToEqualityList<IType>(), Required, IsMulti, ReflectedType), IExpression
{
    public static TypeFromReflection Create(Type reflectedType)
    {
        //var simplifiedNullableType = ReflectionUtil.SimplifyNullableType(reflectedType, !required);
        //var requiredSimplified = required ? true : !ReflectionUtil.IsSimplyfiableNullableType(reflectedType, true);
        //var simplifiedArrayType = ReflectionUtil.SimplifyArrayType(simplifiedNullableType, isMulti);
        //var isMultiSimplified = !ReflectionUtil.IsSimplyfiableArrayType(reflectedType, isMulti) && isMulti;
        //var name = TypeParsing.RemoveGenericPartOfTypeName(simplifiedArrayType.Name);
        var required = !ReflectionUtil.IsNullable(reflectedType);
        var isMulti = reflectedType.IsArray;
        var name = reflectedType.FullName;
        //if (name.StartsWith("Nullable")) 
        name = ReflectionSerialization.SimplifyGenericName(name);
        name = TypeParsing.RemoveGenericAndArrayPart(name);
        name = TypeParsing.RemoveNullablePart(name);
        //name = ReflectionSerialization.SimplifyGenericName(name);
        name = $"{ReflectionSerialization.GetToShortHandName(name.Contains("[") ? name[..name.IndexOf("[")] : name)}{(name.Contains("[") ? name[name.IndexOf("[")..] : "")}";
        ////ReflectionSerialization.SimplifyGenericName(reflectedType.FullName)
        return new TypeFromReflectionImp(reflectedType, name, required, isMulti);
    }

    public static TypeFromReflection Create(ConstructorInfo constructor)
        => Create(constructor.DeclaringType);

    public static TypeFromReflection Create(MethodInfo method)
        => Create(method.ReturnType);

    public static TypeFromReflection Create(FieldInfo field)
        => Create(field.FieldType);

    public static TypeFromReflection Create(PropertyInfo property)
        => Create(property.PropertyType);

    public override IType PlainType()
        => IsMulti ? Create(ReflectedType!.GetElementType()) : !Required ? Create(ReflectedType!.GenericTypeArguments.First()) : this;
    public override IType ToMultiType() => Create(ReflectedType!.MakeArrayType());
    public override IType ToOptionalType() => Create(ReflectionUtil.GetNullableType(ReflectedType!));

    private record TypeFromReflectionImp(Type ReflectedType,
        string TypeName,
        bool Required = true,
        bool IsMulti = false)
    : TypeFromReflection(
        ReflectedType: ReflectedType,
        Required: Required,
        IsMulti: IsMulti,
        TypeName: TypeName);
}
