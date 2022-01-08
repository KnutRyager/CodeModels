using Models;

namespace TestRoslynSourceGenerator.Models
{
    [Model]
    public class ModelA
    {
        public int A_A => 1;
        public int A_B => A_A;
    }
}