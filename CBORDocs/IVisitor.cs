namespace PeterO.DocGen {
  public partial class XmlDoc {
    public interface IVisitor {
      void VisitNode(INode node);
    }
  }
}
