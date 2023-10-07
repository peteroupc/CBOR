namespace PeterO {
  public interface IRandomGenExtended : IRandomGen
  {
    int GetInt32(int maxExclusive);

    long GetInt64(long maxExclusive);
  }
}
