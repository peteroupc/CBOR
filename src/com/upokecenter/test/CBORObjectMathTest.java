package com.upokecenter.test;
/*
 * Created by SharpDevelop.
 * User: PeterRoot
 * Date: 11/7/2013
 * Time: 9:29 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

    /**
     * Tests for math functions in CBORObject.
     */

  public class CBORObjectMathTest {
    @Test
    public void CBORMultiplyTest() {
      {
        CBORObject a = CBORObject.FromObject(9.12506362461426E-6d);
        CBORObject b = CBORObject.FromObject(5.641029925579569E-11d);
        Assert.assertEquals("5.1474756979266608403696403459529410796038238050457548796657151114380471943990657843652277518542957623903788544339477084577083587646484375E-16", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.5268476f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-503285733164839762215"));
        Assert.assertEquals("-1271726347433338951145.9595930576324462890625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-634889541024E+14"));
        CBORObject b = CBORObject.FromObject(-0.39601803371455613d);
        Assert.assertEquals("25142770766226149794078868.282554140023421496152877807617187500000", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(40.20107f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("7996746492.7274681343565519706"));
        Assert.assertEquals("321477756555.4319320007490459024696784973144531250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(0.022119554f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-9970998242E-8"));
        Assert.assertEquals("-2.2055403165173435583710670471191406250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.9863508f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("41913315893308968"));
        Assert.assertEquals("125167863390954038.6309108734130859375000", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("51373759105328576374643305691411448"));
        CBORObject b = CBORObject.FromObject(-4.5222281096959924E-14d);
        Assert.assertEquals("-2323238575268673257254.501371566891879118691399132531527312220315285794225955429137542296302854083478450775146484375000", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-482961178579026628E+6"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("826102917E+12"));
        Assert.assertEquals("-3.98975638421891812411473876E+44", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-9.033945173701306E-5d);
        CBORObject b = CBORObject.FromObject(-2.0573592855473745E-15d);
        Assert.assertEquals("1.8586070988240271295348402259229550852948804235400365473459642740392033758695647858134238214561141872013610119862558889280990115366876125335693359375E-19", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("2156523316826275"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("930720059.845757"));
        Assert.assertEquals("2007119510495321051619264.865175", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-1549"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-0149138346E0"));
        Assert.assertEquals("231015297954", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("9574254"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("41249908590533189242686"));
        Assert.assertEquals("394937102322546749239543406244", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-0121472570951874677E0"));
        CBORObject b = CBORObject.FromObject(-6564123303496238409L);
        Assert.assertEquals("797360933720800814177596036281868893", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-84626937430208985E-5"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-47837170150939961006483074488002"));
        Assert.assertEquals("40483132052018569867862771568011144050367350.97970", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(83.96423266687334d);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-07749556847133167"));
        Assert.assertEquals("-650685594177850651.7836587888087223063848796300590038299560546875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1078924234326945061L);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-70255986437016738376158956"));
        Assert.assertEquals("75800886373442521468941315650954034815116316", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(9.796470955886588E-7d);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-0495371"));
        Assert.assertEquals("-0.48528876138884952104734314530000427456712941420846618711948394775390625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-476244027101E+14"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-656"));
        Assert.assertEquals("3.12416081778256E+28", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-6719311033823066015L);
        CBORObject b = CBORObject.FromObject(-3.7584813f);
        Assert.assertEquals("25254404628381017917.0420634746551513671875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("4499543371883779.26312785061842"));
        CBORObject b = CBORObject.FromObject(-0.05685488f);
        Assert.assertEquals("-255821004520648.74279288946467020799115300178527832031250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("30424603577890669724.32E-18"));
        CBORObject b = CBORObject.FromObject(8330281951735967713L);
        Assert.assertEquals("253445526073624194569.95685530936613088016", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-216.48886f);
        CBORObject b = CBORObject.FromObject(0.0027001811397703835d);
        Assert.assertEquals("-0.5845591396693451473286915745500498010900347622964545735158026218414306640625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("38590484691689397685891"));
        CBORObject b = CBORObject.FromObject(1.1009459375566415E-10d);
        Assert.assertEquals("4248603734965.72066567535336329941927488838145139684907372092104171912296806112863123416900634765625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("676774623"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-47634494821244064152830613"));
        Assert.assertEquals("-32237817274442903908019752495933899", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-05507"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("1E+8"));
        Assert.assertEquals("-5.507E+11", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-832723796052.167"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-39021467614708813503.2375E+6"));
        Assert.assertEquals("32494104639647021516128503963573390662.5", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(1.0216558189301154E-15d);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-5724273202"));
        Assert.assertEquals("-0.000005848237025969023709984342844564627101483494383367053859323902997857658192515373229980468750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(1.4418060160592996E-7d);
        CBORObject b = CBORObject.FromObject(-5225904869088262743L);
        Assert.assertEquals("-753474107960.504388515493593327065454515515351141541344759389176033437252044677734375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2611.4025486401783d);
        CBORObject b = CBORObject.FromObject(-432.74161434287936d);
        Assert.assertEquals("-1130062.5545976603110506039594126209975573529623277084510402090700154076330363750457763671875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-3268148833648183726L);
        CBORObject b = CBORObject.FromObject(10.474402f);
        Assert.assertEquals("-34231906077162329708.75671577453613281250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-02"));
        CBORObject b = CBORObject.FromObject(-5702680986781981196L);
        Assert.assertEquals("11405361973563962392", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(9.818825f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-8990374781030894584337219503247649429"));
        Assert.assertEquals("-88274914574185741393190188566231820133.88379669189453125", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1731184543616306151L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-75117887"));
        Assert.assertEquals("130042924923516256808222937", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-9274297774670234"));
        CBORObject b = CBORObject.FromObject(-3.7425284200877295E-11d);
        Assert.assertEquals("347093.22998059736154967580553904219931632546813091502735382221089821541681885719299316406250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-80831478952"));
        CBORObject b = CBORObject.FromObject(72074398611184807L);
        Assert.assertEquals("-5825880234318042758802682264", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-2.0124694849364912E-14d);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("7876905570E-3"));
        Assert.assertEquals("-1.5852032095351279016951961015040717951256076141008542410015813572954357368871569633483886718750E-7", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.9403898774636966E-6d);
        CBORObject b = CBORObject.FromObject(-73.227356f);
        Assert.assertEquals("-0.000215316976209485610782653428537671523590457667296504951082170009613037109375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1844.0043910488544d);
        CBORObject b = CBORObject.FromObject(3.577965315137519E-4d);
        Assert.assertEquals("-0.6597783752134083319611068631031597182993086653506953622393658986933218102421960793435573577880859375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.6778665f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("1892809.9237E-5"));
        Assert.assertEquals("50.686922077357120990753173828125", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(6556434482543438783L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("0627919640783365.805980265431E-7"));
        Assert.assertEquals("411691398509834894775784356.3566721309839610473", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2460196409348057722L);
        CBORObject b = CBORObject.FromObject(-523684582029466795L);
        Assert.assertEquals("-1288366928339832603911437504542340990", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1332757343747061131L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("909227776E-17"));
        Assert.assertEquals("-12117799956.02807898675174656", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-499148296236.866E+5"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-7389260064602357.0879"));
        Assert.assertEquals("368833657169738093247090913238252.14", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(8335870817615450098L);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-563870621864145974050"));
        Assert.assertEquals("-4700352661708010793197043881265377956900", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1.4152980465365456E-11d);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("77621.58189310594065683"));
        Assert.assertEquals("-0.000001098576732223893356223492868536291894931482577314269116221766287022632013758993707597255706787109375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-463514687832433253675988214902"));
        CBORObject b = CBORObject.FromObject(-4484615119892422017L);
        Assert.assertEquals("2078684977345546220618244518078728370549072297334", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("29638173439801182344"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-89020"));
        Assert.assertEquals("-2638390199611101252262880", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-12.33862f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-4926900679850357315"));
        Assert.assertEquals("60791156182089811119.72905635833740234375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(1661256806824871167L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-577377.37"));
        Assert.assertEquals("-959172086019142164991290.79", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-7247541380132001722E-18"));
        CBORObject b = CBORObject.FromObject(1.3541237329610524E-12d);
        Assert.assertEquals("-9.814067788454043881238312650252126235811196688351305253070200329545968997990712523460388183593750E-12", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-56413.2304411423016489"));
        CBORObject b = CBORObject.FromObject(-2384706973929665461L);
        Assert.assertEquals("134529024054893344711965.0190087256786429", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(6733533195954145787L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("6320087056523446"));
        Assert.assertEquals("42556515996420749375245046867622002", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.901689f);
        CBORObject b = CBORObject.FromObject(-7.967420595813069E-5d);
        Assert.assertEquals("-0.00023118977120185441111747832822020913223445316708737351518720970489084720611572265625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(7303853603793886142L);
        CBORObject b = CBORObject.FromObject(-4.1619286721848676E-4d);
        Assert.assertEquals("-3039811773107054.8767210035855622685162583107576494967361213639378547668457031250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(0.020507919f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-5992214163"));
        Assert.assertEquals("-122887840.71266113780438899993896484375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(5661632373178241576L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("468050559108240"));
        Assert.assertEquals("2649930197731387664356653852186240", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("7585E-6"));
        CBORObject b = CBORObject.FromObject(1.048538682153392E-12d);
        Assert.assertEquals("7.953165904133478602964909855230447451274015946420803402361343614757061004638671875E-15", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1.1449391699652754E-9d);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-8580867092245"));
        Assert.assertEquals("9824.5708461773363347106053394302652805576236798301703601055123726837337017059326171875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(14.719088f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-63"));
        Assert.assertEquals("-927.3025188446044921875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-0.031387743f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("3045954"));
        Assert.assertEquals("-95605.6201502010226249694824218750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1240022903018164741L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("024"));
        Assert.assertEquals("-29760549672435953784", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(2.362720890791882E-11d);
        CBORObject b = CBORObject.FromObject(-2652449361903108095L);
        Assert.assertEquals("-62669975.19136070811944330918553514489337903936255302732305505486465335707180202007293701171875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("583988109357508438741511"));
        CBORObject b = CBORObject.FromObject(-1.6970299566622061E-15d);
        Assert.assertEquals("-991045315.914216249444323438492234978558828936731849875803211105145169336483323974107406684197485446929931640625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-28.330496f);
        CBORObject b = CBORObject.FromObject(-0.03418379f);
        Assert.assertEquals("0.96844369313088662920563365332782268524169921875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-2.5572328352504374E-11d);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("93642E-19"));
        Assert.assertEquals("-2.3946439715852145453060031900333044465505397124793773855344625189900398254394531250E-25", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-0.049292516f);
        CBORObject b = CBORObject.FromObject(-5651575385736113028L);
        Assert.assertEquals("278580369919280926.8629515618085861206054687500", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-0.1054924f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-70703193"));
        Assert.assertEquals("7458649.38592426478862762451171875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-0.096198395f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("483334892742.650574"));
        Assert.assertEquals("-46496040878.39659426896438002586364746093750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-7.8858E+18"));
        CBORObject b = CBORObject.FromObject(1.9232477390530362E-11d);
        Assert.assertEquals("-151663470.2062443270692102216363539231606602508684034091857029125094413757324218750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-677744989818709411E-18"));
        CBORObject b = CBORObject.FromObject(949957726360177009L);
        Assert.assertEquals("-643829089580182507.625997593894131699", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("0247111039693066700.39680963861467394E-17"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("062292"));
        Assert.assertEquals("153930.4088456051090111806600858526907048", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(1030399562791308238L);
        CBORObject b = CBORObject.FromObject(-63.323025f);
        Assert.assertEquals("-65248017016771628246.493293762207031250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(0.09242566f);
        CBORObject b = CBORObject.FromObject(0.4944296f);
        Assert.assertEquals("0.0456979806771613539240206591784954071044921875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("947048618983259414095031115"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-9E-4"));
        Assert.assertEquals("-852343757084933472685528.0035", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-7173498382128361348L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("6688134265092824295E-13"));
        Assert.assertEquals("-4797732033010063208986509.4961633349660", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-2028.920576010507d);
        CBORObject b = CBORObject.FromObject(1.0658400118503382E-7d);
        Assert.assertEquals("-0.0002162504730778433667813842565077532325672641399937932382221494895051770723792827766374102793633937835693359375", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(0.028014688f);
        CBORObject b = CBORObject.FromObject(0.010853733f);
        Assert.assertEquals("0.00030406393989982662129367696479675942100584506988525390625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-40065583103663138597508921275"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("473"));
        Assert.assertEquals("-18951020808032664556621719763075", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(171580011877233958L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("8166976239733008E+9"));
        Assert.assertEquals("1.401289880214477041376031471085664E+42", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(4756503004496483257L);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-3484"));
        Assert.assertEquals("-16571656467665747667388", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(47.861744f);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("5256943339128067199183052"));
        Assert.assertEquals("251606475936106139213206572.928573608398437500", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-8.889726335187815E-7d);
        CBORObject b = CBORObject.FromObject(-119.396965f);
        Assert.assertEquals("0.000106140634434073564219476413377792971139175893895145730283502416568808257579803466796875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(613.0484225691173d);
        CBORObject b = CBORObject.FromObject(-478722774596480457L);
        Assert.assertEquals("-293480241814283440191.365496581981687995721586048603057861328125", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-982389974358784.06347691416415211E-4"));
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("-4187980866582449.37089364048194250807E-10"));
        Assert.assertEquals("41142304161370106.994045393340730603937626242149336883959755783825277", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-1505.0469576735418d);
        CBORObject b = CBORObject.FromObject(-5063480950546518686L);
        Assert.assertEquals("7620776599857971423692.43786716507656819885596632957458496093750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-612239.22621661"));
        CBORObject b = CBORObject.FromObject(1014448531968380355L);
        Assert.assertEquals("-621085184248897141528476.09869655", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(0.6220621f);
        CBORObject b = CBORObject.FromObject(ExtendedDecimal.FromString("052331300.60E+1"));
        Assert.assertEquals("325533180.697489976882934570312500", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(115.93607f);
        CBORObject b = CBORObject.FromObject(6.284829f);
        Assert.assertEquals("728.6384118395872064866125583648681640625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-4264980344"));
        CBORObject b = CBORObject.FromObject(-353911288916818824L);
        Assert.assertEquals("1509424690749937335369195456", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("-26252698366425756223523673798"));
        CBORObject b = CBORObject.FromObject(-4.044065397264691E-12d);
        Assert.assertEquals("106167629048489689.528899553185499035078122372783849572121359674777354986652255774970399215817451477050781250", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(BigInteger.fromString("060678661656686749713123"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-174887388"));
        Assert.assertEquals("-10611932644473698391537830792724", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-6.899643923480183d);
        CBORObject b = CBORObject.FromObject(2.9392164696369233E-12d);
        Assert.assertEquals("-2.0279547054523273904915591065295035975329478756021002630527658702166290201978158869028910117615627228815355920232832431793212890625E-11", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(1.1397807746364475E-9d);
        CBORObject b = CBORObject.FromObject(-2464944374772990973L);
        Assert.assertEquals("-2809496208.9145134348323005109105210690376973426481306550517302866865065880119800567626953125", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-0.0260567f);
        CBORObject b = CBORObject.FromObject(4632526172320899938L);
        Assert.assertEquals("-120708342188588425.867775067687034606933593750", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-3358423620330461413L);
        CBORObject b = CBORObject.FromObject(-55.48197968769103d);
        Assert.assertEquals("186331991085836432753.17888283919311476211078115738928318023681640625", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(9110863928365686743L);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-8380668660385914816256238003566314789196"));
        Assert.assertEquals("-76355131793494813285288405143907259897735748561661816828628", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(ExtendedDecimal.FromString("-2172.821088252086"));
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-8423191408427840522"));
        Assert.assertEquals("18302087922615801441887.468541828892", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-13.964438f);
        CBORObject b = CBORObject.FromObject(6.1772346f);
        Assert.assertEquals("-86.26161298479928518645465373992919921875", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-7183434807273822406L);
        CBORObject b = CBORObject.FromObject(BigInteger.fromString("-991545325414884273004"));
        Assert.assertEquals("7122701203574928729236980610073516127624", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
      {
        CBORObject a = CBORObject.FromObject(-5889139238933726200L);
        CBORObject b = CBORObject.FromObject(-4.806717f);
        Assert.assertEquals("28307425217807462943.1243896484375000", CBORObject.Multiply(a, b).AsExtendedDecimal().toString());
        TestCommon.AssertRoundTrip(a);
        TestCommon.AssertRoundTrip(b);
      }
    }
  }

