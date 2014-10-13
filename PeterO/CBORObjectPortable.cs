/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
namespace PeterO {
 sealed class CBORObjectPortable
  {
    private static void WritePortable(BigInteger bigint, Stream s) {
      if ((s) == null) {
 throw new ArgumentNullException("s");
}
      if ((object)bigint==(object)null) {
        s.WriteByte(0xf6);
        return;
      }
      int datatype = 0;
      if (bigint.Sign< 0) {
        datatype = 1;
        bigint+=(BigInteger)BigInteger.One;
        bigint=-(BigInteger)bigint;
      }
      if (bigint.CompareTo(Int64MaxValue) <= 0) {
        // If the big integer is representable as a long and in
        // major type 0 or 1, write that major type
        // instead of as a bignum
        long ui=(long)(BigInteger)bigint;
        WritePositiveInt64(datatype, ui, s);
      } else {
        using(var ms = new MemoryStream()) {
          long tmp = 0;
          var buffer = new byte[10];
          while (bigint.Sign>0) {
            // To reduce the number of big integer
            // operations, extract the big int 56 bits at a time
            // (not 64, to avoid negative numbers)
            BigInteger tmpbigint = bigint&(BigInteger)FiftySixBitMask;
            tmp=(long)(BigInteger)tmpbigint;
            bigint>>= 56;
            bool isNowZero=(bigint.IsZero);
            int bufferindex = 0;
            for (int i = 0; i<7 && (!isNowZero || tmp>0); ++i) {
              buffer[bufferindex]=(byte)(tmp & 0xff);
              tmp>>= 8;
              ++bufferindex;
            }
            ms.Write(buffer, 0, bufferindex);
          }
          byte[] bytes = ms.ToArray();
          switch(bytes.Length) {
            case 1: // Fits in 1 byte (won't normally happen though)
              buffer[0]=(byte)((datatype << 5)|24);
              buffer[1]=bytes[0];
              s.Write(buffer, 0, 2);
              break;
            case 2: // Fits in 2 bytes (won't normally happen though)
              buffer[0]=(byte)((datatype << 5)|25);
              buffer[1]=bytes[1];
              buffer[2]=bytes[0];
              s.Write(buffer, 0, 3);
              break;
            case 3:
            case 4:
              buffer[0]=(byte)((datatype << 5)|26);
              buffer[1]=(bytes.Length>3) ? bytes[3] : (byte)0;
              buffer[2]=bytes[2];
              buffer[3]=bytes[1];
              buffer[4]=bytes[0];
              s.Write(buffer, 0, 5);
              break;
            case 5:
            case 6:
            case 7:
            case 8:
              buffer[0]=(byte)((datatype << 5)|27);
              buffer[1]=(bytes.Length>7) ? bytes[7] : (byte)0;
              buffer[2]=(bytes.Length>6) ? bytes[6] : (byte)0;
              buffer[3]=(bytes.Length>5) ? bytes[5] : (byte)0;
              buffer[4]=bytes[4];
              buffer[5]=bytes[3];
              buffer[6]=bytes[2];
              buffer[7]=bytes[1];
              buffer[8]=bytes[0];
              s.Write(buffer, 0, 9);
              break;
            default:
              s.WriteByte((datatype == 0) ? (byte)0xc2 :
                          (byte)0xc3);
              WritePositiveInt(2, bytes.Length, s);
              for (int i = bytes.Length-1; i >= 0; --i) {
                s.WriteByte(bytes[i]);
              }
              break;
          }
        }
      }
    }
    private static void PortableDigitCount(BigInteger bi) {
        int kb = 0;
        if (!bi.IsZero) {
          while (true) {
            if (bi.CompareTo((BigInteger)Int32.MaxValue) <= 0) {
              var tmp = (int)bi;
              while (tmp > 0) {
                ++kb;
                tmp /= 10;
              }
              kb=(kb == 0 ? 1 : kb);
              return kb;
            }
            BigInteger q = bi/(BigInteger)bidivisor;
            if (q.IsZero) {
              int b=(int)bi;
              while (b>0) {
                ++kb;
                b/=10;
              }
              break;
            } else {
              kb+=4;
              bi = q;
            }
          }
        } else {
          kb = 1;
        }
        return kb;
    }
    // This is a more "portable" version of ConvertToBigNum,
    // but it's much slower on relatively large BigIntegers.
    private static CBORObject ConvertToBigNumPortable(CBORObject o, bool
    negative) {
      if (o.ItemType != CBORObjectType_ByteString) {
 throw new CBORException("Byte array expected");
}
      byte[] data=(byte[])o.ThisItem;
      if (data.Length <= 7) {
        long x = 0;
        if (data.Length>0) {
 x|=(((long)data[0]) & 0xff) << 48;
}
        if (data.Length>1) {
 x|=(((long)data[1]) & 0xff) << 40;
}
        if (data.Length>2) {
 x|=(((long)data[2]) & 0xff) << 32;
}
        if (data.Length>3) {
 x|=(((long)data[3]) & 0xff) << 24;
}
        if (data.Length>4) {
 x|=(((long)data[4]) & 0xff) << 16;
}
        if (data.Length>5) {
 x|=(((long)data[5]) & 0xff) << 8;
}
        if (data.Length>6) {
 x|=(((long)data[6]) & 0xff);
}
        if (negative) {
 x=-x;
}
        return FromObject(x);
      }
      BigInteger bi = BigInteger.Zero;
      for (int i = 0; i<data.Length; ++i) {
        if (i + 7 <= data.Length) {
          long x=(((long)data[i]) & 0xff) << 48;
          x|=(((long)data[i + 1]) & 0xff) << 40;
          x|=(((long)data[i + 2]) & 0xff) << 32;
          x|=(((long)data[i + 3]) & 0xff) << 24;
          x|=(((long)data[i + 4]) & 0xff) << 16;
          x|=(((long)data[i + 5]) & 0xff) << 8;
          x|=(((long)data[i + 6]) & 0xff);
          bi <<= 56;
          bi|=(BigInteger)x;
          i+=6;
        } else {
          bi <<= 8;
          int x=((int)data[i]) & 0xff;
          bi|=(BigInteger)x;
        }
      }
      if (negative) {
        bi = BigInteger.MinusOne-(BigInteger)bi;  // Convert to a negative
      }
      return RewrapObject(o, FromObject(bi));
    }
  }
}
