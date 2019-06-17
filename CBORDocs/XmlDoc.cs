using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PeterO.DocGen {
  public class XmlDoc {
    private string ReadContent(XmlReader reader) {
      var depth = 0;
      var sb = new StringBuilder();
      while (true) {
        reader.Read();
        if (reader.NodeType == XmlNodeType.EndElement) {
          if (depth <= 0) {
            reader.Read();
            break;
          }
          --depth;
        } else if (reader.NodeType == XmlNodeType.Element) {
          string cref = reader.GetAttribute("cref");
          string name = reader.GetAttribute("name");
          if (cref != null) {
            sb.Append(cref);
          } else if (name != null) {
            sb.Append(name);
          }
          if (!reader.IsEmptyElement) {
            ++depth;
          }
        } else if (reader.NodeType == XmlNodeType.None) {
          throw new XmlException();
        } else if (reader.NodeType == XmlNodeType.SignificantWhitespace || 
          reader.NodeType == XmlNodeType.Whitespace || 
          reader.NodeType == XmlNodeType.Text) {
          sb.Append(reader.Value);
        }
      }
      return sb.ToString();
    }

    private string ReadSummary(XmlReader reader) {
      reader.Read();
      var sb = new StringBuilder();
      while (reader.IsStartElement()) {
        if (reader.LocalName.Equals("summary")) {
          string content = this.ReadContent(reader);
          sb.Append(content);
        } else {
          reader.Skip();
        }
      }
      reader.Skip();
      var summary = sb.ToString();
      summary = Regex.Replace(summary, @"^\s+|\s+$", String.Empty);
      summary = Regex.Replace(summary, @"\s+", " ");
      return summary;
    }

    private readonly IDictionary<string, string> summaries;

    public string GetSummary(string memberID) {
      if (!this.summaries.ContainsKey(memberID)) {
        return null;
      } else {
        return this.summaries[memberID];
      }
    }

    public XmlDoc(string xmlFilename) {
      this.summaries = new Dictionary<string, string>();
      using (var stream = new FileStream(xmlFilename, FileMode.Open)) {
        var reader = XmlReader.Create(stream);
        reader.Read();
        reader.ReadStartElement("doc");
        while (reader.IsStartElement()) {
          // Console.WriteLine(reader.LocalName);
          if (reader.LocalName.Equals("members")) {
            reader.Read();
            while (reader.IsStartElement()) {
              if (reader.LocalName.Equals("member")) {
                string memberName = reader.GetAttribute("name");
                string summary = this.ReadSummary(reader);
                this.summaries[memberName] = summary;
              } else {
                reader.Skip();
              }
            }
            reader.Skip();
          } else {
            reader.Skip();
          }
        }
      }
    }
  }
}
