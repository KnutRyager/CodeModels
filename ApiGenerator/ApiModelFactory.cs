namespace ApiGenerator;

using System.Linq;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Common.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static CodeModels.Factory.CodeModelFactory;

public static class ApiModelFactory
{
    public static ClassDeclaration DbContext(string Name, IEnumerable<ModelModel> models) => Class($"{Name}DbContext",
        members:
        models.Select(x => Property(Type($"DbSet<{x.Name}>"), x.Name, modifier: Modifier.Public))
        .Concat(new IMember[] {
            Constructor(),
            Constructor(
                Param<DbContextOptions>("options"),
                initializer: BaseConstructorInitializer(Identifier("options"))),
            Method("OnConfiguring", Param<DbContextOptionsBuilder>("optionsBuilder"), TypeShorthands.VoidType,
                Block(If(
                    Not(Identifier<DbContextOptionsBuilder>("optionsBuilder").GetPropertyAccess(x => x.IsConfigured)),
                    Statement(Identifier<DbContextOptionsBuilder>("optionsBuilder").GetInvocation(x => x.UseSqlServer(null),Literal("dbString") ))
                    )), modifier: Modifier.Protected|Modifier.Override),
            Method("OnModelCreating", Param<ModelBuilder>(), TypeShorthands.VoidType,
                Block(ForEach("relationship",Identifier<ModelBuilder>()
                    .GetModel(modelBuilder => modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())),
                    Block(
                        Assignment(Identifier<IMutableForeignKey>("relationship").GetPropertyAccess(x => x.DeleteBehavior),
                            StaticIdentifier<DeleteBehavior>().Access(x => DeleteBehavior.Cascade)))),
                    Base().GetInvocation<DbContext>("OnModelCreating", Identifier<ModelBuilder>())),
                modifier: Modifier.Protected|Modifier.Override),
        }),
        baseTypeList: SimpleBases<DbContext>());

    public static ClassDeclaration Service(ModelModel Model, ClassDeclaration dbContext) => Class($"{Model.Name}Service",
        new IMember[] {
            Property(dbContext.Get_Type(), dbContext.Name, new[]{ Accessor(AccessorType.Get), Accessor(AccessorType.Init) }, modifier: Modifier.Private),
            Constructor(
                Param(Type(dbContext)), Block(Assignment(Identifier(dbContext.Get_Type().Name), Identifier(dbContext.Get_Type())))),
            Method("Get", Param<int>("id"), Type(Model.Name),
                Identifier(dbContext.Name).Access(Identifier(Model.Name)).GetInvocation(x => Enumerable.First(T<IEnumerable<object>>(), T<Func<object, bool>>()),
                    new []{
                        Lambda(Identifier("x"), Type<int>(), Equal(MemberAccess("x", "Id"), Identifier("id")))
                    }))
        });
}