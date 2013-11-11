/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Runtime.Serialization;

namespace PeterO {
  /// <summary>
  /// Exception thrown for errors involving CBOR data.
  /// </summary>
  public class CBORException : Exception, ISerializable {
    public CBORException() {
    }

    public CBORException(string message)
      : base(message) {
    }

    public CBORException(string message, Exception innerException)
      : base(message, innerException) {
    }

    // This is needed for serialization.
    protected CBORException(SerializationInfo info, StreamingContext context)
      : base(info, context) {
    }
  }
}