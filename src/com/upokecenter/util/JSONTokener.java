package com.upokecenter.util;
// Modified by Peter O. to use generics and
// to use int and -1 as the terminating
// value rather than char and 0, among
// other things; also moved from org.json.
// Still in the public domain;
// public domain dedication: http://creativecommons.org/publicdomain/zero/1.0/

	
	
	



	/**
	 * A JSONTokener takes a source _string and extracts characters and tokens from
	 * it. It is used by the JSONObject and JSONArray constructors to parse
	 * JSON source strings.
	 * <p>
	 * Public Domain 2002 JSON.org
	 * @author JSON.org
	 * @version 0.1
	 */
	class JSONTokener {

		
		/**
		 * Trailing commas are allowed in the JSON _string.
		 */
		public static final int OPTION_TRAILING_COMMAS = 8;
		/**
		 * Empty array elements are allowed in array notation
		 */
		public static final int OPTION_EMPTY_ARRAY_ELEMENTS = 16;
		/**
		 * Allow forward slashes to be escaped in strings.
		 */
		public static final int OPTION_ESCAPED_SLASHES = 64;
		/**
		 * No duplicates are allowed in the JSON _string.
		 */
		public static final int OPTION_NO_DUPLICATES = 1;
		/**
		 * Will parse Shell-style comments (beginning with "#").
		 */
		public static final int OPTION_SHELL_COMMENTS = 2;
		/**
		 * Will add a "@comment" property to all objects with
		 * comments associated with them. Only applies to JSON
		 * objects, not JSON arrays.
		 */
		public static final int OPTION_ADD_COMMENTS = 4;

		/**
		 * Single quotes are allowed to delimit strings.
		 */
		public static final int OPTION_SINGLE_QUOTES = 32;
		/**
		 * Allows comments in JSON texts.
		 */
		public static final int OPTION_ALLOW_COMMENTS = 128;


		/**
		 * Get the hex value of a character (base16).
		 * @param c A character between '0' and '9' or between 'A' and 'F' or
		 * between 'a' and 'f'.
		 * @return  An int between 0 and 15, or -1 if c was not a hex digit.
		 */
		public static int dehexchar(char c) {
			if (c >= '0' && c <= '9')
				return c - '0';
			if (c >= 'A' && c <= 'F')
				return c + 10 - 'A';
			if (c >= 'a' && c <= 'f')
				return c + 10 - 'a';
			return -1;
		}

		public static String trimSpaces(String str) {
			if(str==null || str.length()==0)return str;
			int index=0;
			int sLength=str.length();
			while(index<sLength){
				char c=str.charAt(index);
				if(c!=0x09 && c!=0x0a && c!=0x0c && c!=0x0d && c!=0x20){
					break;
				}
				index++;
			}
			if(index==sLength)return "";
			int startIndex=index;
			index=sLength-1;
			while(index>=0){
				char c=str.charAt(index);
				if(c!=0x09 && c!=0x0a && c!=0x0c && c!=0x0d && c!=0x20)
					return str.substring(startIndex,index+1);
				index--;
			}
			return "";
		}


		/**
		 * Convert <code>%</code><i>hh</i> sequences to single characters, and convert plus to space.
		 * @param s A _string that may contain <code>+</code>&nbsp;<small>(plus)</small> and <code>%</code><i>hh</i> sequences.
		 * @return The unescaped _string.
		 */
		public static String unescape(String s) {
			int len = s.length();
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < len; ++i) {
				char c = s.charAt(i);
				if (c == '+') {
					c = ' ';
				} else if (c == '%' && i + 2 < len) {
					int d = dehexchar(s.charAt(i + 1));
					int e = dehexchar(s.charAt(i + 2));
					if (d >= 0 && e >= 0) {
						c = (char)(d * 16 + e);
						i += 2;
					}
				}
				b.append(c);
			}
			return b.toString();
		}


		/**
		 * The index of the next character.
		 */
		private int myIndex;


		/**
		 * The source _string being tokenized.
		 */
		private String mySource;


		private int options;

		/**
		 * Construct a JSONTokener from a _string.
		 *
		 * @param s     A source _string.
		 */
		public JSONTokener(String s, int options) {
			myIndex = 0;
			mySource = s;
			this.options=options;
		}


		/**
		 * Back up one character. This provides a sort of lookahead capability,
		 * so that you can test for a digit or letter before attempting to parse
		 * the next number or identifier.
		 */
		public void back() {
			if (myIndex > 0) {
				myIndex -= 1;
			}
		}


		/**
		 * Determine if the source _string still contains characters that next()
		 * can consume.
		 * @return true if not yet at the end of the source.
		 */
		public boolean more() {
			return myIndex < mySource.length();
		}


		/**
		 * Get the next character in the source _string.
		 *
		 * @return The next character, or 0 if past the end of the source _string.
		 */
		public int next() {
			int c = more() ? mySource.charAt(myIndex) : -1;
			myIndex += 1;
			return c;
		}


		/**
		 * Consume the next character, and check that it matches a specified
		 * character.
		 * @param c The character to match.
		 * @return The character.
		 * @ if the character does not match.
		 */
		public int next(char c) {
			int n = next();
			if (n != c)
				throw syntaxError("Expected '" + c + "' and instead saw '" +
				                  n + "'.");
			return n;
		}


		/**
		 * Get the next n characters.
		 * @param n     The number of characters to take.
		 * @return      A _string of n characters.
		 * @exception NumberFormatException
		 *   Substring bounds error if there are not
		 *   n characters remaining in the source _string.
		 */
		public String next(int n) {
			int i = myIndex;
			int j = i + n;
			if (j >= mySource.length())
				throw syntaxError("Substring bounds error");
			myIndex += n;
			return mySource.substring(i,j);
		}

		public int getOptions() {
			return options;
		}

		/**
		 * Get the next comment or comments in the _string, if any.
		 * (Added by Peter O., 5/6/2013)
		 */
		public String nextComment() {
			StringBuilder builder=new StringBuilder();
			while (true) {
				int c = next();
				if(c=='#' && (options & JSONTokener.OPTION_SHELL_COMMENTS)!=0){
					// Shell-style single-line comment
					boolean haveChar=false;
					while(true) {
						c = next();
						if(c != '\n' && c != -1){
							if(haveChar || c>' '){
								if(!haveChar && builder.length()>0)
									builder.append(' '); // append space if comment is continuing
								builder.append((char)c);
								haveChar=true;
							}
						} else
							break; // end of line
					}
				}
				else if (c == '/') {
					switch (next()) {
							case '/':{ // single-line comment
								boolean haveChar=false;
								while(true) {
									c = next();
									if(c != '\n' && c != -1){
										if(haveChar || c>' '){
											if(!haveChar && builder.length()>0)
												builder.append(' '); // append space if comment is continuing
											builder.append((char)c);
											haveChar=true;
										}
									} else
										break; // end of line
								}
								break;
							}
							case '*':{ // multi-line comment
								boolean haveChar=false;
								while (true) {
									c = next();
									if (c == -1)
										throw syntaxError("Unclosed comment.");
									if (c == '*') {
										if (next() == '/') {
											break;
										}
										back();
									}
									if(haveChar || c>' '){
										if(!haveChar && builder.length()>0)
											builder.append(' '); // append space if comment is continuing
										builder.append((char)c);
										haveChar=true;
									}
								}
								break;
							}
						default:
							back();
							return builder.toString();
					}
				} else if (c == -1){
					return builder.toString(); // reached end of _string
				} else if(c>' '){
					// reached an ordinary character
					back();
					return builder.toString();
				}
			}
		}

		public void nextCommentReturnVoid() {
			while (true) {
				int c = next();
				if(c=='#' && (options & JSONTokener.OPTION_SHELL_COMMENTS)!=0){
					// Shell-style single-line comment
					while(true) {
						c = next();
						if(c != '\n' && c != -1){
						} else
							break; // end of line
					}
				}
				else if (c == '/') {
					switch (next()) {
							case '/':{ // single-line comment
								while(true) {
									c = next();
									if(c != '\n' && c != -1){
									} else
										break; // end of line
								}
								break;
							}
							case '*':{ // multi-line comment
								while (true) {
									c = next();
									if (c == -1)
										throw syntaxError("Unclosed comment.");
									if (c == '*') {
										if (next() == '/') {
											break;
										}
										back();
									}
								}
								break;
							}
						default:
							back();
							return;
					}
				} else if (c == -1){
					return; // reached end of _string
				} else if(c>' '){
					// reached an ordinary character
					back();
					return;
				}
			}
		}

		/**
		 * Get the next char in the _string, skipping whitespace
		 * and comments (slashslash and slashstar).
		 * @
		 * @return  A character, or 0 if there are no more characters.
		 */
		public int nextClean() {
			if((options & JSONTokener.OPTION_ALLOW_COMMENTS)!=0)
				nextCommentReturnVoid();
			while (true) {
				int c = next();
				if (c == -1 || c > ' ')
					return c;
			}
		}


		/**
		 * Return the characters up to the next close quote character.
		 * Backslash processing is done. The formal JSON format does not
		 * allow strings in single quotes, but an implementation is allowed to
		 * accept them.
		 * @param quote The quoting character, either <code>"</code>&nbsp;<small>(double quote)</small> or <code>'</code>&nbsp;<small>(single quote)</small>.
		 * @return      A String.
		 * @exception NumberFormatException Unterminated _string.
		 */
		public String nextString(int quote) {
			int c;
			StringBuilder sb = new StringBuilder();
			while (true) {
				c = next();
				if(c==-1 || c<0x20)
					throw syntaxError("Unterminated String");
				switch (c) {
					case '\\':
						c = next();
						switch (c) {
							case '\\':
								sb.append('\\');
								break;
							case '/':
								if((options & JSONTokener.OPTION_ESCAPED_SLASHES)!=0){
									// For compatibility (some JSON texts
									// encode dates with an escaped slash),
									// even though this is not allowed by RFC 4627
									sb.append('/');
								} else {
									throw this.syntaxError("Invalid escaped character");
								}
								break;
							case '\"':
								sb.append('\"');
								break;
							case 'b':
								sb.append('\b');
								break;
							case 't':
								sb.append('\t');
								break;
							case 'n':
								sb.append('\n');
								break;
							case 'f':
								sb.append('\f');
								break;
							case 'r':
								sb.append('\r');
								break;
							case 'u':
								sb.append((char)Integer.parseInt(next(4),16));
								break;
							default:
								throw this.syntaxError("Invalid escaped character");
						}
						break;
					default:
						if (c == quote)
							return sb.toString();
						sb.append((char)c);
						break;
				}
			}
		}


		/**
		 * Skip characters until past the requested _string.
		 * If it is not found, we are left at the end of the source.
		 * @param to A _string to skip past.
		 */
		public void skipPast(String to) {
			myIndex = mySource.indexOf(to,myIndex);
			if (myIndex < 0) {
				myIndex = mySource.length();
			} else {
				myIndex += to.length();
			}
		}


		/**
		 * Skip characters until the next character is the requested character.
		 * If the requested character is not found, no characters are skipped.
		 * @param to A character to skip to.
		 * @return The requested character, or zero if the requested character
		 * is not found.
		 */
		public int skipTo(int to) {
			int c;
			int index = myIndex;
			do {
				c = next();
				if (c == -1) {
					myIndex = index;
					return c;
				}
			} while (c != to);
			back();
			return c;
		}


		/**
		 * Make a NumberFormatException to signal a syntax error.
		 *
		 * @param message The error message.
		 * @return  A NumberFormatException Object, suitable for throwing
		 */
		public NumberFormatException syntaxError(String message) {
			return new NumberFormatException(message + toString());
		}


		/**
		 * Make a printable String of this JSONTokener.
		 *
		 * @return " at character [myIndex] of [mySource]"
		 */
		@Override public String toString() {
			return " at character " + myIndex + " of " + mySource;
		}

		/**
		 * Unescape the source text. Convert <code>%</code><i>hh</i> sequences to single characters,
		 * and convert plus to space. There are Web transport systems that insist on
		 * doing unnecessary URL encoding. This provides a way to undo it.
		 */
		void unescape() {
			mySource = unescape(mySource);
		}
	}
