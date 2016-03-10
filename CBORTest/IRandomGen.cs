using System;

namespace PeterO {
 public interface IRandomGen {
    int GetBytes(byte[] bytes, int offset, int length);
  }
}
