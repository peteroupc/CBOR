using System;

namespace PeterO {
    /// <summary>Interface for random-number generators.</summary>
 public interface IRandomGen {
    /// <summary>Randomly generates a set of bytes.</summary>
    /// <returns>Number of bytes returned.</returns>
    int GetBytes(byte[] bytes, int offset, int length);
  }
}
