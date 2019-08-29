using System;
namespace CBORTest {
    public sealed class FieldClass {
       public int publicFieldA;
       public readonly int ReadonlyFieldA = 33;
       private int PrivateFieldA;
       private readonly int PrivateFieldB = 44;
       private const int StaticFieldA;
       private const int ConstFieldA = 55;
    }
}
