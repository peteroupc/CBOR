package com.upokecenter.cbor;
/*
Written in 2013 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Contains utility methods for processing Uniform Resource Identifiers (URIs)
     * and Internationalized Resource Identifiers (IRIs) under RFC3986 and
     * RFC3987, respectively. In the following documentation, URIs and IRIs
     * include URI references and IRI references, for convenience.
     */
  final class URIUtility {
private URIUtility() {
}
    /**
     * Specifies whether certain characters are allowed when parsing IRIs and URIs.
     */
    enum ParseMode {
    /**
     * The rules follow the syntax for parsing IRIs. In particular, many
     * internationalized characters are allowed. Strings with unpaired
     * surrogate code points are considered invalid.
     */
      IRIStrict,

    /**
     * The rules follow the syntax for parsing IRIs, except that non-ASCII
     * characters are not allowed.
     */
      URIStrict,

    /**
     * The rules only check for the appropriate delimiters when splitting the path,
     * without checking if all the characters in each component are valid.
     * Even with this mode, strings with unpaired surrogate code points are
     * considered invalid.
     */
      IRILenient,

    /**
     * The rules only check for the appropriate delimiters when splitting the path,
     * without checking if all the characters in each component are valid.
     * Non-ASCII characters are not allowed.
     */
      URILenient,

    /**
     * The rules only check for the appropriate delimiters when splitting the path,
     * without checking if all the characters in each component are valid.
     * Unpaired surrogate code points are treated as though they were
     * replacement characters instead for the purposes of these rules, so
     * that strings with those code points are not considered invalid
     * strings.
     */
      IRISurrogateLenient
    }

    private static final String HexChars = "0123456789ABCDEF";

    private static void appendAuthority(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      if (segments[2] >= 0) {
        builder.append("//");
        builder.append(
          refValue.substring(
            segments[2], (
            segments[2]) + (segments[3] - segments[2])));
      }
    }

    private static void appendFragment(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      if (segments[8] >= 0) {
        builder.append('#');
        builder.append(
          refValue.substring(
            segments[8], (
            segments[8]) + (segments[9] - segments[8])));
      }
    }

    private static void appendNormalizedPath(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      builder.append(
        normalizePath(
          refValue.substring(
            segments[4], (
            segments[4]) + (segments[5] - segments[4]))));
    }

    private static void appendPath(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      builder.append(
        refValue.substring(
          segments[4], (
          segments[4]) + (segments[5] - segments[4])));
    }

    private static void appendQuery(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      if (segments[6] >= 0) {
        builder.append('?');
        builder.append(
          refValue.substring(
            segments[6], (
            segments[6]) + (segments[7] - segments[6])));
      }
    }

    private static void appendScheme(
      StringBuilder builder,
      String refValue,
      int[] segments) {
      if (segments[0] >= 0) {
        builder.append(
          refValue.substring(
            segments[0], (
            segments[0]) + (segments[1] - segments[0])));
        builder.append(':');
      }
    }

    /**
     * Escapes characters that cannot appear in URIs or IRIs. The function is
     * idempotent; that is, calling the function again on the result with
     * the same mode doesn't change the result.
     * @param s A string to escape.
     * @param mode A 32-bit signed integer.
     * @return A string object.
     */
    public static String escapeURI(String s, int mode) {
      if (s == null) {
        return null;
      }
      int[] components = null;
      if (mode == 1) {
        components = (
          s == null) ? null : splitIRI(
          s,
          0,
          s.length(),
          ParseMode.IRIStrict);
        if (components == null) {
          return null;
        }
      } else {
        components = (
          s == null) ? null : splitIRI(
          s,
          0,
          s.length(),
          ParseMode.IRISurrogateLenient);
      }
      int index = 0;
      int valueSLength = s.length();
      StringBuilder builder = new StringBuilder();
      while (index < valueSLength) {
        int c = s.charAt(index);
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            s.charAt(index + 1) >= 0xdc00 && s.charAt(index + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (s.charAt(index + 1) - 0xdc00);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          c = 0xfffd;
        }
        if (mode == 0 || mode == 3) {
          if (c == '%' && mode == 3) {
            // Check for illegal percent encoding
            if (index + 2 >= valueSLength || !isHexChar(s.charAt(index + 1)) ||
                !isHexChar(s.charAt(index + 2))) {
              percentEncodeUtf8(builder, c);
            } else {
              if (c <= 0xffff) {
                builder.append((char)c);
              } else if (c <= 0x10ffff) {
                builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) +
                                      0xd800));
                builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
            }
            ++index;
            continue;
          }
          if (c >= 0x7f || c <= 0x20 ||
              ((c & 0x7f) == c &&
               "{}|^\\`<>\"".indexOf((char)c) >= 0)) {
            percentEncodeUtf8(builder, c);
          } else if (c == '[' || c == ']') {
            if (components != null && index >= components[2] && index <
                components[3]) {
              // within the authority component, so don't percent-encode
              if (c <= 0xffff) {
                builder.append((char)c);
              } else if (c <= 0x10ffff) {
                builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) +
                                      0xd800));
                builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
            } else {
              // percent encode
              percentEncodeUtf8(builder, c);
            }
          } else {
            if (c <= 0xffff) {
              builder.append((char)c);
            } else if (c <= 0x10ffff) {
              builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        } else if (mode == 1 || mode == 2) {
          if (c >= 0x80) {
            percentEncodeUtf8(builder, c);
          } else if (c == '[' || c == ']') {
            if (components != null && index >= components[2] && index <
                components[3]) {
              // within the authority component, so don't percent-encode
              if (c <= 0xffff) {
                builder.append((char)c);
              } else if (c <= 0x10ffff) {
                builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) +
                                      0xd800));
                builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
            } else {
              // percent encode
              percentEncodeUtf8(builder, c);
            }
          } else {
            if (c <= 0xffff) {
              builder.append((char)c);
            } else if (c <= 0x10ffff) {
              builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
        ++index;
      }
      return builder.toString();
    }

    /**
     * Determines whether the string is a valid IRI with a scheme component. This
     * can be used to check for relative IRI references. <p>The following
     * cases return true:</p> <code> xx-x:mm example:/ww </code> The
     * following cases return false: <code> x@y:/z /x/y/z example.xyz
     * </code>
     * @param refValue A string object.
     * @return True if the string is a valid IRI with a scheme component;
     * otherwise, false.
     */
    public static boolean hasScheme(String refValue) {
      int[] segments = (
        refValue == null) ? null : splitIRI(
        refValue,
        0,
        refValue.length(),
        ParseMode.IRIStrict);
      return segments != null && segments[0] >= 0;
    }

    /**
     * Determines whether the string is a valid URI with a scheme component. This
     * can be used to check for relative URI references. The following cases
     * return true: <code> http://example/z xx-x:mm example:/ww </code> The
     * following cases return false: <code> x@y:/z /x/y/z example.xyz
     * </code>
     * @param refValue A string object.
     * @return True if the string is a valid URI with a scheme component;
     * otherwise, false.
     */
    public static boolean hasSchemeForURI(String refValue) {
      int[] segments = (refValue == null) ? null : splitIRI(
        refValue,
        0,
        refValue.length(),
        ParseMode.URIStrict);
      return segments != null && segments[0] >= 0;
    }

    private static boolean isHexChar(char c) {
      return (c >= 'a' && c <= 'f') ||
        (c >= 'A' && c <= 'F') ||
        (c >= '0' && c <= '9');
    }

    private static boolean isIfragmentChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/?-._~:@!$&'()*+,;=".indexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) ||
        (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static boolean isIpchar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/-._~:@!$&'()*+,;=".indexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) ||
        (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static boolean isIqueryChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "/?-._~:@!$&'()*+,;=".indexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) ||
        (c >= 0xe000 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0x10fffd && (c & 0xfffe) != 0xfffe);
    }

    private static boolean isIRegNameChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "-._~!$&'()*+,;=".indexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) ||
        (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    private static boolean isIUserInfoChar(int c) {
      // '%' omitted
      return (c >= 'a' && c <= 'z') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= '0' && c <= '9') ||
        ((c & 0x7F) == c && "-._~:!$&'()*+,;=".indexOf((char)c) >= 0) ||
        (c >= 0xa0 && c <= 0xd7ff) ||
        (c >= 0xf900 && c <= 0xfdcf) ||
        (c >= 0xfdf0 && c <= 0xffef) ||
        (c >= 0x10000 && c <= 0xefffd && (c & 0xfffe) != 0xfffe);
    }

    /**
     * Determines whether the substring is a valid CURIE reference under RDFA 1.1.
     * (The CURIE reference is the part after the colon.).
     * @param s A string object.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @return True if the substring is a valid CURIE reference under RDFA 1;
     * otherwise, false.
     */
    public static boolean isValidCurieReference(String s, int offset, int length) {
      if (s == null) {
        return false;
      }
      if (offset < 0) {
   throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0 ");
      }
      if (offset > s.length()) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          s.length());
      }
      if (length < 0) {
        throw new IllegalArgumentException(
          "length (" + length + ") is less than " + "0 ");
      }
      if (length > s.length()) {
        throw new IllegalArgumentException(
          "length (" + length + ") is more than " + s.length());
      }
      if (s.length() - offset < length) {
        throw new IllegalArgumentException(
          "s's length minus " + offset + " (" + (s.length() - offset) +
          ") is less than " + length);
      }
      if (length == 0) {
        return true;
      }
      int index = offset;
      int valueSLength = offset + length;
      int state = 0;
      if (index + 2 <= valueSLength && s.charAt(index) == '/' && s.charAt(index + 1) == '/') {
        // has an authority, which is not allowed
        return false;
      }
      state = 0;  // IRI Path
      while (index < valueSLength) {
        // Get the next Unicode character
        int c = s.charAt(index);
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            s.charAt(index + 1) >= 0xdc00 && s.charAt(index + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (s.charAt(index + 1) - 0xdc00);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          // error
          return false;
        }
        if (c == '%') {
          // Percent encoded character
          if (index + 2 < valueSLength && isHexChar(s.charAt(index + 1)) &&
              isHexChar(s.charAt(index + 2))) {
            index += 3;
            continue;
          }
          return false;
        }
        if (state == 0) {  // Path
          if (c == '?') {
            state = 1;  // move to query state
          } else if (c == '#') {
            state = 2;  // move to fragment state
          } else if (!isIpchar(c)) {
            return false;
          }
          ++index;
        } else if (state == 1) {  // Query
          if (c == '#') {
            state = 2;  // move to fragment state
          } else if (!isIqueryChar(c)) {
            return false;
          }
          ++index;
        } else if (state == 2) {  // Fragment
          if (!isIfragmentChar(c)) {
            return false;
          }
          ++index;
        }
      }
      return true;
    }

    public static boolean isValidIRI(String s) {
      return ((
        s == null) ? null : splitIRI(
                s,
                0,
                s.length(),
                ParseMode.IRIStrict)) != null;
    }

    private static String normalizePath(String path) {
      int len = path.length();
      if (len == 0 || path.equals("..") || path.equals(".")) {
        return "";
      }
      if (path.indexOf("/. ") < 0 &&
          path.indexOf(
"./ ") < 0) {
        return path;
      }
      StringBuilder builder = new StringBuilder();
      int index = 0;
      while (index < len) {
        char c = path.charAt(index);
        if ((index + 3 <= len && c == '/' &&
             path.charAt(index + 1) == '.' &&
             path.charAt(index + 2) == '/') ||
            (index + 2 == len && c == '.' &&
             path.charAt(index + 1) == '.')) {
          // begins with "/./" or is "..";
          // move index by 2
          index += 2;
          continue;
        }
        if (index + 3 <= len && c == '.' &&
            path.charAt(index + 1) == '.' &&
            path.charAt(index + 2) == '/') {
          // begins with "../";
          // move index by 3
          index += 3;
          continue;
        }
        if ((index + 2 <= len && c == '.' &&
             path.charAt(index + 1) == '/') ||
            (index + 1 == len && c == '.')) {
          // begins with "./" or is ".";
          // move index by 1
          ++index;
          continue;
        }
        if (index + 2 == len && c == '/' &&
            path.charAt(index + 1) == '.') {
          // is "/."; append '/' and break
          builder.append('/');
          break;
        }
        if (index + 3 == len && c == '/' &&
            path.charAt(index + 1) == '.' &&
            path.charAt(index + 2) == '.') {
          // is "/.."; remove last segment,
          // append "/" and return
          int index2 = builder.length() - 1;
          while (index2 >= 0) {
            if (builder.charAt(index2) == '/') {
              break;
            }
            --index2;
          }
          if (index2 < 0) {
            index2 = 0;
          }
          builder.setLength(index2);
          builder.append('/');
          break;
        }
        if (index + 4 <= len && c == '/' &&
            path.charAt(index + 1) == '.' &&
            path.charAt(index + 2) == '.' &&
            path.charAt(index + 3) == '/') {
          // begins with "/../"; remove last segment
          int index2 = builder.length() - 1;
          while (index2 >= 0) {
            if (builder.charAt(index2) == '/') {
              break;
            }
            --index2;
          }
          if (index2 < 0) {
            index2 = 0;
          }
          builder.setLength(index2);
          index += 3;
          continue;
        }
        builder.append(c);
        ++index;
        while (index < len) {
          // Move the rest of the
          // path segment until the next '/'
          c = path.charAt(index);
          if (c == '/') {
            break;
          }
          builder.append(c);
          ++index;
        }
      }
      return builder.toString();
    }

    private static int parseDecOctet(
      String s,
      int index,
      int endOffset,
      int c,
      int delim) {
      if (c >= '1' && c <= '9' && index + 2 < endOffset &&
          s.charAt(index + 1) >= '0' && s.charAt(index + 1) <= '9' &&
          s.charAt(index + 2) == delim) {
        return ((c - '0') * 10) + (s.charAt(index + 1) - '0');
      }
      if (c == '2' && index + 3 < endOffset &&
          (s.charAt(index + 1) == '5') &&
          (s.charAt(index + 2) >= '0' && s.charAt(index + 2) <= '5') &&
          s.charAt(index + 3) == delim) {
        return 250 + (s.charAt(index + 2) - '0');
      }
      if (c == '2' && index + 3 < endOffset &&
          s.charAt(index + 1) >= '0' && s.charAt(index + 1) <= '4' &&
          s.charAt(index + 2) >= '0' && s.charAt(index + 2) <= '9' &&
          s.charAt(index + 3) == delim) {
        return 200 + ((s.charAt(index + 1) - '0') * 10) + (s.charAt(index + 2) - '0');
      }
      if (c == '1' && index + 3 < endOffset &&
          s.charAt(index + 1) >= '0' && s.charAt(index + 1) <= '9' &&
          s.charAt(index + 2) >= '0' && s.charAt(index + 2) <= '9' &&
          s.charAt(index + 3) == delim) {
        return 100 + ((s.charAt(index + 1) - '0') * 10) + (s.charAt(index + 2) - '0');
      }
      return (c >= '0' && c <= '9' && index + 1 < endOffset &&
              s.charAt(index + 1) == delim) ? (c - '0') : (-1);
    }

    private static int parseIPLiteral(String s, int offset, int endOffset) {
      int index = offset;
      if (offset == endOffset) {
        return -1;
      }
      // Assumes that the character before offset
      // is a '['
      if (s.charAt(index) == 'v') {
        // IPvFuture
        ++index;
        boolean hex = false;
        while (index < endOffset) {
          char c = s.charAt(index);
          if (isHexChar(c)) {
            hex = true;
          } else {
            break;
          }
          ++index;
        }
        if (!hex) {
          return -1;
        }
        if (index >= endOffset || s.charAt(index) != '.') {
          return -1;
        }
        ++index;
        hex = false;
        while (index < endOffset) {
          char c = s.charAt(index);
          if ((c >= 'a' && c <= 'z') ||
              (c >= 'A' && c <= 'Z') ||
              (c >= '0' && c <= '9') ||
              ((c & 0x7F) == c && ":-._~!$&'()*+,;=".indexOf(c) >= 0)) {
            hex = true;
          } else {
            break;
          }
          ++index;
        }
        if (!hex) {
          return -1;
        }
        if (index >= endOffset || s.charAt(index) != ']') {
          return -1;
        }
        ++index;
        return index;
      }
      if (s.charAt(index) == ':' ||
          isHexChar(s.charAt(index))) {
        // IPv6 Address
        int phase1 = 0;
        int phase2 = 0;
        boolean phased = false;
        boolean expectHex = false;
        boolean expectColon = false;
        while (index < endOffset) {
          char c = s.charAt(index);
          if (c == ':' && !expectHex) {
            if ((phase1 + (phased ? 1 : 0) + phase2) >= 8) {
              return -1;
            }
            ++index;
            if (index < endOffset && s.charAt(index) == ':') {
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
            int decOctet = parseDecOctet(s, index, endOffset, c, '.');
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
                char tmpc = (index < endOffset) ? s.charAt(index) : '\0';
                decOctet = parseDecOctet(
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
                tmpc = (index < endOffset) ? s.charAt(index) : '\0';
                decOctet = parseDecOctet(s, index, endOffset, tmpc, '.');
                if (decOctet >= 100) {
                  index += 4;
                } else if (decOctet >= 10) {
                  index += 3;
                } else if (decOctet >= 0) {
                  index += 2;
                } else {
                  return -1;
                }
                tmpc = (index < endOffset) ? s.charAt(index) : '\0';
                decOctet = parseDecOctet(s, index, endOffset, tmpc, ']');
                if (decOctet < 0) {
                  tmpc = (index < endOffset) ? s.charAt(index) : '\0';
                  decOctet = parseDecOctet(s, index, endOffset, tmpc, '%');
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
          if (isHexChar(c) && !expectColon) {
            if (phased) {
              ++phase2;
            } else {
              ++phase1;
            }
            ++index;
            for (int i = 0; i < 3; ++i) {
              if (index < endOffset && isHexChar(s.charAt(index))) {
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
        if (s.charAt(index) != ']' && s.charAt(index) != '%') {
          return -1;
        }
        if (s.charAt(index) == '%') {
          if (index + 2 < endOffset && s.charAt(index + 1) == '2' &&
              s.charAt(index + 2) == '5') {
            // Zone identifier in an IPv6 address
            // (see RFC6874)
            index += 3;
            boolean haveChar = false;
            while (index < endOffset) {
              char c = s.charAt(index);
              if (c == ']') {
                return haveChar ? index + 1 : -1;
              }
              if (c == '%') {
                if (index + 2 < endOffset && isHexChar(s.charAt(index + 1)) &&
                    isHexChar(s.charAt(index + 2))) {
                  index += 3;
                  haveChar = true;
                  continue;
                }
                return -1;
              }
              if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
             (c >= '0' && c <= '9') || c == '.' || c == '_' || c == '-' ||
                    c ==
                  '~') {
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

    private static String pathParent(
      String refValue,
      int startIndex,
      int endIndex) {
      if (startIndex > endIndex) {
        return "";
      }
      --endIndex;
      while (endIndex >= startIndex) {
        if (refValue.charAt(endIndex) == '/') {
          return refValue.substring(startIndex, (startIndex) + ((endIndex + 1) - startIndex));
        }
        --endIndex;
      }
      return "";
    }

    private static void percentEncode(StringBuilder buffer, int b) {
      buffer.append('%');
      buffer.append(HexChars.charAt((b >> 4) & 0x0f));
      buffer.append(HexChars.charAt(b & 0x0f));
    }

    private static void percentEncodeUtf8(StringBuilder buffer, int cp) {
      if (cp <= 0x7f) {
        buffer.append('%');
        buffer.append(HexChars.charAt((cp >> 4) & 0x0f));
        buffer.append(HexChars.charAt(cp & 0x0f));
      } else if (cp <= 0x7ff) {
        percentEncode(buffer, 0xc0 | ((cp >> 6) & 0x1f));
        percentEncode(buffer, 0x80 | (cp & 0x3f));
      } else if (cp <= 0xffff) {
        percentEncode(buffer, 0xe0 | ((cp >> 12) & 0x0f));
        percentEncode(buffer, 0x80 | ((cp >> 6) & 0x3f));
        percentEncode(buffer, 0x80 | (cp & 0x3f));
      } else {
        percentEncode(buffer, 0xf0 | ((cp >> 18) & 0x07));
        percentEncode(buffer, 0x80 | ((cp >> 12) & 0x3f));
        percentEncode(buffer, 0x80 | ((cp >> 6) & 0x3f));
        percentEncode(buffer, 0x80 | (cp & 0x3f));
      }
    }

    /**
     * Resolves a URI or IRI relative to another URI or IRI.
     * @param refValue A string object. (2).
     * @param baseURI A string object. (3).
     * @return A string object.
     */
    public static String relativeResolve(String refValue, String baseURI) {
      return relativeResolve(refValue, baseURI, ParseMode.IRIStrict);
    }

    /**
     * Resolves a URI or IRI relative to another URI or IRI.
     * @param refValue A string object.
     * @param baseURI A string object. (2).
     * @param parseMode A ParseMode object.
     * @return The resolved IRI, or null if refValue is null or is not a valid IRI.
     * If base is null or is not a valid IRI, returns refValue.
     */
    public static String relativeResolve(
      String refValue,
      String baseURI,
      ParseMode parseMode) {
      int[] segments = (
        refValue == null) ? null : splitIRI(
        refValue,
        0,
        refValue.length(),
        parseMode);
      if (segments == null) {
        return null;
      }
      int[] segmentsBase = (
        baseURI == null) ? null : splitIRI(
        baseURI,
        0,
        baseURI.length(),
        parseMode);
      if (segmentsBase == null) {
        return refValue;
      }
      StringBuilder builder = new StringBuilder();
      if (segments[0] >= 0) {  // scheme present
        appendScheme(builder, refValue, segments);
        appendAuthority(builder, refValue, segments);
        appendNormalizedPath(builder, refValue, segments);
        appendQuery(builder, refValue, segments);
        appendFragment(builder, refValue, segments);
      } else if (segments[2] >= 0) {  // authority present
        appendScheme(builder, baseURI, segmentsBase);
        appendAuthority(builder, refValue, segments);
        appendNormalizedPath(builder, refValue, segments);
        appendQuery(builder, refValue, segments);
        appendFragment(builder, refValue, segments);
      } else if (segments[4] == segments[5]) {
        appendScheme(builder, baseURI, segmentsBase);
        appendAuthority(builder, baseURI, segmentsBase);
        appendPath(builder, baseURI, segmentsBase);
        if (segments[6] >= 0) {
          appendQuery(builder, refValue, segments);
        } else {
          appendQuery(builder, baseURI, segmentsBase);
        }
        appendFragment(builder, refValue, segments);
      } else {
        appendScheme(builder, baseURI, segmentsBase);
        appendAuthority(builder, baseURI, segmentsBase);
        if (segments[4] < segments[5] && refValue.charAt(segments[4]) == '/') {
          appendNormalizedPath(builder, refValue, segments);
        } else {
          StringBuilder merged = new StringBuilder();
          if (segmentsBase[2] >= 0 && segmentsBase[4] == segmentsBase[5]) {
            merged.append('/');
            appendPath(merged, refValue, segments);
            builder.append(normalizePath(merged.toString()));
          } else {
            merged.append(
              pathParent(
                baseURI,
                segmentsBase[4],
                segmentsBase[5]));
            appendPath(merged, refValue, segments);
            builder.append(normalizePath(merged.toString()));
          }
        }
        appendQuery(builder, refValue, segments);
        appendFragment(builder, refValue, segments);
      }
      return builder.toString();
    }

    /**
     * Parses an Internationalized Resource Identifier (IRI) reference under
     * RFC3987. If the IRI reference is syntactically valid, splits the
     * string into its components and returns an array containing the
     * indices into the components. <returns>If the string is a valid IRI
     * reference, returns an array of 10 integers. Each of the five pairs
     * corresponds to the start and end index of the IRI's scheme,
     * authority, path, query, or fragment component, respectively. If a
     * component is absent, both indices in that pair will be -1. If the
     * string is null or is not a valid IRI, returns null.</returns>
     * @param s A string object.
     * @return If the string is a valid IRI reference, returns an array of 10
     * integers. Each of the five pairs corresponds to the start and end
     * index of the IRI's scheme, authority, path, query, or fragment
     * component, respectively. If a component is absent, both indices in
     * that pair will be -1. If the string is null or is not a valid IRI,
     * returns null.
     */
    public static int[] splitIRI(String s) {
      return (s == null) ? null : splitIRI(s, 0, s.length(), ParseMode.IRIStrict);
    }

    /**
     * Parses a substring that represents an Internationalized Resource Identifier
     * (IRI) under RFC3987. If the IRI is syntactically valid, splits the
     * string into its components and returns an array containing the
     * indices into the components.
     * @param s A string object.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @param parseMode A ParseMode object.
     * @return If the string is a valid IRI, returns an array of 10 integers. Each
     * of the five pairs corresponds to the start and end index of the IRI's
     * scheme, authority, path, query, or fragment component, respectively.
     * If a component is absent, both indices in that pair will be -1 (an
     * index won't be less than 0 in any other case). If the string is null
     * or is not a valid IRI, returns null.
     */
    public static int[] splitIRI(
      String s,
      int offset,
      int length,
      ParseMode parseMode) {
      if (s == null) {
        return null;
      }
      if (offset < 0 || length < 0 || offset + length > s.length()) {
        throw new IllegalArgumentException();
      }
      int[] retval = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
      if (length == 0) {
        retval[4] = 0;
        retval[5] = 0;
        return retval;
      }
      boolean asciiOnly = parseMode == ParseMode.URILenient || parseMode ==
        ParseMode.URIStrict;
      boolean strict = parseMode == ParseMode.URIStrict || parseMode ==
        ParseMode.IRIStrict;
      int index = offset;
      int valueSLength = offset + length;
      boolean scheme = false;
      // scheme
      while (index < valueSLength) {
        int c = s.charAt(index);
        if (index > offset && c == ':') {
          scheme = true;
          retval[0] = offset;
          retval[1] = index;
          ++index;
          break;
        }
        if (strict && index == offset && !((c >= 'a' && c <= 'z') ||
                                           (c >= 'A' && c <=
                                                               'Z'))) {
          break;
        }
        if (strict && index > offset &&
        !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' &&
              c <=
                                                                   '9') ||
              c == '+' || c == '-' || c == '.')) {
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
      int state = 0;
      if (index + 2 <= valueSLength && s.charAt(index) == '/' && s.charAt(index + 1) == '/') {
        // authority
        // (index + 2, valueSLength)
        index += 2;
        int authorityStart = index;
        retval[2] = authorityStart;
        retval[3] = valueSLength;
        state = 0;  // userinfo
        // Check for userinfo
        while (index < valueSLength) {
          int c = s.charAt(index);
          if (asciiOnly && c >= 0x80) {
            return null;
          }
          if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
              s.charAt(index + 1) >= 0xdc00 && s.charAt(index + 1) <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) << 10) + (s.charAt(index + 1) - 0xdc00);
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
            if (index + 2 < valueSLength && isHexChar(s.charAt(index + 1)) &&
                isHexChar(s.charAt(index + 2))) {
              index += 3;
              continue;
            }
            return null;
          }
          if (state == 0) {  // User info
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
            if (strict && isIUserInfoChar(c)) {
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
          } else if (state == 1) {  // host
            if (c == '/' || c == '?' || c == '#') {
              // end of authority
              retval[3] = index;
              break;
            }
            if (!strict) {
              ++index;
            } else if (c == '[') {
              ++index;
              index = parseIPLiteral(s, index, valueSLength);
              if (index < 0) {
                return null;
              }
              continue;
            } else if (c == ':') {
              // port
              state = 2;
              ++index;
            } else if (isIRegNameChar(c)) {
              // is valid host name char
              // (note: IPv4 addresses included
              // in ireg-name)
              ++index;
            } else {
              return null;
            }
          } else if (state == 2) {  // Port
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
      boolean colon = false;
      boolean segment = false;
      boolean fullyRelative = index == offset;
      retval[4] = index;  // path offsets
      retval[5] = valueSLength;
      state = 0;  // IRI Path
      while (index < valueSLength) {
        // Get the next Unicode character
        int c = s.charAt(index);
        if (asciiOnly && c >= 0x80) {
          return null;
        }
        if ((c & 0xfc00) == 0xd800 && index + 1 < valueSLength &&
            s.charAt(index + 1) >= 0xdc00 && s.charAt(index + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (s.charAt(index + 1) - 0xdc00);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          // error
          return null;
        }
        if (c == '%' && strict) {
          // Percent encoded character
          if (index + 2 < valueSLength && isHexChar(s.charAt(index + 1)) &&
              isHexChar(s.charAt(index + 2))) {
            index += 3;
            continue;
          }
          return null;
        }
        if (state == 0) {  // Path
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
            state = 1;  // move to query state
          } else if (c == '#') {
            retval[5] = index;
            retval[8] = index + 1;
            retval[9] = valueSLength;
            state = 2;  // move to fragment state
          } else if (strict && !isIpchar(c)) {
            return null;
          }
          ++index;
        } else if (state == 1) {  // Query
          if (c == '#') {
            retval[7] = index;
            retval[8] = index + 1;
            retval[9] = valueSLength;
            state = 2;  // move to fragment state
          } else if (strict && !isIqueryChar(c)) {
            return null;
          }
          ++index;
        } else if (state == 2) {  // Fragment
          if (strict && !isIfragmentChar(c)) {
            return null;
          }
          ++index;
        }
      }
      if (strict && fullyRelative && colon && !segment) {
        return null;  // ex. "x@y:z"
      }
      return retval;
    }

    /**
     * Parses an Internationalized Resource Identifier (IRI) reference under
     * RFC3987. If the IRI is syntactically valid, splits the string into
     * its components and returns an array containing the indices into the
     * components.
     * @param s A string object.
     * @param parseMode A ParseMode object.
     * @return If the string is a valid IRI reference, returns an array of 10
     * integers. Each of the five pairs corresponds to the start and end
     * index of the IRI's scheme, authority, path, query, or fragment
     * component, respectively. If a component is absent, both indices in
     * that pair will be -1. If the string is null or is not a valid IRI,
     * returns null.
     */
    public static int[] splitIRI(String s, ParseMode parseMode) {
      return (s == null) ? null : splitIRI(s, 0, s.length(), parseMode);
    }
  }
