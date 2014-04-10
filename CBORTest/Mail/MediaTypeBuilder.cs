/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/6/2014
 * Time: 5:08 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace PeterO.Mail
{
  internal sealed class MediaTypeBuilder
  {
    private string type;
    private string subtype;
    private IDictionary<string, string> parameters;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string TopLevelType {
      get {
        return this.type;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string SubType {
      get {
        return this.subtype;
      }
    }

    public MediaTypeBuilder() {
      this.parameters = new Dictionary<string, string>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    public MediaTypeBuilder(MediaType mt) {
      if (mt == null) {
        throw new ArgumentNullException("mt");
      }
      this.parameters = new Dictionary<string, string>(mt.Parameters);
      this.type = mt.TopLevelType;
      this.subtype = mt.SubType;
    }

    public MediaTypeBuilder(string type, string subtype) {
      this.parameters = new Dictionary<string, string>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A MediaType object.</returns>
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetTopLevelType(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.Length, null) != str.Length) {
        throw new ArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder RemoveParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      this.parameters.Remove(ParserUtility.ToLowerCaseAscii(name));
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object.</param>
    /// <param name='value'>A string object. (2).</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetParameter(string name, string value) {
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      if (value.Length == 0) {
        throw new ArgumentException("value is empty.");
      }
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(name, 0, name.Length, null) != name.Length) {
        throw new ArgumentException("Not a well-formed parameter name: " + name);
      }
      this.parameters[ParserUtility.ToLowerCaseAscii(name)] = value;
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetSubType(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.Length, null) != str.Length) {
        throw new ArgumentException("Not a well-formed subtype: " + str);
      }
      this.subtype = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.ToMediaType().ToString();
    }
  }
}
