package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */



    /**
     * Exception thrown for errors involving CBOR data.
     */
  public class CBORException extends RuntimeException {
    /**
     * 
     */
    public CBORException() {
    }
    /**
     * 
     * @param message A string object.
     */
    public CBORException(String message){
 super(message);
    }
    /**
     * 
     * @param message A string object.
     * @param innerException A Exception object.
     */
    public CBORException(String message, Throwable innerException){
 super(message, innerException);
    }
    /**
     * @param context A StreamingContext object.
     */
    private static final long serialVersionUID=1;
  }
