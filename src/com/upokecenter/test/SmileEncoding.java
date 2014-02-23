package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/19/2014
 * Time: 3:39 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.io.*;

  public final class SmileEncoding {
private SmileEncoding() {
}
    private static void WriteToInternal(CBORObject obj, OutputStream stream) throws IOException {
    }

    private static void WriteZigzagInt32(int val, OutputStream stream) throws IOException {
    }

    private static void WriteZigzagInt64(long val, OutputStream stream) throws IOException {
    }

    public static void WriteTo(CBORObject obj, OutputStream stream) throws IOException {
      stream.Write(new byte[] {  0x3a, 0x29, 0x0a, 0x04  }, 0, 4);
       switch (obj.getType()) {
          case CBORType.Number: {
            if (obj.CanFitInInt64()) {
              long val = obj.AsInt64();
              if (val >= -16 && val <= 15) {
                byte byteVal = (byte)(0xc0 + (val + 16));
                stream.write(byteVal);
              } else if (val >= Integer.MIN_VALUE && val <= Integer.MAX_VALUE) {
                int intVal = (int)val;
                stream.write(0x24);
                WriteZigzagInt32(intVal, stream);
              } else {
                stream.write(0x25);
                WriteZigzagInt64(val, stream);
              }
            } else if (obj.isIntegral()) {
              BigInteger bigVal = obj.AsBigInteger();
              bigVal.toByteArray(false);
            } else if (obj.CanFitInDouble()) {
              double val = obj.AsDouble();
              // long longVal = Double.doubleToRawLongBits(val);
            }
          }
          break;
      }
    }
  }
