using System.Globalization;
using System.Reflection;

namespace CodeModels.Models.Reflection;

public record ReflectedConstructorInfo(ConstructorInfo Info) : ReflectedMethodBase<ConstructorInfo>(Info), IConstructorInfo
{
    public object Invoke(object[] parameters) => Info.Invoke(parameters);
    public object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        => Info.Invoke(invokeAttr, binder, parameters, culture);
}
