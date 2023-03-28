using System.Collections.Generic;

namespace Common.Interfaces;

public interface ICollectionOf<T>
    where T : class
{
    List<T> AsList(T? typeSpecifier = null);
}
