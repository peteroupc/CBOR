using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PeterO.DocGen
{
    public partial class XmlDoc
    {
        private sealed class Node : INode
        {
            public string LocalName { get; private set; }

            private readonly bool element;
            private readonly string content;
            private readonly IDictionary<string, string> attributes;
            private readonly IList<Node> children;

            public IEnumerable<INode> GetChildren()
            {
                if (this.children != null)
                {
                    foreach (var c in this.children)
                    {
                        yield return c;
                    }
                }
            }

            public IEnumerable<string> GetAttributes()
            {
                if (this.attributes != null)
                {
                    foreach (var c in this.attributes.Keys)
                    {
                        yield return c;
                    }
                }
            }

            internal void AppendChild(Node child)
            {
                if (child == null)
                {
                    throw new ArgumentNullException(nameof(child));
                }
                if (this.children != null)
                {
                    this.children.Add(child);
                }
            }

            internal void SetAttribute(string name, string value)
            {
                if (this.attributes != null)
                {
                    this.attributes[name] = value;
                }
            }

            public string GetAttribute(string str)
            {
                return (this.attributes == null ||
                   !this.attributes.ContainsKey(str)) ? null : this.attributes[str];
            }

            public string GetContent()
            {
                if (!this.element)
                {
                    return this.content;
                }
                var sb = new StringBuilder();
                foreach (var c in this.children)
                {
                    sb.Append(c.GetContent());
                }
                return sb.ToString();
            }

            public Node(string localName, bool element, string content)
            {
                this.element = element;
                this.content = content;
                if (this.element)
                {
                    this.attributes = new Dictionary<string, string>();
                    this.children = new List<Node>();
                    this.LocalName = localName;
                }
                else
                {
                    this.LocalName = String.Empty;
                    this.children = null;
                    this.attributes = null;
                }
            }
        }

        public static void VisitInnerNode(INode node, IVisitor vis)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            foreach (var child in node.GetChildren())
            {
                if (vis == null)
                {
                    throw new ArgumentNullException(nameof(vis));
                }
                vis.VisitNode(child);
            }
        }

        private static INode ReadNode(XmlReader reader)
        {
            var node = new Node(reader.LocalName, true, String.Empty);
            var emptyElement = reader.IsEmptyElement;
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    node.SetAttribute(reader.Name, reader.Value);
                }
            }
            if (emptyElement)
            {
                reader.Read();
                return node;
            }
            var depth = 0;
            var nodeStack = new List<Node>();
            var doread = true;
            nodeStack.Add(node);
            while (true)
            {
                if (doread)
                {
                    reader.Read();
                }
                doread = true;
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (depth <= 0)
                    {
                        reader.Read();
                        break;
                    }
                    nodeStack.RemoveAt(nodeStack.Count - 1);
                    --depth;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    emptyElement = reader.IsEmptyElement;
                    var childNode = new Node(reader.LocalName, true, String.Empty);
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            childNode.SetAttribute(reader.Name, reader.Value);
                        }
                    }
                    nodeStack[nodeStack.Count - 1].AppendChild(childNode);
                    if (!emptyElement)
                    {
                        nodeStack.Add(childNode);
                        ++depth;
                    }
                }
                else if (reader.NodeType == XmlNodeType.None)
                {
                    throw new XmlException();
                }
                else if (reader.NodeType == XmlNodeType.SignificantWhitespace ||
                  reader.NodeType == XmlNodeType.Whitespace ||
                  reader.NodeType == XmlNodeType.Text)
                {
                    var sb = new StringBuilder().Append(reader.Value);
                    reader.Read();
                    while (reader.NodeType == XmlNodeType.SignificantWhitespace ||
                      reader.NodeType == XmlNodeType.Whitespace ||
                      reader.NodeType == XmlNodeType.Text)
                    {
                        sb.Append(reader.Value);
                        reader.Read();
                    }
                    doread = false;
                    nodeStack[nodeStack.Count - 1].AppendChild(
                     new Node(String.Empty, false, sb.ToString()));
                }
            }
            return node;
        }

        private sealed class SummaryVisitor : IVisitor
        {
            private readonly StringBuilder sb;

            public SummaryVisitor()
            {
                this.sb = new StringBuilder();
            }

            public void VisitNode(INode node)
            {
                if (String.IsNullOrEmpty(node.LocalName))
                {
                    this.sb.Append(node.GetContent());
                }
                else
                {
                    var c = node.GetAttribute("cref");
                    var n = node.GetAttribute("name");
                    if (c != null)
                    {
                        this.sb.Append(c);
                    }
                    else if (n != null)
                    {
                        this.sb.Append(n);
                    }
                }
                XmlDoc.VisitInnerNode(node, this);
            }

            public override string ToString()
            {
                var summary = this.sb.ToString();
                summary = Regex.Replace(summary, @"^\s+|\s+$", String.Empty);
                summary = Regex.Replace(summary, @"\s+", " ");
                return summary;
            }
        }

        private readonly IDictionary<string, INode> memberNodes;

        public INode GetMemberNode(string memberID)
        {
            if (!this.memberNodes.ContainsKey(memberID))
            {
                return null;
            }
            else
            {
                return this.memberNodes[memberID];
            }
        }

        public string GetSummary(string memberID)
        {
            if (!this.memberNodes.ContainsKey(memberID))
            {
                return null;
            }
            else
            {
                var mn = this.memberNodes[memberID];
                var sb = new StringBuilder();
                foreach (var c in mn.GetChildren())
                {
                    if (c.LocalName.Equals("summary", StringComparison.Ordinal))
                    {
                        var sv = new SummaryVisitor();
                        sv.VisitNode(c);
                        sb.Append(sv.ToString()).Append("\r\n\r\n");
                    }
                }
                var summary = sb.ToString();
                summary = Regex.Replace(summary, @"^\s+$", String.Empty);
                return summary;
            }
        }

        public XmlDoc(string xmlFilename)
        {
            this.memberNodes = new Dictionary<string, INode>();
            using var stream = new FileStream(xmlFilename, FileMode.Open);
            using var reader = XmlReader.Create(stream);
            reader.Read();
            reader.ReadStartElement("doc");
            while (reader.IsStartElement())
            {
                // Console.WriteLine(reader.LocalName);
                if (reader.LocalName.Equals("members", StringComparison.Ordinal))
                {
                    reader.Read();
                    while (reader.IsStartElement())
                    {
                        if (reader.LocalName.Equals("member",
                             StringComparison.Ordinal))
                        {
                            string memberName = reader.GetAttribute("name");
                            var node = ReadNode(reader);
                            this.memberNodes[memberName] = node;
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                    reader.Skip();
                }
                else
                {
                    reader.Skip();
                }
            }
        }
    }
}
