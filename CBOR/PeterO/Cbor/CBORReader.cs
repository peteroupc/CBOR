/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORReader {
    private readonly Stream stream;
    private readonly CBOREncodeOptions options;
    private int depth;
    private StringRefs stringRefs;
    private bool hasSharableObjects;

    public CBORReader(Stream inStream) : this(inStream,
        CBOREncodeOptions.Default) {
    }

    public CBORReader(Stream inStream, CBOREncodeOptions options) {
      this.stream = inStream;
      this.options = options;
    }

    private static EInteger ToUnsignedEInteger(long val) {
      var lval = (EInteger)(val & ~(1L << 63));
      if ((val >> 63) != 0) {
        EInteger bigintAdd = EInteger.One << 63;
        lval += bigintAdd;
      }
      return lval;
    }

    private void HandleItemTag(long uadditional) {
      int uad = uadditional >= 257 ? 257 : (uadditional < 0 ? 0 :
          (int)uadditional);
      switch (uad) {
        case 256:
          // Tag 256: String namespace
          this.stringRefs = this.stringRefs ?? new StringRefs();
          this.stringRefs.Push();
          break;
        case 25:
          // String reference
          if (this.stringRefs == null) {
            throw new CBORException("No stringref namespace");
          }
          break;
        case 28:
        case 29:
          this.hasSharableObjects = true;
          break;
      }
    }

    private CBORObject ObjectFromByteArray(byte[] data, int lengthHint) {
      var cbor = CBORObject.FromRaw(data);
      this.stringRefs?.AddStringIfNeeded(cbor, lengthHint);
      return cbor;
    }

    private CBORObject ObjectFromUtf8Array(byte[] data, int lengthHint) {
      CBORObject cbor = data.Length == 0 ? CBORObject.FromString(String.Empty) :
         CBORObject.FromRawUtf8(data);
      this.stringRefs?.AddStringIfNeeded(cbor, lengthHint);
      return cbor;
    }

    private static CBORObject ResolveSharedRefs(
      CBORObject obj,
      SharedRefs sharedRefs) {
      if (obj == null) {
        return null;
      }
      CBORType type = obj.Type;
      bool hasTag = obj.HasMostOuterTag(29);
      if (hasTag) {
        CBORObject untagged = obj.UntagOne();
        return untagged.IsTagged || untagged.Type != CBORType.Integer ||
untagged.AsNumber().IsNegative() ?
          throw new CBORException(
            "Shared ref index must be an untagged integer 0 or greater") :
          sharedRefs.GetObject(untagged.AsEIntegerValue());
      }
      hasTag = obj.HasMostOuterTag(28);
      if (hasTag) {
        obj = obj.UntagOne();
        sharedRefs.AddObject(obj);
      }
      if (type == CBORType.Map) {
        foreach (CBORObject key in obj.Keys) {
          CBORObject value = obj[key];
          CBORObject newvalue = ResolveSharedRefs(value, sharedRefs);
          if (value != newvalue) {
            obj[key] = newvalue;
          }
        }
      } else if (type == CBORType.Array) {
        for (int i = 0; i < obj.Count; ++i) {
          obj[i] = ResolveSharedRefs(obj[i], sharedRefs);
        }
      }
      return obj;
    }

    public CBORObject Read() {
      CBORObject obj = this.options.AllowEmpty ?
        this.ReadInternalOrEOF() : this.ReadInternal();
      if (this.options.ResolveReferences && this.hasSharableObjects) {
        var sharedRefs = new SharedRefs();
        return ResolveSharedRefs(obj, sharedRefs);
      }
      return obj;
    }

    private CBORObject ReadInternalOrEOF() {
      if (this.depth > 500) {
        throw new CBORException("Too deeply nested");
      }
      int firstbyte = this.stream.ReadByte();
      if (firstbyte < 0) {
        // End of stream
        return null;
      }
      return this.ReadForFirstByte(firstbyte);
    }

    private CBORObject ReadInternal() {
      if (this.depth > 500) {
        throw new CBORException("Too deeply nested");
      }
      int firstbyte = this.stream.ReadByte();
      return firstbyte < 0 ? throw new CBORException("Premature end of" +
"\u0020data") : this.ReadForFirstByte(firstbyte);
    }

    private CBORObject ReadStringArrayMap(int type, long uadditional) {
      if (type == 2 || type == 3) { // Byte string or text string
        if ((uadditional >> 31) != 0) {
          throw new CBORException("Length of " +
            ToUnsignedEInteger(uadditional).ToString() + " is bigger" +
            "\u0020than supported");
        }
        int hint = (uadditional > Int32.MaxValue ||
            (uadditional >> 63) != 0) ? Int32.MaxValue : (int)uadditional;
        byte[] data = ReadByteData(this.stream, uadditional, null);
        if (type == 3) {
          return !CBORUtilities.CheckUtf8(data) ? throw new
CBORException("Invalid UTF-8") : this.ObjectFromUtf8Array(data, hint);
        } else {
          return this.ObjectFromByteArray(data, hint);
        }
      }
      if (type == 4) { // Array
        if (this.options.Ctap2Canonical && this.depth >= 4) {
          throw new CBORException("Depth too high in canonical CBOR");
        }
        var cbor = CBORObject.NewArray();
        if ((uadditional >> 31) != 0) {
          throw new CBORException("Length of " +
            ToUnsignedEInteger(uadditional).ToString() + " is bigger than" +
"\u0020supported");
        }
        if (PropertyMap.ExceedsKnownLength(this.stream, uadditional)) {
          throw new CBORException("Remaining data too small for array" +
"\u0020length");
        }
        ++this.depth;
        for (long i = 0; i < uadditional; ++i) {
          _ = cbor.Add(
            this.ReadInternal());
        }
        --this.depth;
        return cbor;
      }
      if (type == 5) { // Map, type 5
        if (this.options.Ctap2Canonical && this.depth >= 4) {
          throw new CBORException("Depth too high in canonical CBOR");
        }
        CBORObject cbor = this.options.KeepKeyOrder ?
               CBORObject.NewOrderedMap() : CBORObject.NewMap();
        if ((uadditional >> 31) != 0) {
          throw new CBORException("Length of " +
            ToUnsignedEInteger(uadditional).ToString() + " is bigger than" +
            "\u0020supported");
        }
        if (PropertyMap.ExceedsKnownLength(this.stream, uadditional)) {
          throw new CBORException("Remaining data too small for map" +
"\u0020length");
        }
        CBORObject lastKey = null;
        IComparer<CBORObject> comparer = CBORCanonical.Comparer;
        for (long i = 0; i < uadditional; ++i) {
          ++this.depth;
          CBORObject key = this.ReadInternal();
          CBORObject value = this.ReadInternal();
          --this.depth;
          if (this.options.Ctap2Canonical && lastKey != null) {
            int cmp = comparer.Compare(lastKey, key);
            if (cmp > 0) {
              throw new CBORException("Map key not in canonical order");
            } else if (cmp == 0) {
              throw new CBORException("Duplicate map key");
            }
          }
          if (!this.options.AllowDuplicateKeys) {
            if (cbor.ContainsKey(key)) {
              throw new CBORException("Duplicate key already exists");
            }
          }
          lastKey = key;
          cbor[key] = value;
        }
        return cbor;
      }
      return null;
    }

    private static void ReadHelper(
      Stream stream,
      byte[] bytes,
      int offset,
      int count) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not" +
"\u0020greater or equal to 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("\"offset\" (" + offset + ") is not less" +
"\u0020or equal to " + bytes.Length);
      }
      if (count < 0) {
        throw new ArgumentException(" (" + count + ") is not greater or" +
"\u0020equal to 0");
      }
      if (count > bytes.Length) {
        throw new ArgumentException(" (" + count + ") is not less or equal" +
"\u0020to " + bytes.Length);
      }
      if (bytes.Length - offset < count) {
        throw new ArgumentException("\"bytes\" + \"'s length minus \" +" +
"\u0020offset (" + (bytes.Length - offset) + ") is not greater or equal to " +
count);
      }
      int t = count;
      int tpos = offset;
      while (t > 0) {
        int rcount = stream.Read(bytes, tpos, t);
        if (rcount <= 0) {
          throw new CBORException("Premature end of data");
        }
        if (rcount > t) {
          throw new CBORException("Internal error");
        }
        tpos = checked(tpos + rcount);
        t = checked(t - rcount);
      }
      if (t != 0) {
        throw new CBORException("Internal error");
      }
    }

    public CBORObject ReadForFirstByte(int firstbyte) {
      if (this.depth > 500) {
        throw new CBORException("Too deeply nested");
      }
      if (firstbyte < 0) {
        throw new CBORException("Premature end of data");
      }
      if (firstbyte == 0xff) {
        throw new CBORException("Unexpected break code encountered");
      }
      int type = (firstbyte >> 5) & 0x07;
      int additional = firstbyte & 0x1f;
      long uadditional;
      CBORObject fixedObject;
      if (this.options.Ctap2Canonical) {
        if (additional >= 0x1c) {
          // NOTE: Includes stop byte and indefinite length data items
          throw new CBORException("Invalid canonical CBOR encountered");
        }
        // Check if this represents a fixed object (NOTE: All fixed objects
        // comply with CTAP2 canonical CBOR).
        fixedObject = CBORObject.GetFixedObject(firstbyte);
        if (fixedObject != null) {
          return fixedObject;
        }
        if (type == 6) {
          throw new CBORException("Tags not allowed in canonical CBOR");
        }
        uadditional = ReadDataLength(
          this.stream,
          firstbyte,
          type,
          type == 7);
        if (type == 0) {
          return (uadditional >> 63) != 0 ?
            CBORObject.FromEInteger(ToUnsignedEInteger(uadditional)) :
            CBORObject.FromInt64(uadditional);
        } else if (type == 1) {
          return (uadditional >> 63) != 0 ? CBORObject.FromEInteger(
              ToUnsignedEInteger(uadditional).Add(1).Negate()) :
            CBORObject.FromInt64((-uadditional) - 1L);
        } else if (type == 7) {
          if (additional < 24) {
            return CBORObject.FromSimpleValue(additional);
          } else if (additional == 24 && uadditional < 32) {
            throw new CBORException("Invalid simple value encoding");
          } else if (additional == 24) {
            return CBORObject.FromSimpleValue((int)uadditional);
          } else if (additional == 25) {
            return CBORObject.FromFloatingPointBits(uadditional, 2);
          } else if (additional == 26) {
            return CBORObject.FromFloatingPointBits(uadditional, 4);
          } else if (additional == 27) {
            return CBORObject.FromFloatingPointBits(uadditional, 8);
          }
        } else if (type >= 2 && type <= 5) {
          return this.ReadStringArrayMap(type, uadditional);
        }
        throw new CBORException("Unexpected data encountered");
      }
      int expectedLength = CBORObject.GetExpectedLength(firstbyte);
      // Data checks
      if (expectedLength == -1) {
        // if the head byte is invalid
        throw new CBORException("Unexpected data encountered");
      }
      // Check if this represents a fixed object
      fixedObject = CBORObject.GetFixedObject(firstbyte);
      if (fixedObject != null) {
        return fixedObject;
      }
      // Read fixed-length data
      byte[] data;
      if (expectedLength != 0) {
        data = new byte[expectedLength];
        // include the first byte because GetFixedLengthObject
        // will assume it exists for some head bytes
        data[0] = unchecked((byte)firstbyte);
        if (expectedLength > 1) {
          ReadHelper(this.stream, data, 1, expectedLength - 1);
        }
        CBORObject cbor = CBORObject.GetFixedLengthObject(firstbyte, data);
        if (this.stringRefs != null && (type == 2 || type == 3)) {
          this.stringRefs.AddStringIfNeeded(cbor, expectedLength - 1);
        }
        return cbor;
      }
      if (additional == 31) {
        // Indefinite-length for major types 2 to 5 (other major
        // types were already handled in the call to
        // GetFixedLengthObject).
        switch (type) {
          case 2: {
              // Streaming byte string
              using (var ms = new MemoryStream()) {
                // Requires same type as this one
                while (true) {
                  int nextByte = this.stream.ReadByte();
                  if (nextByte == 0xff) {
                    // break if the "break" code was read
                    break;
                  }
                  long len = ReadDataLength(this.stream, nextByte, 2);
                  if ((len >> 63) != 0 || len > Int32.MaxValue) {
                    throw new CBORException("Length" + ToUnsignedEInteger(len) +
                        " is bigger than supported ");
                  }
                  if (nextByte != 0x40) {
                    // NOTE: 0x40 means the empty byte string
                    _ = ReadByteData(this.stream, len, ms);
                  }
                }
                if (ms.Position > Int32.MaxValue) {
                  throw new
                  CBORException("Length of bytes to be streamed is bigger" +
  "\u0020than supported ");
                }
                data = ms.ToArray();
                return CBORObject.FromRaw(data);
              }
            }
          case 3: {
              // Streaming text string
              var builder = new StringBuilder();
              while (true) {
                int nextByte = this.stream.ReadByte();
                if (nextByte == 0xff) {
                  // break if the "break" code was read
                  break;
                }
                long len = ReadDataLength(this.stream, nextByte, 3);
                if ((len >> 63) != 0 || len > Int32.MaxValue) {
                  throw new CBORException("Length" + ToUnsignedEInteger(len) +
                    " is bigger than supported");
                }
                if (nextByte != 0x60) {
                  // NOTE: 0x60 means the empty string
                  if (PropertyMap.ExceedsKnownLength(this.stream, len)) {
                    throw new CBORException("Premature end of data");
                  }
                  switch (
                    DataUtilities.ReadUtf8(
                      this.stream,
                      (int)len,
                      builder,
                      false)) {
                    case -1:
                      throw new CBORException("Invalid UTF-8");
                    case -2:
                      throw new CBORException("Premature end of data");
                  }
                }
              }
              return CBORObject.FromRaw(builder.ToString());
            }
          case 4: {
              var cbor = CBORObject.NewArray();
              var vtindex = 0;
              // Indefinite-length array
              while (true) {
                int headByte = this.stream.ReadByte();
                if (headByte < 0) {
                  throw new CBORException("Premature end of data");
                }
                if (headByte == 0xff) {
                  // Break code was read
                  break;
                }
                ++this.depth;
                CBORObject o = this.ReadForFirstByte(
                    headByte);
                --this.depth;
                _ = cbor.Add(o);
                ++vtindex;
              }
              return cbor;
            }
          case 5: {
              CBORObject cbor = this.options.KeepKeyOrder ?
                 CBORObject.NewOrderedMap() : CBORObject.NewMap();
              // Indefinite-length map
              while (true) {
                int headByte = this.stream.ReadByte();
                if (headByte < 0) {
                  throw new CBORException("Premature end of data");
                }
                if (headByte == 0xff) {
                  // Break code was read
                  break;
                }
                ++this.depth;
                CBORObject key = this.ReadForFirstByte(headByte);
                CBORObject value = this.ReadInternal();
                --this.depth;
                int oldCount = cbor.Count;
                cbor[key] = value;
                int newCount = cbor.Count;
                if (!this.options.AllowDuplicateKeys && oldCount == newCount) {
                  throw new CBORException("Duplicate key already exists");
                }
              }
              return cbor;
            }
          default: throw new CBORException("Unexpected data encountered");
        }
      }

      _ = EInteger.Zero;
      uadditional = ReadDataLength(this.stream, firstbyte, type);
      // The following doesn't check for major types 0 and 1,
      // since all of them are fixed-length types and are
      // handled in the call to GetFixedLengthObject.
      if (type >= 2 && type <= 5) {
        return this.ReadStringArrayMap(type, uadditional);
      }
      if (type == 6) { // Tagged item
        var haveFirstByte = false;
        var newFirstByte = -1;
        if (this.options.ResolveReferences && (uadditional >> 32) == 0) {
          // NOTE: HandleItemTag treats only certain tags up to 256 specially
          this.HandleItemTag(uadditional);
        }
        ++this.depth;
        CBORObject o = haveFirstByte ? this.ReadForFirstByte(
            newFirstByte) : this.ReadInternal();
        --this.depth;
        if ((uadditional >> 63) != 0) {
          return CBORObject.FromCBORObjectAndTag(o,
              ToUnsignedEInteger(uadditional));
        }
        if (uadditional < 65536) {
          if (this.options.ResolveReferences) {
            int uaddl = uadditional >= 257 ? 257 : (uadditional < 0 ? 0 :
                (int)uadditional);
            switch (uaddl) {
              case 256:
                // string tag
                this.stringRefs.Pop();
                break;
              case 25:
                // stringref tag
                if (o.IsTagged || o.Type != CBORType.Integer) {
                  throw new CBORException("stringref must be an unsigned" +
                    "\u0020integer");
                }
                return this.stringRefs.GetString(o.AsEIntegerValue());
            }
          }
          return CBORObject.FromCBORObjectAndTag(
              o,
              (int)uadditional);
        }
        return CBORObject.FromCBORObjectAndTag(
            o,
            (EInteger)uadditional);
      }
      throw new CBORException("Unexpected data encountered");
    }

    private static readonly byte[] EmptyByteArray = new byte[0];

    private static byte[] ReadByteData(
      Stream stream,
      long uadditional,
      Stream outputStream) {
      if (uadditional == 0) {
        return EmptyByteArray;
      }
      if ((uadditional >> 63) != 0 || uadditional > Int32.MaxValue) {
        throw new CBORException("Length" + ToUnsignedEInteger(uadditional) +
          " is bigger than supported ");
      }
      if (PropertyMap.ExceedsKnownLength(stream, uadditional)) {
        throw new CBORException("Premature end of stream");
      }
      if (uadditional <= 0x10000) {
        // Simple case: small size
        var data = new byte[(int)uadditional];
        ReadHelper(stream, data, 0, data.Length);
        if (outputStream != null) {
          outputStream.Write(data, 0, data.Length);
          return null;
        }
        return data;
      } else {
        var tmpdata = new byte[0x10000];
        var total = (int)uadditional;
        if (outputStream != null) {
          while (total > 0) {
            int bufsize = Math.Min(tmpdata.Length, total);
            ReadHelper(stream, tmpdata, 0, bufsize);
            outputStream.Write(tmpdata, 0, bufsize);
            total -= bufsize;
          }
          return null;
        }
        using (var ms = new MemoryStream(0x10000)) {
          while (total > 0) {
            int bufsize = Math.Min(tmpdata.Length, total);
            ReadHelper(stream, tmpdata, 0, bufsize);
            ms.Write(tmpdata, 0, bufsize);
            total -= bufsize;
          }
          return ms.ToArray();
        }
      }
    }

    private static long ReadDataLength(
      Stream stream,
      int headByte,
      int expectedType) {
      return ReadDataLength(stream, headByte, expectedType, true);
    }

    private static long ReadDataLength(
      Stream stream,
      int headByte,
      int expectedType,
      bool allowNonShortest) {
      if (headByte < 0) {
        throw new CBORException("Unexpected data encountered");
      }
      if (((headByte >> 5) & 0x07) != expectedType) {
        throw new CBORException("Unexpected data encountered");
      }
      headByte &= 0x1f;
      if (headByte < 24) {
        return headByte;
      }
      var data = new byte[8];
      switch (headByte) {
        case 24: {
            int tmp = stream.ReadByte();
            return tmp < 0 ? throw new CBORException("Premature end of data") :
              !allowNonShortest && tmp < 24 ? throw new
CBORException("Non-shortest CBOR form") : tmp;
          }
        case 25: {
            ReadHelper(stream, data, 0, 2);
            int lowAdditional = (data[0] & 0xff) << 8;
            lowAdditional |= data[1] & 0xff;
            return !allowNonShortest && lowAdditional < 256 ? throw new
CBORException("Non-shortest CBOR form") : lowAdditional;
          }
        case 26: {
            ReadHelper(stream, data, 0, 4);
            long uadditional = (data[0] & 0xffL) << 24;
            uadditional |= (data[1] & 0xffL) << 16;
            uadditional |= (data[2] & 0xffL) << 8;
            uadditional |= data[3] & 0xffL;
            return !allowNonShortest && (uadditional >> 16) == 0 ? throw new
CBORException("Non-shortest CBOR form") : uadditional;
          }
        case 27: {
            ReadHelper(stream, data, 0, 8);
            // Treat return value as an unsigned integer
            long uadditional = (data[0] & 0xffL) << 56;
            uadditional |= (data[1] & 0xffL) << 48;
            uadditional |= (data[2] & 0xffL) << 40;
            uadditional |= (data[3] & 0xffL) << 32;
            uadditional |= (data[4] & 0xffL) << 24;
            uadditional |= (data[5] & 0xffL) << 16;
            uadditional |= (data[6] & 0xffL) << 8;
            uadditional |= data[7] & 0xffL;
            return !allowNonShortest && (uadditional >> 32) == 0 ? throw new
CBORException("Non-shortest CBOR form") : uadditional;
          }
        case 28:
        case 29:
        case 30:
          throw new CBORException("Unexpected data encountered");
        case 31:
          throw new CBORException("Indefinite-length data not allowed" +
"\u0020here");
        default: return headByte;
      }
    }

#if !NET20 && !NET40
    /*
    // - - - - - ASYNCHRONOUS METHODS

    private static async Task<int> ReadByteAsync(Stream stream) {
      var bytes = new byte[1];
      if (await stream.ReadAsync(bytes, 0, 1) == 0) {
        return -1;
      } else {
        return bytes[0];
      }
    }
    */
#endif
  }
}
