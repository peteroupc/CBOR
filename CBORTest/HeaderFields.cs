/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace CBORTest
{
  internal interface IHeaderFieldParser {
    bool IsStructured();
    string ReplaceEncodedWords(string str);
  }
  internal interface ITokener {
    int GetState();
    void RestoreState(int state);
    void Commit(int token, int startIndex, int endIndex);
  }

  internal class HeaderFields
  {
    private sealed class Tokener : ITokener, IComparer<int[]> {
      List<int[]> tokenStack = new List<int[]>();
      public int GetState() {
        return tokenStack.Count;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='state'>A 32-bit signed integer.</param>
      public void RestoreState(int state) {
        #if DEBUG
        if (state > tokenStack.Count) {
          throw new ArgumentException("state (" + Convert.ToString((long)(state), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(tokenStack.Count), System.Globalization.CultureInfo.InvariantCulture));
        }
        if (state < 0) {
          throw new ArgumentException("state (" + Convert.ToString((long)(state), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        #endif
        //if (tokenStack.Count != state) {
  //Console.WriteLine("Rolling back from " + tokenStack.Count + " to " + (state));
//}
        while (state<tokenStack.Count) {
          tokenStack.RemoveAt(state);
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='token'>A 32-bit signed integer.</param>
    /// <param name='startIndex'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      public void Commit(int token, int startIndex, int endIndex) {
        //Console.WriteLine("Committing token " + token + ", size now " + (tokenStack.Count+1));
        tokenStack.Add(new int[] { token, startIndex, endIndex});
      }

    /// <summary>Not documented yet.</summary>
      public void Clear() {
        tokenStack.Clear();
      }
      public IList<int[]> GetTokens() {
        tokenStack.Sort(this);
        return tokenStack;
      }

    /// <summary>Compares a int[] object with a int[].</summary>
    /// <param name='x'>A int[] object.</param>
    /// <param name='y'>A int[] object. (2).</param>
    /// <returns>Zero if both values are equal; a negative number if 'x' is
    /// less than 'y', or a positive number if 'x' is greater than 'y'.</returns>
public int Compare(int[] x, int[] y) {
        // Sort by their start indexes
        if (x[1]==y[1]) {
          // Sort by their token numbers
          return (x[0]==y[0]) ? 0 : ((x[0]<y[0]) ? -1 : 1);
        }
        return ((x[1]<y[1]) ? -1 : 1);
      }
    }

    private class UnstructuredHeaderField : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        // For unstructured header fields.
        return Message.ReplaceEncodedWords(str);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
public bool IsStructured() {
        return false;
      }
    }

    private sealed class NoCommentsOrEncodedWords : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        // For structured header fields that don't allow comments
        return str;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
public bool IsStructured() {
        return true;
      }
    }

    private sealed class EncodedWordsInComments : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        #if DEBUG
        if ((str) == null) {
          throw new ArgumentNullException("str");
        }
        #endif

        // For structured header fields that allow comments only wherever whitespace
        // is allowed, and allow parentheses only for comments
        if (str.Length< 9) {
          // too short for encoded words
          return str;
        }
        if (str.IndexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        if (str.IndexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        int lastIndex = 0;
        for (int i = 0;i<str.Length; ++i) {
          if (str[i]=='(') {
            int endIndex = HeaderParser.ParseComment(str, i, str.Length, null);
            if (endIndex != i) {
              // This is a comment, so replace any encoded words
              // in the comment
              string newComment = Message.ReplaceEncodedWords(str, i + 1, endIndex-1, true);
              sb.Append(str.Substring(lastIndex, (i + 1)-lastIndex));
              sb.Append(newComment);
              lastIndex = endIndex-1;
              // Set i to the end of the comment, since
              // comments can nest
              i = endIndex;
            }
          }
        }
        sb.Append(str.Substring(lastIndex, str.Length-lastIndex));
        return sb.ToString();
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
public bool IsStructured() {
        return true;
      }
    }

    private abstract class EncodedWordsInSyntax : IHeaderFieldParser {
      protected abstract int Parse(string str, int index, int endIndex, ITokener tokener);

      private static bool FollowedByEndOrLinearWhitespace(string str, int index, int endIndex) {
        if (index == endIndex) {
 return true;
}
        if (str[index]!=0x09 && str[index]!=0x20 && str[index]!=0x0d) {
 return false;
}
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return false;
        }
        return true;
      }
      private static bool PrecededByStartOrLinearWhitespace(string str, int index) {
        if (index == 0) {
 return true;
}
        if (index-1 >= 0 && (str[index-1]==0x09 || str[index-1]==0x20)) {
          return true;
        }
        return false;
      }
      private static int IndexOfNextPossibleEncodedWord(string str, int index, int endIndex) {
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return -1;
        }
        while (index<cws) {
          if (str[index]=='(') {
            // Has a comment, so no encoded word
            // immediately follows
            return -1;
          }
          ++index;
        }
        if (index+1<endIndex && str[index]=='=' && str[index+1]=='?') {
          // Has a possible encoded word
          return index;
        }
        return -1;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        #if DEBUG
        if ((str) == null) {
          throw new ArgumentNullException("str");
        }
        #endif

        // For structured header fields that allow comments only wherever whitespace
        // is allowed, and allow parentheses only for comments
        if (str.Length < 9) {
          // too short for encoded words
          return str;
        }
        if (str.IndexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        Tokener tokener = new Tokener();
        int endIndex = Parse(str, 0, str.Length, tokener);
        if (endIndex == 0) {
          // The header field is syntactically invalid,
          // so don't decode any encoded words
          // TODO: Reenable this comment when ready
          Console.WriteLine("Invalid syntax: "+this.GetType().Name+", "+str);
          return str;
        }
        int lastIndex = 0;
        int lastPhraseStart=-1;
        int lastPhraseEnd=-1;
        // Get each relevant token sorted by starting index
        foreach(int[] token in tokener.GetTokens()) {
          if (token[0]==1) {
            // This is a comment token
            int startIndex = token[1];
            endIndex = token[2];
            string newComment = Message.ReplaceEncodedWords(str, startIndex + 1, endIndex-1, true);
            sb.Append(str.Substring(lastIndex, startIndex + 1-lastIndex));
            sb.Append(newComment);
            lastIndex = endIndex-1;
          } else if (token[0]==2) {
            // This is a phrase token
            lastPhraseStart = token[1];
            lastPhraseEnd = token[2];
          } else if (token[0]==3) {
            // This is an atom token; only words within
            // a phrase can be encoded words
            if (token[1]>= lastIndex &&
               token[1]>= lastPhraseStart && token[1]<= lastPhraseEnd &&
               token[2]>= lastPhraseStart && token[2]<= lastPhraseEnd) {
              // This is an atom within a phrase
              int wordStart = HeaderParser.ParseCFWS(str, token[1], token[2], null);
              int wordEnd;
              int previousWord = wordStart;
              if (wordStart>= token[2] || str[wordStart]!='=') {
                // Not an encoded word
                continue;
              }
              wordEnd = wordStart;
              while (true) {
                if (!PrecededByStartOrLinearWhitespace(str, wordEnd)) {
                  break;
                }
                // Find the end of the atom
                while (wordEnd < lastPhraseEnd && ((str[wordEnd] >= 47 && str[wordEnd] <= 57) ||
                                                   (str[wordEnd] == 33) ||
                                                   (str[wordEnd] >= 35 && str[wordEnd] <= 39) ||
                                                   (str[wordEnd] == 42) || (str[wordEnd] == 43) ||
                                                   (str[wordEnd] == 45) ||
                                                   (str[wordEnd] == 61) || (str[wordEnd] == 63) ||
                                                   (str[wordEnd] >= 94 && str[wordEnd] <= 126) ||
                                                   (str[wordEnd] >= 65 && str[wordEnd] <= 90))) {
                  ++wordEnd;
                }
                if (!FollowedByEndOrLinearWhitespace(str, wordEnd, lastPhraseEnd)) {
                  // The encoded word is not followed by whitespace, so it's not valid
                  wordEnd = previousWord;
                  break;
                }
                int nextWord = IndexOfNextPossibleEncodedWord(str, wordEnd, lastPhraseEnd);
                if (nextWord< 0) {
                  // The next word isn't an encoded word
                  break;
                }
                previousWord = nextWord;
                wordEnd = nextWord;
              }
              string replacement = Message.ReplaceEncodedWords(str, wordStart, wordEnd, false);
              sb.Append(str.Substring(lastIndex, wordStart-lastIndex));
              sb.Append(replacement);
              lastIndex = wordEnd;
            }
          }
        }
        sb.Append(str.Substring(lastIndex, str.Length-lastIndex));
        return sb.ToString();
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
public bool IsStructured() {
        return true;
      }
    }

    class HeaderAuthenticationResults : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(str, index, endIndex, tokener);
      }
    }
    class HeaderAutoSubmitted : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoSubmitted(str, index, endIndex, tokener);
      }
    }
    class HeaderBcc : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }
    class HeaderContentBase : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentBase(str, index, endIndex, tokener);
      }
    }
    class HeaderContentDisposition : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(str, index, endIndex, tokener);
      }
    }
    class HeaderContentId : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }
    class HeaderContentType : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentType(str, index, endIndex, tokener);
      }
    }
    class HeaderDate : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }
    class HeaderDispositionNotificationOptions : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(str, index, endIndex, tokener);
      }
    }
    class HeaderDispositionNotificationTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(str, index, endIndex, tokener);
      }
    }
    class HeaderEncrypted : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }
    class HeaderFrom : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }
    class HeaderInReplyTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }
    class HeaderKeywords : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }
    class HeaderLanguage : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }
    class HeaderListArchive : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListArchive(str, index, endIndex, tokener);
      }
    }
    class HeaderListId : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }
    class HeaderListPost : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListPost(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsCopyPrecedence : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsExemptedAddress : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsExtendedAuthorisationInfo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsMessageType : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageType(str, index, endIndex, tokener);
      }
    }
    class HeaderObsoletes : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }
    class HeaderOriginalRecipient : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(str, index, endIndex, tokener);
      }
    }
    class HeaderReceived : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }
    class HeaderReceivedSpf : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceivedSpf(str, index, endIndex, tokener);
      }
    }
    class HeaderReturnPath : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReturnPath(str, index, endIndex, tokener);
      }
    }
    class HeaderSender : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }
    class HeaderTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }
    class Mailbox : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXMittente(str, index, endIndex, tokener);
      }
    }

    private static IDictionary<string, IHeaderFieldParser> list = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static IDictionary<string, IHeaderFieldParser> CreateHeaderFieldList() {
      list = new Dictionary<string, IHeaderFieldParser>();
      IHeaderFieldParser structuredNoComments = new NoCommentsOrEncodedWords();
      IHeaderFieldParser structuredComments = new EncodedWordsInComments();
      IHeaderFieldParser unstructured = new UnstructuredHeaderField();
      // These structured header fields won't be parsed for comments.
      list["alternate-recipient"]=structuredNoComments;
      list["archived-at"]=structuredNoComments;
      list["autoforwarded"]=structuredNoComments;
      list["autosubmitted"]=structuredNoComments;
      list["content-alternative"]=structuredNoComments;
      list["content-features"]=structuredNoComments;
      list["content-return"]=structuredNoComments;
      list["conversion"]=structuredNoComments;
      list["conversion-with-loss"]=structuredNoComments;
      list["disclose-recipients"]=structuredNoComments;
      list["dkim-signature"]=structuredNoComments;
      list["ediint-features"]=structuredNoComments;
      list["generate-delivery-report"]=structuredNoComments;
      list["importance"]=structuredNoComments;
      list["incomplete-copy"]=structuredNoComments;
      list["jabber-id"]=structuredNoComments;
      list["mmhs-acp127-message-identifier"]=structuredNoComments;
      list["mmhs-codress-message-indicator"]=structuredNoComments;
      list["mmhs-handling-instructions"]=structuredNoComments;
      list["mmhs-message-instructions"]=structuredNoComments;
      list["mmhs-originator-plad"]=structuredNoComments;
      list["mmhs-originator-reference"]=structuredNoComments;
      list["mmhs-other-recipients-indicator-cc"]=structuredNoComments;
      list["mmhs-other-recipients-indicator-to"]=structuredNoComments;
      list["mmhs-subject-indicator-codes"]=structuredNoComments;
      list["original-subject"]=structuredNoComments;
      list["pics-label"]=structuredNoComments;
      list["prevent-nondelivery-report"]=structuredNoComments;
      list["priority"]=structuredNoComments;
      list["privicon"]=structuredNoComments;
      list["sensitivity"]=structuredNoComments;
      list["solicitation"]=structuredNoComments;
      list["vbr-info"]=structuredNoComments;
      list["x-archived-at"]=structuredNoComments;
      list["x400-content-identifier"]=structuredNoComments;
      list["x400-content-return"]=structuredNoComments;
      list["x400-mts-identifier"]=structuredNoComments;
      list["x400-received"]=structuredNoComments;
      list["x400-trace"]=structuredNoComments;
      // These structured header fields allow comments anywhere
      // they allow whitespace (thus, if a comment occurs anywhere
      // it can't appear, replacing it with a space will result
      // in a syntactically invalid header field).
      // They also don't allow parentheses outside of comments.
      list["accept-language"]=structuredComments;
      list["content-duration"]=structuredComments;
      list["content-language"]=structuredComments;
      list["content-md5"]=structuredComments;
      list["content-transfer-encoding"]=structuredComments;
      list["encoding"]=structuredComments;
      list["message-context"]=structuredComments;
      list["mime-version"]=structuredComments;
      list["mt-priority"]=structuredComments;
      list["x-ricevuta"]=structuredComments;
      list["x-tiporicevuta"]=structuredComments;
      list["x-trasporto"]=structuredComments;
      list["x-verificasicurezza"]=structuredComments;
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      list["apparently-to"]=unstructured;
      list["body"]=unstructured;
      list["comments"]=unstructured;
      list["content-description"]=unstructured;
      list["downgraded-bcc"]=unstructured;
      list["downgraded-cc"]=unstructured;
      list["downgraded-disposition-notification-to"]=unstructured;
      list["downgraded-final-recipient"]=unstructured;
      list["downgraded-from"]=unstructured;
      list["downgraded-in-reply-to"]=unstructured;
      list["downgraded-mail-from"]=unstructured;
      list["downgraded-message-id"]=unstructured;
      list["downgraded-original-recipient"]=unstructured;
      list["downgraded-rcpt-to"]=unstructured;
      list["downgraded-references"]=unstructured;
      list["downgraded-reply-to"]=unstructured;
      list["downgraded-resent-bcc"]=unstructured;
      list["downgraded-resent-cc"]=unstructured;
      list["downgraded-resent-from"]=unstructured;
      list["downgraded-resent-reply-to"]=unstructured;
      list["downgraded-resent-sender"]=unstructured;
      list["downgraded-resent-to"]=unstructured;
      list["downgraded-return-path"]=unstructured;
      list["downgraded-sender"]=unstructured;
      list["downgraded-to"]=unstructured;
      list["errors-to"]=unstructured;
      list["subject"]=unstructured;
      // These header fields have their own syntax rules.
      list["authentication-results"]=new HeaderAuthenticationResults();
      list["auto-submitted"]=new HeaderAutoSubmitted();
      list["base"]=new HeaderContentBase();
      list["bcc"]=new HeaderBcc();
      list["cc"]=new HeaderTo();
      list["content-base"]=new HeaderContentBase();
      list["content-disposition"]=new HeaderContentDisposition();
      list["content-id"]=new HeaderContentId();
      list["content-location"]=new HeaderContentBase();
      list["content-type"]=new HeaderContentType();
      list["date"]=new HeaderDate();
      list["deferred-delivery"]=new HeaderDate();
      list["delivery-date"]=new HeaderDate();
      list["disposition-notification-options"]=new HeaderDispositionNotificationOptions();
      list["disposition-notification-to"]=new HeaderDispositionNotificationTo();
      list["encrypted"]=new HeaderEncrypted();
      list["expires"]=new HeaderDate();
      list["expiry-date"]=new HeaderDate();
      list["from"]=new HeaderFrom();
      list["in-reply-to"]=new HeaderInReplyTo();
      list["keywords"]=new HeaderKeywords();
      list["language"]=new HeaderLanguage();
      list["latest-delivery-time"]=new HeaderDate();
      list["list-archive"]=new HeaderListArchive();
      list["list-help"]=new HeaderListArchive();
      list["list-id"]=new HeaderListId();
      list["list-owner"]=new HeaderListArchive();
      list["list-post"]=new HeaderListPost();
      list["list-subscribe"]=new HeaderListArchive();
      list["list-unsubscribe"]=new HeaderListArchive();
      list["message-id"]=new HeaderContentId();
      list["mmhs-copy-precedence"]=new HeaderMmhsCopyPrecedence();
      list["mmhs-exempted-address"]=new HeaderMmhsExemptedAddress();
      list["mmhs-extended-authorisation-info"]=new HeaderMmhsExtendedAuthorisationInfo();
      list["mmhs-message-type"]=new HeaderMmhsMessageType();
      list["mmhs-primary-precedence"]=new HeaderMmhsCopyPrecedence();
      list["obsoletes"]=new HeaderObsoletes();
      list["original-from"]=new HeaderFrom();
      list["original-message-id"]=new HeaderContentId();
      list["original-recipient"]=new HeaderOriginalRecipient();
      list["received"]=new HeaderReceived();
      list["received-spf"]=new HeaderReceivedSpf();
      list["references"]=new HeaderInReplyTo();
      list["reply-by"]=new HeaderDate();
      list["reply-to"]=new HeaderTo();
      list["resent-bcc"]=new HeaderBcc();
      list["resent-cc"]=new HeaderTo();
      list["resent-date"]=new HeaderDate();
      list["resent-from"]=new HeaderFrom();
      list["resent-message-id"]=new HeaderContentId();
      list["resent-reply-to"]=new HeaderTo();
      list["resent-sender"]=new HeaderSender();
      list["resent-to"]=new HeaderTo();
      list["return-path"]=new HeaderReturnPath();
      list["sender"]=new HeaderSender();
      list["to"]=new HeaderTo();
      list["x-mittente"]=new Mailbox();
      list["x-riferimento-message-id"]=new HeaderContentId();
      list["x400-originator"]=new Mailbox();
      list["x400-recipients"]=new HeaderDispositionNotificationTo();
      return list;
    }

    public static IHeaderFieldParser GetParser(string name) {
      #if DEBUG
      if ((name) == null) {
        throw new ArgumentNullException("name");
      }
      #endif

      name = ParserUtility.ToLowerCaseAscii(name);
      if (list.ContainsKey(name)) {
        return list[name];
      }
      return unstructured;
    }
  }
}
