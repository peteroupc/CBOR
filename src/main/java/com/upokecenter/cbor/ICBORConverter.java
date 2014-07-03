package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Interface implemented by classes that convert objects of arbitrary types to
     * CBOR objects.
     * @param <T> Type to convert to a CBOR object.
     */
  public interface ICBORConverter<T> {

    /**
     * Converts an object to a CBOR object.
     * @param obj An object to convert to a CBOR object.
     * @return A CBOR object.
     */
    CBORObject ToCBORObject(T obj);
  }
