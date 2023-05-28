using TestCommon;

namespace ApiGenerator.Test;

public class DbContextTests
{
    [Fact]
    public void Empty()
    {
        ApiModelFactory.DbContext("Rest").CodeModelEqual("""
public class RestDbContext : DbContext
{
    public RestDbContext()
    {
    }

    public RestDbContext(DbContextOptions options) : base(options)
    {
    }
}
""");
    }
}