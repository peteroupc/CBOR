/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 1:47 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PeterO;

namespace PeterO.Cbor
{
    /// <summary>Description of CborTypeFilter.</summary>
  public class CBORTypeFilter
  {
    private bool any;
    private int types;
    private bool floatingpoint;
    private int arrayLength;
    private bool anyArrayLength;
    private CBORTypeFilter[] elements;
    private BigInteger[] tags;

    public CBORTypeFilter() {
    }

    private CBORTypeFilter Copy() {
      CBORTypeFilter filter = new CBORTypeFilter();
      filter.any = this.any;
      filter.types = this.types;
      filter.floatingpoint = this.floatingpoint;
      filter.arrayLength = this.arrayLength;
      filter.anyArrayLength = this.anyArrayLength;
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
      filter.anyArrayLength = false;
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

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithUnsignedInteger() {
      return this.WithType(0);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithNegativeInteger() {
      return this.WithType(1);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithByteString() {
      return this.WithType(2);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithMap() {
      return this.WithType(5);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithTextString() {
      return this.WithType(3);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tags'>A params object.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithTags(params int[] tags) {
      if (this.any) {
        return this;
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;
      filter.tags = new BigInteger[tags.Length];
      for (int i = 0; i < tags.Length; ++i) {
        filter.tags[i] = (BigInteger)tags[i];
      }
      return filter;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tags'>A BigInteger[] object.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter WithTags(params BigInteger[] tags) {
      if (this.any) {
        return this;
      }
      for (int i = 0; i < tags.Length; ++i) {
        if (tags[i] == null) {
          throw new ArgumentNullException("tags[i]");
        }
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 6;
      filter.tags = new BigInteger[tags.Length];
      Array.Copy(tags, filter.tags, tags.Length);
      return filter;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    /// <param name='arrayLength'>A 32-bit signed integer.</param>
    /// <param name='elements'>A params object.</param>
    public CBORTypeFilter WithArray(int arrayLength, params CBORTypeFilter[] elements) {
      if (this.any) {
        return this;
      }
      if (arrayLength < 0) {
        throw new ArgumentException("arrayLength (" + Convert.ToString((long)arrayLength, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater or equal to " + "0");
      }
      if (arrayLength < elements.Length) {
        throw new ArgumentException("arrayLength (" + Convert.ToString((long)arrayLength, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater or equal to " + Convert.ToString((long)elements.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      CBORTypeFilter filter = this.Copy();
      filter.types |= 1 << 4;
      filter.arrayLength = arrayLength;
      filter.elements = new CBORTypeFilter[elements.Length];
      Array.Copy(elements, filter.elements, elements.Length);
      return filter;
    }

    /// <summary>Not documented yet.</summary>
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
        throw new ArgumentException("type (" + Convert.ToString((long)type, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater or equal to " + "0");
      }
      if (type > 7) {
        throw new ArgumentException("type (" + Convert.ToString((long)type, System.Globalization.CultureInfo.InvariantCulture) + ") is not less or equal to " + "7");
      }
      #endif

      return type >= 0 && type <= 7 && (this.types & (1 << type)) != 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='length'>A 32-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool ArrayLengthMatches(int length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength || this.arrayLength == length);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='length'>A 64-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool ArrayLengthMatches(long length) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength || this.arrayLength == length);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigLength'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool ArrayLengthMatches(BigInteger bigLength) {
      if (bigLength == null) {
        throw new ArgumentNullException("bigLength");
      }
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength || bigLength.CompareTo((BigInteger)this.arrayLength) == 0);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tag'>A 32-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool TagAllowed(int tag) {
      if (this.any) {
        return true;
      }
      return this.TagAllowed((BigInteger)tag);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='tag'>A 64-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool TagAllowed(long tag) {
      if (this.any) {
        return true;
      }
      return this.TagAllowed((BigInteger)tag);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigTag'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool TagAllowed(BigInteger bigTag) {
      if (bigTag == null) {
        throw new ArgumentNullException("bigTag");
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

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
    public bool ArrayIndexAllowed(int index) {
      return (this.types & (1 << 4)) != 0 && (this.anyArrayLength || index < this.arrayLength && index >= 0);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetSubFilter(int index) {
      if (this.anyArrayLength || this.any) {
        return Any;
      }
      if (index < 0 || index >= this.arrayLength) {
        // Index is out of bounds
        return None;
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
      if (this.anyArrayLength) {
        return Any;
      }
      if (index < 0 || index >= this.arrayLength) {
        // Index is out of bounds
        return None;
      }
      if (index >= this.elements.Length) {
        // Index is greater than the number of elements for
        // which a type is defined
        return Any;
      }
      return this.elements[(long)index];
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool NonFPSimpleValueAllowed() {
      return this.MajorTypeMatches(7) && !this.floatingpoint;
    }

    public static readonly CBORTypeFilter None = new CBORTypeFilter();
    public static readonly CBORTypeFilter UnsignedInteger = new CBORTypeFilter().WithUnsignedInteger();
    public static readonly CBORTypeFilter NegativeInteger = new CBORTypeFilter().WithNegativeInteger();
    public static readonly CBORTypeFilter Any = new CBORTypeFilter().WithAny();
    public static readonly CBORTypeFilter ByteString = new CBORTypeFilter().WithByteString();
    public static readonly CBORTypeFilter TextString = new CBORTypeFilter().WithTextString();
  }
}
