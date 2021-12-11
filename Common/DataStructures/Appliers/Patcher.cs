using System;
using System.Linq;
using Common.Reflection;
using static System.Linq.Expressions.Expression;

namespace Common.DataStructures
{
    /// <summary>
    /// The patch allows patching the values of one object onto another,
    /// overwriting only non-default value properties.
    /// It precompiles getters/setters for better performance than live reflection.
    /// </summary>
    public class Patcher<T> where T : class
    {
        private readonly Action<T, T> _patchAction;

        public Patcher() => _patchAction = MakePatchAction();

        public T Apply(T patch, T patchee)
        {
            _patchAction(patch, patchee);
            return patchee;
        }

        private static Action<T, T> MakePatchAction()
        {
            var patch = Parameter(typeof(T), "patch");
            var patchee = Parameter(typeof(T), "patchee");
            // if (patch.property != defaultValue) patchee.property = patch.property;
            return Lambda<Action<T, T>>(Block(ReflectionUtil.GetReadWriteProperties(typeof(T)).Select(property =>
                IfThen(
                    NotEqual(PropertyOrField(patch, property.Name),
                        Constant(ReflectionUtil.GetDefault(property.PropertyType), property.PropertyType)),
                    Assign(PropertyOrField(patchee, property.Name), PropertyOrField(patch, property.Name)))
                )), patch, patchee).Compile();
        }
    }
}