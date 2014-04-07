package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.util.*;

  interface IHeaderFieldParser {
    boolean IsStructured();
    String ReplaceEncodedWords(String str);
  }
  interface ITokener {
    int GetState();
    void RestoreState(int state);
    void Commit(int token, int startIndex, int endIndex);
  }

  class HeaderFields
  {
    private static final class Tokener implements ITokener, IComparer<int[]> {
      ArrayList<int[]> tokenStack = new ArrayList<int[]>();
      public int GetState() {
        return tokenStack.size();
      }

    /**
     * Not documented yet.
     * @param state A 32-bit signed integer.
     */
      public void RestoreState(int state) {

        //if (tokenStack.size() != state) {
  //System.out.println("Rolling back from " + tokenStack.size() + " to " + (state));
//}
        while (state<tokenStack.size()) {
          tokenStack.remove(state);
        }
      }

    /**
     * Not documented yet.
     * @param token A 32-bit signed integer.
     * @param startIndex A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     */
      public void Commit(int token, int startIndex, int endIndex) {
        //System.out.println("Committing token " + token + ", size now " + (tokenStack.size()+1));
        tokenStack.add(new int[] { token, startIndex, endIndex});
      }

    /**
     * Not documented yet.
     */
      public void Clear() {
        tokenStack.Clear();
      }
      public List<int[]> GetTokens() {
        tokenStack.Sort(this);
        return tokenStack;
      }

    /**
     * Compares a int[] object with a int[].
     * @param x A int[] object.
     * @param y A int[] object. (2).
     * @return Zero if both values are equal; a negative number if 'x' is less
     * than 'y', or a positive number if 'x' is greater than 'y'.
     */
public int Compare(int[] x, int[] y) {
        // Sort by their start indexes
        if (x[1]==y[1]) {
          // Sort by their token numbers
          return (x[0]==y[0]) ? 0 : ((x[0]<y[0]) ? -1 : 1);
        }
        return ((x[1]<y[1]) ? -1 : 1);
      }
    }

    private class UnstructuredHeaderField implements IHeaderFieldParser {
    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String ReplaceEncodedWords(String str) {
        // For unstructured header fields.
        return Message.ReplaceEncodedWords(str);
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
public boolean IsStructured() {
        return false;
      }
    }

    private static final class NoCommentsOrEncodedWords implements IHeaderFieldParser {
    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String ReplaceEncodedWords(String str) {
        // For structured header fields that don't allow comments
        return str;
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
public boolean IsStructured() {
        return true;
      }
    }

    private static final class EncodedWordsInComments implements IHeaderFieldParser {
    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String ReplaceEncodedWords(String str) {

        // For structured header fields that allow comments only wherever whitespace
        // is allowed, and allow parentheses only for comments
        if (str.length()< 9) {
          // too short for encoded words
          return str;
        }
        if (str.indexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        if (str.indexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        int lastIndex = 0;
        for (int i = 0;i<str.length(); ++i) {
          if (str.charAt(i)=='(') {
            int endIndex = HeaderParser.ParseComment(str, i, str.length(), null);
            if (endIndex != i) {
              // This is a comment, so replace any encoded words
              // in the comment
              String newComment = Message.ReplaceEncodedWords(str, i + 1, endIndex-1, true);
              sb.append(str.substring(lastIndex,(lastIndex)+((i + 1)-lastIndex)));
              sb.append(newComment);
              lastIndex = endIndex-1;
              // Set i to the end of the comment, since
              // comments can nest
              i = endIndex;
            }
          }
        }
        sb.append(str.substring(lastIndex,(lastIndex)+(str.length()-lastIndex)));
        return sb.toString();
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
public boolean IsStructured() {
        return true;
      }
    }

    private abstract class EncodedWordsInSyntax implements IHeaderFieldParser {
      protected abstract int Parse(String str, int index, int endIndex, ITokener tokener);

      private static boolean FollowedByEndOrLinearWhitespace(String str, int index, int endIndex) {
        if (index == endIndex) {
 return true;
}
        if (str.charAt(index)!=0x09 && str.charAt(index)!=0x20 && str.charAt(index)!=0x0d) {
 return false;
}
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return false;
        }
        return true;
      }
      private static boolean PrecededByStartOrLinearWhitespace(String str, int index) {
        if (index == 0) {
 return true;
}
        if (index-1 >= 0 && (str.charAt(index-1)==0x09 || str.charAt(index-1)==0x20)) {
          return true;
        }
        return false;
      }
      private static int IndexOfNextPossibleEncodedWord(String str, int index, int endIndex) {
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return -1;
        }
        while (index<cws) {
          if (str.charAt(index)=='(') {
            // Has a comment, so no encoded word
            // immediately follows
            return -1;
          }
          ++index;
        }
        if (index+1<endIndex && str.charAt(index)=='=' && str.charAt(index+1)=='?') {
          // Has a possible encoded word
          return index;
        }
        return -1;
      }

    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String ReplaceEncodedWords(String str) {

        // For structured header fields that allow comments only wherever whitespace
        // is allowed, and allow parentheses only for comments
        if (str.length() < 9) {
          // too short for encoded words
          return str;
        }
        if (str.indexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        Tokener tokener = new Tokener();
        int endIndex = Parse(str, 0, str.length(), tokener);
        if (endIndex == 0) {
          // The header field is syntactically invalid,
          // so don't decode any encoded words
          // TODO: Reenable this comment when ready
          System.out.println("Invalid syntax: "+this.getClass().getName()+", "+str);
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
            String newComment = Message.ReplaceEncodedWords(str, startIndex + 1, endIndex-1, true);
            sb.append(str.substring(lastIndex,(lastIndex)+(startIndex + 1-lastIndex)));
            sb.append(newComment);
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
              if (wordStart>= token[2] || str.charAt(wordStart)!='=') {
                // Not an encoded word
                continue;
              }
              wordEnd = wordStart;
              while (true) {
                if (!PrecededByStartOrLinearWhitespace(str, wordEnd)) {
                  break;
                }
                // Find the end of the atom
                while (wordEnd < lastPhraseEnd && ((str.charAt(wordEnd) >= 47 && str.charAt(wordEnd) <= 57) ||
                                                   (str.charAt(wordEnd) == 33) ||
                                                   (str.charAt(wordEnd) >= 35 && str.charAt(wordEnd) <= 39) ||
                                                   (str.charAt(wordEnd) == 42) || (str.charAt(wordEnd) == 43) ||
                                                   (str.charAt(wordEnd) == 45) ||
                                                   (str.charAt(wordEnd) == 61) || (str.charAt(wordEnd) == 63) ||
                                                   (str.charAt(wordEnd) >= 94 && str.charAt(wordEnd) <= 126) ||
                                                   (str.charAt(wordEnd) >= 65 && str.charAt(wordEnd) <= 90))) {
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
              String replacement = Message.ReplaceEncodedWords(str, wordStart, wordEnd, false);
              sb.append(str.substring(lastIndex,(lastIndex)+(wordStart-lastIndex)));
              sb.append(replacement);
              lastIndex = wordEnd;
            }
          }
        }
        sb.append(str.substring(lastIndex,(lastIndex)+(str.length()-lastIndex)));
        return sb.toString();
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
public boolean IsStructured() {
        return true;
      }
    }

    class HeaderAuthenticationResults : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(str, index, endIndex, tokener);
      }
    }
    class HeaderAutoSubmitted : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoSubmitted(str, index, endIndex, tokener);
      }
    }
    class HeaderBcc : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }
    class HeaderContentBase : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentBase(str, index, endIndex, tokener);
      }
    }
    class HeaderContentDisposition : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(str, index, endIndex, tokener);
      }
    }
    class HeaderContentId : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }
    class HeaderContentType : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentType(str, index, endIndex, tokener);
      }
    }
    class HeaderDate : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }
    class HeaderDispositionNotificationOptions : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(str, index, endIndex, tokener);
      }
    }
    class HeaderDispositionNotificationTo : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(str, index, endIndex, tokener);
      }
    }
    class HeaderEncrypted : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }
    class HeaderFrom : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }
    class HeaderInReplyTo : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }
    class HeaderKeywords : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }
    class HeaderLanguage : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }
    class HeaderListArchive : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListArchive(str, index, endIndex, tokener);
      }
    }
    class HeaderListId : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }
    class HeaderListPost : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListPost(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsCopyPrecedence : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsExemptedAddress : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsExtendedAuthorisationInfo : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(str, index, endIndex, tokener);
      }
    }
    class HeaderMmhsMessageType : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageType(str, index, endIndex, tokener);
      }
    }
    class HeaderObsoletes : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }
    class HeaderOriginalRecipient : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(str, index, endIndex, tokener);
      }
    }
    class HeaderReceived : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }
    class HeaderReceivedSpf : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceivedSpf(str, index, endIndex, tokener);
      }
    }
    class HeaderReturnPath : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReturnPath(str, index, endIndex, tokener);
      }
    }
    class HeaderSender : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }
    class HeaderTo : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }
    class Mailbox : EncodedWordsInSyntax {
      protected override int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXMittente(str, index, endIndex, tokener);
      }
    }

    private static Map<String, IHeaderFieldParser> list = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static Map<String, IHeaderFieldParser> CreateHeaderFieldList() {
      list = new HashMap<String, IHeaderFieldParser>();
      IHeaderFieldParser structuredNoComments = new NoCommentsOrEncodedWords();
      IHeaderFieldParser structuredComments = new EncodedWordsInComments();
      IHeaderFieldParser unstructured = new UnstructuredHeaderField();
      // These structured header fields won't be parsed for comments.
      list.set("alternate-recipient",structuredNoComments);
      list.set("archived-at",structuredNoComments);
      list.set("autoforwarded",structuredNoComments);
      list.set("autosubmitted",structuredNoComments);
      list.set("content-alternative",structuredNoComments);
      list.set("content-features",structuredNoComments);
      list.set("content-return",structuredNoComments);
      list.set("conversion",structuredNoComments);
      list.set("conversion-with-loss",structuredNoComments);
      list.set("disclose-recipients",structuredNoComments);
      list.set("dkim-signature",structuredNoComments);
      list.set("ediint-features",structuredNoComments);
      list.set("generate-delivery-report",structuredNoComments);
      list.set("importance",structuredNoComments);
      list.set("incomplete-copy",structuredNoComments);
      list.set("jabber-id",structuredNoComments);
      list.set("mmhs-acp127-message-identifier",structuredNoComments);
      list.set("mmhs-codress-message-indicator",structuredNoComments);
      list.set("mmhs-handling-instructions",structuredNoComments);
      list.set("mmhs-message-instructions",structuredNoComments);
      list.set("mmhs-originator-plad",structuredNoComments);
      list.set("mmhs-originator-reference",structuredNoComments);
      list.set("mmhs-other-recipients-indicator-cc",structuredNoComments);
      list.set("mmhs-other-recipients-indicator-to",structuredNoComments);
      list.set("mmhs-subject-indicator-codes",structuredNoComments);
      list.set("original-subject",structuredNoComments);
      list.set("pics-label",structuredNoComments);
      list.set("prevent-nondelivery-report",structuredNoComments);
      list.set("priority",structuredNoComments);
      list.set("privicon",structuredNoComments);
      list.set("sensitivity",structuredNoComments);
      list.set("solicitation",structuredNoComments);
      list.set("vbr-info",structuredNoComments);
      list.set("x-archived-at",structuredNoComments);
      list.set("x400-content-identifier",structuredNoComments);
      list.set("x400-content-return",structuredNoComments);
      list.set("x400-mts-identifier",structuredNoComments);
      list.set("x400-received",structuredNoComments);
      list.set("x400-trace",structuredNoComments);
      // These structured header fields allow comments anywhere
      // they allow whitespace (thus, if a comment occurs anywhere
      // it can't appear, replacing it with a space will result
      // in a syntactically invalid header field).
      // They also don't allow parentheses outside of comments.
      list.set("accept-language",structuredComments);
      list.set("content-duration",structuredComments);
      list.set("content-language",structuredComments);
      list.set("content-md5",structuredComments);
      list.set("content-transfer-encoding",structuredComments);
      list.set("encoding",structuredComments);
      list.set("message-context",structuredComments);
      list.set("mime-version",structuredComments);
      list.set("mt-priority",structuredComments);
      list.set("x-ricevuta",structuredComments);
      list.set("x-tiporicevuta",structuredComments);
      list.set("x-trasporto",structuredComments);
      list.set("x-verificasicurezza",structuredComments);
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      list.set("apparently-to",unstructured);
      list.set("body",unstructured);
      list.set("comments",unstructured);
      list.set("content-description",unstructured);
      list.set("downgraded-bcc",unstructured);
      list.set("downgraded-cc",unstructured);
      list.set("downgraded-disposition-notification-to",unstructured);
      list.set("downgraded-final-recipient",unstructured);
      list.set("downgraded-from",unstructured);
      list.set("downgraded-in-reply-to",unstructured);
      list.set("downgraded-mail-from",unstructured);
      list.set("downgraded-message-id",unstructured);
      list.set("downgraded-original-recipient",unstructured);
      list.set("downgraded-rcpt-to",unstructured);
      list.set("downgraded-references",unstructured);
      list.set("downgraded-reply-to",unstructured);
      list.set("downgraded-resent-bcc",unstructured);
      list.set("downgraded-resent-cc",unstructured);
      list.set("downgraded-resent-from",unstructured);
      list.set("downgraded-resent-reply-to",unstructured);
      list.set("downgraded-resent-sender",unstructured);
      list.set("downgraded-resent-to",unstructured);
      list.set("downgraded-return-path",unstructured);
      list.set("downgraded-sender",unstructured);
      list.set("downgraded-to",unstructured);
      list.set("errors-to",unstructured);
      list.set("subject",unstructured);
      // These header fields have their own syntax rules.
      list.set("authentication-results",new HeaderAuthenticationResults());
      list.set("auto-submitted",new HeaderAutoSubmitted());
      list.set("base",new HeaderContentBase());
      list.set("bcc",new HeaderBcc());
      list.set("cc",new HeaderTo());
      list.set("content-base",new HeaderContentBase());
      list.set("content-disposition",new HeaderContentDisposition());
      list.set("content-id",new HeaderContentId());
      list.set("content-location",new HeaderContentBase());
      list.set("content-type",new HeaderContentType());
      list.set("date",new HeaderDate());
      list.set("deferred-delivery",new HeaderDate());
      list.set("delivery-date",new HeaderDate());
      list.set("disposition-notification-options",new HeaderDispositionNotificationOptions());
      list.set("disposition-notification-to",new HeaderDispositionNotificationTo());
      list.set("encrypted",new HeaderEncrypted());
      list.set("expires",new HeaderDate());
      list.set("expiry-date",new HeaderDate());
      list.set("from",new HeaderFrom());
      list.set("in-reply-to",new HeaderInReplyTo());
      list.set("keywords",new HeaderKeywords());
      list.set("language",new HeaderLanguage());
      list.set("latest-delivery-time",new HeaderDate());
      list.set("list-archive",new HeaderListArchive());
      list.set("list-help",new HeaderListArchive());
      list.set("list-id",new HeaderListId());
      list.set("list-owner",new HeaderListArchive());
      list.set("list-post",new HeaderListPost());
      list.set("list-subscribe",new HeaderListArchive());
      list.set("list-unsubscribe",new HeaderListArchive());
      list.set("message-id",new HeaderContentId());
      list.set("mmhs-copy-precedence",new HeaderMmhsCopyPrecedence());
      list.set("mmhs-exempted-address",new HeaderMmhsExemptedAddress());
      list.set("mmhs-extended-authorisation-info",new HeaderMmhsExtendedAuthorisationInfo());
      list.set("mmhs-message-type",new HeaderMmhsMessageType());
      list.set("mmhs-primary-precedence",new HeaderMmhsCopyPrecedence());
      list.set("obsoletes",new HeaderObsoletes());
      list.set("original-from",new HeaderFrom());
      list.set("original-message-id",new HeaderContentId());
      list.set("original-recipient",new HeaderOriginalRecipient());
      list.set("received",new HeaderReceived());
      list.set("received-spf",new HeaderReceivedSpf());
      list.set("references",new HeaderInReplyTo());
      list.set("reply-by",new HeaderDate());
      list.set("reply-to",new HeaderTo());
      list.set("resent-bcc",new HeaderBcc());
      list.set("resent-cc",new HeaderTo());
      list.set("resent-date",new HeaderDate());
      list.set("resent-from",new HeaderFrom());
      list.set("resent-message-id",new HeaderContentId());
      list.set("resent-reply-to",new HeaderTo());
      list.set("resent-sender",new HeaderSender());
      list.set("resent-to",new HeaderTo());
      list.set("return-path",new HeaderReturnPath());
      list.set("sender",new HeaderSender());
      list.set("to",new HeaderTo());
      list.set("x-mittente",new Mailbox());
      list.set("x-riferimento-message-id",new HeaderContentId());
      list.set("x400-originator",new Mailbox());
      list.set("x400-recipients",new HeaderDispositionNotificationTo());
      return list;
    }

    public static IHeaderFieldParser GetParser(String name) {

      name = ParserUtility.ToLowerCaseAscii(name);
      if (list.containsKey(name)) {
        return list.get(name);
      }
      return unstructured;
    }
  }
