package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

    /**
     * Specifies what kinds of CBOR objects a tag can be. This class is used when a
     * CBOR object is being read from a data stream. This class cannot be
     * inherited; this is a change in version 2.0 from previous versions,
     * where the class was inadvertently left inheritable.
     */
  public final class CBORTypeFilter {

    private boolean any;
    private int types;
    private boolean floatingpoint;
    private int arrayLength;
    private boolean anyArrayLength;
    private boolean arrayMinLength;
    private CBORTypeFilter[] elements;
    private BigInteger[] tags;

    private CBORTypeFilter Copy() {
      CBORTypeFilter filter = new CBORTypeFilter();
      filter.any = this.any;
      filter.types = this.types;
      filter.floatingpoint = this.floatingpoint;
      filter.arrayLength = this.arrayLength;
      filter.anyArrayLength = this.anyArrayLength;
      filter.arrayMinLength = this.arrayMinLength;
      filter.elements = this.elements;
      filter.tags = this.tags;
      return filter;
    }

    private CBORTypeFilter WithAny() {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.any = true;
      filter.anyArrayLength = true;
      filter.types = 0xff;
      return filter;
    }

    private CBORTypeFilter WithType(int type) {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << type;
      return filter;
    }

    /**
     * Copies this filter and includes unsigned integers in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithUnsignedInteger() {
      return this.WithType(0);
    }

    /**
     * Copies this filter and includes negative integers in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithNegativeInteger() {
      return this.WithType(1);
    }

    /**
     * Copies this filter and includes byte strings in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithByteString() {
      return this.WithType(2).WithTags(25);
    }

    /**
     * Copies this filter and includes maps in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithMap() {
      return this.WithType(5);
    }

    /**
     * Copies this filter and includes text strings in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithTextString() {
      return this.WithType(3).WithTags(25);
    }

    /**
     * Not documented yet.
     * @param tags An integer array of tags allowed.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithTags(int... tags) {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;  // Always include the "tag" major type
      int startIndex = 0;
      if (filter.tags != null) {
        BigInteger[] newTags = new BigInteger[tags.length + filter.tags.length];
        System.arraycopy(filter.tags, 0, newTags, 0, filter.tags.length);
        startIndex = filter.tags.length;
        filter.tags = newTags;
      } else {
        filter.tags = new BigInteger[tags.length];
      }
      for (int i = 0; i < tags.length; ++i) {
        filter.tags[startIndex + i] = BigInteger.valueOf(tags[i]);
      }
      return filter;
    }

    /**
     * Not documented yet.
     * @param tags A BigInteger[] object.
     * @return A CBORTypeFilter object.
     * @throws NullPointerException The parameter "tags[i]" is null.
     */
    public CBORTypeFilter WithTags(BigInteger... tags) {
      if (this.any) {
        return this;
      }
      for (int i = 0; i < tags.length; ++i) {
        if (tags[i] == null) {
          throw new NullPointerException("tags");
        }
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;  // Always include the "tag" major type
      int startIndex = 0;
      if (filter.tags != null) {
        BigInteger[] newTags = new BigInteger[tags.length + filter.tags.length];
        System.arraycopy(filter.tags, 0, newTags, 0, filter.tags.length);
        startIndex = filter.tags.length;
        filter.tags = newTags;
      } else {
        filter.tags = new BigInteger[tags.length];
      }
      System.arraycopy(tags, 0, filter.tags, startIndex, tags.length);
      return filter;
    }

    /**
     * Copies this filter and includes CBOR arrays with an exact length to the new
     * filter.
     * @param arrayLength The desired maximum length of an array.
     * @param elements An array containing the allowed types for each element in
     * the array. There must be at least as many elements here as given in
     * the arrayLength parameter.
     * @return A CBORTypeFilter object.
     * @throws IllegalArgumentException The parameter arrayLength is less than 0.
     * @throws NullPointerException The parameter elements is null.
     * @throws IllegalArgumentException The parameter elements has fewer elements than
     * specified in arrayLength.
     */
    public CBORTypeFilter WithArrayExactLength(
int arrayLength,
CBORTypeFilter... elements) {
      if (this.any) {
        return this;
      }
      if (elements == null) {
        throw new NullPointerException("elements");
      }
      if (arrayLength < 0) {
        throw new IllegalArgumentException("arrayLength (" + arrayLength +
          ") is less than " + "0");
      }
      if (arrayLength < elements.length) {
        throw new IllegalArgumentException("arrayLength (" + arrayLength +
          ") is less than " + elements.length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.arrayLength = arrayLength;
      filter.arrayMinLength = false;
      filter.elements = new CBORTypeFilter[elements.length];
      System.arraycopy(elements, 0, filter.elements, 0, elements.length);
      return filter;
    }

    /**
     * Copies this filter and includes CBOR arrays with at least a given length to
     * the new filter.
     * @param arrayLength The desired minimum length of an array.
     * @param elements An array containing the allowed types for each element in
     * the array. There must be at least as many elements here as given in
     * the arrayLength parameter.
     * @return A CBORTypeFilter object.
     * @throws IllegalArgumentException The parameter arrayLength is less than 0.
     * @throws NullPointerException The parameter elements is null.
     * @throws IllegalArgumentException The parameter elements has fewer elements than
     * specified in arrayLength.
     */
    public CBORTypeFilter WithArrayMinLength(
int arrayLength,
CBORTypeFilter... elements) {
      if (this.any) {
        return this;
      }
      if (elements == null) {
        throw new NullPointerException("elements");
      }
      if (arrayLength < 0) {
        throw new IllegalArgumentException("arrayLength (" + arrayLength +
          ") is less than " + "0");
      }
      if (arrayLength < elements.length) {
        throw new IllegalArgumentException("arrayLength (" + arrayLength +
          ") is less than " + elements.length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.arrayLength = arrayLength;
      filter.arrayMinLength = true;
      filter.elements = new CBORTypeFilter[elements.length];
      System.arraycopy(elements, 0, filter.elements, 0, elements.length);
      return filter;
    }

    /**
     * Copies this filter and includes arrays of any length in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithArrayAnyLength() {
      if (this.any) {
        return this;
      }
      if (this.arrayLength < 0) {
        throw new IllegalArgumentException("this.arrayLength (" + this.arrayLength +
          ") is less than " + "0");
      }
      if (this.arrayLength < this.elements.length) {
        throw new IllegalArgumentException("this.arrayLength (" + this.arrayLength +
          ") is less than " + this.elements.length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.anyArrayLength = true;
      return filter;
    }

    /**
     * Copies this filter and includes floating-point numbers in the new filter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithFloatingPoint() {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.floatingpoint = true;
      return filter;
    }

    /**
     * Not documented yet.
     * @param type A 32-bit signed integer.
     * @return A Boolean object.
     */
    public boolean MajorTypeMatches(int type) {
      return type >= 0 && type <= 7 && (this.types & (1 << type)) != 0;
    }

    /**
     * Returns whether an array's length is allowed under this filter.
     * @param length The length of a CBOR array.
     * @return True if this filter allows CBOR arrays and an array's length is
     * allowed under this filter; otherwise, false.
     */
    public boolean ArrayLengthMatches(int length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                                              (this.arrayMinLength ?
                                                this.arrayLength >= length :
                                                this.arrayLength ==
                                                length));
    }

    /**
     * Returns whether an array's length is allowed under a filter.
     * @param length The length of a CBOR array.
     * @return True if this filter allows CBOR arrays and an array's length is
     * allowed under a filter; otherwise, false.
     */
    public boolean ArrayLengthMatches(long length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                                              (this.arrayMinLength ?
                                                this.arrayLength >= length :
                                                this.arrayLength ==
                                                length));
    }

    /**
     * Returns whether an array's length is allowed under a filter.
     * @param bigLength A BigInteger object.
     * @return True if this filter allows CBOR arrays and an array's length is
     * allowed under a filter; otherwise, false.
     * @throws NullPointerException The parameter {@code bigLength} is null.
     */
    public boolean ArrayLengthMatches(BigInteger bigLength) {
      if (bigLength == null) {
        throw new NullPointerException("bigLength");
      }
      return ((this.types & (1 << 4)) == 0) && (this.anyArrayLength ||
        ((!this.arrayMinLength &&
        bigLength.compareTo(BigInteger.valueOf(this.arrayLength)) == 0) ||
        (this.arrayMinLength &&
        bigLength.compareTo(BigInteger.valueOf(this.arrayLength)) >= 0)));
    }

    /**
     * Gets a value indicating whether CBOR objects can have the given tag number.
     * @param tag A tag number. Returns false if this is less than 0.
     * @return True if CBOR objects can have the given tag number; otherwise,
     * false.
     */
    public boolean TagAllowed(int tag) {
      return this.any || this.TagAllowed(BigInteger.valueOf(tag));
    }

    /**
     * Gets a value indicating whether CBOR objects can have the given tag number.
     * @param tag A tag number. Returns false if this is less than 0.
     * @return True if CBOR objects can have the given tag number; otherwise,
     * false.
     */
    public boolean TagAllowed(long tag) {
      return this.any || this.TagAllowed(BigInteger.valueOf(tag));
    }

    /**
     * Gets a value indicating whether CBOR objects can have the given tag number.
     * @param bigTag A tag number. Returns false if this is less than 0.
     * @return True if CBOR objects can have the given tag number; otherwise,
     * false.
     * @throws NullPointerException The parameter {@code bigTag} is null.
     */
    public boolean TagAllowed(BigInteger bigTag) {
      if (bigTag == null) {
        throw new NullPointerException("bigTag");
      }
      if (bigTag.signum() < 0) {
        return false;
      }
      if (this.any) {
        return true;
      }
      if ((this.types & (1 << 6)) == 0) {
        return false;
      }
      if (this.tags == null) {
        return true;
      }
      for (BigInteger tag : this.tags) {
        if (bigTag.equals(tag)) {
          return true;
        }
      }
      return false;
    }

    /**
     * Determines whether this type filter allows CBOR arrays and the given array
     * index is allowed under this type filter.
     * @param index An array index, starting from 0.
     * @return True if this type filter allows CBOR arrays and the given array
     * index is allowed under this type filter; otherwise, false.
     */
    public boolean ArrayIndexAllowed(int index) {
   return (this.types & (1 << 4)) != 0 && index >= 0 && (this.anyArrayLength ||

            ((this.arrayMinLength || index < this.arrayLength) && index >=
                                                0));
    }

    /**
     * Not documented yet.
     * @param index A 32-bit signed integer.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetSubFilter(int index) {
      if (this.anyArrayLength || this.any) {
        return Any;
      }
      if (index < 0) {
        return None;
      }
      if (index >= this.arrayLength) {
        // Index is out of bounds
        return this.arrayMinLength ? Any : None;
      }
      if (this.elements == null) {
        return Any;
      }
      if (index >= this.elements.length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      return this.elements[index];
    }

    /**
     * Not documented yet.
     * @param index A 64-bit signed integer.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetSubFilter(long index) {
      if (this.anyArrayLength || this.any) {
        return Any;
      }
      if (index < 0) {
        return None;
      }
      if (index >= this.arrayLength) {
        // Index is out of bounds
        return this.arrayMinLength ? Any : None;
      }
      if (this.elements == null) {
        return Any;
      }
      // NOTE: Index shouldn't be greater than Integer.MAX_VALUE,
      // since the length is an int
      if (index >= this.elements.length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      return this.elements[(int)index];
    }

    /**
     * Returns whether this filter allows simple values that are not floating-point
     * numbers.
     * @return True if this filter allows simple values that are not floating-point
     * numbers; otherwise, false.
     */
    public boolean NonFPSimpleValueAllowed() {
      return this.MajorTypeMatches(7) && !this.floatingpoint;
    }

    /**
     * A filter that allows no CBOR types.
     */
    public static final CBORTypeFilter None = new CBORTypeFilter();

    /**
     * A filter that allows unsigned integers.
     */
    public static final CBORTypeFilter UnsignedInteger = new
      CBORTypeFilter().WithUnsignedInteger();

    /**
     * A filter that allows negative integers.
     */
    public static final CBORTypeFilter NegativeInteger = new
      CBORTypeFilter().WithNegativeInteger();

    /**
     * A filter that allows any CBOR object.
     */
    public static final CBORTypeFilter Any = new CBORTypeFilter().WithAny();

    /**
     * A filter that allows byte strings.
     */
    public static final CBORTypeFilter ByteString = new
      CBORTypeFilter().WithByteString();

    /**
     * A filter that allows text strings.
     */
    public static final CBORTypeFilter TextString = new
      CBORTypeFilter().WithTextString();
  }
