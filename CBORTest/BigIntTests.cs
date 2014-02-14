/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:22 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using PeterO;

namespace Test
{
  [TestFixture]
  public class BigIntTests
  {
    [Test]
    public void TestShifts() {
      TestCommon.DoTestShiftLeft("-123811", 139, "-86283673867977343180521212970464571450529415168");
      TestCommon.DoTestShiftRight("-1042721", 127, "-1");
      TestCommon.DoTestShiftLeft("28289813377724706864", 42, "124419915025685412732996321017856");
      TestCommon.DoTestShiftRight("-88556196", 89, "-1");
      TestCommon.DoTestShiftLeft("95420016902683", 124, "2029359325205057921124007216852651400359607982358528");
      TestCommon.DoTestShiftRight("54724161556258550", 46, "777");
      TestCommon.DoTestShiftLeft("3155330479", 172, "18888788629006754555486743276665668286507090108498535077904384");
      TestCommon.DoTestShiftRight("505922772707026529601", 162, "0");
      TestCommon.DoTestShiftLeft("43", 179, "32948654128616610454704461083731647447802891126947446784");
      TestCommon.DoTestShiftRight("55107506188695852", 155, "0");
      TestCommon.DoTestShiftLeft("139927165764765735", 98, "44344688917485059031953365589728191494058147840");
      TestCommon.DoTestShiftRight("-129681850326606", 82, "-1");
      TestCommon.DoTestShiftLeft("-5932507074570", 65, "-218870879440128273010605152010240");
      TestCommon.DoTestShiftRight("-164856599394495851262", 136, "-1");
      TestCommon.DoTestShiftLeft("977182433954", 76, "73833817180053605702782459779743744");
      TestCommon.DoTestShiftRight("806", 59, "0");
      TestCommon.DoTestShiftLeft("-622514242515706286301", 48, "-175222181914160088088864870373523456");
      TestCommon.DoTestShiftRight("-3705319301976962591455", 198, "-1");
      TestCommon.DoTestShiftLeft("-3291470468389122029764629", 164, "-76967831660432252522639645744788730028057863137718831277031149803315134464");
      TestCommon.DoTestShiftRight("-7874", 6, "-124");
      TestCommon.DoTestShiftLeft("-98", 122, "-521057374347687022178292367629895073792");
      TestCommon.DoTestShiftRight("-68211003", 140, "-1");
      TestCommon.DoTestShiftLeft("-49", 91, "-121318123849967266940114173952");
      TestCommon.DoTestShiftRight("6068902883", 26, "90");
      TestCommon.DoTestShiftLeft("-648511782422", 3, "-5188094259376");
      TestCommon.DoTestShiftRight("-40179201736463413834", 119, "-1");
      TestCommon.DoTestShiftLeft("404345", 2, "1617380");
      TestCommon.DoTestShiftRight("3885671755362666632018353", 98, "0");
      TestCommon.DoTestShiftLeft("6693", 78, "2022835127670178266577108992");
      TestCommon.DoTestShiftRight("-46103832", 141, "-1");
      TestCommon.DoTestShiftLeft("-529", 125, "-22501171512647055896515645916425672982528");
      TestCommon.DoTestShiftRight("9513521405775", 126, "0");
      TestCommon.DoTestShiftLeft("874040407364540365", 46, "61505125826776570556365259407360");
      TestCommon.DoTestShiftRight("-841759", 42, "-1");
      TestCommon.DoTestShiftLeft("5217653305", 38, "1434217619637844049920");
      TestCommon.DoTestShiftRight("45", 23, "0");
      TestCommon.DoTestShiftLeft("-96686542299678294981", 195, "-4855290099662786731169262873707071307803974155563754428764583550664097092599808");
      TestCommon.DoTestShiftRight("-30237963515951208960", 165, "-1");
      TestCommon.DoTestShiftLeft("8499707889996682", 44, "149528442524012194768944627712");
      TestCommon.DoTestShiftRight("-8360715847", 13, "-1020596");
      TestCommon.DoTestShiftLeft("-13500389315847", 74, "-255015144043389332767251838216962048");
      TestCommon.DoTestShiftRight("644885602", 15, "19680");
      TestCommon.DoTestShiftLeft("0", 168, "0");
      TestCommon.DoTestShiftRight("883", 161, "0");
      TestCommon.DoTestShiftLeft("-846142254218060", 43, "-7442745978122825221254676480");
      TestCommon.DoTestShiftRight("2429", 70, "0");
      TestCommon.DoTestShiftLeft("70238713904", 122, "373453059636496095662395719277785618832808738816");
      TestCommon.DoTestShiftRight("682796443119308234148", 24, "40697839446026");
      TestCommon.DoTestShiftLeft("6477547737146361502", 81, "15661749414445103242955092351813456656072704");
      TestCommon.DoTestShiftRight("-3903", 103, "-1");
      TestCommon.DoTestShiftLeft("-266357775008", 122, "-1416200845742453577740480658355175655915684626432");
      TestCommon.DoTestShiftRight("3647870029988653182652", 184, "0");
      TestCommon.DoTestShiftLeft("-79809915886898858", 3, "-638479327095190864");
      TestCommon.DoTestShiftRight("3", 121, "0");
      TestCommon.DoTestShiftLeft("-2422110", 17, "-317470801920");
      TestCommon.DoTestShiftRight("-401218", 66, "-1");
      TestCommon.DoTestShiftLeft("992748386", 3, "7941987088");
      TestCommon.DoTestShiftRight("4778037404628", 13, "583256519");
      TestCommon.DoTestShiftLeft("63975617419684836311569", 90, "79197978341831104917323202958826721363112499347456");
      TestCommon.DoTestShiftRight("96435828997095659644", 111, "0");
      TestCommon.DoTestShiftLeft("-5", 36, "-343597383680");
      TestCommon.DoTestShiftRight("260454703626230583606495", 9, "508700593019981608606");
      TestCommon.DoTestShiftLeft("-601433411293917760", 28, "-161446052014318363932098560");
      TestCommon.DoTestShiftRight("4305", 29, "0");
      TestCommon.DoTestShiftLeft("5", 14, "81920");
      TestCommon.DoTestShiftRight("638492240656249", 126, "0");
      TestCommon.DoTestShiftLeft("-174114439481063087626", 37, "-23930106346641189525326472937472");
      TestCommon.DoTestShiftRight("-86865914012216193570", 67, "-1");
      TestCommon.DoTestShiftLeft("7448512080846", 56, "536721859708202934021553913856");
      TestCommon.DoTestShiftRight("-363324644456667305376877", 54, "-20168570");
      TestCommon.DoTestShiftLeft("-59703", 93, "-591269873323640468418419308363776");
      TestCommon.DoTestShiftRight("5512", 3, "689");
      TestCommon.DoTestShiftLeft("71487", 98, "22655134614628858806198705510678528");
      TestCommon.DoTestShiftRight("-147781341312", 189, "-1");
      TestCommon.DoTestShiftLeft("24", 11, "49152");
      TestCommon.DoTestShiftRight("0", 165, "0");
      TestCommon.DoTestShiftLeft("3", 193, "37662610412320084583014736539245998496614132666784207077376");
      TestCommon.DoTestShiftRight("-49", 121, "-1");
      TestCommon.DoTestShiftLeft("67819", 52, "305429623128639668224");
      TestCommon.DoTestShiftRight("2234156114144769734811162", 160, "0");
      TestCommon.DoTestShiftLeft("700", 134, "15244650038058043163159182412943215873228800");
      TestCommon.DoTestShiftRight("-77942712427873200200", 42, "-17722121");
      TestCommon.DoTestShiftLeft("6276576613950791521892", 136, "546766936667080900941860629192067668342714170840790299953856512");
      TestCommon.DoTestShiftRight("-903805304660224075644", 107, "-1");
      TestCommon.DoTestShiftLeft("-2836032695181159871589597", 43, "-24946007400836748172953351472782770176");
      TestCommon.DoTestShiftRight("346", 102, "0");
      TestCommon.DoTestShiftLeft("73180669", 179, "56074524460041292967155038590510394807031584948934374034767872");
      TestCommon.DoTestShiftRight("-462574", 63, "-1");
      TestCommon.DoTestShiftLeft("-61846691821771550761", 96, "-4899999750624939552759129778674981145424687005696");
      TestCommon.DoTestShiftRight("4070", 171, "0");
      TestCommon.DoTestShiftLeft("5", 157, "913438523331814323877303020447676887284957839360");
      TestCommon.DoTestShiftRight("89205727177314305", 129, "0");
      TestCommon.DoTestShiftLeft("7353164484186685777542992", 192, "46156561544271952617470445314182180457600033770710223735869306004759221308418424832");
      TestCommon.DoTestShiftRight("39", 29, "0");
      TestCommon.DoTestShiftLeft("-45706435372006593296132", 73, "-431685076904423040042061488261919348300447744");
      TestCommon.DoTestShiftRight("-50477768822581203", 32, "-11752772");
      TestCommon.DoTestShiftLeft("233613", 53, "2104198839497807364096");
      TestCommon.DoTestShiftRight("-381495", 158, "-1");
      TestCommon.DoTestShiftLeft("501640124", 112, "2604664439960221389894664116371513780731904");
      TestCommon.DoTestShiftRight("-8961068350471589879", 162, "-1");
      TestCommon.DoTestShiftLeft("-1", 137, "-174224571863520493293247799005065324265472");
      TestCommon.DoTestShiftRight("9717938618514529802101200", 172, "0");
      TestCommon.DoTestShiftLeft("-69361174639252149690567", 80, "-83852514900191336137353800653459076861543841792");
      TestCommon.DoTestShiftRight("3555206", 136, "0");
      TestCommon.DoTestShiftLeft("9003966547185242187", 99, "5706941798987135232828143652732062465862880198656");
      TestCommon.DoTestShiftRight("7362294472154", 137, "0");
      TestCommon.DoTestShiftLeft("26157856168344161230", 83, "252983261661416388301424194976434643366051840");
      TestCommon.DoTestShiftRight("891123745507778486444707", 182, "0");
      TestCommon.DoTestShiftLeft("-6296435485198158", 177, "-1206157413051990810652567234011004812833057585885263191632909199998976");
      TestCommon.DoTestShiftRight("-382988301589610888064772", 58, "-1328758");
      TestCommon.DoTestShiftLeft("62784", 30, "67413806678016");
      TestCommon.DoTestShiftRight("-3291246071", 51, "-1");
      TestCommon.DoTestShiftLeft("-1907", 115, "-79213680873807330300861251998581784576");
      TestCommon.DoTestShiftRight("412", 38, "0");
      TestCommon.DoTestShiftLeft("-5814660581727282747714", 181, "-17821882826291432240087538109542473506206387058060837927869725636311098851328");
      TestCommon.DoTestShiftRight("4999343069658270652", 49, "8880");
      TestCommon.DoTestShiftLeft("9426764351514347372", 165, "440871409103065790281574788744142448682977823185765935880479445090304");
      TestCommon.DoTestShiftRight("503385687552", 90, "0");
      TestCommon.DoTestShiftLeft("7982017008", 159, "5832865463197557408549322571506502950717625933067561467904");
      TestCommon.DoTestShiftRight("-9453086317049542866898158", 129, "-1");
      TestCommon.DoTestShiftLeft("694015497", 177, "132946956805540458297541352018274290221602936299449548678365184");
      TestCommon.DoTestShiftRight("82561056005194903194926", 175, "0");
      TestCommon.DoTestShiftLeft("-346368781400224616899", 15, "-11349812228922560246546432");
      TestCommon.DoTestShiftRight("-2132051051606692161325415", 149, "-1");
      TestCommon.DoTestShiftLeft("578", 14, "9469952");
      TestCommon.DoTestShiftRight("-617445132045510", 7, "-4823790094106");
      TestCommon.DoTestShiftLeft("321390741239", 44, "5653965712829249616871424");
      TestCommon.DoTestShiftRight("394866477762133948329677", 7, "3084894357516671471325");
      TestCommon.DoTestShiftLeft("-3571856767", 41, "-7854596096133781520384");
      TestCommon.DoTestShiftRight("-77740673380579212872779", 126, "-1");
      TestCommon.DoTestShiftLeft("554716281", 87, "85838186836672505470349979961786368");
      TestCommon.DoTestShiftRight("-2491111508224483833843", 98, "-1");
      TestCommon.DoTestShiftLeft("-43763", 100, "-55476193217788003297700222376869888");
      TestCommon.DoTestShiftRight("-71899707484548", 48, "-1");
      TestCommon.DoTestShiftLeft("6", 191, "18831305206160042291507368269622999248307066333392103538688");
      TestCommon.DoTestShiftRight("-291706875275252552049", 105, "-1");
      TestCommon.DoTestShiftLeft("28", 93, "277298568799925181577403826176");
      TestCommon.DoTestShiftRight("-9150980379467", 33, "-1066");
      TestCommon.DoTestShiftLeft("-1964241106476506", 185, "-96326103579543201568342980649038011122960462193050200876974918223265792");
      TestCommon.DoTestShiftRight("-788549887", 79, "-1");
      TestCommon.DoTestShiftLeft("28156466", 89, "17428008313088737003633922321416192");
      TestCommon.DoTestShiftRight("-265905120797", 193, "-1");
      TestCommon.DoTestShiftLeft("486756148166895328075886", 171, "1456936770614888654996623992625677003450662958466370311338933684396598755328");
      TestCommon.DoTestShiftRight("-39002800", 12, "-9523");
      TestCommon.DoTestShiftLeft("32628", 172, "195321345731922740900453698444766750168407109682056921088");
      TestCommon.DoTestShiftRight("70179081893360304", 39, "127655");
      TestCommon.DoTestShiftLeft("-2895683052002632477775333", 15, "-94885742248022261031742111744");
      TestCommon.DoTestShiftRight("-806780351465401907682719", 105, "-1");
      TestCommon.DoTestShiftLeft("-52028176198826166407898", 65, "-1919500901923225862483227951132725882126336");
      TestCommon.DoTestShiftRight("3740338", 180, "0");
      TestCommon.DoTestShiftLeft("238058", 159, "173961078389860043450866401953386451546625994657890304");
      TestCommon.DoTestShiftRight("-3748690276920105573", 187, "-1");
      TestCommon.DoTestShiftLeft("-3284484", 154, "-75004355371674270943645493345301689336431186601312256");
      TestCommon.DoTestShiftRight("3567941358019290278", 59, "6");
      TestCommon.DoTestShiftLeft("-4", 36, "-274877906944");
      TestCommon.DoTestShiftRight("-25273360741553115441", 21, "-12051277514245");
      TestCommon.DoTestShiftLeft("431789683521346388758", 120, "573946935627682376381303076078844989588951071785492676608");
      TestCommon.DoTestShiftRight("13671044744856", 81, "0");
      TestCommon.DoTestShiftLeft("67174626506", 16, "4402356322697216");
      TestCommon.DoTestShiftRight("-26625", 99, "-1");
      TestCommon.DoTestShiftLeft("867588022729552", 123, "9225778308958403539119154468855817316895316944879616");
      TestCommon.DoTestShiftRight("-13358105978685665", 190, "-1");
      TestCommon.DoTestShiftLeft("-45756607993", 126, "-3892541717532892924092346256291420636607730941952");
      TestCommon.DoTestShiftRight("-855389270870349808174741", 71, "-363");
      TestCommon.DoTestShiftLeft("-2304118921677598987040", 34, "-39584461658800292420558111375360");
      TestCommon.DoTestShiftRight("-38", 9, "-1");
      TestCommon.DoTestShiftLeft("-39", 162, "-227994255423620855239774833903740151066325476704256");
      TestCommon.DoTestShiftLeft("520096021644", 75, "19648672163883769218344286569889792");
      TestCommon.DoTestShiftLeft("-90", 100, "-114088554020540646134703288483840");
      TestCommon.DoTestShiftRight("-29337", 193, "-1");
      TestCommon.DoTestShiftLeft("-73311599717", 9, "-37535539055104");
      TestCommon.DoTestShiftLeft("7409431609", 15, "242792254963712");
      TestCommon.DoTestShiftRight("-9", 187, "-1");
      TestCommon.DoTestShiftLeft("-400705", 175, "-19189957053208312343390045172320770044435839373580634685440");
      TestCommon.DoTestShiftRight("-669716254155788", 137, "-1");
      TestCommon.DoTestShiftLeft("5", 42, "21990232555520");
      TestCommon.DoTestShiftRight("-5463532", 80, "-1");
      TestCommon.DoTestShiftLeft("205127767121", 111, "532542130410316001526540173233402599920631808");
      TestCommon.DoTestShiftRight("-868", 33, "-1");
      TestCommon.DoTestShiftLeft("-748", 130, "-1018124841827447882682416825435850488676352");
      TestCommon.DoTestShiftRight("7669818074419242", 131, "0");
      TestCommon.DoTestShiftLeft("-32785410697205", 30, "-35203066682606008401920");
      TestCommon.DoTestShiftRight("403415734450823", 174, "0");
      TestCommon.DoTestShiftLeft("-90899630931933073401267", 163, "-1062799675518359803915259742908872710540972551013718838276842631762804736");
      TestCommon.DoTestShiftRight("6095284762439", 25, "181653");
      TestCommon.DoTestShiftLeft("9079", 34, "155976032321536");
      TestCommon.DoTestShiftLeft("2347045339", 76, "177337731882704386294221692207104");
      TestCommon.DoTestShiftRight("2341844084040", 25, "69792");
      TestCommon.DoTestShiftLeft("1970639750289749185", 174, "47187447343314208396017248234478749860829861211041535274881408607191040");
      TestCommon.DoTestShiftLeft("-3954026059581734380670", 48, "-1112959393034095598235580141949419520");
      TestCommon.DoTestShiftRight("17952157", 7, "140251");
      TestCommon.DoTestShiftLeft("66885411437995", 75, "2526859401341868697653840355430236160");
      TestCommon.DoTestShiftLeft("429895", 146, "38347915428489288941721990427280669478453803745280");
    }

