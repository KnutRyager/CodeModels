﻿using System;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    [Flags]
    public enum Modifier
    {
        None = 0,
        // Access
        Private = 1,
        Protected = 2,
        Internal = 4,
        Public = 8,
        // Static
        Static = 16,
        // Member
        Class = 32,
        Struct = 64,
        Enum = 128,
        Interface = 256,
        // Const
        Const = 512,
        Readonly = 1024,
        // Abstract
        Abstract = 2048,
        Virtual = 4096,
        Override = 8192,
        // Record
        Record = 16384,
        // Property type
        Field = 32768,
        Property = 65536,
    }

    public enum ModifierType
    {
        None,
        Access,
        Static,
        Member,
        Const,
        Abstract,
        Record,
        Field,
    }

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
    }

    public static class MethodHolderTypes
    {
        public const Modifier PublicClass = Modifier.Public | Modifier.Class;
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
            _ => Modifier.None
        };

        public static SyntaxToken AbstractModifierToken(this Modifier modifier) => modifier switch
        {
            _ when modifier.HasFlag(Modifier.Abstract) => Token(SyntaxKind.AbstractKeyword),
            _ when modifier.HasFlag(Modifier.Virtual) => Token(SyntaxKind.VirtualKeyword),
            _ when modifier.HasFlag(Modifier.Override) => Token(SyntaxKind.OverrideKeyword),
            _ => Token(SyntaxKind.None)
        };

        public static Modifier RecordModifier(this Modifier modifier) => modifier switch
        {
            _ when modifier.HasFlag(Modifier.Record) => Modifier.Record,
            _ => Modifier.None
        };

        public static SyntaxToken RecordModifierToken(this Modifier modifier) => modifier switch
        {
            _ when modifier.HasFlag(Modifier.Record) => Token(SyntaxKind.RecordDeclaration),
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
            return modifier;
        }
        public static Modifier Validate(this Modifier modifier)
        {
            if (modifier.HasFlag(Modifier.Static) && modifier.HasFlag(Modifier.Const)) modifier = modifier.ClearFlags(Modifier.Static);
            return modifier;
        }

        public static SyntaxTokenList Modifiers(this Modifier modifier)
        {
            modifier = modifier.Validate();
            return TokenList(new SyntaxToken[] {
                AccessModifierToken (modifier), StaticModifierToken(modifier),AbstractModifierToken(modifier),
                ConstModifierToken(modifier), RecordModifierToken(modifier), MemberTypeModifierToken(modifier)
            }.Where(x => x.Kind() is not SyntaxKind.None).ToArray());
        }

        public static bool IsWritable(this Modifier modifier) =>
            !(modifier.HasFlag(Modifier.Readonly) || modifier.HasFlag(Modifier.Const) || modifier.HasFlag(Modifier.Record));

        public static bool IsField(this Modifier modifier) =>
            modifier.IsFlagSet(Modifier.Field) || modifier.IsFlagSet(Modifier.Const) || modifier.IsFlagSet(Modifier.Readonly);
    }
}
