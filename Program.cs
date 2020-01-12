using BenchmarkDotNet.Running;

namespace TackleBigONetCore
{
  class Program
  {
    static void Main()
    {
      BenchmarkRunner.Run<BigOBenchmarks>();
    }
  }
}
