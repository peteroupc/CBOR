package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Description of ICBORConverter.
     * @param <T> Type to convert to a CBOR object.
     */
  public interface ICBORConverter<T>
  {
    CBORObject ToCBORObject(T obj);
  }
