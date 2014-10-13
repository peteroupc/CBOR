/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using PeterO;

namespace PeterO.Cbor {
    /// <summary>Specifies what kinds of CBOR objects a tag can be. This class is
    /// used when a CBOR object is being read from a data stream. This class cannot
    /// be inherited; this is a change in version 2.0 from previous versions, where
    /// the class was inadvertently left inheritable.</summary>
  public sealed class CBORTypeFilter
  {
    private bool any;
    private int types;
    private bool floatingpoint;
    private int arrayLength;
    private bool anyArrayLength;
    private bool arrayMinLength;
    private CBORTypeFilter[] elements;
    private BigInteger[] tags;

    private CBORTypeFilter Copy() {
      var filter = new CBORTypeFilter();
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

    /// <summary>Copies this filter and includes unsigned integers in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithUnsignedInteger() {
      return this.WithType(0);
    }

    /// <summary>Copies this filter and includes negative integers in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithNegativeInteger() {
      return this.WithType(1);
    }

    /// <summary>Copies this filter and includes byte strings in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithByteString() {
      return this.WithType(2).WithTags(25);
    }

    /// <summary>Copies this filter and includes maps in the new filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithMap() {
      return this.WithType(5);
    }

    /// <summary>Copies this filter and includes text strings in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithTextString() {
      return this.WithType(3).WithTags(25);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tags'>An integer array of tags allowed.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithTags(params int[] tags) {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;  // Always include the "tag" major type
      int startIndex = 0;
      if (filter.tags != null) {
        var newTags = new BigInteger[tags.Length + filter.tags.Length];
        Array.Copy(filter.tags, newTags, filter.tags.Length);
        startIndex = filter.tags.Length;
        filter.tags = newTags;
      } else {
        filter.tags = new BigInteger[tags.Length];
      }
      for (var i = 0; i < tags.Length; ++i) {
        filter.tags[startIndex + i] = (BigInteger)tags[i];
      }
      return filter;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tags'>A BigInteger[] object.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter "tags[i]" is
    /// null.</exception>
    public CBORTypeFilter WithTags(params BigInteger[] tags) {
      if (this.any) {
        return this;
      }
      for (var i = 0; i < tags.Length; ++i) {
        if (tags[i] == null) {
          throw new ArgumentNullException("tags");
        }
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;  // Always include the "tag" major type
      int startIndex = 0;
      if (filter.tags != null) {
        var newTags = new BigInteger[tags.Length + filter.tags.Length];
        Array.Copy(filter.tags, newTags, filter.tags.Length);
        startIndex = filter.tags.Length;
        filter.tags = newTags;
      } else {
        filter.tags = new BigInteger[tags.Length];
      }
      Array.Copy(tags, 0, filter.tags, startIndex, tags.Length);
      return filter;
    }

    /// <summary>Copies this filter and includes CBOR arrays with an exact length to
    /// the new filter.</summary>
    /// <param name='arrayLength'>The desired maximum length of an array.</param>
    /// <param name='elements'>An array containing the allowed types for each
    /// element in the array. There must be at least as many elements here as given
    /// in the arrayLength parameter.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    /// <exception cref='ArgumentException'>The parameter arrayLength is less than
    /// 0.</exception>
    /// <exception cref='ArgumentNullException'>The parameter elements is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter elements has fewer
    /// elements than specified in arrayLength.</exception>
    public CBORTypeFilter WithArrayExactLength(
int arrayLength,
params CBORTypeFilter[] elements) {
      if (this.any) {
        return this;
      }
      if (elements == null) {
        throw new ArgumentNullException("elements");
      }
      if (arrayLength < 0) {
        throw new ArgumentException("arrayLength (" + arrayLength +
          ") is less than " + "0");
      }
      if (arrayLength < elements.Length) {
        throw new ArgumentException("arrayLength (" + arrayLength +
          ") is less than " + elements.Length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.arrayLength = arrayLength;
      filter.arrayMinLength = false;
      filter.elements = new CBORTypeFilter[elements.Length];
      Array.Copy(elements, filter.elements, elements.Length);
      return filter;
    }

    /// <summary>Copies this filter and includes CBOR arrays with at least a given
    /// length to the new filter.</summary>
    /// <param name='arrayLength'>The desired minimum length of an array.</param>
    /// <param name='elements'>An array containing the allowed types for each
    /// element in the array. There must be at least as many elements here as given
    /// in the arrayLength parameter.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    /// <exception cref='ArgumentException'>The parameter arrayLength is less than
    /// 0.</exception>
    /// <exception cref='ArgumentNullException'>The parameter elements is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter elements has fewer
    /// elements than specified in arrayLength.</exception>
    public CBORTypeFilter WithArrayMinLength(
int arrayLength,
params CBORTypeFilter[] elements) {
      if (this.any) {
        return this;
      }
      if (elements == null) {
        throw new ArgumentNullException("elements");
      }
      if (arrayLength < 0) {
        throw new ArgumentException("arrayLength (" + arrayLength +
          ") is less than " + "0");
      }
      if (arrayLength < elements.Length) {
        throw new ArgumentException("arrayLength (" + arrayLength +
          ") is less than " + elements.Length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.arrayLength = arrayLength;
      filter.arrayMinLength = true;
      filter.elements = new CBORTypeFilter[elements.Length];
      Array.Copy(elements, filter.elements, elements.Length);
      return filter;
    }

    /// <summary>Copies this filter and includes arrays of any length in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithArrayAnyLength() {
      if (this.any) {
        return this;
      }
      if (this.arrayLength < 0) {
        throw new ArgumentException("this.arrayLength (" + this.arrayLength +
          ") is less than " + "0");
      }
      if (this.arrayLength < this.elements.Length) {
        throw new ArgumentException("this.arrayLength (" + this.arrayLength +
          ") is less than " + this.elements.Length);
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.anyArrayLength = true;
      return filter;
    }

    /// <summary>Copies this filter and includes floating-point numbers in the new
    /// filter.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithFloatingPoint() {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.floatingpoint = true;
      return filter;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='type'>A 32-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool MajorTypeMatches(int type) {
      #if DEBUG
      if (type < 0) {
        throw new ArgumentException("type (" + type + ") is less than " + "0");
      }
      if (type > 7) {
        throw new ArgumentException("type (" + type + ") is more than " + "7");
      }
      #endif

      return type >= 0 && type <= 7 && (this.types & (1 << type)) != 0;
    }

    /// <summary>Returns whether an array's length is allowed under this
    /// filter.</summary>
    /// <param name='length'>The length of a CBOR array.</param>
    /// <returns>True if this filter allows CBOR arrays and an array's length is
    /// allowed under this filter; otherwise, false.</returns>
    public bool ArrayLengthMatches(int length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                (this.arrayMinLength ? this.arrayLength >= length :
                this.arrayLength == length));
    }

    /// <summary>Returns whether an array's length is allowed under a
    /// filter.</summary>
    /// <param name='length'>The length of a CBOR array.</param>
    /// <returns>True if this filter allows CBOR arrays and an array's length is
    /// allowed under a filter; otherwise, false.</returns>
    public bool ArrayLengthMatches(long length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength ||
                (this.arrayMinLength ? this.arrayLength >= length :
                this.arrayLength == length));
    }

    /// <summary>Returns whether an array's length is allowed under a
    /// filter.</summary>
    /// <param name='bigLength'>A BigInteger object.</param>
    /// <returns>True if this filter allows CBOR arrays and an array's length is
    /// allowed under a filter; otherwise, false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigLength'/> is null.</exception>
    public bool ArrayLengthMatches(BigInteger bigLength) {
      if (bigLength == null) {
        throw new ArgumentNullException("bigLength");
      }
      return ((this.types & (1 << 4)) == 0) && (this.anyArrayLength ||
        ((!this.arrayMinLength &&
        bigLength.CompareTo((BigInteger)this.arrayLength) == 0) ||
        (this.arrayMinLength &&
        bigLength.CompareTo((BigInteger)this.arrayLength) >= 0)));
    }

    /// <summary>Gets a value indicating whether CBOR objects can have the given tag
    /// number.</summary>
    /// <param name='tag'>A tag number. Returns false if this is less than
    /// 0.</param>
    /// <returns>True if CBOR objects can have the given tag number; otherwise,
    /// false.</returns>
    public bool TagAllowed(int tag) {
      return this.any || this.TagAllowed((BigInteger)tag);
    }

    /// <summary>Gets a value indicating whether CBOR objects can have the given tag
    /// number.</summary>
    /// <param name='tag'>A tag number. Returns false if this is less than
    /// 0.</param>
    /// <returns>True if CBOR objects can have the given tag number; otherwise,
    /// false.</returns>
    public bool TagAllowed(long tag) {
      return this.any || this.TagAllowed((BigInteger)tag);
    }

    /// <summary>Gets a value indicating whether CBOR objects can have the given tag
    /// number.</summary>
    /// <param name='bigTag'>A tag number. Returns false if this is less than
    /// 0.</param>
    /// <returns>True if CBOR objects can have the given tag number; otherwise,
    /// false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigTag'/> is null.</exception>
    public bool TagAllowed(BigInteger bigTag) {
      if (bigTag == null) {
        throw new ArgumentNullException("bigTag");
      }
      if (bigTag.Sign < 0) {
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
      foreach (BigInteger tag in this.tags) {
        if (bigTag.Equals(tag)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>Determines whether this type filter allows CBOR arrays and the
    /// given array index is allowed under this type filter.</summary>
    /// <param name='index'>An array index, starting from 0.</param>
    /// <returns>True if this type filter allows CBOR arrays and the given array
    /// index is allowed under this type filter; otherwise, false.</returns>
    public bool ArrayIndexAllowed(int index) {
   return (this.types & (1 << 4)) != 0 && index >= 0 && (this.anyArrayLength||
        ((this.arrayMinLength || index < this.arrayLength) && index >=
                                                0));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <returns>A CBORTypeFilter object.</returns>
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
      if (index >= this.elements.Length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      return this.elements[index];
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 64-bit signed integer.</param>
    /// <returns>A CBORTypeFilter object.</returns>
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
      // NOTE: Index shouldn't be greater than Int32.MaxValue,
      // since the length is an int
      if (index >= this.elements.Length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      return this.elements[(int)index];
    }

    /// <summary>Returns whether this filter allows simple values that are not
    /// floating-point numbers.</summary>
    /// <returns>True if this filter allows simple values that are not
    /// floating-point numbers; otherwise, false.</returns>
    public bool NonFPSimpleValueAllowed() {
      return this.MajorTypeMatches(7) && !this.floatingpoint;
    }

    /// <summary>A filter that allows no CBOR types.</summary>
    public static readonly CBORTypeFilter None = new CBORTypeFilter();

    /// <summary>A filter that allows unsigned integers.</summary>
    public static readonly CBORTypeFilter UnsignedInteger = new
      CBORTypeFilter().WithUnsignedInteger();

    /// <summary>A filter that allows negative integers.</summary>
    public static readonly CBORTypeFilter NegativeInteger = new
      CBORTypeFilter().WithNegativeInteger();

    /// <summary>A filter that allows any CBOR object.</summary>
    public static readonly CBORTypeFilter Any = new CBORTypeFilter().WithAny();

    /// <summary>A filter that allows byte strings.</summary>
    public static readonly CBORTypeFilter ByteString = new
      CBORTypeFilter().WithByteString();

    /// <summary>A filter that allows text strings.</summary>
    public static readonly CBORTypeFilter TextString = new
      CBORTypeFilter().WithTextString();
  }
}
