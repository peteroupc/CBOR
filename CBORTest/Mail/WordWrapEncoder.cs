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
  internal sealed class WordWrapEncoder {
    private const int MaxLineLength = 76;

    private string lastSpaces;
    private StringBuilder fullString;
    private int lineLength;

    public WordWrapEncoder(string c) {
      this.fullString = new StringBuilder();
      this.fullString.Append(c);
      if (this.fullString.Length >= MaxLineLength) {
        this.fullString.Append("\r\n");
        this.lastSpaces = " ";
        this.lineLength = 0;
      } else {
        this.lastSpaces = " ";
        this.lineLength = this.fullString.Length;
      }
    }

    private void AppendSpaces(string str) {
      if (this.lineLength + this.lastSpaces.Length + str.Length > MaxLineLength) {
        // Too big to fit the current line
        this.lastSpaces = " ";
      } else {
        this.lastSpaces = str;
      }
    }

    private void AppendWord(string str) {
      if (this.lineLength + this.lastSpaces.Length + str.Length > MaxLineLength) {
        // Too big to fit the current line,
        // create a new line
        this.fullString.Append("\r\n");
        this.lastSpaces = " ";
        this.lineLength = 0;
      }
      this.fullString.Append(this.lastSpaces);
      this.fullString.Append(str);
      this.lineLength += this.lastSpaces.Length;
      this.lineLength += str.Length;
      this.lastSpaces = String.Empty;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    public void AddString(string str) {
      int wordStart = 0;
      for (int j = 0; j < str.Length; ++j) {
        int c = str[j];
        if (c == 0x20 || c == 0x09) {
          int wordEnd = j;
          if (wordStart != wordEnd) {
            this.AppendWord(str.Substring(wordStart, wordEnd - wordStart));
          }
          while (j < str.Length) {
            if (str[j] == 0x20 || str[j] == 0x09) {
              ++j;
            } else {
              break;
            }
          }
          wordStart = j;
          this.AppendSpaces(str.Substring(wordEnd, wordStart - wordEnd));
          --j;
        }
      }
      if (wordStart != str.Length) {
        this.AppendWord(str.Substring(wordStart, str.Length - wordStart));
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.fullString.ToString();
    }
  }
}
