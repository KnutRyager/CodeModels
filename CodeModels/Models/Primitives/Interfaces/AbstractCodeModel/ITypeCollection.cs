using System.Collections.Generic;
using Common.Interfaces;

namespace CodeModels.Models;

public interface ITypeCollection :
    ICollectionOf<IType>
{
    /// <summary>
    /// Most general type.
    /// </summary>
    IType BaseType();
}
