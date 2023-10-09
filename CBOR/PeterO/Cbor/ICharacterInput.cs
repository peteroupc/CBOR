using System;

namespace PeterO.Cbor {
  /// <summary>An interface for reading Unicode characters from a data
  /// source.</summary>
  internal interface ICharacterInput
  {
    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
    int ReadChar();

    /// <summary>Reads a sequence of Unicode code points from a data
    /// source.</summary>
    /// <param name='chars'>Output buffer.</param>
    /// <param name='index'>Index in the output buffer to start writing
    /// to.</param>
    /// <param name='length'>Maximum number of code points to
    /// write.</param>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
    /// <exception cref='ArgumentException'>Either &#x22;index&#x22; or
    /// &#x22;length&#x22; is less than 0 or greater than
    /// &#x22;chars&#x22;&#x27;s length, or &#x22;chars&#x22;&#x27;s length
    /// minus &#x22;index&#x22; is less than
    /// &#x22;length&#x22;.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    int Read(int[] chars, int index, int length);
  }
}
