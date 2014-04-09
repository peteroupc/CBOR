/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
  internal sealed class EncodedWordEncoder {
    private StringBuilder currentWord;
    private StringBuilder fullString;
    private int spaceCount;

    private static string hex = "0123456789ABCDEF";

    public EncodedWordEncoder(string c) {
      this.currentWord = new StringBuilder();
      this.fullString = new StringBuilder();
      this.fullString.Append(c);
      this.spaceCount = (c.Length > 0) ? 1 : 0;
    }

    private void AppendChar(char ch) {
      this.PrepareToAppend(1);
      this.currentWord.Append(ch);
    }

    private void PrepareToAppend(int numChars) {
      // 2 for the ending "?="
      if (this.currentWord.Length + numChars + 2 > 75) {
        this.spaceCount = 1;
      }
      if (this.currentWord.Length + numChars + 2 > 75) {
        // Encoded word would be too big,
        // so output that word
        if (this.spaceCount > 0) {
          this.fullString.Append(' ');
        }
        this.fullString.Append(this.currentWord);
        this.fullString.Append("?=");
        this.currentWord.Clear();
        this.currentWord.Append("=?utf-8?q?");
        this.spaceCount = 1;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An EncodedWordEncoder object.</returns>
    /// <param name='suffix'>A string object.</param>
    public EncodedWordEncoder FinalizeEncoding(string suffix) {
      if (this.currentWord.Length > 0) {
        if (this.currentWord.Length + 2 + suffix.Length > 75) {
          // Too big to fit the current line,
          // create a new line
          this.spaceCount = 1;
        }
        if (this.spaceCount > 0) {
          this.fullString.Append(' ');
        }
        this.fullString.Append(this.currentWord);
        this.fullString.Append("?=");
        if (suffix.Length > 0) {
          this.fullString.Append(suffix);
        }
        this.spaceCount = 1;
        this.currentWord.Clear();
      }
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An EncodedWordEncoder object.</returns>
    public EncodedWordEncoder FinalizeEncoding() {
      return this.FinalizeEncoding(String.Empty);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
    public EncodedWordEncoder AddPrefix(string str) {
      if (!String.IsNullOrEmpty(str)) {
        this.FinalizeEncoding();
        this.currentWord.Append(str);
        this.currentWord.Append("=?utf-8?q?");
        this.spaceCount = 0;
      }
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='length'>A 32-bit signed integer. (2).</param>
    public EncodedWordEncoder AddString(string str, int index, int length) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((long)(str.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      for (int j = index; j < index + length; ++j) {
        int c = str[j];
        if (c >= 0xd800 && c <= 0xdbff && j + 1 < str.Length &&
            str[j + 1] >= 0xdc00 && str[j + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) * 0x400) + (str[j + 1] - 0xdc00);
          ++j;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          c = 0xfffd;
        }
        this.AddChar(c);
      }
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
    public EncodedWordEncoder AddString(string str) {
      return this.AddString(str, 0, str.Length);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='ch'>A 32-bit signed integer.</param>
    public void AddChar(int ch) {
      if (this.currentWord.Length == 0) {
        this.currentWord.Append("=?utf-8?q?");
        this.spaceCount = 1;
      }
      if (ch == 0x20) {
        this.AppendChar('_');
      } else if (ch < 0x80 && ch > 0x20 && ch != (char)'"' && ch != (char)',' &&
                 "?()<>[]:;@\\.=_".IndexOf((char)ch) < 0) {
        this.AppendChar((char)ch);
      } else if (ch < 0x80) {
        this.PrepareToAppend(3);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[ch >> 4]);
        this.currentWord.Append(hex[ch & 15]);
      } else if (ch < 0x800) {
        int w = (byte)(0xc0 | ((ch >> 6) & 0x1f));
        int x = (byte)(0x80 | (ch & 0x3f));
        this.PrepareToAppend(6);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[w >> 4]);
        this.currentWord.Append(hex[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[x >> 4]);
        this.currentWord.Append(hex[x & 15]);
      } else if (ch < 0x10000) {
        this.PrepareToAppend(9);
        int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
        int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int y = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.Append('=');
        this.currentWord.Append(hex[w >> 4]);
        this.currentWord.Append(hex[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[x >> 4]);
        this.currentWord.Append(hex[x & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[y >> 4]);
        this.currentWord.Append(hex[y & 15]);
      } else {
        this.PrepareToAppend(12);
        int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
        int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
        int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int z = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.Append('=');
        this.currentWord.Append(hex[w >> 4]);
        this.currentWord.Append(hex[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[x >> 4]);
        this.currentWord.Append(hex[x & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[y >> 4]);
        this.currentWord.Append(hex[y & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(hex[z >> 4]);
        this.currentWord.Append(hex[z & 15]);
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.fullString.ToString();
    }
  }
}
