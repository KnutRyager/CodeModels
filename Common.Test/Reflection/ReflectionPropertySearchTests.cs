using Common.Reflection;
using Common.Reflection.Member;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Reflection;

public class ReflectionPropertySearchTests
{
    [Fact]
    public void FindPath_ObjectToString()
        => ReflectionPropertySearch.FindPath(typeof(object), typeof(string))
        .Should().BeEquivalentTo(new List<IMemberAccess>() {
            MemberAccessFactory.Create(ExpressionReflectionUtil.GetMethodInfo(x => x.ToString)) },
            options => options.RespectingRuntimeTypes());

    [Fact]
    public void FindPath_Properties_ABC()
        => ReflectionPropertySearch.FindPath(typeof(A), typeof(C))
        .Should().BeEquivalentTo(new List<IMemberAccess>() {
            MemberAccessFactory.Create(ExpressionReflectionUtil.GetPropertyInfo<A>(x => x.B).GetMethod),
            MemberAccessFactory.Create(ExpressionReflectionUtil.GetPropertyInfo<B>(x => x.C).GetMethod) },
            options => options.RespectingRuntimeTypes());

    [Fact]
    public void FindPath_Interface()
        => ReflectionPropertySearch.FindPath(typeof(Garage), typeof(ICar))
        .Should().BeEquivalentTo(new List<IMemberAccess>() {
            MemberAccessFactory.Create(ExpressionReflectionUtil.GetPropertyInfo<Garage>(x => x.Car).GetMethod) },
            options => options.RespectingRuntimeTypes());

    [Fact]
    public void FindPath_Interface_General()
        => ReflectionPropertySearch.FindPath(typeof(Garage), typeof(IVehicle))
        .Should().BeEquivalentTo(new List<IMemberAccess>() {
            MemberAccessFactory.Create(ExpressionReflectionUtil.GetPropertyInfo<Garage>(x => x.Car).GetMethod) },
            options => options.RespectingRuntimeTypes());

    [Fact]
    public void FindPath_Interface_Illegal_General()
        => ReflectionPropertySearch.FindPath(typeof(VehicleStore), typeof(IBoat))
        .Should().BeNull();
}

file record A(B B);
file record B(A A, B TheB, C C);
file record C(int I);


file interface IVehicle { }
file interface ICar : IVehicle { }
file interface IBoat : IVehicle { }
file record Garage(ICar Car);
file record Harbor(IBoat Boat);
file record VehicleStore(IVehicle Vehicle);
