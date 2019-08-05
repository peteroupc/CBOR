using System.Collections.Generic;

namespace PeterO.DocGen {
  public partial class XmlDoc {
    public interface INode {
      string LocalName { get; }

      string GetContent();

      IEnumerable<string> GetAttributes();

      string GetAttribute(string str);

      IEnumerable<INode> GetChildren();
    }
  }
}
