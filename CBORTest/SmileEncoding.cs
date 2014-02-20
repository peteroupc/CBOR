/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/19/2014
 * Time: 3:39 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace PeterO
{
  public static class SmileEncoding
  {
    private static void WriteToInternal(CBORObject obj, Stream stream) {
    }

    private static void WriteZigzagInt32(int val, Stream stream) {
    }

    private static void WriteZigzagInt64(long val, Stream stream) {
    }

    public static void WriteTo(CBORObject obj, Stream stream) {
      stream.Write(new byte[] { 0x3a, 0x29, 0x0a, 0x04 }, 0, 4);
       switch (obj.Type) {
          case CBORType.Number: {
            if (obj.CanFitInInt64()) {
              long val = obj.AsInt64();
              if (val >= -16 && val <= 15) {
                byte byteVal = (byte)(0xc0 + (val + 16));
                stream.WriteByte(byteVal);
              } else if (val >= Int32.MinValue && val <= Int32.MaxValue) {
                int intVal = (int)val;
                stream.WriteByte(0x24);
                WriteZigzagInt32(intVal, stream);
              } else {
                stream.WriteByte(0x25);
                WriteZigzagInt64(val, stream);
              }
            } else if (obj.IsIntegral) {
              BigInteger bigVal = obj.AsBigInteger();
              bigVal.toByteArray(false);
            } else if (obj.CanFitInDouble()) {
              double val = obj.AsDouble();
              // long longVal = BitConverter.ToInt64(BitConverter.GetBytes((double)val), 0);
            }
          }
          break;
      }
    }
  }
}
