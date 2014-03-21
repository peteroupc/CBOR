package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 1:47 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import com.upokecenter.util.*;

    /**
     * Description of CBORTypeFilter.
     */
  public class CBORTypeFilter
  {
    private boolean any;
    private int types;
    private boolean floatingpoint;
    private int arrayLength;
    private boolean anyArrayLength;
    private boolean arrayMinLength;
    private CBORTypeFilter[] elements;
    private BigInteger[] tags;

    public CBORTypeFilter () {
    }

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
      filter.types = 0xFF;
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
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithUnsignedInteger() {
      return this.WithType(0);
    }

    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithNegativeInteger() {
      return this.WithType(1);
    }

    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithByteString() {
      return this.WithType(2).WithTags(25);
    }

    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithMap() {
      return this.WithType(5);
    }

    /**
     * Not documented yet.
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
      filter.types |= 1 << 6;
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
     */
    public CBORTypeFilter WithTags(BigInteger... tags) {
      if (this.any) {
        return this;
      }
      for (int i = 0; i < tags.length; ++i) {
        if (tags[i] == null) {
          throw new NullPointerException("tags[i]");
        }
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;
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
     * Not documented yet.
     * @param arrayLength A 32-bit signed integer.
     * @param elements An array of CBORTypeFilter.
     * @return A CBORTypeFilter object.
     * @deprecated Use WithArrayExactLength instead.
 */
@Deprecated
    public CBORTypeFilter WithArray(int arrayLength, CBORTypeFilter... elements) {
      return this.WithArrayExactLength(arrayLength, elements);
    }

    /**
     * Not documented yet.
     * @param arrayLength A 32-bit signed integer.
     * @param elements An array of CBORTypeFilter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithArrayExactLength(int arrayLength, CBORTypeFilter... elements) {
      if (this.any) {
        return this;
      }
      if (arrayLength < 0) {
        throw new IllegalArgumentException("arrayLength (" + Long.toString((long)arrayLength) + ") is less than " + "0");
      }
      if (arrayLength < elements.length) {
        throw new IllegalArgumentException("arrayLength (" + Long.toString((long)arrayLength) + ") is less than " + Long.toString((long)elements.length));
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
     * Not documented yet.
     * @param arrayLength A 32-bit signed integer.
     * @param elements An array of CBORTypeFilter.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithArrayMinLength(int arrayLength, CBORTypeFilter... elements) {
      if (this.any) {
        return this;
      }
      if (arrayLength < 0) {
        throw new IllegalArgumentException("arrayLength (" + Long.toString((long)arrayLength) + ") is less than " + "0");
      }
      if (arrayLength < elements.length) {
        throw new IllegalArgumentException("arrayLength (" + Long.toString((long)arrayLength) + ") is less than " + Long.toString((long)elements.length));
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
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter WithArrayAnyLength() {
      if (this.any) {
        return this;
      }
      if (this.arrayLength < 0) {
        throw new IllegalArgumentException("this.arrayLength (" + Long.toString((long)this.arrayLength) + ") is less than " + "0");
      }
      if (this.arrayLength < this.elements.length) {
        throw new IllegalArgumentException("this.arrayLength (" + Long.toString((long)this.arrayLength) + ") is less than " + Long.toString((long)this.elements.length));
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.anyArrayLength = true;
      return filter;
    }

    /**
     * Not documented yet.
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
     * Not documented yet.
     * @param length A 32-bit signed integer.
     * @return A Boolean object.
     */
    public boolean ArrayLengthMatches(int length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                                              (this.arrayMinLength ? this.arrayLength >= length : this.arrayLength == length));
    }

    /**
     * Not documented yet.
     * @param length A 64-bit signed integer.
     * @return A Boolean object.
     */
    public boolean ArrayLengthMatches(long length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                                              (this.arrayMinLength ? this.arrayLength >= length : this.arrayLength == length));
    }

    /**
     * Not documented yet.
     * @param bigLength A BigInteger object.
     * @return A Boolean object.
     */
    public boolean ArrayLengthMatches(BigInteger bigLength) {
      if (bigLength == null) {
        throw new NullPointerException("bigLength");
      }
      if ((this.types & (1 << 4)) != 0) {
        return false;
      }
      if (this.anyArrayLength) {
        return true;
      }
      if (!this.arrayMinLength && bigLength.compareTo(BigInteger.valueOf(this.arrayLength)) == 0) {
        return true;
      }
      if (this.arrayMinLength && bigLength.compareTo(BigInteger.valueOf(this.arrayLength)) >= 0) {
        return true;
      }
      return false;
    }

    /**
     * Not documented yet.
     * @param tag A 32-bit signed integer.
     * @return A Boolean object.
     */
    public boolean TagAllowed(int tag) {
      if (this.any) {
        return true;
      }
      return this.TagAllowed(BigInteger.valueOf(tag));
    }

    /**
     * Not documented yet.
     * @param tag A 64-bit signed integer.
     * @return A Boolean object.
     */
    public boolean TagAllowed(long tag) {
      if (this.any) {
        return true;
      }
      return this.TagAllowed(BigInteger.valueOf(tag));
    }

    /**
     * Not documented yet.
     * @param bigTag A BigInteger object.
     * @return A Boolean object.
     */
    public boolean TagAllowed(BigInteger bigTag) {
      if (bigTag == null) {
        throw new NullPointerException("bigTag");
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
      for(BigInteger tag : this.tags) {
        if (bigTag.equals(tag)) {
          return true;
        }
      }
      return false;
    }

    /**
     * Not documented yet.
     * @param index A 32-bit signed integer.
     * @return A Boolean object.
     */
    public boolean ArrayIndexAllowed(int index) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                                              ((this.arrayMinLength || index < this.arrayLength) && index >= 0));
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
      if (index >= this.elements.length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      // NOTE: Index shouldn't be greater than Integer.MAX_VALUE,
      // since the length is an int
      return this.elements[(int)index];
    }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
    public boolean NonFPSimpleValueAllowed() {
      return this.MajorTypeMatches(7) && !this.floatingpoint;
    }

    public static final CBORTypeFilter None = new CBORTypeFilter();
    public static final CBORTypeFilter UnsignedInteger = new CBORTypeFilter().WithUnsignedInteger();
    public static final CBORTypeFilter NegativeInteger = new CBORTypeFilter().WithNegativeInteger();
    public static final CBORTypeFilter Any = new CBORTypeFilter().WithAny();
    public static final CBORTypeFilter ByteString = new CBORTypeFilter().WithByteString();
    public static final CBORTypeFilter TextString = new CBORTypeFilter().WithTextString();
  }
