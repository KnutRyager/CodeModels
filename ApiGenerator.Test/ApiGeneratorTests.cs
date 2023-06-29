using System.Net;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using TestCommon;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.AspNetCore.Mvc;

namespace ApiGenerator.Test;

public class ApiGeneratorTests
{
    [Fact]
    public void DbContext()
    {
        ApiModelFactory.DbContext("Rest", new[] { ModelModel.Create("A") }).CodeModelEqual("""
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
            optionsBuilder.UseSqlServer("dbString");
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
""");

    }
    [Fact]
    public void Service()
    {
        var model = ApiModelFactory.Model("A", NamedValues(NamedValue<int>("P1")));
        ApiModelFactory.Service(model, ApiModelFactory.DbContext("Rest", new[] { model })).CodeModelEqual("""
public class AService
{
    private RestDbContext RestDbContext { get; init; }

    public AService(RestDbContext restDbContext)
    {
        RestDbContext = restDbContext;
    }

    public A Get(int id) => RestDbContext.A.First(x => x.Id == id);
}
""");
    }

    [Fact]
    public void Dto()
    {
        var model = ApiModelFactory.Model("A", NamedValues(NamedValue<int>("P1")));
        ApiModelFactory.Dto(model).CodeModelEqual("""
public record ADto(int P1);
""");
    }

    [Fact]
    public void Controller()
    {
        var model = ApiModelFactory.Model("A", NamedValues(NamedValue<int>("P1")));
        var dto = ApiModelFactory.Dto(model);
        var service = ApiModelFactory.Service(model, ApiModelFactory.DbContext("Rest", new[] { model }));
        ApiModelFactory.Controller(model, service).CodeModelEqual("""
public static class AController
{
    [ProducesResponseType(typeof(ADto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Exception), (int)HttpStatusCode.BadRequest)]
    public static ActionResult<ADto> Get([FromServices] AService api, [FromServices] AMapper mapper, int id) => mapper.Map(api.Get(id));
}
""");
    }
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

public static class AController
{
    [ProducesResponseType(typeof(ADto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Exception), (int)HttpStatusCode.BadRequest)]
    public static ActionResult<ADto> Get([FromServices] AService api, [FromServices] AMapper mapper, int id) => mapper.Map(api.Get(id));
}