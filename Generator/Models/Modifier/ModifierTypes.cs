using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class ModifierTypes
{
    public const Modifier Access = Modifier.Private | Modifier.Protected | Modifier.Internal | Modifier.Public;
    public const Modifier NonAccess = ((Modifier)int.MaxValue) ^ Access;
    public const Modifier Static = Modifier.Static;
    public const Modifier NonStatic = ((Modifier)int.MaxValue) ^ Static;
    public const Modifier Member = Modifier.Class | Modifier.Struct | Modifier.Enum | Modifier.Interface;
    public const Modifier NonMember = ((Modifier)int.MaxValue) ^ Member;
    public const Modifier Const = Modifier.Static | Modifier.Protected;
    public const Modifier NonConst = ((Modifier)int.MaxValue) ^ Const;
    public const Modifier Abstract = Modifier.Abstract | Modifier.Virtual | Modifier.Override;
    public const Modifier NonAbstract = ((Modifier)int.MaxValue) ^ Abstract;
    public const Modifier Record = Modifier.Record;
    public const Modifier NonRecord = ((Modifier)int.MaxValue) ^ Record;
    public const Modifier Field = Modifier.Field | Modifier.Property;
    public const Modifier NonField = ((Modifier)int.MaxValue) ^ Field;
    public const Modifier Async = Modifier.Async | Modifier.Await;
    public const Modifier NonAsync = ((Modifier)int.MaxValue) ^ Async;
    public const Modifier Using = Modifier.Using;
    public const Modifier NonUsing = ((Modifier)int.MaxValue) ^ Using;
    public const Modifier Partial = Modifier.Partial;
    public const Modifier NonPartial = ((Modifier)int.MaxValue) ^ Partial;
}

public static class MethodHolderTypes
{
    public const Modifier PublicClass = Modifier.Public | Modifier.Class;
    public const Modifier PublicPartialClass = Modifier.Public | Modifier.Partial | Modifier.Class;
    public const Modifier PublicPartialStaticClass = Modifier.Public | Modifier.Partial | Modifier.Static | Modifier.Class;
    public const Modifier PublicInterface = Modifier.Public | Modifier.Interface;
    public const Modifier PublicAbstractInterface = Modifier.Public | Modifier.Abstract | Modifier.Interface;
    public const Modifier PublicStaticClass = Modifier.Public | Modifier.Static | Modifier.Class;
    public const Modifier PublicRecord = Modifier.Public | Modifier.Record;
    public const Modifier PublicStruct = Modifier.Public | Modifier.Struct;
    public const Modifier PublicRecordClass = Modifier.Public | Modifier.Record | Modifier.Class;
}

public static class PropertyAndFieldTypes
{
    public const Modifier PublicConst = Modifier.Public | Modifier.Const;
    public const Modifier PrivateConst = Modifier.Private | Modifier.Const;
    public const Modifier PublicStaticReadonlyField = Modifier.Public | Modifier.Static | Modifier.Readonly;
    public const Modifier PublicStaticField = Modifier.Public | Modifier.Static;
    public const Modifier PublicField = Modifier.Public | Modifier.Field;
    public const Modifier PrivateStaticReadonly = Modifier.Private | Modifier.Static | Modifier.Readonly;
    public const Modifier PrivateStaticField = Modifier.Private | Modifier.Static;
    public const Modifier PrivateReadonlyField = Modifier.Private | Modifier.Readonly;
    public const Modifier PrivateField = Modifier.Private | Modifier.Field;
    public const Modifier PublicStaticReadonly = Modifier.Public | Modifier.Static | Modifier.Readonly;
    public const Modifier PublicReadonly = Modifier.Public | Modifier.Readonly;
    public const Modifier PublicReadWrite = Modifier.Public;
    //public const Modifier PublicReadPrivateWrite = Modifier.Public;
    public const Modifier PrivateProperty = Modifier.Private | Modifier.Property;
}

