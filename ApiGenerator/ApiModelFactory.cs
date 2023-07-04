namespace ApiGenerator;

using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using ApiGenerator.Settings;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Common.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static CodeModels.Factory.CodeModelFactory;

public static class ApiModelFactory
{
    public static ModelModel Model(string name, NamedValueCollection? properties = null)
        => ModelModel.Create(name, properties);

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

    public static ClassDeclaration Service(ModelModel Model, ClassDeclaration dbContext, RestApiSettings? apiSettings = null)
    {
        apiSettings ??= RestApiSettings.Default;
        var settings = apiSettings.GetSettings<ServiceSettings>() ?? ServiceSettings.Default;
        return Class($"{Model.Name}{settings.NamePrefix}",
        new IMember?[] {
            Property(dbContext.Get_Type(), dbContext.Name, Accessors(Accessor(AccessorType.Get), Accessor(AccessorType.Init)), modifier: Modifier.Private),
            Constructor(
                Param(Type(dbContext)), Block(Assignment(Identifier(dbContext.Get_Type().Name), Identifier(dbContext.Get_Type())))),
           settings is { IncludeGet: BoolSetting.True or BoolSetting.Auto }
           ? Method("Get", Param<int>("id"), Type(Model.Name),
                Identifier(dbContext.Name).Access(Identifier(Model.Name)).GetInvocation(x => Enumerable.First(T<IEnumerable<object>>(), T<Func<object, bool>>()),
                    new []{
                        Lambda(Identifier("x"), Type<int>(), Equal(MemberAccess("x", "Id"), Identifier("id")))
                    }))
            : null
        });
    }

    public static RecordDeclaration Dto(ModelModel model) => Record($"{model.Name}Dto",
        members: model.Properties.ToConstructorProperties());

    public static ClassDeclaration Mapper(ModelModel model, RecordDeclaration dto) => Class($"{model.Name}Mapper",
        members: new IMember[] { Method("Map", dto.ToType(), Identifier("x")) });

    public static ClassDeclaration Controller(ModelModel model, ClassDeclaration service, RestApiSettings? apiSettings = null)
    {
        apiSettings ??= RestApiSettings.Default;
        var controllerSettings = apiSettings.GetSettings<ControllerSettings>() ?? ControllerSettings.Default;
        return Class($"{model.Name}{controllerSettings.NamePrefix}",
            members: new[] {
                RestMethod("Get", model.ToType(), service, Mapper(model, Dto(model)), apiSettings)
            },
        modifier: TypeDeclarationTypes.PublicStaticClass);
    }

    public static Method RestMethod(string name, IType type, ClassDeclaration service, ClassDeclaration mapper, RestApiSettings? apiSettings = null)
    {
        apiSettings ??= RestApiSettings.Default;
        var controllerSettings = apiSettings.GetSettings<ControllerSettings>() ?? ControllerSettings.Default;
        var settings = apiSettings.GetSettings<RestEndpointSettings>() ?? RestEndpointSettings.Default;
        var isStatic = settings.IsStatic.ReadBool(controllerSettings.InstanceType is InstanceType.Static);
        return Method(name,
        ParamList(
            Param("api", service.ToType(), attributes: Attribute<FromServicesAttribute>()),
            Param("mapper", mapper.ToType(), attributes: Attribute<FromServicesAttribute>()),
            Param("id", Type<int>())
            ), GenericType<ActionResult>(mapper.GetMemberType("Map")),
        body: Identifier("mapper").GetInvocation((mapper.GetMethod("Map") as Method)!,
            Identifier("api").GetInvocation((service.GetMethod(name) as Method)!, Identifier<int>("id"))),
        attributes: AttributesList(
            Attribute<ProducesResponseTypeAttribute>(TypeOf<ADto>(), Cast<int>(Expression(x => HttpStatusCode.OK))),
            settings is { IncludeErrorAnnotation: BoolSetting.True or BoolSetting.Auto }
                ? Attribute<ProducesResponseTypeAttribute>(TypeOf<Exception>(), Cast<int>(Expression(x => HttpStatusCode.BadRequest))) : null),
        modifier: isStatic ? MethodTypes.PublicStatic : MethodTypes.PublicInstance);
    }
}

public static class AController
{
    [ProducesResponseType(typeof(ADto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Exception), (int)HttpStatusCode.BadRequest)]
    public static ActionResult<ADto> Get([FromServices] AService api, [FromServices] AMapper mapper, int id) => mapper.Map(api.Get(id));
}
public record A(int Id);
public record ADto(int Id);

public class RestDbContext : DbContext
{
    public DbSet<A> A { get; set; }

    public RestDbContext()
    {
    }

    public RestDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=SemBot;Trusted_Connection=True;Encrypt=False;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }

        base.OnModelCreating(modelBuilder);
    }
}

public class AService
{
    private RestDbContext DbContext { get; init; }

    AService(RestDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public A Get(int id) => DbContext.A.First(x => x.Id == id);
}

public class AMapper
{
    public ADto Map(A a) => new(a.Id);
}