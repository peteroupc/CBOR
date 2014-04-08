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
    internal String type;
    internal String subtype;
    internal Map<String, String> parameters;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
public String getTopLevelType() {
        return this.type;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
public String getSubType() {
        return this.subtype;
      }

    public MediaTypeBuilder () {
      this.parameters = new HashMap<String, String>();
      this.type="application";
      this.subtype="octet-stream";
    }

    public MediaTypeBuilder (MediaType mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getTopLevelType();
      this.subtype = mt.getSubType();
    }

    public MediaTypeBuilder (String type, String subtype) {
      this.parameters = new HashMap<String, String>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /**
     * Not documented yet.
     * @return A MediaType object.
     */
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetTopLevelType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder RemoveParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      this.parameters.Remove(ParserUtility.ToLowerCaseAscii(name));
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @param value A string object. (2).
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetParameter(String name, String value) {
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (value.length() == 0) {
        throw new IllegalArgumentException("value is empty.");
      }
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
        throw new IllegalArgumentException("Not a well-formed parameter name: " + name);
      }
      this.parameters.put(ParserUtility.ToLowerCaseAscii(name),value);
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A MediaTypeBuilder object.
     */
    public MediaTypeBuilder SetSubType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed subtype: " + str);
      }
      this.subtype = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToMediaType().toString();
    }
  }
