using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PeterO.DocGen {
  public class XmlDoc {
    public interface INode {
      string LocalName { get; }
      string GetContent();
      IEnumerable<string> GetAttributes();
      string GetAttribute(string str);
      IEnumerable<INode> GetChildren();
    }
    public interface IVisitor {
      void VisitNode(INode node);
    }

    private sealed class Node : INode {
      public string LocalName { get; private set; }
      private readonly bool element;
      private readonly string content;
      private readonly IDictionary<string, string> attributes;
      private readonly IList<Node> children;
      public IEnumerable<INode> GetChildren() {
        if (children != null) {
          foreach (var c in children) {
            yield return c;
          }
        }
      }
      public IEnumerable<string> GetAttributes() {
        if (attributes != null) {
          foreach (var c in attributes.Keys) {
            yield return c;
          }
        }
      }
      internal void AppendChild(Node child) {
        if ((child) == null) {
  throw new ArgumentNullException(nameof(child));
}
        if (children != null) {
 children.Add(child);
}
      }
      internal void SetAttribute(string name, string value) {
        if (attributes != null) {
          attributes[name] = value;
        }
      }
      public string GetAttribute(string str) {
        return (attributes == null || !attributes.ContainsKey(str)) ? (null):
          (attributes[str]);
      }
      public string GetContent() {
        if (!element) {
 return content;
}
        var sb = new StringBuilder();
        foreach (var c in children) {
          sb.Append(c.GetContent());
        }
        return sb.ToString();
      }
      public Node(string localName, bool element, string content) {
        this.element = element;
        this.content = content;
        if (this.element) {
          attributes = new Dictionary<string, string>();
          children = new List<Node>();
          this.LocalName = localName;
        } else {
          this.LocalName = String.Empty;
          children = null;
          attributes = null;
        }
      }
    }

    public static void VisitInnerNode(INode node, IVisitor vis) {
      foreach (var child in node.GetChildren()) {
        vis.VisitNode(node);
      }
    }

    private INode ReadNode(XmlReader reader) {
      var node = new Node(reader.LocalName, true, String.Empty);
      var emptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes) {
        while (reader.MoveToNextAttribute()) {
          node.SetAttribute(reader.Name, reader.Value);
        }
      }
      if (emptyElement) {
        reader.Read();
        return node;
      }
      var depth = 0;
      var nodeStack = new List<Node>();
      var doread = true;
      nodeStack.Add(node);
      while (true) {
        if (doread) {
 reader.Read();
}
        doread = true;
        if (reader.NodeType == XmlNodeType.EndElement) {
          if (depth <= 0) {
            reader.Read();
            break;
          }
          nodeStack.RemoveAt(nodeStack.Count - 1);
          --depth;
        } else if (reader.NodeType == XmlNodeType.Element) {
          emptyElement = reader.IsEmptyElement;
          var childNode = new Node(reader.LocalName, true, "");
          if (reader.HasAttributes) {
            while (reader.MoveToNextAttribute()) {
              childNode.SetAttribute(reader.Name, reader.Value);
            }
          }
          nodeStack[nodeStack.Count - 1].AppendChild(childNode);
          if (!emptyElement) {
            nodeStack.Add(childNode);
            ++depth;
          }
        } else if (reader.NodeType == XmlNodeType.None) {
          throw new XmlException();
        } else if (reader.NodeType == XmlNodeType.SignificantWhitespace ||
          reader.NodeType == XmlNodeType.Whitespace ||
          reader.NodeType == XmlNodeType.Text) {
          var sb = new StringBuilder().Append(reader.Value);
          reader.Read();
          while (reader.NodeType == XmlNodeType.SignificantWhitespace ||
            reader.NodeType == XmlNodeType.Whitespace ||
            reader.NodeType == XmlNodeType.Text) {
            sb.Append(reader.Value);
            reader.Read();
          }
          doread = false;
          nodeStack[nodeStack.Count - 1].AppendChild(
           new Node("", false, sb.ToString()));
        }
      }
      return node;
    }

    private sealed class SummaryVisitor : IVisitor {
      private readonly StringBuilder sb;
      public SummaryVisitor() {
        sb = new StringBuilder();
      }
      public void VisitNode(INode node) {
        if (String.IsNullOrEmpty(node.LocalName)) {
          sb.Append(node.GetContent());
        } else {
          var c = node.GetAttribute("cref");
          var n = node.GetAttribute("name");
          if (c != null) {
 sb.Append(c);
  } else if (n != null) {
 sb.Append(n);
}
        }
        XmlDoc.VisitInnerNode(node, this);
      }
      public override string ToString() {
        var summary = sb.ToString();
        summary = Regex.Replace(summary, @"^\s+|\s+$", String.Empty);
        summary = Regex.Replace(summary, @"\s+", " ");
        return summary;
      }
    }
    private readonly IDictionary<string, INode> memberNodes;

    public INode GetMemberNode(string memberID) {
      if (!this.memberNodes.ContainsKey(memberID)) {
        return null;
      } else {
        return memberNodes[memberID];
      }
    }

    public string GetSummary(string memberID) {
      if (!this.memberNodes.ContainsKey(memberID)) {
        return null;
      } else {
        var mn = memberNodes[memberID];
        var sb = new StringBuilder();
        foreach (var c in mn.GetChildren()) {
          if (c.LocalName.Equals("summary")) {
            var sv = new SummaryVisitor();
            sv.VisitNode(c);
            sb.Append(sv.ToString()) .Append("\r\n\r\n");
          }
        }
        var summary = sb.ToString();
        summary = Regex.Replace(summary, @"^\s+$", String.Empty);
        return summary;
      }
    }

    public XmlDoc(string xmlFilename) {
      this.memberNodes = new Dictionary<string, INode>();
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
                var node = this.ReadNode(reader);
                this.memberNodes[memberName] = node;
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
