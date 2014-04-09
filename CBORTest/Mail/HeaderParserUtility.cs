/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/8/2014
 * Time: 3:50 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal class HeaderParserUtility
  {
    internal const int TokenComment = 1;
    internal const int TokenPhraseAtom = 3;
    internal const int TokenPhraseAtomOrDot = 4;
    internal const int TokenPhrase = 2;
    internal const int TokenGroup = 5;
    internal const int TokenMailbox = 6;
    internal const int TokenQuotedString = 7;
    internal const int TokenLocalPart = 8;
    internal const int TokenDomain = 9;

    private static string ParseDotAtomAfterCFWS(string str, int index, int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a dot-atom
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          break;
        }
        builder.Append(str.Substring(start, index - start));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str[index] == '.') {
          builder.Append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.ToString();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <returns>A string object.</returns>
    public static string ParseLocalPart(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "local-part"
      // The local part is either a quoted string or a set of words
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str[index] == '"') {
        // It's a quoted string
        StringBuilder builder = new StringBuilder();
        MediaType.skipQuotedString(str, index, endIndex, builder);
        return builder.ToString();
      } else {
        // It's a dot-atom
        return ParseDotAtomAfterCFWS(str, index, endIndex);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <returns>A string object.</returns>
    public static string ParseDomain(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "domain"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str[index] == '[') {
        // It's a domain literal
        ++index;
        StringBuilder builder = new StringBuilder();
        builder.Append('[');
        while (index < endIndex) {
          index = HeaderParser.ParseFWS(str, index, endIndex, null);
          if (index >= endIndex) {
            break;
          }
          if (str[index] == ']') {
            break;
          }
          if (str[index] == '\\') {
            int startQuote = index;
            index = MediaType.skipQuotedPair(str, index, endIndex);
            if (index == startQuote) {
              builder.Append(str.Substring(startQuote + 1, index - (startQuote + 1)));
            } else {
              ++index;
            }
          } else {
            builder.Append(str[index]);
            ++index;
          }
        }
        builder.Append(']');
        return builder.ToString();
      } else {
        // It's a dot-atom
        return ParseDotAtomAfterCFWS(str, index, endIndex);
      }
    }

    public static IList<NamedAddress> ParseAddressList(string str, int index, int endIndex, IList<int[]> tokens) {
      int lastIndex = index;
      IList<NamedAddress> addresses = new List<NamedAddress>();
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenGroup) {
            addresses.Add(ParseGroup(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          } else if (tokenKind == TokenMailbox) {
            addresses.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          }
        }
      }
      return addresses;
    }

    public static NamedAddress ParseGroup(string str, int index, int endIndex, IList<int[]> tokens) {
      string displayName = null;
      bool haveDisplayName = false;
      IList<NamedAddress> mailboxes = new List<NamedAddress>();
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenPhrase && !haveDisplayName) {
            // Phrase
            displayName = ParsePhrase(str, tokenIndex, tokenEnd, tokens);
            haveDisplayName = true;
          } else if (tokenKind == TokenMailbox) {
            mailboxes.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
          }
        }
      }
      #if DEBUG
      if (displayName == null) {
        throw new ArgumentNullException("displayName");
      }
      #endif

      return new NamedAddress(displayName, mailboxes);
    }

    public static NamedAddress ParseMailbox(string str, int index, int endIndex, IList<int[]> tokens) {
      string displayName = null;
      string localPart = null;
      string domain = null;
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenPhrase) {
            // Phrase
            displayName = ParsePhrase(str, tokenIndex, tokenEnd, tokens);
          } else if (tokenKind == TokenLocalPart) {
            localPart = ParseLocalPart(str, tokenIndex, tokenEnd);
          } else if (tokenKind == TokenDomain) {
            domain = ParseDomain(str, tokenIndex, tokenEnd);
          }
        }
      }
      #if DEBUG
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      #endif

      return new NamedAddress(displayName, localPart, domain);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <param name='tokens'>An IList object.</param>
    /// <returns>A string object.</returns>
    public static string ParsePhrase(string str, int index, int endIndex, IList<int[]> tokens) {
      StringBuilder builder = new StringBuilder();
      bool appendSpace = false;
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          bool hasCFWS = false;
          if (tokenKind == TokenPhraseAtom || tokenKind == TokenPhraseAtomOrDot) {
            // Phrase atom
            if (appendSpace) {
              builder.Append(' ');
              appendSpace = false;
            }
            builder.Append(str.Substring(tokenIndex, tokenEnd - tokenIndex));
            hasCFWS = HeaderParser.ParseCFWS(str, tokenEnd, endIndex, null) != tokenEnd;
          } else if (tokenKind == TokenQuotedString) {
            if (appendSpace) {
              builder.Append(' ');
              appendSpace = false;
            }
            tokenIndex = MediaType.skipQuotedString(str, tokenIndex, tokenEnd, builder);
            // tokenIndex is now just after the end quote
            hasCFWS = HeaderParser.ParseCFWS(str, tokenIndex, endIndex, null) != tokenEnd;
          }
          if (hasCFWS) {
            // Add a space character if CFWS follows the atom or
            // quoted string and if additional words follow
            appendSpace = true;
          }
        }
      }
      // Replace encoded words in the string, if any
      return Message.ReplaceEncodedWords(builder.ToString());
    }
  }
}
