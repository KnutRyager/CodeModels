using Common.Reflection;
using Common.Util;
using System.ComponentModel;
using Xunit;

namespace Common.Tests
{
    public class ExpressionUtilTests
    {

        public class A
        {
            public string Prop { get; set; }
            public string Method() => "1337";
            public string MethodOfArg(string input) => input;
        }

        [Fact] public void GetPropertyFromExpression() => Assert.Equal("test", ExpressionUtil.GetPropertyFromExpression<A, string>(x => x.Prop).GetValue(new A() { Prop = "test" }));
        [Fact] public void GetMethodFromExpression() => Assert.Equal(typeof(A).GetMethod(nameof(A.Method)), ExpressionUtil.GetMethodFromExpression<A, string>(x => x.Method()));
        [Fact] public void GetMethodFromExpressionWithArg() => Assert.Equal(typeof(A).GetMethod(nameof(A.MethodOfArg)), ExpressionUtil.GetMethodFromExpression<A, string>(x => x.MethodOfArg(default)));
    }
}
