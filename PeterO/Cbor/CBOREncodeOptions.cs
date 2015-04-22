using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeterO.Cbor {
    /// <summary>Specifies options for encoding CBOR objects to bytes.</summary>
  [Flags]
  public enum CBOREncodeOptions {
    /// <summary>No special options for encoding.</summary>
    None = 0,

    /// <summary>Always encode strings with a definite-length encoding.</summary>
    NoIndefLengthStrings = 1
  }
}