public static class MethodHolderTypeExtensions
{
    public static Modifier AccessModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Private) => Modifier.Private,
        _ when modifier.HasFlag(Modifier.Protected) => Modifier.Protected,
        _ when modifier.HasFlag(Modifier.Internal) => Modifier.Internal,
        _ when modifier.HasFlag(Modifier.Public) => Modifier.Public,
        _ => Modifier.None
    };

    public static SyntaxToken AccessModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Private) => Token(SyntaxKind.PrivateKeyword),
        _ when modifier.HasFlag(Modifier.Protected) => Token(SyntaxKind.ProtectedKeyword),
        _ when modifier.HasFlag(Modifier.Internal) => Token(SyntaxKind.InternalKeyword),
        _ when modifier.HasFlag(Modifier.Public) => Token(SyntaxKind.PublicKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier StaticModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Static) => Modifier.Static,
        _ => Modifier.None
    };

    public static SyntaxToken StaticModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Static) => Token(SyntaxKind.StaticKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier ConstModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Const) => Modifier.Const,
        _ when modifier.HasFlag(Modifier.Readonly) => Modifier.Readonly,
        _ => Modifier.None
    };

    public static SyntaxToken ConstModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Const) => Token(SyntaxKind.ConstKeyword),
        _ when modifier.HasFlag(Modifier.Readonly) => Token(SyntaxKind.ReadOnlyKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier AbstractModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Abstract) => Modifier.Abstract,
        _ when modifier.HasFlag(Modifier.Virtual) => Modifier.Virtual,
        _ when modifier.HasFlag(Modifier.Override) => Modifier.Override,
        _ when modifier.HasFlag(Modifier.New) => Modifier.New,
        _ => Modifier.None
    };

    public static SyntaxToken AbstractModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Abstract) => Token(SyntaxKind.AbstractKeyword),
        _ when modifier.HasFlag(Modifier.Virtual) => Token(SyntaxKind.VirtualKeyword),
        _ when modifier.HasFlag(Modifier.Override) => Token(SyntaxKind.OverrideKeyword),
        _ when modifier.HasFlag(Modifier.New) => Token(SyntaxKind.NewKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier RecordModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Record) => Modifier.Record,
        _ => Modifier.None
    };

    public static SyntaxToken RecordModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Record) => Token(SyntaxKind.RecordKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier MemberTypeModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Class) => Modifier.Class,
        _ when modifier.HasFlag(Modifier.Struct) => Modifier.Struct,
        _ when modifier.HasFlag(Modifier.Enum) => Modifier.Enum,
        _ when modifier.HasFlag(Modifier.Interface) => Modifier.Interface,
        _ => Modifier.None
    };

    public static Modifier FieldModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Field) => Modifier.Field,
        _ when modifier.HasFlag(Modifier.Property) => Modifier.Property,
        _ => Modifier.None
    };

    public static SyntaxToken MemberTypeModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Class) => Token(SyntaxKind.ClassKeyword),
        _ when modifier.HasFlag(Modifier.Struct) => Token(SyntaxKind.StructKeyword),
        _ when modifier.HasFlag(Modifier.Enum) => Token(SyntaxKind.EnumKeyword),
        _ when modifier.HasFlag(Modifier.Interface) => Token(SyntaxKind.InterfaceKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier UsingModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Using) => Modifier.Using,
        _ => Modifier.None
    };

    public static SyntaxToken UsingModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Using) => Token(SyntaxKind.UsingKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier AsyncModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Async) => Modifier.Async,
        _ when modifier.HasFlag(Modifier.Await) => Modifier.Await,
        _ => Modifier.None
    };

    public static SyntaxToken AsyncModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Async) => Token(SyntaxKind.AsyncKeyword),
        _ when modifier.HasFlag(Modifier.Await) => Token(SyntaxKind.AwaitKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static Modifier PartialModifier(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Partial) => Modifier.Partial,
        _ => Modifier.None
    };

    public static SyntaxToken PartialModifierToken(this Modifier modifier) => modifier switch
    {
        _ when modifier.HasFlag(Modifier.Partial) => Token(SyntaxKind.PartialKeyword),
        _ => Token(SyntaxKind.None)
    };

    public static ModifierType GetModifierType(this Modifier modifier) => modifier switch
    {
        (<= Modifier.None) => ModifierType.None,
        (<= Modifier.Public) => ModifierType.Access,
        (<= Modifier.Static) => ModifierType.Static,
        (<= Modifier.Class) => ModifierType.Member,
        (<= Modifier.Const) => ModifierType.Const,
        (<= Modifier.Abstract) => ModifierType.Abstract,
        (<= Modifier.Record) => ModifierType.Record,
        (<= Modifier.Field) => ModifierType.Field,
        (<= Modifier.Async) => ModifierType.Async,
        (<= Modifier.Using) => ModifierType.Using,
        (<= Modifier.Partial) => ModifierType.Partial,
        _ => ModifierType.None
    };

    public static Modifier GetNonModifierType(this Modifier modifier) => modifier switch
    {
        (<= Modifier.None) => Modifier.None,
        (<= Modifier.Public) => ModifierTypes.NonAccess,
        (<= Modifier.Static) => ModifierTypes.NonStatic,
        (<= Modifier.Class) => ModifierTypes.NonMember,
        (<= Modifier.Const) => ModifierTypes.NonConst,
        (<= Modifier.Abstract) => ModifierTypes.NonAbstract,
        (<= Modifier.Record) => ModifierTypes.NonRecord,
        (<= Modifier.Field) => ModifierTypes.NonField,
        (<= Modifier.Async) => ModifierTypes.NonAsync,
        (<= Modifier.Using) => ModifierTypes.NonUsing,
        (<= Modifier.Partial) => ModifierTypes.NonPartial,
        _ => Modifier.None
    };

    public static bool IsModifier(this Modifier modifier, Modifier query) => modifier.IsFlagSet(query);

    public static Modifier SetModifier(this Modifier modifier, Modifier newModifier) => (modifier & newModifier.GetNonModifierType()).SetFlags(newModifier);
    public static Modifier SetModifiers(this Modifier modifier, Modifier newModifier)
    {
        if (newModifier.AccessModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.AccessModifier());
        if (newModifier.StaticModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.StaticModifier());
        if (newModifier.ConstModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.ConstModifier());
        if (newModifier.AbstractModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.AbstractModifier());
        if (newModifier.RecordModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.RecordModifier());
        if (newModifier.MemberTypeModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.MemberTypeModifier());
        if (newModifier.FieldModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.FieldModifier());
        if (modifier.HasFlag(Modifier.Static) && modifier.HasFlag(Modifier.Const)) modifier.ClearFlags(Modifier.Static);
        if (newModifier.PartialModifier() is not Modifier.None) modifier = modifier.SetModifier(newModifier.PartialModifier());
        return modifier;
    }
    public static Modifier SetModifiers(this Modifier modifier, IEnumerable<Modifier> newModifiers)
    {
        foreach (var newModifier in newModifiers) modifier = modifier.SetModifiers(newModifier);
        return modifier;
    }
    public static Modifier Validate(this Modifier modifier)
    {
        if (modifier.HasFlag(Modifier.Static) && modifier.HasFlag(Modifier.Const)) modifier = modifier.ClearFlags(Modifier.Static);
        return modifier;
    }

    public static SyntaxTokenList Syntax(this Modifier modifier)
    {
        modifier = modifier.Validate();
        return TokenList(new SyntaxToken[] {
                UsingModifierToken (modifier), AccessModifierToken (modifier), StaticModifierToken(modifier),
                AbstractModifierToken(modifier), AsyncModifierToken(modifier), ConstModifierToken(modifier),
                RecordModifierToken(modifier), PartialModifierToken(modifier), MemberTypeModifierToken(modifier)
            }.Where(x => x.Kind() is not SyntaxKind.None).ToArray());
    }

    public static bool IsWritable(this Modifier modifier) =>
        !(modifier.HasFlag(Modifier.Readonly) || modifier.HasFlag(Modifier.Const) || modifier.HasFlag(Modifier.Record));

    public static bool IsField(this Modifier modifier) =>
        modifier.IsFlagSet(Modifier.Field) || modifier.IsFlagSet(Modifier.Const) || modifier.IsFlagSet(Modifier.Readonly);
}
