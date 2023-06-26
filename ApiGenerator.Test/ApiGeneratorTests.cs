using Microsoft.EntityFrameworkCore;
using TestCommon;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

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
        var model = new ModelModel("A", NamedValues(NamedValue<int>("P1")));
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
}

public class A
{
    public int Id { get; set; }
}

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