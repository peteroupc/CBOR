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
    [Serializable]
    public partial class CBORException : ISerializable {
    /// <summary> </summary>
    /// <param name='info'> A SerializationInfo object.</param>
    /// <param name='context'> A StreamingContext object.</param>
    protected CBORException(SerializationInfo info, StreamingContext context)
      : base(info, context) {
    }
  }
}