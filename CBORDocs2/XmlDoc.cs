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
                    foreach (Node c in this.children)
                    {
                        yield return c;
                    }
                }
            }

            public IEnumerable<string> GetAttributes()
            {
                if (this.attributes != null)
                {
                    foreach (string c in this.attributes.Keys)
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
                StringBuilder sb = new();
                foreach (Node c in this.children)
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
                    this.LocalName = string.Empty;
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
            foreach (INode child in node.GetChildren())
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
            Node node = new(reader.LocalName, true, string.Empty);
            bool emptyElement = reader.IsEmptyElement;
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
            int depth = 0;
            List<Node> nodeStack = new();
            bool doread = true;
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
                    Node childNode = new(reader.LocalName, true, string.Empty);
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            childNode.SetAttribute(reader.Name, reader.Value);
                        }
                    }
                    nodeStack[^1].AppendChild(childNode);
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
                else if (reader.NodeType is XmlNodeType.SignificantWhitespace or
                  XmlNodeType.Whitespace or
                  XmlNodeType.Text)
                {
                    StringBuilder sb = new StringBuilder().Append(reader.Value);
                    reader.Read();
                    while (reader.NodeType is XmlNodeType.SignificantWhitespace or
                      XmlNodeType.Whitespace or
                      XmlNodeType.Text)
                    {
                        sb.Append(reader.Value);
                        reader.Read();
                    }
                    doread = false;
                    nodeStack[^1].AppendChild(
                     new Node(string.Empty, false, sb.ToString()));
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
                if (string.IsNullOrEmpty(node.LocalName))
                {
                    this.sb.Append(node.GetContent());
                }
                else
                {
                    string c = node.GetAttribute("cref");
                    string n = node.GetAttribute("name");
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
                string summary = this.sb.ToString();
                summary = Regex.Replace(summary, @"^\s+|\s+$", string.Empty);
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
                INode mn = this.memberNodes[memberID];
                StringBuilder sb = new();
                foreach (INode c in mn.GetChildren())
                {
                    if (c.LocalName.Equals("summary", StringComparison.Ordinal))
                    {
                        SummaryVisitor sv = new();
                        sv.VisitNode(c);
                        sb.Append(sv.ToString()).Append("\r\n\r\n");
                    }
                }
                string summary = sb.ToString();
                summary = Regex.Replace(summary, @"^\s+$", string.Empty);
                return summary;
            }
        }

        public XmlDoc(string xmlFilename)
        {
            this.memberNodes = new Dictionary<string, INode>();
            using FileStream stream = new(xmlFilename, FileMode.Open);
            using XmlReader reader = XmlReader.Create(stream);
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
                            INode node = ReadNode(reader);
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