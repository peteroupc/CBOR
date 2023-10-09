namespace PeterO {
  /// <summary>Interface for random-number generators.</summary>
  public interface IRandomGen
  {
    /// <summary>Randomly generates a set of bytes.</summary>
    /// <param name='bytes'>Byte buffer to store the random bytes.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='bytes'/> begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <returns>Number of bytes returned.</returns>
    int GetBytes(byte[] bytes, int offset, int length);
  }
}
