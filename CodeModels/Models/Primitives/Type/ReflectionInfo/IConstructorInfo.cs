using System.Globalization;
using System.Reflection;

namespace CodeModels.Models.Reflection;

public interface IConstructorInfo : IMethodBase
{
    object Invoke(object[] parameters);
    abstract object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);
}
