using System.Collections.Generic;
using System.Linq;

namespace Common.Interfaces;

public interface IConvertibleCollectionOf<T, TTo>
    : ICollectionOf<T>
    where T : class, IConvertibleTo<TTo>
    where TTo : class
{
    //List<TTo> ConvertToList(TTo? typeSpecifier = null)
    //    => AsList().Select(x => x.As()).ToList();
}