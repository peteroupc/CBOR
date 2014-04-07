package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/6/2014
 * Time: 5:08 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.util.*;

  final class MediaTypeBuilder
  {
    String type;
    String subtype;
    Map<String, String> parameters;

    public MediaTypeBuilder () {
      parameters = new HashMap<String, String>();
      type="application";
      subtype="octet-stream";
    }

    public MediaTypeBuilder (MediaType mt) {
      if ((mt) == null) {
        throw new NullPointerException("mt");
      }
      parameters = new HashMap<String, String>(mt.getParameters());
      type = mt.getTopLevelType();
      subtype = mt.getSubType();
    }

    public MediaTypeBuilder (String type, String subtype) {
      parameters = new HashMap<String, String>();
      SetTopLevelType(type);
      SetSubType(subtype);
    }

    /**
     * Not documented yet.
     * @return A MediaType object.
     */
    public MediaType ToMediaType() {
      return new MediaType(type, subtype, parameters);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetTopLevelType(String str) {
      if ((str) == null) {
        throw new NullPointerException("str");
      }if ((str).length == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed top level type: "+str);
      }
      type = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder RemoveParameter(String name) {
      if ((name) == null) {
        throw new NullPointerException("name");
      }
      parameters.Remove(ParserUtility.ToLowerCaseAscii(name));
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @param value A string object. (2).
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetParameter(String name, String value) {
      if ((value) == null) {
        throw new NullPointerException("value");
      }
      if ((value).length == 0) {
        throw new IllegalArgumentException("value is empty.");
      }
      if ((name) == null) {
        throw new NullPointerException("name");
      }
      if ((name).length == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
        throw new IllegalArgumentException("Not a well-formed parameter name: "+name);
      }
      parameters.put(ParserUtility.ToLowerCaseAscii(name),value);
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetSubType(String str) {
      if ((str) == null) {
        throw new NullPointerException("str");
      }
      if ((str).length == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed subtype: "+str);
      }
      subtype = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return ToMediaType().toString();
    }
  }
