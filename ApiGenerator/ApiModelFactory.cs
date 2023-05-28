namespace ApiGenerator;

using CodeModels.Models.Primitives.Member;
using Microsoft.EntityFrameworkCore;

using static CodeModels.Factory.CodeModelFactory;

public class ApiModelFactory
{
    public static ClassDeclaration DbContext(string Name) => Class($"{Name}DbContext",
            members: new[] {
                Constructor(),
                Constructor(
                    Param<DbContextOptions>("options"),
                    initializer: BaseConstructorInitializer(Identifier("options"))) },
            baseTypeList: new[] { SimpleBase<DbContext>() });
}