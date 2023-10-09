using System;

// One property returns an array for testing purposes
namespace Test {
  public sealed class PODClass {
    /// <summary>Initializes a new instance of the
    /// <see cref='PODClass'/> class.</summary>
    public PODClass() {
      this.PropA = 0;
      this.PropB = 1;
      this.IsPropC = false;
      this.PrivatePropA = 2;
      this.FloatProp = 0;
      this.DoubleProp = 0;
      this.StringProp = String.Empty;
      this.StringArray = null;
    }

    public bool HasGoodPrivateProp() {
      int ppa = this.PrivatePropA;
      return ppa == 2;
    }

    private int PrivatePropA
    {
      get;
    }

    public static int StaticPropA
    {
      get;
      set;
    }

    public int PropA
    {
      get;
      set;
    }

    public int PropB
    {
      get;
      set;
    }

    public bool IsPropC
    {
      get;
      set;
    }

    public float FloatProp
    {
      get;
      set;
    }

    public double DoubleProp
    {
      get;
      set;
    }

    public string StringProp
    {
      get;
      set;
    }

    public string[] StringArray
    {
      get;
      set;
    }
  }
}