    // Test some specific cases
    [Test]
    public void TestSpecificCases() {
      TestCommon.DoTestMultiply("39258416159456516340113264558732499166970244380745050", "39258416159456516340113264558732499166970244380745051", "1541223239349076530208308657654362309553698742116222355477449713742236585667505604058123112521437480247550");
      TestCommon.DoTestMultiply(
        "5786426269322750882632312999752639738983363095641642905722171221986067189342123124290107105663618428969517616421742429671402859775667602123564",
        "331378991485809774307751183645559883724387697397707434271522313077548174328632968616330900320595966360728317363190772921",
        "1917500101435169880779183578665955372346028226046021044867189027856189131730889958057717187493786883422516390996639766012958050987359732634213213442579444095928862861132583117668061032227577386757036981448703231972963300147061503108512300577364845823910107210444");
      TestCommon.DoTestDivide("9999999999999999999999", "281474976710655", "35527136");
    }

    [Test]
    public void TestToString() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String s = bigintA.ToString();
        BigInteger big2 = BigInteger.fromString(s);
        Assert.AreEqual(big2.ToString(), s);
      }
    }

    [Test]
    public void TestShiftRight() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        int smallint = r.NextValue(0x7FFFFFFF);
        BigInteger bigintA = (BigInteger)smallint;
        string str = bigintA.ToString();
        for (int j = 32; j < 80; ++j) {
          TestCommon.DoTestShiftRight(str, j, "0");
          TestCommon.DoTestShiftRight("-" + str, j, "-1");
        }
      }
    }

    [Test]
    public void TestDigitCount() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        String str = bigintA.ToString();
        Assert.AreEqual(str.Length, bigintA.getDigitCount());
      }
    }

    [Test]
    public void TestMultiply() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA + BigInteger.One;
        BigInteger bigintC = bigintA * (BigInteger)bigintB;
        // Test near-squaring
        if (bigintA.IsZero || bigintB.IsZero) {
          Assert.AreEqual(BigInteger.Zero, bigintC);
        }
        if (bigintA.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintB, bigintC);
        }
        if (bigintB.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintA, bigintC);
        }
        bigintB = bigintA;
        // Test squaring
        bigintC = bigintA * (BigInteger)bigintB;
        if (bigintA.IsZero || bigintB.IsZero) {
          Assert.AreEqual(BigInteger.Zero, bigintC);
        }
        if (bigintA.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintB, bigintC);
        }
        if (bigintB.Equals(BigInteger.One)) {
          Assert.AreEqual(bigintA, bigintC);
        }
      }
    }

    [Test]
    public void TestMultiplyDivide() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 4000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        // Test that A*B/A = B and A*B/B = A
        BigInteger bigintC = bigintA * (BigInteger)bigintB;
        BigInteger bigintRem;
        BigInteger bigintE;
        BigInteger bigintD;
        if (!bigintB.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintB, out bigintRem);
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(bigintA, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(BigInteger.Zero, bigintRem, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          bigintE = bigintC / (BigInteger)bigintB;
          if (!bigintD.Equals(bigintE)) {
            // Testing that divideWithRemainder and division method return the same value
            Assert.AreEqual(bigintD, bigintE, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          bigintE = bigintC % (BigInteger)bigintB;
          if (!bigintRem.Equals(bigintE)) {
            Assert.AreEqual(bigintRem, bigintE, "TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
          if (bigintE.Sign > 0 && !bigintC.mod(bigintB).Equals(bigintE)) {
            Assert.Fail("TestMultiplyDivide " + bigintA + "; " + bigintB + ";\n" + bigintC);
          }
        }
        if (!bigintA.IsZero) {
          bigintD = BigInteger.DivRem(bigintC, bigintA, out bigintRem);
          if (!bigintD.Equals(bigintB)) {
            Assert.AreEqual(bigintB, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
          if (!bigintRem.IsZero) {
            Assert.AreEqual(BigInteger.Zero, bigintRem, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
        if (!bigintB.IsZero) {
          bigintC = BigInteger.DivRem(bigintA, bigintB, out bigintRem);
          bigintD = bigintB * (BigInteger)bigintC;
          bigintD += (BigInteger)bigintRem;
          if (!bigintD.Equals(bigintA)) {
            Assert.AreEqual(bigintA, bigintD, "TestMultiplyDivide " + bigintA + "; " + bigintB);
          }
        }
      }
    }

    [Test]
    public void TestPow() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 200; ++i) {
        int power = 1 + r.NextValue(8);
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = bigintA;
        for (int j = 1; j < power; ++j) {
          bigintB *= bigintA;
        }
        TestCommon.DoTestPow(bigintA.ToString(), power, bigintB.ToString());
      }
    }

    [Test]
    public void TestSquareRoot() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        if (bigintA.Sign < 0) {
          bigintA = -bigintA;
        }
        if (bigintA.Sign == 0) {
          bigintA = BigInteger.One;
        }
        BigInteger sr = bigintA.sqrt();
        BigInteger srsqr = sr * (BigInteger)sr;
        sr += BigInteger.One;
        BigInteger sronesqr = sr * (BigInteger)sr;
        if (srsqr.CompareTo(bigintA) > 0) {
          Assert.Fail(srsqr + " not " + bigintA + " or less (TestSqrt, sqrt=" + sr + ")");
        }
        if (sronesqr.CompareTo(bigintA) <= 0) {
          Assert.Fail(srsqr + " not greater than " + bigintA + " (TestSqrt, sqrt=" + sr + ")");
        }
      }
    }

    [Test]
    public void TestSmallIntDivide() {
      int a, b;
      FastRandom fr = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        a = fr.NextValue(0x1000000);
        b = fr.NextValue(0x1000000);
        if (b == 0) {
          continue;
        }
        int c = a / b;
        BigInteger bigintA = (BigInteger)a;
        BigInteger bigintB = (BigInteger)b;
        BigInteger bigintC = bigintA / (BigInteger)bigintB;
        Assert.AreEqual((int)bigintC, c);
      }
    }

    [Test]
    public void TestMiscellaneous() {
     Assert.AreEqual(1, BigInteger.Zero.getDigitCount());
     BigInteger minValue = (BigInteger)Int32.MinValue;
      BigInteger minValueTimes2 = minValue + (BigInteger)minValue;
      Assert.AreEqual(Int32.MinValue, (int)minValue);
      try {
        Console.WriteLine((int)minValueTimes2);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      BigInteger verybig = BigInteger.One << 80;
      try {
        Console.WriteLine((int)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Console.WriteLine((long)verybig);
        Assert.Fail("Should have failed");
      } catch (OverflowException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divideAndRemainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.pow(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        (BigInteger.Zero - BigInteger.One).PowBigIntVar(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(BigInteger.One.Equals(BigInteger.Zero));
      Assert.IsFalse(verybig.Equals(BigInteger.Zero));
      Assert.IsFalse(BigInteger.One.Equals(BigInteger.Zero - BigInteger.One));
      Assert.AreEqual(1, BigInteger.One.CompareTo(null));
      BigInteger[] tmpsqrt = BigInteger.Zero.sqrtWithRemainder();
      Assert.AreEqual(BigInteger.Zero, tmpsqrt[0]);
    }

    [Test]
    public void TestExceptions() {
      try {
        BigInteger.fromString("xyz");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(String.Empty);
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.fromSubstring(null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.Zero.testBit(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromByteArray(null, false);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.fromSubstring("123", -1, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 4, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 4);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 1, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromSubstring("123", 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("x11");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString(".");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("..");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.fromString("e200");
        Assert.Fail("Should have failed");
      } catch (FormatException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      try {
        BigInteger.One.mod((BigInteger)(-1));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.add(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.subtract(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.multiply(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divide(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.mod(BigInteger.Zero);
        Assert.Fail("Should have failed");
      } catch (DivideByZeroException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.remainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        BigInteger.One.divideAndRemainder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual(BigInteger.One, ((BigInteger)13).mod((BigInteger)4));
      Assert.AreEqual((BigInteger)3, ((BigInteger)(-13)).mod((BigInteger)4));
      try {
        ((BigInteger)13).mod(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)13).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        ((BigInteger)(-13)).mod((BigInteger)(-4));
        Assert.Fail("Should have failed");
      } catch (ArithmeticException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestAddSubtract() {
      FastRandom r = new FastRandom();
      for (int i = 0; i < 10000; ++i) {
        BigInteger bigintA = CBORTest.RandomBigInteger(r);
        BigInteger bigintB = CBORTest.RandomBigInteger(r);
        BigInteger bigintC = bigintA + (BigInteger)bigintB;
        BigInteger bigintD = bigintC - (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(bigintA, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintD = bigintC - (BigInteger)bigintA;
        if (!bigintD.Equals(bigintB)) {
          Assert.AreEqual(bigintB, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
        bigintC = bigintA - (BigInteger)bigintB;
        bigintD = bigintC + (BigInteger)bigintB;
        if (!bigintD.Equals(bigintA)) {
          Assert.AreEqual(bigintA, bigintD, "TestAddSubtract " + bigintA + "; " + bigintB);
        }
      }
    }

    [Test]
    public void TestBitLength() {
      Assert.AreEqual(0, BigInteger.valueOf(0).bitLength());
      Assert.AreEqual(1, BigInteger.valueOf(1).bitLength());
      Assert.AreEqual(2, BigInteger.valueOf(2).bitLength());
      Assert.AreEqual(2, BigInteger.valueOf(2).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(Int32.MaxValue).bitLength());
      Assert.AreEqual(31, BigInteger.valueOf(Int32.MinValue).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(65535).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(-65535).bitLength());
      Assert.AreEqual(17, BigInteger.valueOf(65536).bitLength());
      Assert.AreEqual(16, BigInteger.valueOf(-65536).bitLength());
      Assert.AreEqual(0, BigInteger.valueOf(-1).bitLength());
      Assert.AreEqual(1, BigInteger.valueOf(-2).bitLength());
    }

    public static int ModPow(int x, int pow, int mod) {
      if (x < 0) {
        throw new ArgumentException("x (" + Convert.ToString((long)x, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater or equal to " + "0");
      }
      if (pow <= 0) {
        throw new ArgumentException("pow (" + Convert.ToString((long)pow, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater than " + "0");
      }
      if (mod <= 0) {
        throw new ArgumentException("mod (" + Convert.ToString((long)mod, System.Globalization.CultureInfo.InvariantCulture) + ") is not greater than " + "0");
      }
      int r = 1;
      int v = x;
      while (pow != 0) {
        if ((pow & 1) != 0) {
          r = (int)(((long)r * (long)v) % mod);
        }
        pow >>= 1;
        if (pow != 0) {
          v = (int)(((long)v * (long)v) % mod);
        }
      }
      return r;
    }

    public static bool IsPrime(int n) {
      // Use a deterministic Rabin-Miller test
      if (n < 2) {
        return false;
      }
      if (n == 2) {
        return true;
      }
      if (n % 2 == 0) {
        return false;
      }
      int d = n - 1;
      while ((d & 1) == 0) {
        d >>= 1;
      }
      int mp = 0;
      // For all 32-bit integers it's enough
      // to check the strong pseudoprime
      // bases 2, 7, and 61
      if (n > 2) {
        mp = ModPow(2, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      if (n > 7) {
        mp = ModPow(7, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      if (n > 61) {
        mp = ModPow(61, d, n);
        if (mp != 1 && mp + 1 != n) {
          return false;
        }
      }
      return true;
    }

    [Test]
    public void TestGcd() {
      int prime = 0;
      FastRandom rand = new FastRandom();
      for (int i = 0; i < 1000; ++i) {
        while (true) {
          prime = rand.NextValue(0x7FFFFFFF);
          prime |= 1;
          if (IsPrime(prime)) {
            break;
          }
        }
        BigInteger bigprime = (BigInteger)prime;
        BigInteger ba = CBORTest.RandomBigInteger(rand);
        if (ba.IsZero) {
          continue;
        }
        ba *= (BigInteger)bigprime;
        Assert.AreEqual(bigprime, BigInteger.GreatestCommonDivisor(bigprime, ba));
      }
    }
  }
}
