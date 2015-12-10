using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;

namespace Test {
  [TestClass]
  public class ExtendedDecimalTest {
    [TestMethod]
    public void TestScaling() {
      {
        string stringTemp = ExtendedDecimal.FromString(
          "5.000").ScaleByPowerOfTen(5).ToString();
        Assert.AreEqual(
          "5.000E+5",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "5.000").ScaleByPowerOfTen(-5).ToString();
        Assert.AreEqual(
          "0.00005000",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "500000").MovePointRight(5).ToString();
        Assert.AreEqual(
          "50000000000",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "500000").MovePointRight(-5).ToString();
        Assert.AreEqual(
          "5.00000",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "500000").MovePointLeft(-5).ToString();
        Assert.AreEqual(
          "50000000000",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "500000").MovePointLeft(5).ToString();
        Assert.AreEqual(
          "5.00000",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestAbs() {
      // not implemented yet
    }
    [TestMethod]
    public void TestAdd() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareTo() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareToBinary() {
    Assert.AreEqual(1, ExtendedDecimal.NegativeInfinity.CompareToBinary(null));
    Assert.AreEqual(1, ExtendedDecimal.PositiveInfinity.CompareToBinary(null));
      Assert.AreEqual(1, ExtendedDecimal.NaN.CompareToBinary(null));
      Assert.AreEqual(1, ExtendedDecimal.SignalingNaN.CompareToBinary(null));
      Assert.AreEqual(1, ExtendedDecimal.Zero.CompareToBinary(null));
      Assert.AreEqual(1, ExtendedDecimal.One.CompareToBinary(null));

    Assert.AreEqual(0, ExtendedDecimal.NaN.CompareToBinary(ExtendedFloat.NaN));

  {
long numberTemp =
  ExtendedDecimal.SignalingNaN.CompareToBinary(ExtendedFloat.NaN);
Assert.AreEqual(0, numberTemp);
}

  {
long numberTemp =
  ExtendedDecimal.NaN.CompareToBinary(ExtendedFloat.SignalingNaN);
Assert.AreEqual(0, numberTemp);
}

  {
long numberTemp =
  ExtendedDecimal.SignalingNaN.CompareToBinary(ExtendedFloat.SignalingNaN);
Assert.AreEqual(0, numberTemp);
}
    Assert.AreEqual(1, ExtendedDecimal.NaN.CompareToBinary(ExtendedFloat.Zero));

  {
long numberTemp =
  ExtendedDecimal.SignalingNaN.CompareToBinary(ExtendedFloat.Zero);
Assert.AreEqual(1, numberTemp);
}
    }
    [TestMethod]
    public void TestCompareToSignal() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCompareToWithContext() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCreate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestCreateNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivide() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToIntegerNaturalScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToIntegerZeroScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestDivideToSameExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
    }
    [TestMethod]
    public void TestExp() {
      // not implemented yet
    }
    [TestMethod]
    public void TestExponent() {
      Assert.AreEqual(
        -(BigInteger)7,
        ExtendedDecimal.FromString("1.265e-4").Exponent);
      Assert.AreEqual(
        (BigInteger)(-4),
        ExtendedDecimal.FromString("0.000E-1").Exponent);
      Assert.AreEqual(
        (BigInteger)(-16),
        ExtendedDecimal.FromString("0.57484848535648e-2").Exponent);
      Assert.AreEqual(
        (BigInteger)(-22),
        ExtendedDecimal.FromString("0.485448e-16").Exponent);
      Assert.AreEqual(
        (BigInteger)(-20),
        ExtendedDecimal.FromString(
          "0.5657575351495151495649565150e+8").Exponent);
      Assert.AreEqual(
        (BigInteger)(-10),
        ExtendedDecimal.FromString("0e-10").Exponent);
      Assert.AreEqual(
        (BigInteger)(-17),
        ExtendedDecimal.FromString("0.504952e-11").Exponent);
      Assert.AreEqual(
        (BigInteger)(-13),
        ExtendedDecimal.FromString("0e-13").Exponent);
      Assert.AreEqual(
        (BigInteger)(-43),
        ExtendedDecimal.FromString(
          "0.49495052535648555757515648e-17").Exponent);
      Assert.AreEqual(
        (BigInteger)7,
        ExtendedDecimal.FromString("0.485654575150e+19").Exponent);
      Assert.AreEqual(
        BigInteger.Zero,
        ExtendedDecimal.FromString("0.48515648e+8").Exponent);
      Assert.AreEqual(
        (BigInteger)(-45),
        ExtendedDecimal.FromString(
          "0.49485251485649535552535451544956e-13").Exponent);
      Assert.AreEqual(
        (BigInteger)(-6),
        ExtendedDecimal.FromString("0.565754515152575448505257e+18").Exponent);
      Assert.AreEqual(
        (BigInteger)16,
        ExtendedDecimal.FromString("0e+16").Exponent);
      Assert.AreEqual(
        (BigInteger)6,
        ExtendedDecimal.FromString("0.5650e+10").Exponent);
      Assert.AreEqual(
        (BigInteger)(-5),
        ExtendedDecimal.FromString("0.49555554575756575556e+15").Exponent);
      Assert.AreEqual(
        (BigInteger)(-37),
        ExtendedDecimal.FromString("0.57494855545057534955e-17").Exponent);
      Assert.AreEqual(
        (BigInteger)(-25),
        ExtendedDecimal.FromString("0.4956504855525748575456e-3").Exponent);
      Assert.AreEqual(
        (BigInteger)(-26),
        ExtendedDecimal.FromString(
          "0.55575355495654484948525354545053494854e+12").Exponent);
      Assert.AreEqual(
        (BigInteger)(-22),
        ExtendedDecimal.FromString(
          "0.484853575350494950575749545057e+8").Exponent);
      Assert.AreEqual(
        (BigInteger)11,
        ExtendedDecimal.FromString("0.52545451e+19").Exponent);
      Assert.AreEqual(
        (BigInteger)(-29),
        ExtendedDecimal.FromString("0.48485654495751485754e-9").Exponent);
      Assert.AreEqual(
        (BigInteger)(-38),
        ExtendedDecimal.FromString(
          "0.56525456555549545257535556495655574848e+0").Exponent);
      Assert.AreEqual(
        (BigInteger)(-15),
        ExtendedDecimal.FromString(
          "0.485456485657545752495450554857e+15").Exponent);
      Assert.AreEqual(
        (BigInteger)(-37),
        ExtendedDecimal.FromString("0.485448525554495048e-19").Exponent);
      Assert.AreEqual(
        (BigInteger)(-29),
        ExtendedDecimal.FromString("0.494952485550514953565655e-5").Exponent);
      Assert.AreEqual(
        (BigInteger)(-8),
        ExtendedDecimal.FromString(
          "0.50495454554854505051534950e+18").Exponent);
      Assert.AreEqual(
        (BigInteger)(-37),
        ExtendedDecimal.FromString(
          "0.5156524853575655535351554949525449e-3").Exponent);
      Assert.AreEqual(
        (BigInteger)3,
        ExtendedDecimal.FromString("0e+3").Exponent);
      Assert.AreEqual(
        (BigInteger)(-8),
        ExtendedDecimal.FromString(
          "0.51505056554957575255555250e+18").Exponent);
      Assert.AreEqual(
        (BigInteger)(-14),
        ExtendedDecimal.FromString("0.5456e-10").Exponent);
      Assert.AreEqual(
        (BigInteger)(-36),
        ExtendedDecimal.FromString("0.494850515656505252555154e-12").Exponent);
      Assert.AreEqual(
        (BigInteger)(-42),
        ExtendedDecimal.FromString(
          "0.535155525253485757525253555749575749e-6").Exponent);
      Assert.AreEqual(
        (BigInteger)(-29),
        ExtendedDecimal.FromString(
          "0.56554952554850525552515549564948e+3").Exponent);
      Assert.AreEqual(
        (BigInteger)(-40),
        ExtendedDecimal.FromString(
          "0.494855545257545656515554495057e-10").Exponent);
      Assert.AreEqual(
        (BigInteger)(-18),
        ExtendedDecimal.FromString("0.5656504948515252555456e+4").Exponent);
      Assert.AreEqual(
        (BigInteger)(-17),
        ExtendedDecimal.FromString("0e-17").Exponent);
      Assert.AreEqual(
        (BigInteger)(-32),
        ExtendedDecimal.FromString("0.55535551515249535049495256e-6").Exponent);
      Assert.AreEqual(
        (BigInteger)(-31),
        ExtendedDecimal.FromString(
          "0.4948534853564853565654514855e-3").Exponent);
      Assert.AreEqual(
        (BigInteger)(-38),
        ExtendedDecimal.FromString("0.5048485057535249555455e-16").Exponent);
      Assert.AreEqual(
        (BigInteger)(-16),
        ExtendedDecimal.FromString("0e-16").Exponent);
      Assert.AreEqual(
        (BigInteger)5,
        ExtendedDecimal.FromString("0.5354e+9").Exponent);
      Assert.AreEqual(
        BigInteger.One,
        ExtendedDecimal.FromString("0.54e+3").Exponent);
      Assert.AreEqual(
        (BigInteger)(-38),
        ExtendedDecimal.FromString(
          "0.4849525755545751574853494948e-10").Exponent);
      Assert.AreEqual(
        (BigInteger)(-33),
        ExtendedDecimal.FromString("0.52514853565252565251565548e-7").Exponent);
      Assert.AreEqual(
        (BigInteger)(-13),
        ExtendedDecimal.FromString("0.575151545652e-1").Exponent);
      Assert.AreEqual(
        (BigInteger)(-22),
        ExtendedDecimal.FromString("0.49515354514852e-8").Exponent);
      Assert.AreEqual(
        (BigInteger)(-24),
        ExtendedDecimal.FromString("0.54535357515356545554e-4").Exponent);
      Assert.AreEqual(
        (BigInteger)(-11),
        ExtendedDecimal.FromString("0.574848e-5").Exponent);
      Assert.AreEqual(
        (BigInteger)(-3),
        ExtendedDecimal.FromString("0.565055e+3").Exponent);
    }
    [TestMethod]
    public void TestFromBigInteger() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromDouble() {
      string stringTemp;
      {
        stringTemp = ExtendedDecimal.FromDouble(0.75).ToString();
        Assert.AreEqual(
          "0.75",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.5).ToString();
        Assert.AreEqual(
          "0.5",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.25).ToString();
        Assert.AreEqual(
          "0.25",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.875).ToString();
        Assert.AreEqual(
          "0.875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.125).ToString();
        Assert.AreEqual(
          "0.125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.2133).ToString();
        Assert.AreEqual(
          "0.213299999999999989608312489508534781634807586669921875",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(2.2936E-7).ToString();
        Assert.AreEqual(
   "2.29360000000000010330982488752915582352898127282969653606414794921875E-7",
   stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.8932E9).ToString();
        Assert.AreEqual(
          "3893200000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(128230.0).ToString();
        Assert.AreEqual(
          "128230",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(127210.0).ToString();
        Assert.AreEqual(
          "127210",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.26723).ToString();
        Assert.AreEqual(
          "0.267230000000000023074875343809253536164760589599609375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.30233).ToString();
        Assert.AreEqual(
          "0.302329999999999987636556397774256765842437744140625",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(1.9512E-6).ToString();
        Assert.AreEqual(
    "0.0000019512000000000000548530838806460252499164198525249958038330078125",
    stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(199500.0).ToString();
        Assert.AreEqual(
          "199500",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.6214E7).ToString();
        Assert.AreEqual(
          "36214000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.9133E12).ToString();
        Assert.AreEqual(
          "1913300000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(2.1735E-4).ToString();
        Assert.AreEqual(
          "0.0002173499999999999976289799530349000633577816188335418701171875",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.1035E-5).ToString();
        Assert.AreEqual(
       "0.0000310349999999999967797807698399736864303122274577617645263671875",
       stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.275).ToString();
        Assert.AreEqual(
          "1.274999999999999911182158029987476766109466552734375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(214190.0).ToString();
        Assert.AreEqual(
          "214190",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.9813E9).ToString();
        Assert.AreEqual(
          "3981300000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1092700.0).ToString();
        Assert.AreEqual(
          "1092700",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.02361).ToString();
        Assert.AreEqual(
          "0.023609999999999999042987752773115062154829502105712890625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(12.322).ToString();
        Assert.AreEqual(
          "12.321999999999999175770426518283784389495849609375",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.002587).ToString();
        Assert.AreEqual(
          "0.002586999999999999889921387108415729016996920108795166015625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.322E9).ToString();
        Assert.AreEqual(
          "1322000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(9.531E10).ToString();
        Assert.AreEqual(
          "95310000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(142.38).ToString();
        Assert.AreEqual(
          "142.3799999999999954525264911353588104248046875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2252.5).ToString();
        Assert.AreEqual(
          "2252.5",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.636E11).ToString();
        Assert.AreEqual(
          "363600000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.237E-6).ToString();
        Assert.AreEqual(
   "0.00000323700000000000009386523676380154057596882921643555164337158203125",
   stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(728000.0).ToString();
        Assert.AreEqual(
          "728000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.5818E7).ToString();
        Assert.AreEqual(
          "25818000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1090000.0).ToString();
        Assert.AreEqual(
          "1090000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.551).ToString();
        Assert.AreEqual(
          "1.5509999999999999342747969421907328069210052490234375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(26.035).ToString();
        Assert.AreEqual(
          "26.035000000000000142108547152020037174224853515625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(8.33E8).ToString();
        Assert.AreEqual(
          "833000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(8.123E11).ToString();
        Assert.AreEqual(
          "812300000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2622.9).ToString();
        Assert.AreEqual(
          "2622.90000000000009094947017729282379150390625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.291).ToString();
        Assert.AreEqual(
          "1.290999999999999925393012745189480483531951904296875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(286140.0).ToString();
        Assert.AreEqual(
          "286140",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.06733).ToString();
        Assert.AreEqual(
          "0.06733000000000000095923269327613525092601776123046875",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.2516E-4).ToString();
        Assert.AreEqual(
          "0.000325160000000000010654532811571471029310487210750579833984375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.8323E8).ToString();
        Assert.AreEqual(
          "383230000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.028433).ToString();
        Assert.AreEqual(
          "0.02843299999999999994049204588009160943329334259033203125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(8.37E8).ToString();
        Assert.AreEqual(
          "837000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.01608).ToString();
        Assert.AreEqual(
          "0.0160800000000000005428990590417015482671558856964111328125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.621E12).ToString();
        Assert.AreEqual(
          "3621000000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(78.12).ToString();
        Assert.AreEqual(
          "78.1200000000000045474735088646411895751953125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.308E9).ToString();
        Assert.AreEqual(
          "1308000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.031937).ToString();
        Assert.AreEqual(
          "0.031937000000000000110578213252665591426193714141845703125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1581500.0).ToString();
        Assert.AreEqual(
          "1581500",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(244200.0).ToString();
        Assert.AreEqual(
          "244200",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(2.2818E-7).ToString();
        Assert.AreEqual(
   "2.28179999999999995794237200343046456652018605382181704044342041015625E-7",
   stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(39.734).ToString();
        Assert.AreEqual(
          "39.73400000000000176214598468504846096038818359375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1614.0).ToString();
        Assert.AreEqual(
          "1614",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.8319E-4).ToString();
        Assert.AreEqual(
          "0.0003831899999999999954607143859419693399104289710521697998046875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(543.4).ToString();
        Assert.AreEqual(
          "543.3999999999999772626324556767940521240234375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.1931E8).ToString();
        Assert.AreEqual(
          "319310000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1429000.0).ToString();
        Assert.AreEqual(
          "1429000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.6537E12).ToString();
        Assert.AreEqual(
          "2653700000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(7.22E8).ToString();
        Assert.AreEqual(
          "722000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(27.2).ToString();
        Assert.AreEqual(
          "27.199999999999999289457264239899814128875732421875",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.8025E-6).ToString();
        Assert.AreEqual(
   "0.00000380250000000000001586513038998038638283105683512985706329345703125",
   stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.6416E-5).ToString();
        Assert.AreEqual(
          "0.0000364159999999999982843446044711299691698513925075531005859375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2006000.0).ToString();
        Assert.AreEqual(
          "2006000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.6812E9).ToString();
        Assert.AreEqual(
          "2681200000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.7534E10).ToString();
        Assert.AreEqual(
          "27534000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.9116E-7).ToString();
        Assert.AreEqual(
     "3.911600000000000165617541382501176627783934236504137516021728515625E-7",
     stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.0028135).ToString();
        Assert.AreEqual(
          "0.0028135000000000000286437540353290387429296970367431640625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.9119).ToString();
        Assert.AreEqual(
          "0.91190000000000004387601393318618647754192352294921875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2241200.0).ToString();
        Assert.AreEqual(
          "2241200",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(32.45).ToString();
        Assert.AreEqual(
          "32.4500000000000028421709430404007434844970703125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.38E10).ToString();
        Assert.AreEqual(
          "13800000000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.0473).ToString();
        Assert.AreEqual(
          "0.047300000000000001765254609153998899273574352264404296875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(205.34).ToString();
        Assert.AreEqual(
          "205.340000000000003410605131648480892181396484375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.9819).ToString();
        Assert.AreEqual(
          "3.981899999999999995026200849679298698902130126953125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1152.8).ToString();
        Assert.AreEqual(
          "1152.799999999999954525264911353588104248046875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1322000.0).ToString();
        Assert.AreEqual(
          "1322000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(1.3414E-4).ToString();
        Assert.AreEqual(
          "0.00013414000000000001334814203612921801322954706847667694091796875",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(3.445E-7).ToString();
        Assert.AreEqual(
    "3.4449999999999999446924077266263264363033158588223159313201904296875E-7",
    stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(1.361E-7).ToString();
        Assert.AreEqual(
    "1.3610000000000000771138253079228785935583800892345607280731201171875E-7",
    stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.609E7).ToString();
        Assert.AreEqual(
          "26090000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(9.936).ToString();
        Assert.AreEqual(
          "9.93599999999999994315658113919198513031005859375",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(6.0E-6).ToString();
        Assert.AreEqual(
      "0.00000600000000000000015200514458246772164784488268196582794189453125",
      stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(260.31).ToString();
        Assert.AreEqual(
          "260.31000000000000227373675443232059478759765625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(344.6).ToString();
        Assert.AreEqual(
          "344.6000000000000227373675443232059478759765625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.4237).ToString();
        Assert.AreEqual(
          "3.423700000000000187583282240666449069976806640625",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.3421E9).ToString();
        Assert.AreEqual(
          "2342100000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(2.331E-4).ToString();
        Assert.AreEqual(
          "0.00023310000000000000099260877295392901942250318825244903564453125",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.734).ToString();
        Assert.AreEqual(
          "0.7339999999999999857891452847979962825775146484375",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.015415).ToString();
        Assert.AreEqual(
          "0.01541499999999999988287147090204598498530685901641845703125",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.0035311).ToString();
        Assert.AreEqual(
          "0.0035311000000000001240729741169843691750429570674896240234375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(1.2217E12).ToString();
        Assert.AreEqual(
          "1221700000000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(0.483).ToString();
        Assert.AreEqual(
          "0.48299999999999998490096686509787105023860931396484375",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(2.872E-4).ToString();
        Assert.AreEqual(
          "0.0002871999999999999878506906636488338335766457021236419677734375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(96.11).ToString();
        Assert.AreEqual(
          "96.1099999999999994315658113919198513031005859375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(36570.0).ToString();
        Assert.AreEqual(
          "36570",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(1.83E-5).ToString();
        Assert.AreEqual(
      "0.00001830000000000000097183545932910675446692039258778095245361328125",
      stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.0131E8).ToString();
        Assert.AreEqual(
          "301310000",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(382200.0).ToString();
        Assert.AreEqual(
          "382200",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(2.4835E8).ToString();
        Assert.AreEqual(
          "248350000",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(0.001584).ToString();
        Assert.AreEqual(
          "0.0015839999999999999046040866090834242640994489192962646484375",
          stringTemp);
      }

      {
        stringTemp = ExtendedDecimal.FromDouble(7.62E-4).ToString();
        Assert.AreEqual(
          "0.000761999999999999982035203682784185730270110070705413818359375",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromDouble(3.133E11).ToString();
        Assert.AreEqual(
          "313300000000",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestFromExtendedFloat() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromInt32() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromInt64() {
      // not implemented yet
    }
    [TestMethod]
    public void TestFromSingle() {
      string stringTemp;
      {
        stringTemp = ExtendedDecimal.FromSingle(0.75f).ToString();
        Assert.AreEqual(
          "0.75",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromSingle(0.5f).ToString();
        Assert.AreEqual(
          "0.5",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromSingle(0.25f).ToString();
        Assert.AreEqual(
          "0.25",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromSingle(0.875f).ToString();
        Assert.AreEqual(
          "0.875",
          stringTemp);
      }
      {
        stringTemp = ExtendedDecimal.FromSingle(0.125f).ToString();
        Assert.AreEqual(
          "0.125",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestFromString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsFinite() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsInfinity() {
      Assert.IsTrue(ExtendedDecimal.PositiveInfinity.IsInfinity());
      Assert.IsTrue(ExtendedDecimal.NegativeInfinity.IsInfinity());
      Assert.IsFalse(ExtendedDecimal.Zero.IsInfinity());
      Assert.IsFalse(ExtendedDecimal.NaN.IsInfinity());
    }
    [TestMethod]
    public void TestIsNaN() {
      Assert.IsFalse(ExtendedDecimal.PositiveInfinity.IsNaN());
      Assert.IsFalse(ExtendedDecimal.NegativeInfinity.IsNaN());
      Assert.IsFalse(ExtendedDecimal.Zero.IsNaN());
      Assert.IsTrue(ExtendedDecimal.NaN.IsNaN());
    }
    [TestMethod]
    public void TestIsNegative() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsNegativeInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsPositiveInfinity() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsQuietNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsSignalingNaN() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsZero() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLog() {
      // not implemented yet
    }
    [TestMethod]
    public void TestLog10() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMantissa() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMax() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMaxMagnitude() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMin() {
      try {
        ExtendedDecimal.Min(null, ExtendedDecimal.One);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ExtendedDecimal.Min(ExtendedDecimal.One, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestMovePointLeft() {
      {
string stringTemp = ExtendedDecimal.FromString(
"1").MovePointLeft(BigInteger.Zero, null).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.1").MovePointLeft(BigInteger.Zero, null).ToString();
Assert.AreEqual(
"0.1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.01").MovePointLeft(BigInteger.Zero, null).ToString();
Assert.AreEqual(
"0.01",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"1").MovePointLeft(0, null).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.1").MovePointLeft(0, null).ToString();
Assert.AreEqual(
"0.1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.01").MovePointLeft(0, null).ToString();
Assert.AreEqual(
"0.01",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString("1").MovePointLeft(0).ToString();
Assert.AreEqual(
"1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.1").MovePointLeft(0).ToString();
Assert.AreEqual(
"0.1",
stringTemp);
}
      {
string stringTemp = ExtendedDecimal.FromString(
"0.01").MovePointLeft(0).ToString();
Assert.AreEqual(
"0.01",
stringTemp);
}
    }
    [TestMethod]
    public void TestMinMagnitude() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiply() {
      // not implemented yet
    }
    [TestMethod]
    public void TestMultiplyAndAdd() {
      try {
 ExtendedDecimal.One.MultiplyAndAdd(null, ExtendedDecimal.Zero, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndAdd(ExtendedDecimal.Zero, null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndAdd(null, null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndAdd(null, ExtendedDecimal.Zero);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndAdd(ExtendedDecimal.Zero, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndAdd(null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [TestMethod]
    public void TestMultiplyAndSubtract() {
      try {
 ExtendedDecimal.One.MultiplyAndSubtract(null, ExtendedDecimal.Zero, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndSubtract(ExtendedDecimal.Zero, null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 ExtendedDecimal.One.MultiplyAndSubtract(null, null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
Console.Write(String.Empty);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [TestMethod]
    public void TestNegate() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextMinus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextPlus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestNextToward() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPI() {
      ExtendedDecimal pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(3));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.14",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(4));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.142",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(5));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.1416",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(6));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.14159",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(7));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.141593",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(8));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.1415927",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(9));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.14159265",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(10));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.141592654",
          stringTemp);
      }
      pi = ExtendedDecimal.PI(PrecisionContext.ForPrecision(25));
      {
        string stringTemp = pi.ToPlainString();
        Assert.AreEqual(
          "3.141592653589793238462643",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestPlus() {
      // not implemented yet
    }
    [TestMethod]
    public void TestPow() {
      // not implemented yet
    }
    [TestMethod]
    public void TestQuantize() {
      // not implemented yet
    }
    [TestMethod]
    public void TestReduce() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainder() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainderNaturalScale() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRemainderNear() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToBinaryPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToExponent() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToExponentExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToIntegralExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToIntegralNoRoundedFlag() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRoundToPrecision() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSign() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSquareRoot() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubtract() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToBigInteger() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToBigIntegerExact() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToDouble() {
      // test for correct rounding
      double dbl;
      dbl = ExtendedDecimal.FromString(
        "1.972579273363468721491642554610734805464744567871093749999999999999")
        .ToDouble();
      {
        string stringTemp = ExtendedFloat.FromDouble(dbl).ToPlainString();
        Assert.AreEqual(
          "1.9725792733634686104693400920950807631015777587890625",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestToEngineeringString() {
      {
        string stringTemp = ExtendedDecimal.FromString(
          "89.12E-1").ToEngineeringString();
        Assert.AreEqual(
          "8.912",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "242.31E-4").ToEngineeringString();
        Assert.AreEqual(
          "0.024231",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "229.18E5").ToEngineeringString();
        Assert.AreEqual(
          "22.918E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "326.18E-7").ToEngineeringString();
        Assert.AreEqual(
          "0.000032618",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("55.0E6").ToEngineeringString();
        Assert.AreEqual(
          "55.0E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "224.36E3").ToEngineeringString();
        Assert.AreEqual(
          "224.36E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "230.12E9").ToEngineeringString();
        Assert.AreEqual(
          "230.12E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "113.20E-7").ToEngineeringString();
        Assert.AreEqual(
          "0.000011320",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "317.7E-9").ToEngineeringString();
        Assert.AreEqual(
          "317.7E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "339.3E-2").ToEngineeringString();
        Assert.AreEqual(
          "3.393",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "271.35E8").ToEngineeringString();
        Assert.AreEqual(
          "27.135E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "377.19E-9").ToEngineeringString();
        Assert.AreEqual(
          "377.19E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "321.27E7").ToEngineeringString();
        Assert.AreEqual(
          "3.2127E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "294.22E-2").ToEngineeringString();
        Assert.AreEqual(
          "2.9422",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "110.31E-8").ToEngineeringString();
        Assert.AreEqual(
          "0.0000011031",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "243.24E-2").ToEngineeringString();
        Assert.AreEqual(
          "2.4324",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "64.12E-5").ToEngineeringString();
        Assert.AreEqual(
          "0.0006412",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "142.23E1").ToEngineeringString();
        Assert.AreEqual(
          "1422.3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"293.0E0").ToEngineeringString();
        Assert.AreEqual(
          "293.0",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "253.20E-8").ToEngineeringString();
        Assert.AreEqual(
          "0.0000025320",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"366.6E8").ToEngineeringString();
        Assert.AreEqual(
          "36.66E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "345.26E10").ToEngineeringString();
        Assert.AreEqual(
          "3.4526E+12",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "270.4E-2").ToEngineeringString();
        Assert.AreEqual(
          "2.704",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("4.32E8").ToEngineeringString();
        Assert.AreEqual(
          "432E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "224.22E0").ToEngineeringString();
        Assert.AreEqual(
          "224.22",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "315.30E-7").ToEngineeringString();
        Assert.AreEqual(
          "0.000031530",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "115.32E5").ToEngineeringString();
        Assert.AreEqual(
          "11.532E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "394.20E2").ToEngineeringString();
        Assert.AreEqual(
          "39420",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "67.24E-9").ToEngineeringString();
        Assert.AreEqual(
          "67.24E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "349.33E2").ToEngineeringString();
        Assert.AreEqual(
          "34933",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"67.8E-9").ToEngineeringString();
        Assert.AreEqual(
          "67.8E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "192.31E5").ToEngineeringString();
        Assert.AreEqual(
          "19.231E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "173.17E7").ToEngineeringString();
        Assert.AreEqual(
          "1.7317E+9",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("43.9E0").ToEngineeringString();
        Assert.AreEqual(
          "43.9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "168.12E-8").ToEngineeringString();
        Assert.AreEqual(
          "0.0000016812",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "371.5E10").ToEngineeringString();
        Assert.AreEqual(
          "3.715E+12",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"42.4E-8").ToEngineeringString();
        Assert.AreEqual(
          "424E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "161.23E10").ToEngineeringString();
        Assert.AreEqual(
          "1.6123E+12",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"302.8E6").ToEngineeringString();
        Assert.AreEqual(
          "302.8E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "175.13E0").ToEngineeringString();
        Assert.AreEqual(
          "175.13",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "298.20E-9").ToEngineeringString();
        Assert.AreEqual(
          "298.20E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "362.23E8").ToEngineeringString();
        Assert.AreEqual(
          "36.223E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "277.39E2").ToEngineeringString();
        Assert.AreEqual(
          "27739",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "117.34E-4").ToEngineeringString();
        Assert.AreEqual(
          "0.011734",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "190.13E-9").ToEngineeringString();
        Assert.AreEqual(
          "190.13E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "350.19E-2").ToEngineeringString();
        Assert.AreEqual(
          "3.5019",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "383.27E-9").ToEngineeringString();
        Assert.AreEqual(
          "383.27E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "242.17E5").ToEngineeringString();
        Assert.AreEqual(
          "24.217E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "299.23E7").ToEngineeringString();
        Assert.AreEqual(
          "2.9923E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "302.22E-2").ToEngineeringString();
        Assert.AreEqual(
          "3.0222",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "45.21E-3").ToEngineeringString();
        Assert.AreEqual(
          "0.04521",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "150.0E-1").ToEngineeringString();
        Assert.AreEqual(
          "15.00",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("29.0E4").ToEngineeringString();
        Assert.AreEqual(
          "290E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "263.37E3").ToEngineeringString();
        Assert.AreEqual(
          "263.37E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "283.21E-1").ToEngineeringString();
        Assert.AreEqual(
          "28.321",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"21.32E0").ToEngineeringString();
        Assert.AreEqual(
          "21.32",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "69.20E-6").ToEngineeringString();
        Assert.AreEqual(
          "0.00006920",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"72.8E-3").ToEngineeringString();
        Assert.AreEqual(
          "0.0728",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"164.6E7").ToEngineeringString();
        Assert.AreEqual(
          "1.646E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "118.17E-2").ToEngineeringString();
        Assert.AreEqual(
          "1.1817",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "262.35E-7").ToEngineeringString();
        Assert.AreEqual(
          "0.000026235",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"233.7E5").ToEngineeringString();
        Assert.AreEqual(
          "23.37E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "391.24E0").ToEngineeringString();
        Assert.AreEqual(
          "391.24",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "221.36E1").ToEngineeringString();
        Assert.AreEqual(
          "2213.6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "353.32E0").ToEngineeringString();
        Assert.AreEqual(
          "353.32",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "129.31E-4").ToEngineeringString();
        Assert.AreEqual(
          "0.012931",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "176.26E-5").ToEngineeringString();
        Assert.AreEqual(
          "0.0017626",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"207.5E3").ToEngineeringString();
        Assert.AreEqual(
          "207.5E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "314.10E0").ToEngineeringString();
        Assert.AreEqual(
          "314.10",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "379.20E9").ToEngineeringString();
        Assert.AreEqual(
          "379.20E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "379.12E-6").ToEngineeringString();
        Assert.AreEqual(
          "0.00037912",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "74.38E-8").ToEngineeringString();
        Assert.AreEqual(
          "743.8E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "234.17E-9").ToEngineeringString();
        Assert.AreEqual(
          "234.17E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"13.26E7").ToEngineeringString();
        Assert.AreEqual(
          "132.6E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"251.5E5").ToEngineeringString();
        Assert.AreEqual(
          "25.15E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"87.32E0").ToEngineeringString();
        Assert.AreEqual(
          "87.32",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "331.16E7").ToEngineeringString();
        Assert.AreEqual(
          "3.3116E+9",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("61.4E8").ToEngineeringString();
        Assert.AreEqual(
          "6.14E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "209.7E-6").ToEngineeringString();
        Assert.AreEqual(
          "0.0002097",
          stringTemp);
      }
      {
 string stringTemp = ExtendedDecimal.FromString("5.4E6").ToEngineeringString();
        Assert.AreEqual(
          "5.4E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"219.9E0").ToEngineeringString();
        Assert.AreEqual(
          "219.9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "26.31E-6").ToEngineeringString();
        Assert.AreEqual(
          "0.00002631",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"48.28E7").ToEngineeringString();
        Assert.AreEqual(
          "482.8E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"267.8E0").ToEngineeringString();
        Assert.AreEqual(
          "267.8",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "320.9E-3").ToEngineeringString();
        Assert.AreEqual(
          "0.3209",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "300.15E-3").ToEngineeringString();
        Assert.AreEqual(
          "0.30015",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "260.11E4").ToEngineeringString();
        Assert.AreEqual(
          "2.6011E+6",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "114.29E-2").ToEngineeringString();
        Assert.AreEqual(
          "1.1429",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "306.0E-6").ToEngineeringString();
        Assert.AreEqual(
          "0.0003060",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("97.7E3").ToEngineeringString();
        Assert.AreEqual(
          "97.7E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "122.29E8").ToEngineeringString();
        Assert.AreEqual(
          "12.229E+9",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("69.4E2").ToEngineeringString();
        Assert.AreEqual(
          "6.94E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"383.5E0").ToEngineeringString();
        Assert.AreEqual(
          "383.5",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "315.30E3").ToEngineeringString();
        Assert.AreEqual(
          "315.30E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "130.38E9").ToEngineeringString();
        Assert.AreEqual(
          "130.38E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "206.16E9").ToEngineeringString();
        Assert.AreEqual(
          "206.16E+9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "304.28E-9").ToEngineeringString();
        Assert.AreEqual(
          "304.28E-9",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
"66.13E4").ToEngineeringString();
        Assert.AreEqual(
          "661.3E+3",
          stringTemp);
      }
      {
        string stringTemp = ExtendedDecimal.FromString(
          "185.33E-2").ToEngineeringString();
        Assert.AreEqual(
          "1.8533",
          stringTemp);
      }
      {
string stringTemp = ExtendedDecimal.FromString("70.7E6").ToEngineeringString();
        Assert.AreEqual(
          "70.7E+6",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestToExtendedFloat() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToPlainString() {
      {
string stringTemp = ExtendedDecimal.NegativeZero.ToPlainString();
Assert.AreEqual(
"-0",
stringTemp);
}
      {
    string stringTemp = ExtendedDecimal.FromString("277.22E9").ToPlainString();
        Assert.AreEqual(
          "277220000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("391.19E4").ToPlainString();
        Assert.AreEqual(
          "3911900",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("383.27E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000038327",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("47.33E9").ToPlainString();
        Assert.AreEqual(
          "47330000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("322.21E3").ToPlainString();
        Assert.AreEqual(
          "322210",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("191.3E-2").ToPlainString();
        Assert.AreEqual(
          "1.913",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("119.17E2").ToPlainString();
        Assert.AreEqual(
          "11917",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("159.6E-6").ToPlainString();
        Assert.AreEqual(
          "0.0001596",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("70.16E9").ToPlainString();
        Assert.AreEqual(
          "70160000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("166.24E9").ToPlainString();
        Assert.AreEqual(
          "166240000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("235.25E3").ToPlainString();
        Assert.AreEqual(
          "235250",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("37.22E7").ToPlainString();
        Assert.AreEqual(
          "372200000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("320.26E8").ToPlainString();
        Assert.AreEqual(
          "32026000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("127.11E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000012711",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("97.29E-7").ToPlainString();
        Assert.AreEqual(
          "0.000009729",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("175.13E9").ToPlainString();
        Assert.AreEqual(
          "175130000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("38.21E-7").ToPlainString();
        Assert.AreEqual(
          "0.000003821",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("6.28E1").ToPlainString();
        Assert.AreEqual(
          "62.8",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("138.29E6").ToPlainString();
        Assert.AreEqual(
          "138290000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("160.19E1").ToPlainString();
        Assert.AreEqual(
          "1601.9",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("358.12E2").ToPlainString();
        Assert.AreEqual(
          "35812",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("249.28E10").ToPlainString();
        Assert.AreEqual(
          "2492800000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("311.23E-6").ToPlainString();
        Assert.AreEqual(
          "0.00031123",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("164.33E-3").ToPlainString();
        Assert.AreEqual(
          "0.16433",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("299.20E-1").ToPlainString();
        Assert.AreEqual(
          "29.920",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("105.39E3").ToPlainString();
        Assert.AreEqual(
          "105390",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("382.5E4").ToPlainString();
        Assert.AreEqual(
          "3825000",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("90.9E1").ToPlainString();
        Assert.AreEqual(
          "909",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("329.15E8").ToPlainString();
        Assert.AreEqual(
          "32915000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("245.23E8").ToPlainString();
        Assert.AreEqual(
          "24523000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("97.19E-8").ToPlainString();
        Assert.AreEqual(
          "0.0000009719",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("55.12E7").ToPlainString();
        Assert.AreEqual(
          "551200000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("12.38E2").ToPlainString();
        Assert.AreEqual(
          "1238",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("250.20E-5").ToPlainString();
        Assert.AreEqual(
          "0.0025020",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("53.20E2").ToPlainString();
        Assert.AreEqual(
          "5320",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("141.5E8").ToPlainString();
        Assert.AreEqual(
          "14150000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("338.34E-5").ToPlainString();
        Assert.AreEqual(
          "0.0033834",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("160.39E9").ToPlainString();
        Assert.AreEqual(
          "160390000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("152.17E6").ToPlainString();
        Assert.AreEqual(
          "152170000",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("13.3E9").ToPlainString();
        Assert.AreEqual(
          "13300000000",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("1.38E1").ToPlainString();
        Assert.AreEqual(
          "13.8",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("348.21E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000034821",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("52.5E7").ToPlainString();
        Assert.AreEqual(
          "525000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("215.21E10").ToPlainString();
        Assert.AreEqual(
          "2152100000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("234.28E9").ToPlainString();
        Assert.AreEqual(
          "234280000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("310.24E9").ToPlainString();
        Assert.AreEqual(
          "310240000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("345.39E9").ToPlainString();
        Assert.AreEqual(
          "345390000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("116.38E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000011638",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("276.25E10").ToPlainString();
        Assert.AreEqual(
          "2762500000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("158.32E-8").ToPlainString();
        Assert.AreEqual(
          "0.0000015832",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("272.5E2").ToPlainString();
        Assert.AreEqual(
          "27250",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("389.33E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000038933",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("381.15E7").ToPlainString();
        Assert.AreEqual(
          "3811500000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("280.0E3").ToPlainString();
        Assert.AreEqual(
          "280000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("274.2E-6").ToPlainString();
        Assert.AreEqual(
          "0.0002742",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("387.14E-7").ToPlainString();
        Assert.AreEqual(
          "0.000038714",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("227.7E-7").ToPlainString();
        Assert.AreEqual(
          "0.00002277",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("201.21E2").ToPlainString();
        Assert.AreEqual(
          "20121",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("255.4E3").ToPlainString();
        Assert.AreEqual(
          "255400",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("187.27E-7").ToPlainString();
        Assert.AreEqual(
          "0.000018727",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("169.7E-4").ToPlainString();
        Assert.AreEqual(
          "0.01697",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("69.9E9").ToPlainString();
        Assert.AreEqual(
          "69900000000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("3.20E-2").ToPlainString();
        Assert.AreEqual(
          "0.0320",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("236.30E2").ToPlainString();
        Assert.AreEqual(
          "23630",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("220.22E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000022022",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("287.30E-1").ToPlainString();
        Assert.AreEqual(
          "28.730",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("156.3E-9").ToPlainString();
        Assert.AreEqual(
          "0.0000001563",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("136.23E-1").ToPlainString();
        Assert.AreEqual(
          "13.623",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("125.27E8").ToPlainString();
        Assert.AreEqual(
          "12527000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("180.30E-7").ToPlainString();
        Assert.AreEqual(
          "0.000018030",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("351.5E7").ToPlainString();
        Assert.AreEqual(
          "3515000000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("28.28E9").ToPlainString();
        Assert.AreEqual(
          "28280000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("288.4E-3").ToPlainString();
        Assert.AreEqual(
          "0.2884",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("12.22E4").ToPlainString();
        Assert.AreEqual(
          "122200",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("257.5E-5").ToPlainString();
        Assert.AreEqual(
          "0.002575",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("389.20E3").ToPlainString();
        Assert.AreEqual(
          "389200",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("394.9E-4").ToPlainString();
        Assert.AreEqual(
          "0.03949",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("134.26E-7").ToPlainString();
        Assert.AreEqual(
          "0.000013426",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("58.29E5").ToPlainString();
        Assert.AreEqual(
          "5829000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("88.5E-5").ToPlainString();
        Assert.AreEqual(
          "0.000885",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("193.29E-4").ToPlainString();
        Assert.AreEqual(
          "0.019329",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("71.35E10").ToPlainString();
        Assert.AreEqual(
          "713500000000",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("252.0E1").ToPlainString();
        Assert.AreEqual(
          "2520",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("53.2E-8").ToPlainString();
        Assert.AreEqual(
          "0.000000532",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("181.20E-1").ToPlainString();
        Assert.AreEqual(
          "18.120",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("55.21E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000005521",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("57.31E0").ToPlainString();
        Assert.AreEqual(
          "57.31",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("113.13E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000011313",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("53.23E1").ToPlainString();
        Assert.AreEqual(
          "532.3",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("368.37E-7").ToPlainString();
        Assert.AreEqual(
          "0.000036837",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("187.4E-4").ToPlainString();
        Assert.AreEqual(
          "0.01874",
          stringTemp);
      }
      {
      string stringTemp = ExtendedDecimal.FromString("5.26E8").ToPlainString();
        Assert.AreEqual(
          "526000000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("308.32E4").ToPlainString();
        Assert.AreEqual(
          "3083200",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("76.15E-2").ToPlainString();
        Assert.AreEqual(
          "0.7615",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("117.38E7").ToPlainString();
        Assert.AreEqual(
          "1173800000",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("15.37E-4").ToPlainString();
        Assert.AreEqual(
          "0.001537",
          stringTemp);
      }
      {
     string stringTemp = ExtendedDecimal.FromString("145.3E0").ToPlainString();
        Assert.AreEqual(
          "145.3",
          stringTemp);
      }
      {
    string stringTemp = ExtendedDecimal.FromString("226.29E8").ToPlainString();
        Assert.AreEqual(
          "22629000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("224.26E10").ToPlainString();
        Assert.AreEqual(
          "2242600000000",
          stringTemp);
      }
      {
   string stringTemp = ExtendedDecimal.FromString("268.18E-9").ToPlainString();
        Assert.AreEqual(
          "0.00000026818",
          stringTemp);
      }
    }
    [TestMethod]
    public void TestToSingle() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestUnsignedMantissa() {
      // not implemented yet
    }
  }
}
