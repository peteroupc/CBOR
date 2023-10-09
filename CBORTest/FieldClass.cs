namespace Test {
#pragma warning disable CS0169
#pragma warning disable CS0414
#pragma warning disable CA1051
#pragma warning disable SA1401
#pragma warning disable SA1307
  public sealed class FieldClass {
    private const int ConstFieldA = 55;
    private static readonly byte[] StaticFieldA = new byte[2];
    public readonly int ReadonlyFieldA = 33;
    private readonly int privateFieldB = 44;
    public int publicFieldA = 66;
    private readonly int privateFieldA = 67;
  }
}
