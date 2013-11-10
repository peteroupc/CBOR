/*
 * Created by SharpDevelop.
 * User: PeterRoot
 * Date: 11/7/2013
 * Time: 9:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using PeterO;
using System.Globalization;
using System.Numerics;

namespace Test
{
	[TestFixture]
	public class CBORObjectMathTest
	{
		[Test]
		public void CBORMultiplyTest(){
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-634889541024E+14"));
				CBORObject b=CBORObject.FromObject(-0.39601803371455613d);
				Assert.AreEqual("25142770766226149794078868.282554140023421496152877807617187500000",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(40.20107f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("7996746492.7274681343565519706"));
				Assert.AreEqual("321477756555.4319320007490459024696784973144531250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.022119554f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-9970998242E-8"));
				Assert.AreEqual("-2.2055403165173435583710670471191406250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.5268476f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-503285733164839762215",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-1271726347433338951145.9595930576324462890625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(9.12506362461426E-6d);
				CBORObject b=CBORObject.FromObject(5.641029925579569E-11d);
				Assert.AreEqual("5.1474756979266608403696403459529410796038238050457548796657151114380471943990657843652277518542957623903788544339477084577083587646484375E-16",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.9863508f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("41913315893308968",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("125167863390954038.6309108734130859375000",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("51373759105328576374643305691411448",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-4.5222281096959924E-14d);
				Assert.AreEqual("-2323238575268673257254.501371566891879118691399132531527312220315285794225955429137542296302854083478450775146484375000",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-482961178579026628E+6"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("826102917E+12"));
				Assert.AreEqual("-3.98975638421891812411473876E+44",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-9.033945173701306E-5d);
				CBORObject b=CBORObject.FromObject(-2.0573592855473745E-15d);
				Assert.AreEqual("1.8586070988240271295348402259229550852948804235400365473459642740392033758695647858134238214561141872013610119862558889280990115366876125335693359375E-19",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("2156523316826275"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("930720059.845757"));
				Assert.AreEqual("2007119510495321051619264.865175",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-1549"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-0149138346E0"));
				Assert.AreEqual("231015297954",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("9574254"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("41249908590533189242686",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("394937102322546749239543406244",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-0121472570951874677E0"));
				CBORObject b=CBORObject.FromObject(-6564123303496238409L);
				Assert.AreEqual("797360933720800814177596036281868893",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-84626937430208985E-5"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-47837170150939961006483074488002",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("40483132052018569867862771568011144050367350.97970",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(83.96423266687334d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-07749556847133167",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-650685594177850651.7836587888087223063848796300590038299560546875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1078924234326945061L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-70255986437016738376158956",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("75800886373442521468941315650954034815116316",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(9.796470955886588E-7d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-0495371",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-0.48528876138884952104734314530000427456712941420846618711948394775390625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-476244027101E+14"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-656",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("3.12416081778256E+28",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-6719311033823066015L);
				CBORObject b=CBORObject.FromObject(-3.7584813f);
				Assert.AreEqual("25254404628381017917.0420634746551513671875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("4499543371883779.26312785061842"));
				CBORObject b=CBORObject.FromObject(-0.05685488f);
				Assert.AreEqual("-255821004520648.74279288946467020799115300178527832031250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("30424603577890669724.32E-18"));
				CBORObject b=CBORObject.FromObject(8330281951735967713L);
				Assert.AreEqual("253445526073624194569.95685530936613088016",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-216.48886f);
				CBORObject b=CBORObject.FromObject(0.0027001811397703835d);
				Assert.AreEqual("-0.5845591396693451473286915745500498010900347622964545735158026218414306640625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("38590484691689397685891",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(1.1009459375566415E-10d);
				Assert.AreEqual("4248603734965.72066567535336329941927488838145139684907372092104171912296806112863123416900634765625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("676774623",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-47634494821244064152830613",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-32237817274442903908019752495933899",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-05507"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("1E+8"));
				Assert.AreEqual("-5.507E+11",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-832723796052.167"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-39021467614708813503.2375E+6"));
				Assert.AreEqual("32494104639647021516128503963573390662.5",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1.0216558189301154E-15d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-5724273202",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-0.000005848237025969023709984342844564627101483494383367053859323902997857658192515373229980468750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1.4418060160592996E-7d);
				CBORObject b=CBORObject.FromObject(-5225904869088262743L);
				Assert.AreEqual("-753474107960.504388515493593327065454515515351141541344759389176033437252044677734375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2611.4025486401783d);
				CBORObject b=CBORObject.FromObject(-432.74161434287936d);
				Assert.AreEqual("-1130062.5545976603110506039594126209975573529623277084510402090700154076330363750457763671875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3268148833648183726L);
				CBORObject b=CBORObject.FromObject(10.474402f);
				Assert.AreEqual("-34231906077162329708.75671577453613281250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-02",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-5702680986781981196L);
				Assert.AreEqual("11405361973563962392",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(9.818825f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-8990374781030894584337219503247649429",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-88274914574185741393190188566231820133.88379669189453125",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1731184543616306151L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-75117887"));
				Assert.AreEqual("130042924923516256808222937",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-9274297774670234",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-3.7425284200877295E-11d);
				Assert.AreEqual("347093.22998059736154967580553904219931632546813091502735382221089821541681885719299316406250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-80831478952"));
				CBORObject b=CBORObject.FromObject(72074398611184807L);
				Assert.AreEqual("-5825880234318042758802682264",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-2.0124694849364912E-14d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("7876905570E-3"));
				Assert.AreEqual("-1.5852032095351279016951961015040717951256076141008542410015813572954357368871569633483886718750E-7",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.9403898774636966E-6d);
				CBORObject b=CBORObject.FromObject(-73.227356f);
				Assert.AreEqual("-0.000215316976209485610782653428537671523590457667296504951082170009613037109375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1844.0043910488544d);
				CBORObject b=CBORObject.FromObject(3.577965315137519E-4d);
				Assert.AreEqual("-0.6597783752134083319611068631031597182993086653506953622393658986933218102421960793435573577880859375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.6778665f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("1892809.9237E-5"));
				Assert.AreEqual("50.686922077357120990753173828125",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(6556434482543438783L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("0627919640783365.805980265431E-7"));
				Assert.AreEqual("411691398509834894775784356.3566721309839610473",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2460196409348057722L);
				CBORObject b=CBORObject.FromObject(-523684582029466795L);
				Assert.AreEqual("-1288366928339832603911437504542340990",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1332757343747061131L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("909227776E-17"));
				Assert.AreEqual("-12117799956.02807898675174656",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-499148296236.866E+5"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-7389260064602357.0879"));
				Assert.AreEqual("368833657169738093247090913238252.14",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(8335870817615450098L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-563870621864145974050",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-4700352661708010793197043881265377956900",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.4152980465365456E-11d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("77621.58189310594065683"));
				Assert.AreEqual("-0.000001098576732223893356223492868536291894931482577314269116221766287022632013758993707597255706787109375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-463514687832433253675988214902",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-4484615119892422017L);
				Assert.AreEqual("2078684977345546220618244518078728370549072297334",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("29638173439801182344"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-89020",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-2638390199611101252262880",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-12.33862f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-4926900679850357315",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("60791156182089811119.72905635833740234375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1661256806824871167L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-577377.37"));
				Assert.AreEqual("-959172086019142164991290.79",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-7247541380132001722E-18"));
				CBORObject b=CBORObject.FromObject(1.3541237329610524E-12d);
				Assert.AreEqual("-9.814067788454043881238312650252126235811196688351305253070200329545968997990712523460388183593750E-12",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-56413.2304411423016489"));
				CBORObject b=CBORObject.FromObject(-2384706973929665461L);
				Assert.AreEqual("134529024054893344711965.0190087256786429",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(6733533195954145787L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("6320087056523446"));
				Assert.AreEqual("42556515996420749375245046867622002",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.901689f);
				CBORObject b=CBORObject.FromObject(-7.967420595813069E-5d);
				Assert.AreEqual("-0.00023118977120185441111747832822020913223445316708737351518720970489084720611572265625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(7303853603793886142L);
				CBORObject b=CBORObject.FromObject(-4.1619286721848676E-4d);
				Assert.AreEqual("-3039811773107054.8767210035855622685162583107576494967361213639378547668457031250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.020507919f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-5992214163",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-122887840.71266113780438899993896484375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(5661632373178241576L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("468050559108240"));
				Assert.AreEqual("2649930197731387664356653852186240",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("7585E-6"));
				CBORObject b=CBORObject.FromObject(1.048538682153392E-12d);
				Assert.AreEqual("7.953165904133478602964909855230447451274015946420803402361343614757061004638671875E-15",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.1449391699652754E-9d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-8580867092245",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("9824.5708461773363347106053394302652805576236798301703601055123726837337017059326171875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(14.719088f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-63"));
				Assert.AreEqual("-927.3025188446044921875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.031387743f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("3045954",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-95605.6201502010226249694824218750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1240022903018164741L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("024"));
				Assert.AreEqual("-29760549672435953784",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.362720890791882E-11d);
				CBORObject b=CBORObject.FromObject(-2652449361903108095L);
				Assert.AreEqual("-62669975.19136070811944330918553514489337903936255302732305505486465335707180202007293701171875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("583988109357508438741511",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-1.6970299566622061E-15d);
				Assert.AreEqual("-991045315.914216249444323438492234978558828936731849875803211105145169336483323974107406684197485446929931640625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-28.330496f);
				CBORObject b=CBORObject.FromObject(-0.03418379f);
				Assert.AreEqual("0.96844369313088662920563365332782268524169921875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-2.5572328352504374E-11d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("93642E-19"));
				Assert.AreEqual("-2.3946439715852145453060031900333044465505397124793773855344625189900398254394531250E-25",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.049292516f);
				CBORObject b=CBORObject.FromObject(-5651575385736113028L);
				Assert.AreEqual("278580369919280926.8629515618085861206054687500",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.1054924f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-70703193"));
				Assert.AreEqual("7458649.38592426478862762451171875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.096198395f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("483334892742.650574"));
				Assert.AreEqual("-46496040878.39659426896438002586364746093750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-7.8858E+18"));
				CBORObject b=CBORObject.FromObject(1.9232477390530362E-11d);
				Assert.AreEqual("-151663470.2062443270692102216363539231606602508684034091857029125094413757324218750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-677744989818709411E-18"));
				CBORObject b=CBORObject.FromObject(949957726360177009L);
				Assert.AreEqual("-643829089580182507.625997593894131699",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("0247111039693066700.39680963861467394E-17"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("062292",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("153930.4088456051090111806600858526907048",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1030399562791308238L);
				CBORObject b=CBORObject.FromObject(-63.323025f);
				Assert.AreEqual("-65248017016771628246.493293762207031250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.09242566f);
				CBORObject b=CBORObject.FromObject(0.4944296f);
				Assert.AreEqual("0.0456979806771613539240206591784954071044921875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("947048618983259414095031115",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-9E-4"));
				Assert.AreEqual("-852343757084933472685528.0035",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-7173498382128361348L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("6688134265092824295E-13"));
				Assert.AreEqual("-4797732033010063208986509.4961633349660",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-2028.920576010507d);
				CBORObject b=CBORObject.FromObject(1.0658400118503382E-7d);
				Assert.AreEqual("-0.0002162504730778433667813842565077532325672641399937932382221494895051770723792827766374102793633937835693359375",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.028014688f);
				CBORObject b=CBORObject.FromObject(0.010853733f);
				Assert.AreEqual("0.00030406393989982662129367696479675942100584506988525390625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-40065583103663138597508921275",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("473",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-18951020808032664556621719763075",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(171580011877233958L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("8166976239733008E+9"));
				Assert.AreEqual("1.401289880214477041376031471085664E+42",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(4756503004496483257L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-3484"));
				Assert.AreEqual("-16571656467665747667388",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(47.861744f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("5256943339128067199183052",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("251606475936106139213206572.928573608398437500",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-8.889726335187815E-7d);
				CBORObject b=CBORObject.FromObject(-119.396965f);
				Assert.AreEqual("0.000106140634434073564219476413377792971139175893895145730283502416568808257579803466796875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(613.0484225691173d);
				CBORObject b=CBORObject.FromObject(-478722774596480457L);
				Assert.AreEqual("-293480241814283440191.365496581981687995721586048603057861328125",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-982389974358784.06347691416415211E-4"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-4187980866582449.37089364048194250807E-10"));
				Assert.AreEqual("41142304161370106.994045393340730603937626242149336883959755783825277",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1505.0469576735418d);
				CBORObject b=CBORObject.FromObject(-5063480950546518686L);
				Assert.AreEqual("7620776599857971423692.43786716507656819885596632957458496093750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-612239.22621661"));
				CBORObject b=CBORObject.FromObject(1014448531968380355L);
				Assert.AreEqual("-621085184248897141528476.09869655",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.6220621f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("052331300.60E+1"));
				Assert.AreEqual("325533180.697489976882934570312500",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(115.93607f);
				CBORObject b=CBORObject.FromObject(6.284829f);
				Assert.AreEqual("728.6384118395872064866125583648681640625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-4264980344",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-353911288916818824L);
				Assert.AreEqual("1509424690749937335369195456",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-26252698366425756223523673798",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-4.044065397264691E-12d);
				Assert.AreEqual("106167629048489689.528899553185499035078122372783849572121359674777354986652255774970399215817451477050781250",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("060678661656686749713123",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-174887388",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-10611932644473698391537830792724",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-6.899643923480183d);
				CBORObject b=CBORObject.FromObject(2.9392164696369233E-12d);
				Assert.AreEqual("-2.0279547054523273904915591065295035975329478756021002630527658702166290201978158869028910117615627228815355920232832431793212890625E-11",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1.1397807746364475E-9d);
				CBORObject b=CBORObject.FromObject(-2464944374772990973L);
				Assert.AreEqual("-2809496208.9145134348323005109105210690376973426481306550517302866865065880119800567626953125",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.0260567f);
				CBORObject b=CBORObject.FromObject(4632526172320899938L);
				Assert.AreEqual("-120708342188588425.867775067687034606933593750",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3358423620330461413L);
				CBORObject b=CBORObject.FromObject(-55.48197968769103d);
				Assert.AreEqual("186331991085836432753.17888283919311476211078115738928318023681640625",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(9110863928365686743L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-8380668660385914816256238003566314789196",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-76355131793494813285288405143907259897735748561661816828628",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-2172.821088252086"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-8423191408427840522",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("18302087922615801441887.468541828892",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-13.964438f);
				CBORObject b=CBORObject.FromObject(6.1772346f);
				Assert.AreEqual("-86.26161298479928518645465373992919921875",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-7183434807273822406L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-991545325414884273004",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("7122701203574928729236980610073516127624",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-5889139238933726200L);
				CBORObject b=CBORObject.FromObject(-4.806717f);
				Assert.AreEqual("28307425217807462943.1243896484375000",CBORObject.Multiply(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(1.2205979f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(-0.14995685f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(DecimalFraction.FromString("4310268018194"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(BigInteger.Parse("-11455275312320262539482205058",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(0.12440690306802586d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(1.7928118469091356d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(4.643360738610487E-4d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(-829735834988622602L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(BigInteger.Parse("0694747557297",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(-2.7874117743461464E-8d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(-46.968563f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(444965269622467066L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(3.0601084310878607E-15d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(2.82955689133123E-11d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(0.027867362f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-3351188881870481154L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(DecimalFraction.FromString("-289"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(DecimalFraction.FromString("-734789"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(7000734143326731874L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-3716023503170226149L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(1.5166079335666195E-13d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-8.220424f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(8170883058720593388L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-2.6966395632014205E-15d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(BigInteger.Parse("-57698063111489",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(-6.147896963914895E-16d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("6741304989218497671449227825882685277187",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("16890648775547279842225596545",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(-10.620593f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Multiply(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("-79289615410806439.169851689646729"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
		}
		
		[Test]
		public void CBORAdditionTest()
		{
			Assert.AreEqual("1089595639448406721.00006187844301460638314800466464049577552941627800464630126953125",CBORObject.Addition(
				CBORObject.FromObject(6.187844301460638E-5d),
				CBORObject.FromObject(1089595639448406721L)).AsDecimalFraction().ToString());
			Assert.AreEqual("99.7502799107018773838717606849968433380126953125",CBORObject.Addition(
				CBORObject.FromObject(93.96945477260984d),
				CBORObject.FromObject(5.780825f)).AsDecimalFraction().ToString());
			Assert.AreEqual("4606920435721410785.39855587430286",CBORObject.Addition(
				CBORObject.FromObject(4606920435721410781L),
				CBORObject.FromObject(DecimalFraction.FromString("439855587430286E-14"))).AsDecimalFraction().ToString());
			Assert.AreEqual("2073300000000000.382769405841827392578125",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("20733E+11")),
				CBORObject.FromObject(0.3827694f)).AsDecimalFraction().ToString());
			Assert.AreEqual("29.9117383696138858795166015625",CBORObject.Addition(
				CBORObject.FromObject(29.866377f),
				CBORObject.FromObject(0.045361493f)).AsDecimalFraction().ToString());
			Assert.AreEqual("8178971242448397364682749104983471600952.147532757547350075",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("8178971242448397364682749104983471595392",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(DecimalFraction.FromString("5560.147532757547350075E0"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-453.000000644357521854",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-453")),
				CBORObject.FromObject(DecimalFraction.FromString("-644357521854E-18"))).AsDecimalFraction().ToString());
			Assert.AreEqual("7389999999999999994.919809818267822265625",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("739E+16")),
				CBORObject.FromObject(-5.08019f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-8.8565318361805548371811302093150125980377197265625",CBORObject.Addition(
				CBORObject.FromObject(-8.85653158994297d),
				CBORObject.FromObject(DecimalFraction.FromString("-2462375.853192887508374E-13"))).AsDecimalFraction().ToString());
			Assert.AreEqual("3520774562396536772.0480528660118579864501953125",CBORObject.Addition(
				CBORObject.FromObject(3520774562396536772L),
				CBORObject.FromObject(0.048052866f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.1993306465446949005126953125",CBORObject.Addition(
				CBORObject.FromObject(-0.008629937f),
				CBORObject.FromObject(-0.19070071f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-1815028339341501192.10944442426839977298",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("046391051542425.89055557573160022702")),
				CBORObject.FromObject(-1815074730393043618L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-519960066703964460108515896670316288.9999999999997440069579494794312995321310723426883938259879247567596394219435751438140869140625",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-519960066703964460108515896670316289",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(2.5599304205052057E-13d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-5774272840211.96140985556334178892",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-28658.81672066060911293099")),
				CBORObject.FromObject(DecimalFraction.FromString("-5774272811553.14468919495422885793"))).AsDecimalFraction().ToString());
			Assert.AreEqual("2677708415311134816.99980529900131910394658028840009222903972840867936611175537109375",CBORObject.Addition(
				CBORObject.FromObject(2677708415311134817L),
				CBORObject.FromObject(-1.9470099868089605E-4d)).AsDecimalFraction().ToString());
			Assert.AreEqual("196.0000000000004361158497704044100351267718988865573575240686121645694584003649652004241943359375",CBORObject.Addition(
				CBORObject.FromObject(4.361158497704044E-13d),
				CBORObject.FromObject(BigInteger.Parse("196",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-664618218304341090",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("52075881",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(-664618218356416971L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.109522778439673969426426669571128513780422508716583251953125",CBORObject.Addition(
				CBORObject.FromObject(0.006665725362148693d),
				CBORObject.FromObject(-0.116188504f)).AsDecimalFraction().ToString());
			Assert.AreEqual("319.022416012128452045037054409936001786451953421419602818787097930908203125",CBORObject.Addition(
				CBORObject.FromObject(319.0224151483916d),
				CBORObject.FromObject(8.637368645090117E-7d)).AsDecimalFraction().ToString());
			Assert.AreEqual("505307335938983026287.0000000000000254036367714822863522527075608310506813110329904503714715247042477130889892578125",CBORObject.Addition(
				CBORObject.FromObject(2.5403636771482286E-14d),
				CBORObject.FromObject(BigInteger.Parse("505307335938983026287",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-617387129107737043.999999999999999053766145904815677282101789974982820780825191857299483189081001910381019115447998046875",CBORObject.Addition(
				CBORObject.FromObject(-617387129107737044L),
				CBORObject.FromObject(9.462338540951843E-16d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-7175811072841485965.99999999999998708352369376714029624399130184160311811945344440122340756715857423841953277587890625",CBORObject.Addition(
				CBORObject.FromObject(-7175811072841485966L),
				CBORObject.FromObject(1.291647630623286E-14d)).AsDecimalFraction().ToString());
			Assert.AreEqual("6904209373046255396.337371826171875",CBORObject.Addition(
				CBORObject.FromObject(-231.66263f),
				CBORObject.FromObject(6904209373046255628L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-28953893666576455.757697875911372125",CBORObject.Addition(
				CBORObject.FromObject(-48.002857f),
				CBORObject.FromObject(DecimalFraction.FromString("-28953893666576407.7548406676594190"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-520884419424.61322356999573171517127968610583728323284023633936434816149585458333604037761688232421875",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-520884419424.61322357")),
				CBORObject.FromObject(4.268284828720314E-12d)).AsDecimalFraction().ToString());
			Assert.AreEqual("274.7731781005859375",CBORObject.Addition(
				CBORObject.FromObject(200.77318f),
				CBORObject.FromObject(BigInteger.Parse("74",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("7427116322946124575.937",CBORObject.Addition(
				CBORObject.FromObject(7752701834987853219L),
				CBORObject.FromObject(DecimalFraction.FromString("-325585512041728643.063"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-45482331089169.8144082200000242",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-242E-16")),
				CBORObject.FromObject(DecimalFraction.FromString("-45482331089169.81440822"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-7481917355839215314.7463704101736112761500407941639423370361328125",CBORObject.Addition(
				CBORObject.FromObject(-7481917355839215389L),
				CBORObject.FromObject(74.25362958982639d)).AsDecimalFraction().ToString());
			Assert.AreEqual("74789814406496421741896877.91579650395361955617090643499977886676788330078125",CBORObject.Addition(
				CBORObject.FromObject(-1.0842034960463804d),
				CBORObject.FromObject(BigInteger.Parse("74789814406496421741896879",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-6477456953547722229.02291305650466",CBORObject.Addition(
				CBORObject.FromObject(-6477456910580984122L),
				CBORObject.FromObject(DecimalFraction.FromString("-42966738107.02291305650466E0"))).AsDecimalFraction().ToString());
			Assert.AreEqual("8929332621123891869.000000000000007606115152000987410208939999319702473333363636209814018229735665954649448394775390625",CBORObject.Addition(
				CBORObject.FromObject(7.606115152000987E-15d),
				CBORObject.FromObject(8929332621123891869L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-584395324193248590440770658864787296959.24425329267978668212890625",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-584395324193248590440770658864787296959",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(-0.2442533f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.81541771843453924280802389694771639856718525862788737867958843708038330078125",CBORObject.Addition(
				CBORObject.FromObject(-0.81541777f),
				CBORObject.FromObject(4.8136505679066976E-8d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-131449.135633005",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-131449.135633")),
				CBORObject.FromObject(DecimalFraction.FromString("-5E-9"))).AsDecimalFraction().ToString());
			Assert.AreEqual("30.330950698807495058417771360836923122406005859375",CBORObject.Addition(
				CBORObject.FromObject(-0.1734072f),
				CBORObject.FromObject(30.50435789580609d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-2185571397002111610",CBORObject.Addition(
				CBORObject.FromObject(-6925578532851223727L),
				CBORObject.FromObject(4740007135849112117L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-2355210269134529296695685232067204272817.0000000000113264001277221350281676941416423806154034625848225914523936808109283447265625",CBORObject.Addition(
				CBORObject.FromObject(-1.1326400127722135E-11d),
				CBORObject.FromObject(BigInteger.Parse("-2355210269134529296695685232067204272817",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("3992884460.013753409010889826098884107587323466503903546254150569438934326171875",CBORObject.Addition(
				CBORObject.FromObject(2.350375920596099E-6d),
				CBORObject.FromObject(DecimalFraction.FromString("3992884460.01375105863496923"))).AsDecimalFraction().ToString());
			Assert.AreEqual("3396324703085949596.829345703125",CBORObject.Addition(
				CBORObject.FromObject(-392.17065f),
				CBORObject.FromObject(3396324703085949989L)).AsDecimalFraction().ToString());
			Assert.AreEqual("27.405853268514803471569181365751269240592889742114124373983941040933132171630859375",CBORObject.Addition(
				CBORObject.FromObject(-2.9695715284308186E-9d),
				CBORObject.FromObject(27.405853f)).AsDecimalFraction().ToString());
			Assert.AreEqual("66481777451203681265.7292242620849609375",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("664817774512036812.6180684E+2")),
				CBORObject.FromObject(3.9223843f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-1345359418235447854.4046027660369873046875",CBORObject.Addition(
				CBORObject.FromObject(-2.4046028f),
				CBORObject.FromObject(-1345359418235447852L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-451166010862059305103980796929",CBORObject.Addition(
				CBORObject.FromObject(5934072623496291035L),
				CBORObject.FromObject(BigInteger.Parse("-451166010867993377727477087964",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-9132396782676477677",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("18490535853713671")),
				CBORObject.FromObject(-9150887318530191348L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-12985503111787812921",CBORObject.Addition(
				CBORObject.FromObject(-4169866500538325196L),
				CBORObject.FromObject(-8815636611249487725L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-671908935408255004.0600409097969532012939453125",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-671908935408255004")),
				CBORObject.FromObject(-0.06004091f)).AsDecimalFraction().ToString());
			Assert.AreEqual("48415.340236229740624451789443583038519136607646942138671875",CBORObject.Addition(
				CBORObject.FromObject(0.34023622974062445d),
				CBORObject.FromObject(BigInteger.Parse("48415",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("6062532686759789525",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-2481833818")),
				CBORObject.FromObject(6062532689241623343L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-2299754205679346361.0134203245183425294573265063036160427145659923553466796875",CBORObject.Addition(
				CBORObject.FromObject(-0.01342032451834253d),
				CBORObject.FromObject(-2299754205679346361L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-5967105489410287096",CBORObject.Addition(
				CBORObject.FromObject(-5967105489484864936L),
				CBORObject.FromObject(BigInteger.Parse("074577840",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-721984209962316794",CBORObject.Addition(
				CBORObject.FromObject(-721984209575477115L),
				CBORObject.FromObject(BigInteger.Parse("-386839679",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("5003286091572763681.99999999999965449148032827406788",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-3455.0851967172593212E-16")),
				CBORObject.FromObject(5003286091572763682L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-949957184966461115.54095458984375",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-949957184966461302",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(186.45905f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-2581392322354672752.71290332356456618",CBORObject.Addition(
				CBORObject.FromObject(-2581392322354672743L),
				CBORObject.FromObject(DecimalFraction.FromString("-971290332356456618E-17"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-54.3132591498356278401",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-54.31403321")),
				CBORObject.FromObject(DecimalFraction.FromString("7740601643721599E-19"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-6484545743689361726.768890380859375",CBORObject.Addition(
				CBORObject.FromObject(-6484545743689361932L),
				CBORObject.FromObject(205.23111f)).AsDecimalFraction().ToString());
			Assert.AreEqual("42.83542127907276153564453125",CBORObject.Addition(
				CBORObject.FromObject(0.109106734f),
				CBORObject.FromObject(42.726315f)).AsDecimalFraction().ToString());
			Assert.AreEqual("990.7975674143545783466617576777935028076171875",CBORObject.Addition(
				CBORObject.FromObject(989.7069187947126d),
				CBORObject.FromObject(DecimalFraction.FromString("1.090648619642"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-52893642944917412137.3037872314453125",CBORObject.Addition(
				CBORObject.FromObject(166.69621f),
				CBORObject.FromObject(DecimalFraction.FromString("-52893642944917412304"))).AsDecimalFraction().ToString());
			Assert.AreEqual("3547362333613881151",CBORObject.Addition(
				CBORObject.FromObject(3547362333656868955L),
				CBORObject.FromObject(BigInteger.Parse("-42987804",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("1.719438790524952785806451814669227888174828089429269084575935266911983489990234375",CBORObject.Addition(
				CBORObject.FromObject(-7.500716282560482E-10d),
				CBORObject.FromObject(1.7194388f)).AsDecimalFraction().ToString());
			Assert.AreEqual("163.68128857474880375197376596709274418991265608959833070985834257982105555129237473011016845703125",CBORObject.Addition(
				CBORObject.FromObject(163.6812885747488d),
				CBORObject.FromObject(-1.5792089001558206E-15d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-3876462703.9999999997993961696451869913648328300998326979842634187889416352845728397369384765625",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-3876462704")),
				CBORObject.FromObject(2.00603830354813E-10d)).AsDecimalFraction().ToString());
			Assert.AreEqual("605268840750377993642875589292.0000000000673741908417059793446812410023355997823468754859277396462857723236083984375",CBORObject.Addition(
				CBORObject.FromObject(6.737419084170598E-11d),
				CBORObject.FromObject(BigInteger.Parse("605268840750377993642875589292",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-6908482993895962072",CBORObject.Addition(
				CBORObject.FromObject(-6800296482755871402L),
				CBORObject.FromObject(-108186511140090670L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-33.3064439065180542339265230111777782440185546875",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-038318E-10")),
				CBORObject.FromObject(-33.306440074718054d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-343519648247563226036829471420805538.3926694671",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-47027392.3926694671")),
				CBORObject.FromObject(BigInteger.Parse("-343519648247563226036829471373778146",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("0.01014821045101625957843883570070679826411602689656792628848658299745011390768922865390777587890625",CBORObject.Addition(
				CBORObject.FromObject(9.37023517711695E-15d),
				CBORObject.FromObject(0.01014821f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.0012349257939086495452376806696292987908236682415008544921875",CBORObject.Addition(
				CBORObject.FromObject(-0.0012211708448616495d),
				CBORObject.FromObject(DecimalFraction.FromString("-13754949047E-15"))).AsDecimalFraction().ToString());
			Assert.AreEqual("16495732629526387",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-4384692")),
				CBORObject.FromObject(BigInteger.Parse("16495732633911079",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("83610650978021660746969",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-13184424043991881")),
				CBORObject.FromObject(DecimalFraction.FromString("83610664162445704.73885E+6"))).AsDecimalFraction().ToString());
			Assert.AreEqual("2299978209967190436",CBORObject.Addition(
				CBORObject.FromObject(2299978209960559074L),
				CBORObject.FromObject(DecimalFraction.FromString("6631362"))).AsDecimalFraction().ToString());
			Assert.AreEqual("5156900312324973219",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-044210278",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(5156900312369183497L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.020452566710115233257483158268996703554876148700714111328125",CBORObject.Addition(
				CBORObject.FromObject(-0.023175428f),
				CBORObject.FromObject(0.0027228609800331203d)).AsDecimalFraction().ToString());
			Assert.AreEqual("39442872161250971595130963678825012548.00000000000019723399005158128943079495891271894127279827590992766772615141235291957855224609375",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("39442872161250971595130963678825012548",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(1.972339900515813E-13d)).AsDecimalFraction().ToString());
			Assert.AreEqual("0.9657888412475585937500",CBORObject.Addition(
				CBORObject.FromObject(0.6693976f),
				CBORObject.FromObject(0.29639125f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-72380371946635640615180",CBORObject.Addition(
				CBORObject.FromObject(-7062715607420615180L),
				CBORObject.FromObject(DecimalFraction.FromString("-7237330923102822E+7"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-94.99999998985461031906424068743137662830522405243982575484551489353179931640625",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-95")),
				CBORObject.FromObject(1.014538968093576E-8d)).AsDecimalFraction().ToString());
			Assert.AreEqual("2942054755239162649205",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-2157565")),
				CBORObject.FromObject(BigInteger.Parse("2942054755239164806770",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("-9127049424127225624.3934326171875",CBORObject.Addition(
				CBORObject.FromObject(-9127049424127225872L),
				CBORObject.FromObject(247.60657f)).AsDecimalFraction().ToString());
			Assert.AreEqual("4.3664895975791235827045966289006173610687255859375",CBORObject.Addition(
				CBORObject.FromObject(0.029691467f),
				CBORObject.FromObject(4.336798130517485d)).AsDecimalFraction().ToString());
			Assert.AreEqual("245471752608903",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("8",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(DecimalFraction.FromString("245471752608895"))).AsDecimalFraction().ToString());
			Assert.AreEqual("9588937679668095241365312434335",CBORObject.Addition(
				CBORObject.FromObject(1431159519751909270L),
				CBORObject.FromObject(BigInteger.Parse("9588937679666664081845560525065",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("35.47073268890380859375",CBORObject.Addition(
				CBORObject.FromObject(6.177203f),
				CBORObject.FromObject(29.29353f)).AsDecimalFraction().ToString());
			Assert.AreEqual("21676518924170358002724.9934866162",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("-6.8065133838")),
				CBORObject.FromObject(DecimalFraction.FromString("216765.18924170358002731800E+17"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-0.0000145210778548097872581329424441880238116908685697126202285289764404296875",CBORObject.Addition(
				CBORObject.FromObject(-1.146507717614387E-7d),
				CBORObject.FromObject(-1.4406427083048349E-5d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-6067619165207136619",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-7879135",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(-6067619165199257484L)).AsDecimalFraction().ToString());
			Assert.AreEqual("305333204.9",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("72504")),
				CBORObject.FromObject(DecimalFraction.FromString("3052607009E-1"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-43867089609984380.849133567119906729203648865222930908203125",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-043867089609985679",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(1298.15086643288d)).AsDecimalFraction().ToString());
			Assert.AreEqual("-3834646720497382654.99",CBORObject.Addition(
				CBORObject.FromObject(DecimalFraction.FromString("98218707201E-2")),
				CBORObject.FromObject(-3834646721479569727L)).AsDecimalFraction().ToString());
			Assert.AreEqual("31.97142636775970458984375",CBORObject.Addition(
				CBORObject.FromObject(32.57107f),
				CBORObject.FromObject(-0.59964526f)).AsDecimalFraction().ToString());
			Assert.AreEqual("-699969717822311411.8548126220703125",CBORObject.Addition(
				CBORObject.FromObject(-157.85481f),
				CBORObject.FromObject(-699969717822311254L)).AsDecimalFraction().ToString());
			Assert.AreEqual("30236256464397847083096477697",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("30236256464424345990870548261",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(BigInteger.Parse("-26498907774070564",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction().ToString());
			Assert.AreEqual("8354770905378496469447814154339536259983",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("8354770905378496469443438347004241101530",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(4375807335295158453L)).AsDecimalFraction().ToString());
			Assert.AreEqual("-5881222127378372.881434210850",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("0424329459910556",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(DecimalFraction.FromString("-6305551587288928.881434210850"))).AsDecimalFraction().ToString());
			Assert.AreEqual("2887639108262666868.09448297321796417236328125",CBORObject.Addition(
				CBORObject.FromObject(2887639108262666868L),
				CBORObject.FromObject(0.09448297f)).AsDecimalFraction().ToString());
			Assert.AreEqual("435112100000000000000.02269179932773113250732421875",CBORObject.Addition(
				CBORObject.FromObject(0.0226918f),
				CBORObject.FromObject(DecimalFraction.FromString("4351121E+14"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-250265219105.815186338185234375",CBORObject.Addition(
				CBORObject.FromObject(-2.086544f),
				CBORObject.FromObject(DecimalFraction.FromString("-0250265219103.72864230132"))).AsDecimalFraction().ToString());
			Assert.AreEqual("-33085332812210998933439569.317413330078125",CBORObject.Addition(
				CBORObject.FromObject(BigInteger.Parse("-33085332812210998933439383",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)),
				CBORObject.FromObject(-186.31741f)).AsDecimalFraction().ToString());
			try { CBORObject.Addition(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(3.5023837f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(3.812574964908644E-6d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(23.474629887387266d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(-166.82784f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(5.993239296979466E-10d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(BigInteger.Parse("-043703874024659752541",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(0.23853742f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(-164.26022f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(BigInteger.Parse("-3454197004716216037476684",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(3.6649623f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(4.920178f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("-2775394107351658213.29237024329"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("5354394.3316793E-16"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("35",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("-724734631380034337",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-1.6585551286059796E-6d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(DecimalFraction.FromString("4470484868626699E-12"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(51.165462f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-8526307050682317212L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-5178277071860853800L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-4.501634965667704E-11d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(DecimalFraction.FromString("-087829913964415877"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(DecimalFraction.FromString("914.9E-19"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-0.10745481f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(3624504802515453345L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(8200506367764127039L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("-3594433687266085256412604739746153",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("-75230249292446971480280879652518",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(1.9599492372504113E-13d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Addition(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(322.3261f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
		}
		
		[Test]
		public void CBORSubtractTest(){
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("346356007189210337696095210412",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(6066514617521137153L);
				Assert.AreEqual("346356007183143823078574073259",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("84812561899554690973554871212596",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(62.707386f);
				Assert.AreEqual("84812561899554690973554871212533.292613983154296875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(12.6878729827517d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("904494447134823735435963838",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-904494447134823735435963825.3121270172483008309427532367408275604248046875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-8851947516818411559L);
				CBORObject b=CBORObject.FromObject(0.24109761f);
				Assert.AreEqual("-8851947516818411559.24109761416912078857421875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("6112"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-02555515724580714396344640862713",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("2555515724580714396344640868825",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-089108.5980319587332970E-5"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-7418265198052351.70196"));
				Assert.AreEqual("7418265198052350.810874019680412667030",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("177"));
				CBORObject b=CBORObject.FromObject(5363123330033215072L);
				Assert.AreEqual("-5363123330033214895",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("116003900156"));
				CBORObject b=CBORObject.FromObject(-1766220146835292942L);
				Assert.AreEqual("1766220262839193098",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.050294317f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-5141608421350562853"));
				Assert.AreEqual("5141608421350562853.050294317305088043212890625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(70.48056f);
				CBORObject b=CBORObject.FromObject(27.830172f);
				Assert.AreEqual("42.6503887176513671875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("6421583965841160548432660774",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-35720713.25140849364684112333E0"));
				Assert.AreEqual("6421583965841160548468381487.25140849364684112333",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("1268897246813768.356705944452013"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("9063685280691",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("1259833561533077.356705944452013",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("21777470211",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(8602542517353909603L);
				Assert.AreEqual("-8602542495576439392",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-24678161897287",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-1.8961014f);
				Assert.AreEqual("-24678161897285.10389864444732666015625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.8539986f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-87203",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("87202.146001398563385009765625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(24.693441627116798d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("767526676781.1331037E-5"));
				Assert.AreEqual("-7675242.0743697039202024235089193098247051239013671875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-35.30134022539256d);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-5812259802630750159",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("5812259802630750123.69865977460744232985234702937304973602294921875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("0065014463571599289421135114033921933",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("4187366"));
				Assert.AreEqual("65014463571599289421135114029734567",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.12892367f);
				CBORObject b=CBORObject.FromObject(-7246298462423693395L);
				Assert.AreEqual("7246298462423693395.12892366945743560791015625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-94"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("704606082.67827750491205565"));
				Assert.AreEqual("-704606176.67827750491205565",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-1725369539938057436926650371711554",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("593971731",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-1725369539938057436926650965683285",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(64.667465f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("98448994861345341"));
				Assert.AreEqual("-98448994861345276.3325347900390625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-830554765810011917",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("84124116623685514529253120240585557",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-84124116623685515359807886050597474",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1578971607839072424L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("40027536"));
				Assert.AreEqual("-1578971607879099960",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-7466E+8"));
				CBORObject b=CBORObject.FromObject(-4675086604394870559L);
				Assert.AreEqual("4675085857794870559",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(3.5925012421663193E-14d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("4361537998981300516.19060E0"));
				Assert.AreEqual("-4361537998981300516.1905999999999640749875783368067539729155978001598305336498906381592632897081784904003143310546875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(7887546843621803069L);
				CBORObject b=CBORObject.FromObject(2757361815058305617L);
				Assert.AreEqual("5130185028563497452",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-7426267158300916121L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-08149",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-7426267158300907972",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-24.148534633278974d);
				CBORObject b=CBORObject.FromObject(-19.581343f);
				Assert.AreEqual("-4.567191936135419183528938447125256061553955078125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(234070582574353153L);
				CBORObject b=CBORObject.FromObject(3703525906849000058L);
				Assert.AreEqual("-3469455324274646905",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1.4741322521622959E-9d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("808148124317512"));
				Assert.AreEqual("-808148124317511.9999999985258677478377041259586734364456402091025921663458575494587421417236328125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-8891063857392216127L);
				CBORObject b=CBORObject.FromObject(11.363351f);
				Assert.AreEqual("-8891063857392216138.36335086822509765625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("945280335.0492217949426E+7"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("46419759641040973822153",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-46419750188237623329935.050574",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-9E+5"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-948464401260730066392399291208062745218",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("948464401260730066392399291208061845218",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1687400540524960108L);
				CBORObject b=CBORObject.FromObject(9006105132058753898L);
				Assert.AreEqual("-7318704591533793790",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("7203105488637",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-707174126682050733618747359543",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("707174126682050740821852848180",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("4182638311653725098276780006908721906",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(6516955250379052302L);
				Assert.AreEqual("4182638311653725091759824756529669604",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-4837810682711540947L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("1352426033190046182380854104920611367",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-1352426033190046187218664787632152314",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-9176035024138904707703421252791815",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-340360067445785977924850115",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-9176034683778837261917443327941700",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3559740940911358818L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-224495.6214592715253"));
				Assert.AreEqual("-3559740940911134322.3785407284747",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3072016931551999845L);
				CBORObject b=CBORObject.FromObject(-5978771699599996983L);
				Assert.AreEqual("2906754768047997138",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.1230959029827813E-15d);
				CBORObject b=CBORObject.FromObject(-0.009448553f);
				Assert.AreEqual("0.00944855343550331339701693909370818438426369323279531745561621303153287954046390950679779052734375",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("21475",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(0.17010021f);
				Assert.AreEqual("21474.82989978790283203125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-46479853",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-1.5589319373737706E-6d);
				Assert.AreEqual("-46479852.999998441068062626229418044334663895345016726423637010157108306884765625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-6154419980858052181144074744956",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-4.405259099122617E-10d);
				Assert.AreEqual("-6154419980858052181144074744955.9999999995594740900877383167594178380271734141171435794603894464671611785888671875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.028461827f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-14",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("13.97153817303478717803955078125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.032547350158160446d);
				CBORObject b=CBORObject.FromObject(-1793587243808968416L);
				Assert.AreEqual("1793587243808968416.03254735015816044574332721595055772922933101654052734375",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(4400387861681855036L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("5716539614078652",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("4394671322067776384",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(4.1450223168680926E-7d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("204368293288E-19"));
				Assert.AreEqual("3.9406540235800925940623679427921022266900763497687876224517822265625E-7",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.33502686f);
				CBORObject b=CBORObject.FromObject(-7960929196703574626L);
				Assert.AreEqual("7960929196703574626.33502686023712158203125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.04748803f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("5299001030608385631246631135642897968",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-5299001030608385631246631135642897967.9525119699537754058837890625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2.710699056337099E-6d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("165159382437537987E+8"));
				Assert.AreEqual("-16515938243753798699999999.9999972893009436629010997020332307538836857929709367454051971435546875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.2700341f);
				CBORObject b=CBORObject.FromObject(-5.2997461066280877E-11d);
				Assert.AreEqual("-0.270034104532650121941531623376742861603893339043913623953585556591860949993133544921875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1.828788542235912E-8d);
				CBORObject b=CBORObject.FromObject(30.886937321923906d);
				Assert.AreEqual("-30.88693730363602041909957068741306011362690586707913098507560789585113525390625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(1286881464988230957L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("4489021825990710533141652732533066275",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-4489021825990710531854771267544835318",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("513183489570708",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-689079E-5"));
				Assert.AreEqual("513183489570714.89079",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(5196430339365233860L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-707904038800149736465454850832436848858",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("707904038800149736470651281171802082718",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3.584564327094063E-5d);
				CBORObject b=CBORObject.FromObject(-0.84281087f);
				Assert.AreEqual("0.842775023573648004681616204003535841593475197441875934600830078125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("05128269048443668887E+5"));
				CBORObject b=CBORObject.FromObject(-42.29741748521973d);
				Assert.AreEqual("512826904844366888700042.29741748521973221386360819451510906219482421875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1174823795085951245L);
				CBORObject b=CBORObject.FromObject(-4930621708856897950L);
				Assert.AreEqual("3755797913770946705",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-975349.43"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("59810631114361567E+14"));
				Assert.AreEqual("-5981063111436156700000000975349.43",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-6289691266985113942",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-3415038455543573772L);
				Assert.AreEqual("-2874652811441540170",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-35535509E-11"));
				CBORObject b=CBORObject.FromObject(3815404146896658757L);
				Assert.AreEqual("-3815404146896658757.00035535509",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("53875238184785825769472569363328138",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(0.031660635f);
				Assert.AreEqual("53875238184785825769472569363328137.9683393649756908416748046875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-56626028120864918969145275364872623",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("1278",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-56626028120864918969145275364873901",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.6535442048385653E-6d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-08075273E+14"));
				Assert.AreEqual("807527299999999999999.9999983464557951614346570259721225237825592557783238589763641357421875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(13.345074f);
				CBORObject b=CBORObject.FromObject(-209.63f);
				Assert.AreEqual("222.975078582763671875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-307590.865139925958077E+6"));
				CBORObject b=CBORObject.FromObject(-1007.5293177913348d);
				Assert.AreEqual("-307590864132.3966402856651527570211328566074371337890625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.1535023f);
				CBORObject b=CBORObject.FromObject(-3231.97171820982d);
				Assert.AreEqual("3230.8182158647350661340169608592987060546875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(4.486967107594844E-11d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("115.577678"));
				Assert.AreEqual("-115.577677999955130328924051559064475374862118197370752792352277538157068192958831787109375",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("246768527102897",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("60351378",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("246768466751519",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-2.7152982684510718E-11d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-230820109609889.510708384372E+11"));
				Assert.AreEqual("23082010960988951070838437.19999999997284701731548928248441347053484455457972313041636880370788276195526123046875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-98.766396f);
				CBORObject b=CBORObject.FromObject(-3.246561057194117E-5d);
				Assert.AreEqual("-98.766363103237084308826977674389269168386817909777164459228515625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.0082877595f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("8738070260863467"));
				Assert.AreEqual("-8738070260863466.99171224050223827362060546875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("2116899332579229981380455566654774",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-8131490868909175794L);
				Assert.AreEqual("2116899332579238112871324475830568",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-6031553598326517235L);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("7722527005258594.58245424700908187"));
				Assert.AreEqual("-6039276125331775829.58245424700908187",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-3884601511006138881L);
				CBORObject b=CBORObject.FromObject(-0.48894623f);
				Assert.AreEqual("-3884601511006138880.5110537707805633544921875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(5.542882081879096E-13d);
				CBORObject b=CBORObject.FromObject(-9.957308456966182E-5d);
				Assert.AreEqual("0.000099573085123950032157938077618879975216077202605606533136750613266485743224620819091796875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2225286216387634322L);
				CBORObject b=CBORObject.FromObject(-0.38981488f);
				Assert.AreEqual("2225286216387634322.3898148834705352783203125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.1314829f);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("3767251807982482291226",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-3767251807982482291226.131482899188995361328125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(2094526990703050134L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-268737658",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("2094526990971787792",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-45689836528464.05"));
				CBORObject b=CBORObject.FromObject(3.438367817744033E-12d);
				Assert.AreEqual("-45689836528464.0500000000034383678177440330346790778569708707241951717303862778862821869552135467529296875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-4672104073755041115L);
				CBORObject b=CBORObject.FromObject(-326907265532196721L);
				Assert.AreEqual("-4345196808222844394",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-66",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(-8.643573666898676E-16d);
				Assert.AreEqual("-65.99999999999999913564263331013235388276148038570974828026321178098267949962973943911492824554443359375",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-1.003540046404883E-12d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("08454440435"));
				Assert.AreEqual("-8454440435.00000000000100354004640488303624913661807860389423192881697133316265535540878772735595703125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-136.64556347868253d);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("578647485877"));
				Assert.AreEqual("-578647486013.645563478682532831953722052276134490966796875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(BigInteger.Parse("-593498483552893385656602605890152677",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				CBORObject b=CBORObject.FromObject(347651556843215946L);
				Assert.AreEqual("-593498483552893386004254162733368623",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("5595313E+8"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-82941447507188861501.8839617286773540"));
				Assert.AreEqual("82942007038488861501.8839617286773540",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("3958980491230284488.4701121E+5"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("945745218574825"));
				Assert.AreEqual("395898048177283230272186.21",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("6421366491164E+5"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("-1",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("642136649116400001",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(0.4124196447588869d);
				CBORObject b=CBORObject.FromObject(0.027489457f);
				Assert.AreEqual("0.38493018747944318658227302876184694468975067138671875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-0874365842.754430332"));
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("0522611140756858421"));
				Assert.AreEqual("-522611141631224263.754430332",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-059978"));
				CBORObject b=CBORObject.FromObject(116.37721f);
				Assert.AreEqual("-60094.3772125244140625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-6022273210237766169L);
				CBORObject b=CBORObject.FromObject(-0.20898747f);
				Assert.AreEqual("-6022273210237766168.7910125255584716796875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("28038804917443.80696"));
				CBORObject b=CBORObject.FromObject(0.091602504f);
				Assert.AreEqual("28038804917443.715357495746612548828125",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(-0.020705052f);
				CBORObject b=CBORObject.FromObject(-8164305834977742521L);
				Assert.AreEqual("8164305834977742520.979294948279857635498046875",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(26.160181f);
				CBORObject b=CBORObject.FromObject(DecimalFraction.FromString("-92859642645009.65399224E+8"));
				Assert.AreEqual("9285964264500965399250.1601810455322265625",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-58725562493615.630046131436867E+16"));
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("494165",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-587255624936156300461314862835",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(DecimalFraction.FromString("-5886408069291538.455140188581087E+13"));
				CBORObject b=CBORObject.FromObject(0.061350547f);
				Assert.AreEqual("-58864080692915384551401885810.931350546777248382568359375",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			{
				CBORObject a=CBORObject.FromObject(6206910003785181554L);
				CBORObject b=CBORObject.FromObject(BigInteger.Parse("1800035314513106996497",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture));
				Assert.AreEqual("-1793828404509321814943",CBORObject.Subtract(a,b).AsDecimalFraction().ToString());
				TestCommon.AssertRoundTrip(a);
				TestCommon.AssertRoundTrip(b);
			}
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(99.74439f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(0.04503661680757691d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(DecimalFraction.FromString("961.056025725133"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(-2.66673f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NaN),CBORObject.FromObject(-3249200021658530613L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(-3082676751896642153L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(0.37447542485458996d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(DecimalFraction.FromString("6695270"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(8.645616f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.PositiveInfinity),CBORObject.FromObject(10.918599534632621d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(1.1195766122143437E-7d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(-27.678854f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("51444344646435.890"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("-795755897.41124405443"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Double.NegativeInfinity),CBORObject.FromObject(DecimalFraction.FromString("282349190160173.8945458982215192141"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-4742894673080640195L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-8.057984695058738E-10d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(-6832707275063219586L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(BigInteger.Parse("3037587108614072",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NaN),CBORObject.FromObject(DecimalFraction.FromString("-21687"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(21.02954f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-280.74258f)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(3.295564645540288E-15d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(-1.8643148756498468E-14d)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.PositiveInfinity),CBORObject.FromObject(DecimalFraction.FromString("56E-9"))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("06842884252556766213171069781",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(-6381263349646471084L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(9127378784365184230L)).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("300921783316",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
			try { CBORObject.Subtract(CBORObject.FromObject(Single.NegativeInfinity),CBORObject.FromObject(BigInteger.Parse("-5806763724610384900094490266237212718",NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture))).AsDecimalFraction(); } catch(OverflowException){ } catch(Exception ex){ Assert.Fail(ex.ToString()); }
		}
	}
	
	
}
