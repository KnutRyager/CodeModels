﻿using System;
using System.Globalization;
using System.Reflection;
using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models.Reflection;

public record SymbolConstructorInfo(IMethodSymbol Info, IProgramContext Context) : SymbolMethodBase<IMethodSymbol>(Info, Context), IConstructorInfo
{
    public override MemberTypes MemberType => MemberTypes.Constructor;
    public object Invoke(object[] parameters) => throw new NotImplementedException();
        //Info.Invoke(parameters);
    public object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        => throw new NotImplementedException(); 
    //Info.Invoke(invokeAttr, binder, parameters, culture);
}
