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
namespace CBORTest
{
  internal sealed class MediaTypeBuilder
  {
    string type;
    string subtype;
    IDictionary<string, string> parameters;

    public MediaTypeBuilder() {
      parameters = new Dictionary<string, string>();
      type="application";
      subtype="octet-stream";
    }

    public MediaTypeBuilder(MediaType mt) {
      if ((mt) == null) {
        throw new ArgumentNullException("mt");
      }
      parameters = new Dictionary<string, string>(mt.Parameters);
      type = mt.TopLevelType;
      subtype = mt.SubType;
    }

    public MediaTypeBuilder(string type, string subtype) {
      parameters = new Dictionary<string, string>();
      SetTopLevelType(type);
      SetSubType(subtype);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A MediaType object.</returns>
    public MediaType ToMediaType() {
      return new MediaType(type, subtype, parameters);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetTopLevelType(string str) {
      if ((str) == null) {
        throw new ArgumentNullException("str");
      }if ((str).Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.Length, null) != str.Length) {
        throw new ArgumentException("Not a well-formed top level type: "+str);
      }
      type = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder RemoveParameter(string name) {
      if ((name) == null) {
        throw new ArgumentNullException("name");
      }
      parameters.Remove(ParserUtility.ToLowerCaseAscii(name));
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object.</param>
    /// <param name='value'>A string object. (2).</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetParameter(string name, string value) {
      if ((value) == null) {
        throw new ArgumentNullException("value");
      }
      if ((value).Length == 0) {
        throw new ArgumentException("value is empty.");
      }
      if ((name) == null) {
        throw new ArgumentNullException("name");
      }
      if ((name).Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(name, 0, name.Length, null) != name.Length) {
        throw new ArgumentException("Not a well-formed parameter name: "+name);
      }
      parameters[ParserUtility.ToLowerCaseAscii(name)]=value;
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A MediaTypeBuilder object.</returns>
    public MediaTypeBuilder SetSubType(string str) {
      if ((str) == null) {
        throw new ArgumentNullException("str");
      }
      if ((str).Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.Length, null) != str.Length) {
        throw new ArgumentException("Not a well-formed subtype: "+str);
      }
      subtype = ParserUtility.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return ToMediaType().ToString();
    }
  }
}
