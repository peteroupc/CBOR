/*
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
namespace PeterO {
    /// <summary> Exception thrown for errors involving CBOR data. </summary>
    public partial class CBORException : Exception {
    /// <summary> </summary>
    public CBORException() {
    }
    /// <summary> </summary>
    /// <param name='message'> A string object.</param>
    public CBORException(string message)
      : base(message) {
    }
    /// <summary> </summary>
    /// <param name='message'> A string object.</param>
    /// <param name='innerException'> A Exception object.</param>
    public CBORException(string message, Exception innerException)
      : base(message, innerException) {
    }
  }
}