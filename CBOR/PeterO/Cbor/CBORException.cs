/*
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  /// <summary>Exception thrown for errors involving CBOR data.
  /// <para>This library may throw exceptions of this type in certain
  /// cases, notably when errors occur, and may supply messages to those
  /// exceptions (the message can be accessed through the <c>Message</c>
  /// property in.NET or the <c>getMessage()</c> method in Java). These
  /// messages are intended to be read by humans to help diagnose the
  /// error (or other cause of the exception); they are not intended to
  /// be parsed by computer programs, and the exact text of the messages
  /// may change at any time between versions of this
  /// library.</para></summary>
#if NET20 || NET40
[Serializable]
#endif
public sealed class CBORException : Exception {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBORException'/> class.</summary>
    public CBORException() {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBORException'/> class.</summary>
    /// <param name='message'>The parameter <paramref name='message'/> is a
    /// text string.</param>
    public CBORException(string message) : base(message) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBORException'/> class. Uses the given
    /// message and inner exception.</summary>
    /// <param name='message'>The parameter <paramref name='message'/> is a
    /// text string.</param>
    /// <param name='innerException'>The parameter <paramref
    /// name='innerException'/> is an Exception object.</param>
    public CBORException(string message, Exception innerException)
      : base(message, innerException) {
    }

    #if NET20 || NET40
    private CBORException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context) {
    }
    #endif
  }
}
