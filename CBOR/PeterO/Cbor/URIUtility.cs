/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

using System;
using System.Text;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.URIUtility"]/*'/>
  internal static class URIUtility {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.URIUtility.ParseMode"]/*'/>
    internal enum ParseMode {
    /// <summary>The rules follow the syntax for parsing IRIs. In
    /// particular, many code points outside the Basic Latin range (U +
    /// 0000 to U + 007F) are allowed. Strings with unpaired surrogate code
    /// points are considered invalid.</summary>
      IRIStrict,

    /// <summary>The rules follow the syntax for parsing IRIs, except that
    /// code points outside the Basic Latin range (U + 0000 to U + 007F)
    /// are not allowed.</summary>
      URIStrict,

    /// <summary>The rules only check for the appropriate delimiters when
    /// splitting the path, without checking if all the characters in each
    /// component are valid. Even with this mode, strings with unpaired
    /// surrogate code points are considered invalid.</summary>
      IRILenient,

    /// <summary>The rules only check for the appropriate delimiters when
    /// splitting the path, without checking if all the characters in each
    /// component are valid. Code points outside the Basic Latin range (U +
    /// 0000 to U + 007F) are not allowed.</summary>
      URILenient,

    /// <summary>The rules only check for the appropriate delimiters when
    /// splitting the path, without checking if all the characters in each
    /// component are valid. Unpaired surrogate code points are treated as
    /// though they were replacement characters instead for the purposes of
    /// these rules, so that strings with those code points are not
    /// considered invalid strings.</summary>
      IRISurrogateLenient,
    }

    private const string HexChars = "0123456789ABCDEF";

    private static void AppendAuthority(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      if (segments[2] >= 0) {
        builder.Append("//");
        builder.Append(
  refValue.Substring(
    segments[2],
    segments[3] - segments[2]));
      }
    }

    private static void AppendFragment(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      if (segments[8] >= 0) {
        builder.Append('#');
        builder.Append(
  refValue.Substring(
    segments[8],
    segments[9] - segments[8]));
      }
    }

    private static void AppendNormalizedPath(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      builder.Append(
        NormalizePath(
  refValue.Substring(
    segments[4],
    segments[5] - segments[4])));
    }

    private static void AppendPath(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      builder.Append(
  refValue.Substring(
    segments[4],
    segments[5] - segments[4]));
    }

    private static void AppendQuery(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      if (segments[6] >= 0) {
        builder.Append('?');
        builder.Append(
  refValue.Substring(
    segments[6],
    segments[7] - segments[6]));
      }
    }

    private static void AppendScheme(
      StringBuilder builder,
      string refValue,
      int[] segments) {
      if (segments[0] >= 0) {
        builder.Append(
          refValue.Substring(
            segments[0],
            segments[1] - segments[0]));
        builder.Append(':');
      }
    }

    public static string EscapeURI(string s, int mode) {
      if (s == null) {
        return null;
      }
      int[] components = null;
      if (mode == 1) {
        components = (
          s == null) ? null : SplitIRI(
  s,
  0,
  s.Length,
  ParseMode.IRIStrict);
        if (components == null) {
          return null;
        }
      } else {
        components = (s == null) ? null : SplitIRI(
          s,
          0,
          s.Length,
          ParseMode.IRISurrogateLenient);
      }
      var index = 0;
      int valueSLength = s.Length;
      var builder = new StringBuilder();
      while (index < valueSLength) {
        int c = s[index];
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            (s[index + 1] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (s[index + 1] & 0x3ff);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          c = 0xfffd;
        }
        if (mode == 0 || mode == 3) {
          if (c == '%' && mode == 3) {
            // Check for illegal percent encoding
            if (index + 2 >= valueSLength || !IsHexChar(s[index + 1]) ||
                !IsHexChar(s[index + 2])) {
              PercentEncodeUtf8(builder, c);
            } else {
              if (c <= 0xffff) {
                builder.Append((char)c);
              } else if (c <= 0x10ffff) {
                builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) |
0xd800));
                builder.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
            }
            ++index;
            continue;
          }
          if (c >= 0x7f || c <= 0x20 ||
              ((c & 0x7f) == c && "{}|^\\`<>\"".IndexOf((char)c) >= 0)) {
            PercentEncodeUtf8(builder, c);
          } else if (c == '[' || c == ']') {
            if (components != null && index >= components[2] && index <
                components[3]) {
              // within the authority component, so don't percent-encode
              if (c <= 0xffff) {
                builder.Append((char)c);
              } else if (c <= 0x10ffff) {
                builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) |
0xd800));
                builder.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
            } else {
              // percent encode
              PercentEncodeUtf8(builder, c);
            }
          } else {
            if (c <= 0xffff) {
              builder.Append((char)c);
            } else if (c <= 0x10ffff) {
              builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
              builder.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
            }
          }
        } else if (mode == 1 || mode == 2) {
          if (c >= 0x80) {
            PercentEncodeUtf8(builder, c);
          } else if (c == '[' || c == ']') {
            if (components != null && index >= components[2] && index <
                components[3]) {
              // within the authority component, so don't percent-encode
              if (c <= 0xffff) {
                builder.Append((char)c);
              } else if (c <= 0x10ffff) {
                builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) |
0xd800));
                builder.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
            } else {
              // percent encode
              PercentEncodeUtf8(builder, c);
            }
          } else {
            if (c <= 0xffff) {
              builder.Append((char)c);
            } else if (c <= 0x10ffff) {
              builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
              builder.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
            }
          }
        }
        ++index;
      }
      return builder.ToString();
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.URIUtility.HasScheme(System.String)"]/*'/>
    public static bool HasScheme(string refValue) {
      int[] segments = (refValue == null) ? null : SplitIRI(
        refValue,
        0,
        refValue.Length,
        ParseMode.IRIStrict);
      return segments != null && segments[0] >= 0;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.URIUtility.HasSchemeForURI(System.String)"]/*'/>
    public static bool HasSchemeForURI(string refValue) {
      int[] segments = (refValue == null) ? null : SplitIRI(
        refValue,
        0,
        refValue.Length,
        ParseMode.URIStrict);
      return segments != null && segments[0] >= 0;
    }

    private static bool IsHexChar(char c) {
      return (c >= 'a' && c <= 'f') ||
        (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
    }

    private static bool IsIfragmentChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/?-._~:@!$&'()*+,;=".IndexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) || (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static bool IsIpchar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/-._~:@!$&'()*+,;=".IndexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) || (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static bool IsIqueryChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/?-._~:@!$&'()*+,;=".IndexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) || (c >= 0xe000 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0x10fffd && (c & 0xfffe) != 0xfffe);
    }

    private static bool IsIRegNameChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "-._~!$&'()*+,;=".IndexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) || (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static bool IsIUserInfoChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "-._~:!$&'()*+,;=".IndexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) || (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    public static bool IsValidCurieReference(string s, int offset, int length) {
      if (s == null) {
        return false;
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset + ") is less than " +
               "0 ");
      }
      if (offset > s.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          s.Length);
      }
      if (length < 0) {
        throw new ArgumentException(
          "length (" + length + ") is less than " + "0 ");
      }
      if (length > s.Length) {
        throw new ArgumentException(
          "length (" + length + ") is more than " + s.Length);
      }
      if (s.Length - offset < length) {
        throw new ArgumentException(
          "s's length minus " + offset + " (" + (s.Length - offset) +
          ") is less than " + length);
      }
      if (length == 0) {
        return true;
      }
      int index = offset;
      int valueSLength = offset + length;
      var state = 0;
      if (index + 2 <= valueSLength && s[index] == '/' && s[index + 1] == '/') {
        // has an authority, which is not allowed
        return false;
      }
      state = 0; // IRI Path
      while (index < valueSLength) {
        // Get the next Unicode character
        int c = s[index];
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            (s[index + 1] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (s[index + 1] & 0x3ff);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          // error
          return false;
        }
        if (c == '%') {
          // Percent encoded character
          if (index + 2 < valueSLength && IsHexChar(s[index + 1]) &&
              IsHexChar(s[index + 2])) {
            index += 3;
            continue;
          }
          return false;
        }
        if (state == 0) { // Path
          if (c == '?') {
            state = 1; // move to query state
          } else if (c == '#') {
            state = 2; // move to fragment state
          } else if (!IsIpchar(c)) {
            return false;
          }
          ++index;
        } else if (state == 1) { // Query
          if (c == '#') {
            state = 2; // move to fragment state
          } else if (!IsIqueryChar(c)) {
            return false;
          }
          ++index;
        } else if (state == 2) { // Fragment
          if (!IsIfragmentChar(c)) {
            return false;
          }
          ++index;
        }
      }
      return true;
    }

    public static bool IsValidIRI(string s) {
      return ((s == null) ?
  null : SplitIRI(
    s,
    0,
    s.Length,
    ParseMode.IRIStrict)) != null;
    }

    private const string ValueDotSlash = "." + "/";
    private const string ValueSlashDot = "/" + ".";

    private static string NormalizePath(string path) {
      int len = path.Length;
      if (len == 0 || path.Equals("..", StringComparison.Ordinal) ||
path.Equals(".", StringComparison.Ordinal)) {
        return String.Empty;
      }
      if (path.IndexOf(ValueSlashDot, StringComparison.Ordinal) < 0 &&
          path.IndexOf(
            ValueDotSlash,
            StringComparison.Ordinal) < 0) {
        return path;
      }
      var builder = new StringBuilder();
      var index = 0;
      while (index < len) {
        char c = path[index];
        if ((index + 3 <= len && c == '/' && path[index + 1] == '.' &&
             path[index + 2] == '/') || (index + 2 == len && c == '.' &&
             path[index + 1] == '.')) {
          // begins with "/./" or is "..";
          // move index by 2
          index += 2;
          continue;
        }
        if (index + 3 <= len && c == '.' &&
            path[index + 1] == '.' && path[index + 2] == '/') {
          // begins with "../";
          // move index by 3
          index += 3;
          continue;
        }
        if ((index + 2 <= len && c == '.' &&
             path[index + 1] == '/') || (index + 1 == len && c == '.')) {
          // begins with "./" or is ".";
          // move index by 1
          ++index;
          continue;
        }
        if (index + 2 == len && c == '/' &&
            path[index + 1] == '.') {
          // is "/."; append '/' and break
          builder.Append('/');
          break;
        }
        if (index + 3 == len && c == '/' &&
            path[index + 1] == '.' && path[index + 2] == '.') {
          // is "/.."; remove last segment,
          // append "/" and return
          int index2 = builder.Length - 1;
          string builderString = builder.ToString();
          while (index2 >= 0) {
            if (builderString[index2] == '/') {
              break;
            }
            --index2;
          }
          if (index2 < 0) {
            index2 = 0;
          }
          builder.Length = index2;
          builder.Append('/');
          break;
        }
        if (index + 4 <= len && c == '/' && path[index + 1] == '.' &&
            path[index + 2] == '.' && path[index + 3] == '/') {
          // begins with "/../"; remove last segment
          int index2 = builder.Length - 1;
          string builderString = builder.ToString();
          while (index2 >= 0) {
            if (builderString[index2] == '/') {
              break;
            }
            --index2;
          }
          if (index2 < 0) {
            index2 = 0;
          }
          builder.Length = index2;
          index += 3;
          continue;
        }
        builder.Append(c);
        ++index;
        while (index < len) {
          // Move the rest of the
          // path segment until the next '/'
          c = path[index];
          if (c == '/') {
            break;
          }
          builder.Append(c);
          ++index;
        }
      }
      return builder.ToString();
    }

    private static int ParseDecOctet(
      string s,
      int index,
      int endOffset,
      int c,
      int delim) {
      if (c >= '1' && c <= '9' && index + 2 < endOffset &&
          s[index + 1] >= '0' && s[index + 1] <= '9' &&
          s[index + 2] == delim) {
        return ((c - '0') * 10) + (s[index + 1] - '0');
      }
      if (c == '2' && index + 3 < endOffset &&
       (s[index + 1] == '5') && (s[index + 2] >= '0' && s[index + 2] <= '5') &&
          s[index + 3] == delim) {
        return 250 + (s[index + 2] - '0');
      }
      if (c == '2' && index + 3 < endOffset &&
          s[index + 1] >= '0' && s[index + 1] <= '4' &&
          s[index + 2] >= '0' && s[index + 2] <= '9' &&
          s[index + 3] == delim) {
        return 200 + ((s[index + 1] - '0') * 10) + (s[index + 2] - '0');
      }
      if (c == '1' && index + 3 < endOffset &&
          s[index + 1] >= '0' && s[index + 1] <= '9' &&
          s[index + 2] >= '0' && s[index + 2] <= '9' &&
          s[index + 3] == delim) {
        return 100 + ((s[index + 1] - '0') * 10) + (s[index + 2] - '0');
      }
      return (c >= '0' && c <= '9' && index + 1 < endOffset &&
              s[index + 1] == delim) ? (c - '0') : (-1);
    }

    private static int ParseIPLiteral(string s, int offset, int endOffset) {
      int index = offset;
      if (offset == endOffset) {
        return -1;
      }
      // Assumes that the character before offset
      // is a '['
      if (s[index] == 'v') {
        // IPvFuture
        ++index;
        var hex = false;
        while (index < endOffset) {
          char c = s[index];
          if (IsHexChar(c)) {
            hex = true;
          } else {
            break;
          }
          ++index;
        }
        if (!hex) {
          return -1;
        }
        if (index >= endOffset || s[index] != '.') {
          return -1;
        }
        ++index;
        hex = false;
        while (index < endOffset) {
          char c = s[index];
          if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
              (c >= '0' && c <= '9') ||
              ((c & 0x7F) == c && ":-._~!$&'()*+,;=".IndexOf(c) >= 0)) {
            hex = true;
          } else {
            break;
          }
          ++index;
        }
        if (!hex) {
          return -1;
        }
        if (index >= endOffset || s[index] != ']') {
          return -1;
        }
        ++index;
        return index;
      }
      if (s[index] == ':' ||
          IsHexChar(s[index])) {
        // IPv6 Address
        var phase1 = 0;
        var phase2 = 0;
        var phased = false;
        var expectHex = false;
        var expectColon = false;
        while (index < endOffset) {
          char c = s[index];
          if (c == ':' && !expectHex) {
            if ((phase1 + (phased ? 1 : 0) + phase2) >= 8) {
              return -1;
            }
            ++index;
            if (index < endOffset && s[index] == ':') {
              if (phased) {
                return -1;
              }
              phased = true;
              ++index;
            }
            expectHex = true;
            expectColon = false;
            continue;
          }
          if ((c >= '0' && c <= '9') && !expectColon &&
              (phased || (phase1 + (phased ? 1 : 0) + phase2) == 6)) {
            // Check for IPv4 address
            int decOctet = ParseDecOctet(s, index, endOffset, c, '.');
            if (decOctet >= 0) {
              if ((phase1 + (phased ? 1 : 0) + phase2) > 6) {
                // IPv4 address illegal at this point
                return -1;
              } else {
                // Parse the rest of the IPv4 address
                phase2 += 2;
                if (decOctet >= 100) {
                  index += 4;
                } else if (decOctet >= 10) {
                  index += 3;
                } else {
                  index += 2;
                }
                char tmpc = (index < endOffset) ? s[index] : '\0';
                decOctet = ParseDecOctet(
                  s,
                  index,
                  endOffset,
                  tmpc,
                  '.');
                if (decOctet >= 100) {
                  index += 4;
                } else if (decOctet >= 10) {
                  index += 3;
                } else if (decOctet >= 0) {
                  index += 2;
                } else {
                  return -1;
                }
                tmpc = (index < endOffset) ? s[index] : '\0';
                decOctet = ParseDecOctet(s, index, endOffset, tmpc, '.');
                if (decOctet >= 100) {
                  index += 4;
                } else if (decOctet >= 10) {
                  index += 3;
                } else if (decOctet >= 0) {
                  index += 2;
                } else {
                  return -1;
                }
                tmpc = (index < endOffset) ? s[index] : '\0';
                decOctet = ParseDecOctet(s, index, endOffset, tmpc, ']');
                if (decOctet < 0) {
                  tmpc = (index < endOffset) ? s[index] : '\0';
                  decOctet = ParseDecOctet(s, index, endOffset, tmpc, '%');
                }
                if (decOctet >= 100) {
                  index += 3;
                } else if (decOctet >= 10) {
                  index += 2;
                } else if (decOctet >= 0) {
                  ++index;
                } else {
                  return -1;
                }
                break;
              }
            }
          }
          if (IsHexChar(c) && !expectColon) {
            if (phased) {
              ++phase2;
            } else {
              ++phase1;
            }
            ++index;
            for (var i = 0; i < 3; ++i) {
              if (index < endOffset && IsHexChar(s[index])) {
                ++index;
              } else {
                break;
              }
            }
            expectHex = false;
            expectColon = true;
          } else {
            break;
          }
        }
        if ((phase1 + phase2) != 8 && !phased) {
          return -1;
        }
        if (phase1 + 1 + phase2 > 8 && phased) {
          return -1;
        }
        if (index >= endOffset) {
          return -1;
        }
        if (s[index] != ']' && s[index] != '%') {
          return -1;
        }
        if (s[index] == '%') {
          if (index + 2 < endOffset && s[index + 1] == '2' &&
              s[index + 2] == '5') {
            // Zone identifier in an IPv6 address
            // (see RFC6874)
            index += 3;
            var haveChar = false;
            while (index < endOffset) {
              char c = s[index];
              if (c == ']') {
                return haveChar ? index + 1 : -1;
              }
              if (c == '%') {
                if (index + 2 < endOffset && IsHexChar(s[index + 1]) &&
                    IsHexChar(s[index + 2])) {
                  index += 3;
                  haveChar = true;
                  continue;
                }
                return -1;
              }
              if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
             (c >= '0' && c <= '9') || c == '.' || c == '_' || c == '-' ||
                c == '~') {
                // unreserved character under RFC3986
                ++index;
                haveChar = true;
                continue;
              }
              return -1;
            }
            return -1;
          }
          return -1;
        }
        ++index;
        return index;
      }
      return -1;
    }

    private static string PathParent(
      string refValue,
      int startIndex,
      int endIndex) {
      if (startIndex > endIndex) {
        return String.Empty;
      }
      --endIndex;
      while (endIndex >= startIndex) {
        if (refValue[endIndex] == '/') {
          return refValue.Substring(startIndex, (endIndex + 1) - startIndex);
        }
        --endIndex;
      }
      return String.Empty;
    }

    private static void PercentEncode(StringBuilder buffer, int b) {
      buffer.Append('%');
      buffer.Append(HexChars[(b >> 4) & 0x0f]);
      buffer.Append(HexChars[b & 0x0f]);
    }

    private static void PercentEncodeUtf8(StringBuilder buffer, int cp) {
      if (cp <= 0x7f) {
        buffer.Append('%');
        buffer.Append(HexChars[(cp >> 4) & 0x0f]);
        buffer.Append(HexChars[cp & 0x0f]);
      } else if (cp <= 0x7ff) {
        PercentEncode(buffer, 0xc0 | ((cp >> 6) & 0x1f));
        PercentEncode(buffer, 0x80 | (cp & 0x3f));
      } else if (cp <= 0xffff) {
        PercentEncode(buffer, 0xe0 | ((cp >> 12) & 0x0f));
        PercentEncode(buffer, 0x80 | ((cp >> 6) & 0x3f));
        PercentEncode(buffer, 0x80 | (cp & 0x3f));
      } else {
        PercentEncode(buffer, 0xf0 | ((cp >> 18) & 0x07));
        PercentEncode(buffer, 0x80 | ((cp >> 12) & 0x3f));
        PercentEncode(buffer, 0x80 | ((cp >> 6) & 0x3f));
        PercentEncode(buffer, 0x80 | (cp & 0x3f));
      }
    }

    public static string RelativeResolve(string refValue, string baseURI) {
      return RelativeResolve(refValue, baseURI, ParseMode.IRIStrict);
    }

    public static string RelativeResolve(
      string refValue,
      string baseURI,
      ParseMode parseMode) {
      int[] segments = (refValue == null) ? null : SplitIRI(
        refValue,
        0,
        refValue.Length,
        parseMode);
      if (segments == null) {
        return null;
      }
      int[] segmentsBase = (
        baseURI == null) ? null : SplitIRI(
  baseURI,
  0,
  baseURI.Length,
  parseMode);
      if (segmentsBase == null) {
        return refValue;
      }
      var builder = new StringBuilder();
      if (segments[0] >= 0) { // scheme present
        AppendScheme(builder, refValue, segments);
        AppendAuthority(builder, refValue, segments);
        AppendNormalizedPath(builder, refValue, segments);
        AppendQuery(builder, refValue, segments);
        AppendFragment(builder, refValue, segments);
      } else if (segments[2] >= 0) { // authority present
        AppendScheme(builder, baseURI, segmentsBase);
        AppendAuthority(builder, refValue, segments);
        AppendNormalizedPath(builder, refValue, segments);
        AppendQuery(builder, refValue, segments);
        AppendFragment(builder, refValue, segments);
      } else if (segments[4] == segments[5]) {
        AppendScheme(builder, baseURI, segmentsBase);
        AppendAuthority(builder, baseURI, segmentsBase);
        AppendPath(builder, baseURI, segmentsBase);
        if (segments[6] >= 0) {
          AppendQuery(builder, refValue, segments);
        } else {
          AppendQuery(builder, baseURI, segmentsBase);
        }
        AppendFragment(builder, refValue, segments);
      } else {
        AppendScheme(builder, baseURI, segmentsBase);
        AppendAuthority(builder, baseURI, segmentsBase);
        if (segments[4] < segments[5] && refValue[segments[4]] == '/') {
          AppendNormalizedPath(builder, refValue, segments);
        } else {
          var merged = new StringBuilder();
          if (segmentsBase[2] >= 0 && segmentsBase[4] == segmentsBase[5]) {
            merged.Append('/');
            AppendPath(merged, refValue, segments);
            builder.Append(NormalizePath(merged.ToString()));
          } else {
            merged.Append(
              PathParent(
                baseURI,
                segmentsBase[4],
                segmentsBase[5]));
            AppendPath(merged, refValue, segments);
            builder.Append(NormalizePath(merged.ToString()));
          }
        }
        AppendQuery(builder, refValue, segments);
        AppendFragment(builder, refValue, segments);
      }
      return builder.ToString();
    }

    public static int[] SplitIRI(string s) {
      return (s == null) ? null : SplitIRI(s, 0, s.Length, ParseMode.IRIStrict);
    }

    public static int[] SplitIRI(
      string s,
      int offset,
      int length,
      ParseMode parseMode) {
      if (s == null) {
        return null;
      }
      if (s == null) {
        throw new ArgumentNullException(nameof(s));
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset +
          ") is less than 0");
      }
      if (offset > s.Length) {
        throw new ArgumentException("offset (" + offset +
          ") is more than " + s.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length +
          ") is less than 0");
      }
      if (length > s.Length) {
        throw new ArgumentException("length (" + length +
          ") is more than " + s.Length);
      }
      if (s.Length - offset < length) {
        throw new ArgumentException("s's length minus " + offset + " (" +
          (s.Length - offset) + ") is less than " + length);
      }
      int[] retval = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
      if (length == 0) {
        retval[4] = 0;
        retval[5] = 0;
        return retval;
      }
      bool asciiOnly = parseMode == ParseMode.URILenient || parseMode ==
        ParseMode.URIStrict;
      bool strict = parseMode == ParseMode.URIStrict || parseMode ==
        ParseMode.IRIStrict;
      int index = offset;
      int valueSLength = offset + length;
      var scheme = false;
      // scheme
      while (index < valueSLength) {
        int c = s[index];
        if (index > offset && c == ':') {
          scheme = true;
          retval[0] = offset;
          retval[1] = index;
          ++index;
          break;
        }
        if (strict && index == offset && !((c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z'))) {
          break;
        }
        if (strict && index > offset &&
        !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' &&
              c <= '9') || c == '+' || c == '-' || c == '.')) {
          break;
        }
        if (!strict && (c == '#' || c == ':' || c == '?' || c == '/')) {
          break;
        }
        ++index;
      }
      if (!scheme) {
        index = offset;
      }
      var state = 0;
      if (index + 2 <= valueSLength && s[index] == '/' && s[index + 1] == '/') {
        // authority
        // (index + 2, valueSLength)
        index += 2;
        int authorityStart = index;
        retval[2] = authorityStart;
        retval[3] = valueSLength;
        state = 0; // userinfo
        // Check for userinfo
        while (index < valueSLength) {
          int c = s[index];
          if (asciiOnly && c >= 0x80) {
            return null;
          }
          if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
              (s[index + 1] & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (s[index + 1] & 0x3ff);
            ++index;
          } else if ((c & 0xf800) == 0xd800) {
            if (parseMode == ParseMode.IRISurrogateLenient) {
              c = 0xfffd;
            } else {
              return null;
            }
          }
          if (c == '%' && (state == 0 || state == 1) && strict) {
            // Percent encoded character (except in port)
            if (index + 2 < valueSLength && IsHexChar(s[index + 1]) &&
                IsHexChar(s[index + 2])) {
              index += 3;
              continue;
            }
            return null;
          }
          if (state == 0) { // User info
            if (c == '/' || c == '?' || c == '#') {
              // not user info
              state = 1;
              index = authorityStart;
              continue;
            }
            if (strict && c == '@') {
              // is user info
              ++index;
              state = 1;
              continue;
            }
            if (strict && IsIUserInfoChar(c)) {
              ++index;
              if (index == valueSLength) {
                // not user info
                state = 1;
                index = authorityStart;
                continue;
              }
            } else {
              // not user info
              state = 1;
              index = authorityStart;
              continue;
            }
          } else if (state == 1) { // host
            if (c == '/' || c == '?' || c == '#') {
              // end of authority
              retval[3] = index;
              break;
            }
            if (!strict) {
              ++index;
            } else if (c == '[') {
              ++index;
              index = ParseIPLiteral(s, index, valueSLength);
              if (index < 0) {
                return null;
              }
              continue;
            } else if (c == ':') {
              // port
              state = 2;
              ++index;
            } else if (IsIRegNameChar(c)) {
              // is valid host name char
              // (note: IPv4 addresses included
              // in ireg-name)
              ++index;
            } else {
              return null;
            }
          } else if (state == 2) { // Port
            if (c == '/' || c == '?' || c == '#') {
              // end of authority
              retval[3] = index;
              break;
            }
            if (c >= '0' && c <= '9') {
              ++index;
            } else {
              return null;
            }
          }
        }
      }
      var colon = false;
      var segment = false;
      bool fullyRelative = index == offset;
      retval[4] = index; // path offsets
      retval[5] = valueSLength;
      state = 0; // IRI Path
      while (index < valueSLength) {
        // Get the next Unicode character
        int c = s[index];
        if (asciiOnly && c >= 0x80) {
          return null;
        }
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            (s[index + 1] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (s[index + 1] & 0x3ff);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          // error
          return null;
        }
        if (c == '%' && strict) {
          // Percent encoded character
          if (index + 2 < valueSLength && IsHexChar(s[index + 1]) &&
              IsHexChar(s[index + 2])) {
            index += 3;
            continue;
          }
          return null;
        }
        if (state == 0) { // Path
          if (c == ':' && fullyRelative) {
            colon = true;
          } else if (c == '/' && fullyRelative && !segment) {
            // noscheme path can't have colon before slash
            if (strict && colon) {
              return null;
            }
            segment = true;
          }
          if (c == '?') {
            retval[5] = index;
            retval[6] = index + 1;
            retval[7] = valueSLength;
            state = 1; // move to query state
          } else if (c == '#') {
            retval[5] = index;
            retval[8] = index + 1;
            retval[9] = valueSLength;
            state = 2; // move to fragment state
          } else if (strict && !IsIpchar(c)) {
            return null;
          }
          ++index;
        } else if (state == 1) { // Query
          if (c == '#') {
            retval[7] = index;
            retval[8] = index + 1;
            retval[9] = valueSLength;
            state = 2; // move to fragment state
          } else if (strict && !IsIqueryChar(c)) {
            return null;
          }
          ++index;
        } else if (state == 2) { // Fragment
          if (strict && !IsIfragmentChar(c)) {
            return null;
          }
          ++index;
        }
      }
      if (strict && fullyRelative && colon && !segment) {
        return null; // ex. "x@y:z"
      }
      return retval;
    }

    public static int[] SplitIRI(string s, ParseMode parseMode) {
      return (s == null) ? null : SplitIRI(s, 0, s.Length, parseMode);
    }
  }
}
