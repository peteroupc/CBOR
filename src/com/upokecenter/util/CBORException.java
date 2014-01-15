package com.upokecenter.util;
/*
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

    /**
     * Exception thrown for errors involving CBOR data.
     */
    public class CBORException extends RuntimeException {
private static final long serialVersionUID=1L;
    /**
     * Not documented yet.
     */
    public CBORException () {
    }
    /**
     * Not documented yet.
     * @param message A string object.
     */
    public CBORException (String message) {
 super(message);
    }
    /**
     * Not documented yet.
     * @param message A string object.
     * @param innerException An Exception object.
     */
    public CBORException (String message, Throwable innerException) {
 super(message, innerException);
    }
  }

