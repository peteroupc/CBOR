/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Cbor {
    /// <summary>Implemented by classes that validate CBOR objects belonging to a
    /// specific tag.</summary>
  public interface ICBORTag
  {
    /// <summary>Gets a type filter specifying what kinds of CBOR objects are
    /// supported by this tag.</summary>
    /// <returns>A CBOR type filter.</returns>
    CBORTypeFilter GetTypeFilter();

    /// <summary>Generates a CBOR object based on the data of another object. If the
    /// data is not valid, should throw a CBORException.</summary>
    /// <param name='obj'>A CBOR object with the corresponding tag handled by the
    /// ICBORTag object.</param>
    /// <returns>A CBORObject object. Note that this method may choose to return the
    /// same object as the parameter.</returns>
    CBORObject ValidateObject(CBORObject obj);
  }
}
