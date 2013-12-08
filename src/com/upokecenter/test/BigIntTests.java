package com.upokecenter.test;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 12/1/2013
 * Time: 11:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;

  
  public class BigIntTests
  {
    
    
    
    @Test
    public void TestSquaring() {
      Assert.assertEquals("152587890625",((BigInteger.valueOf(390625)).multiply(BigInteger.valueOf(390625))).toString());
      Assert.assertEquals("338813178901720135627329000271856784820556640625",
                                      (BigInteger.valueOf(5)).pow(BigInteger.valueOf(68)).toString());
    }
    
    @Test
    public void TestDivide() {
      { BigInteger bigintA=BigInteger.fromString("43569"); // 43569
        BigInteger bigintB=BigInteger.fromString("334558199138390434829164799015810366752"); // 334558199138390434829164799015810366752
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("833272832730475642197827"); // 833272832730475642197827
        BigInteger bigintB=BigInteger.fromString("576734746886592117601685404914135826"); // 576734746886592117601685404914135826
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1176845874825103014377456884685643370208171580545915667079"); // 1176845874825103014377456884685643370208171580545915667079
        BigInteger bigintB=BigInteger.fromString("72"); // 72
        Assert.assertEquals("16345081594793097421909123398411713475113494174248828709",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("9126440073362022353"); // 9126440073362022353
        BigInteger bigintB=BigInteger.fromString("82743614979513280142683241749715085686833736691479460036660"); // 82743614979513280142683241749715085686833736691479460036660
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1508557146551837567"); // 1508557146551837567
        BigInteger bigintB=BigInteger.fromString("12460369537138174699059543147"); // 12460369537138174699059543147
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("55411734099"); // 55411734099
        BigInteger bigintB=BigInteger.fromString("51660934724301252611081245143385753310"); // 51660934724301252611081245143385753310
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("426475990353422835626019301115321"); // 426475990353422835626019301115321
        BigInteger bigintB=BigInteger.fromString("7461578"); // 7461578
        Assert.assertEquals("57156273157423648942089635",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("927075005348451446758598274071516665265273802100"); // 927075005348451446758598274071516665265273802100
        BigInteger bigintB=BigInteger.fromString("743699551102358520373659"); // 743699551102358520373659
        Assert.assertEquals("1246571957686786590019715",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("22209706229752449"); // 22209706229752449
        BigInteger bigintB=BigInteger.fromString("4031915733837283707093248746004561"); // 4031915733837283707093248746004561
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("45101438808264724967839658787"); // 45101438808264724967839658787
        BigInteger bigintB=BigInteger.fromString("751005672976810082803331"); // 751005672976810082803331
        Assert.assertEquals("60054",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1447731260503552142386826104874341753981079717"); // 1447731260503552142386826104874341753981079717
        BigInteger bigintB=BigInteger.fromString("2280721331466043815180200117394260396236820992973622875"); // 2280721331466043815180200117394260396236820992973622875
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1994458623057324"); // 1994458623057324
        BigInteger bigintB=BigInteger.fromString("9095236316543887"); // 9095236316543887
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("11062449725548093142869805683376852459253788"); // 11062449725548093142869805683376852459253788
        BigInteger bigintB=BigInteger.fromString("4222307502652794452555"); // 4222307502652794452555
        Assert.assertEquals("2620000963595799008232",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("134893350935955217906329622488647617783"); // 134893350935955217906329622488647617783
        BigInteger bigintB=BigInteger.fromString("3641987330450989028074697710"); // 3641987330450989028074697710
        Assert.assertEquals("37038391047",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("389427829745286709131546623179879443091344728899803558106616"); // 389427829745286709131546623179879443091344728899803558106616
        BigInteger bigintB=BigInteger.fromString("535243612076408164134228808274615134805585696382"); // 535243612076408164134228808274615134805585696382
        Assert.assertEquals("727571186201",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("712902792505088904593802362030688604408488667556548312"); // 712902792505088904593802362030688604408488667556548312
        BigInteger bigintB=BigInteger.fromString("1506182499829057076819874890076967"); // 1506182499829057076819874890076967
        Assert.assertEquals("473317670724496673424",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("42457337294559"); // 42457337294559
        BigInteger bigintB=BigInteger.fromString("951402653467437296624727967896776742329851313437"); // 951402653467437296624727967896776742329851313437
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("461561672668649645167943"); // 461561672668649645167943
        BigInteger bigintB=BigInteger.fromString("3283018649879244544638794960459737344724698"); // 3283018649879244544638794960459737344724698
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("17360503477631601292"); // 17360503477631601292
        BigInteger bigintB=BigInteger.fromString("5087852475367423879552745752203219643439566038"); // 5087852475367423879552745752203219643439566038
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("551354247"); // 551354247
        BigInteger bigintB=BigInteger.fromString("52601521242333166883560311"); // 52601521242333166883560311
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3085309280766013540267687953187221"); // 3085309280766013540267687953187221
        BigInteger bigintB=BigInteger.fromString("241900694612150"); // 241900694612150
        Assert.assertEquals("12754445727048553190",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1030168054235155257478"); // 1030168054235155257478
        BigInteger bigintB=BigInteger.fromString("323724078793634014542078208861775769573195498341293"); // 323724078793634014542078208861775769573195498341293
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("215001220943843805640182638357432174855"); // 215001220943843805640182638357432174855
        BigInteger bigintB=BigInteger.fromString("15786219895"); // 15786219895
        Assert.assertEquals("13619550619077690583517786374",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("37680874693269194465745530841"); // 37680874693269194465745530841
        BigInteger bigintB=BigInteger.fromString("29"); // 29
        Assert.assertEquals("1299340506664454981577432097",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("261190815634209"); // 261190815634209
        BigInteger bigintB=BigInteger.fromString("239269669930690808850213593178087921011070358568518"); // 239269669930690808850213593178087921011070358568518
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("978848711624273461886647116874158848497250392799"); // 978848711624273461886647116874158848497250392799
        BigInteger bigintB=BigInteger.fromString("1816171221773737533500669024346849505775"); // 1816171221773737533500669024346849505775
        Assert.assertEquals("538962791",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("156175649383412914610076880662303575090"); // 156175649383412914610076880662303575090
        BigInteger bigintB=BigInteger.fromString("5813451160379989834200479000583145941020914934614180377989"); // 5813451160379989834200479000583145941020914934614180377989
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("14686084"); // 14686084
        BigInteger bigintB=BigInteger.fromString("1228405624538168865421882832308474860455018596162949329830615"); // 1228405624538168865421882832308474860455018596162949329830615
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("9915659164094622381435883945311578487047519"); // 9915659164094622381435883945311578487047519
        BigInteger bigintB=BigInteger.fromString("59571692356734"); // 59571692356734
        Assert.assertEquals("166449176980176115873790145581",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("193345531773344434111188590"); // 193345531773344434111188590
        BigInteger bigintB=BigInteger.fromString("11739902957235250871"); // 11739902957235250871
        Assert.assertEquals("16469091",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3774693997569948457253691274157534133761957978311441823088"); // 3774693997569948457253691274157534133761957978311441823088
        BigInteger bigintB=BigInteger.fromString("369265857706694931615959087594345322766328605698872364"); // 369265857706694931615959087594345322766328605698872364
        Assert.assertEquals("10222",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("497468732175912549895020364347825757"); // 497468732175912549895020364347825757
        BigInteger bigintB=BigInteger.fromString("42042981068065582"); // 42042981068065582
        Assert.assertEquals("11832384848508586694",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3087638940847340406151759432782"); // 3087638940847340406151759432782
        BigInteger bigintB=BigInteger.fromString("3258034339959761534361672910565006966610935551"); // 3258034339959761534361672910565006966610935551
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("753314029370096628636417344074504184857992143392166965400"); // 753314029370096628636417344074504184857992143392166965400
        BigInteger bigintB=BigInteger.fromString("3248111591"); // 3248111591
        Assert.assertEquals("231923691124839998958156897902743325070075199701",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("6822"); // 6822
        BigInteger bigintB=BigInteger.fromString("12565746161612442501726391026818240297965019"); // 12565746161612442501726391026818240297965019
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("163"); // 163
        BigInteger bigintB=BigInteger.fromString("127501572222728754465800"); // 127501572222728754465800
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("241443437116248"); // 241443437116248
        BigInteger bigintB=BigInteger.fromString("401665352926891225014588960020379851469622592"); // 401665352926891225014588960020379851469622592
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("14882"); // 14882
        BigInteger bigintB=BigInteger.fromString("41249221776448348010525489145897401633477"); // 41249221776448348010525489145897401633477
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("775575082"); // 775575082
        BigInteger bigintB=BigInteger.fromString("28820679948498772645638136104"); // 28820679948498772645638136104
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("17879"); // 17879
        BigInteger bigintB=BigInteger.fromString("2737780178086134179354178704171278939886681536804092871263"); // 2737780178086134179354178704171278939886681536804092871263
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("622769029589945873005232371262384412181909916422262001473567"); // 622769029589945873005232371262384412181909916422262001473567
        BigInteger bigintB=BigInteger.fromString("1507420124985305512788541855896162414171635237"); // 1507420124985305512788541855896162414171635237
        Assert.assertEquals("413135674167821",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("60954953801178458"); // 60954953801178458
        BigInteger bigintB=BigInteger.fromString("570057564875168993466939571121810933961859657635716594799098"); // 570057564875168993466939571121810933961859657635716594799098
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("359374484509787454982044"); // 359374484509787454982044
        BigInteger bigintB=BigInteger.fromString("1390587253187522242260769234049077005190564409594"); // 1390587253187522242260769234049077005190564409594
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2044004063509533758174"); // 2044004063509533758174
        BigInteger bigintB=BigInteger.fromString("70743282149493"); // 70743282149493
        Assert.assertEquals("28893260",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("19653107961266251"); // 19653107961266251
        BigInteger bigintB=BigInteger.fromString("21584511176251635456695969423077777024359545038118842027"); // 21584511176251635456695969423077777024359545038118842027
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("521290529635535707669271060498360669"); // 521290529635535707669271060498360669
        BigInteger bigintB=BigInteger.fromString("70873877258950305038572247219735913955849035213971110"); // 70873877258950305038572247219735913955849035213971110
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("15529844"); // 15529844
        BigInteger bigintB=BigInteger.fromString("318796289371873006872619780872414830136604243577224"); // 318796289371873006872619780872414830136604243577224
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("997781860468116100945213"); // 997781860468116100945213
        BigInteger bigintB=BigInteger.fromString("85"); // 85
        Assert.assertEquals("11738610123154307069943",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("10390997"); // 10390997
        BigInteger bigintB=BigInteger.fromString("7263052"); // 7263052
        Assert.assertEquals("1",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1834254468402623386855"); // 1834254468402623386855
        BigInteger bigintB=BigInteger.fromString("138"); // 138
        Assert.assertEquals("13291699046395821643",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("345565926475240587412182"); // 345565926475240587412182
        BigInteger bigintB=BigInteger.fromString("60357704310609"); // 60357704310609
        Assert.assertEquals("5725299370",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("178"); // 178
        BigInteger bigintB=BigInteger.fromString("30351782716070217"); // 30351782716070217
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16084698"); // 16084698
        BigInteger bigintB=BigInteger.fromString("1595076187715652132522052636134821"); // 1595076187715652132522052636134821
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("88025501099452005497977014958359573304170962029050046"); // 88025501099452005497977014958359573304170962029050046
        BigInteger bigintB=BigInteger.fromString("9079171"); // 9079171
        Assert.assertEquals("9695323625852184687123638816623188758551960529",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("102838175554476020997972509524075585154"); // 102838175554476020997972509524075585154
        BigInteger bigintB=BigInteger.fromString("3539440503"); // 3539440503
        Assert.assertEquals("29054924208306722029386379976",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("7172775267603108383759259253720536430069363"); // 7172775267603108383759259253720536430069363
        BigInteger bigintB=BigInteger.fromString("2006683711"); // 2006683711
        Assert.assertEquals("3574442363928227642726531931130294",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("348146096843"); // 348146096843
        BigInteger bigintB=BigInteger.fromString("34141776210740590819535"); // 34141776210740590819535
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("35720159738216576890622638403883597421589"); // 35720159738216576890622638403883597421589
        BigInteger bigintB=BigInteger.fromString("170"); // 170
        Assert.assertEquals("210118586695391628768368461199315278950",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2513923076639033"); // 2513923076639033
        BigInteger bigintB=BigInteger.fromString("1955690563"); // 1955690563
        Assert.assertEquals("1285440",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3578520592566537249776965361795865564238916319845858867699"); // 3578520592566537249776965361795865564238916319845858867699
        BigInteger bigintB=BigInteger.fromString("13173248606424076349132503925896171058622528734535197129"); // 13173248606424076349132503925896171058622528734535197129
        Assert.assertEquals("271",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5136970352171473683753084927236395"); // 5136970352171473683753084927236395
        BigInteger bigintB=BigInteger.fromString("506635944976550336765"); // 506635944976550336765
        Assert.assertEquals("10139372073983",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("314363964550693882421739145311637117"); // 314363964550693882421739145311637117
        BigInteger bigintB=BigInteger.fromString("53520902645022897371325941384061191668439"); // 53520902645022897371325941384061191668439
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("558364300078"); // 558364300078
        BigInteger bigintB=BigInteger.fromString("645312994147058817207"); // 645312994147058817207
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("4183967"); // 4183967
        BigInteger bigintB=BigInteger.fromString("30750349838172048367781373742606065696571515993211231"); // 30750349838172048367781373742606065696571515993211231
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("10711829"); // 10711829
        BigInteger bigintB=BigInteger.fromString("157296205791620"); // 157296205791620
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("18650251349785"); // 18650251349785
        BigInteger bigintB=BigInteger.fromString("230838811645452070702773453610596949434455611743619"); // 230838811645452070702773453610596949434455611743619
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("129103875613919811226822695741276808532638636902179"); // 129103875613919811226822695741276808532638636902179
        BigInteger bigintB=BigInteger.fromString("51232"); // 51232
        Assert.assertEquals("2519985079909427920573522324743847761801972144",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16059203431301460374027561609511986029792326"); // 16059203431301460374027561609511986029792326
        BigInteger bigintB=BigInteger.fromString("10989787"); // 10989787
        Assert.assertEquals("1461284320733555652536992901637855768",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("119961701848064028633484088"); // 119961701848064028633484088
        BigInteger bigintB=BigInteger.fromString("640199495007253894108689273313779145999560979996112143700338"); // 640199495007253894108689273313779145999560979996112143700338
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5488900000996014474645395720994990969476858214"); // 5488900000996014474645395720994990969476858214
        BigInteger bigintB=BigInteger.fromString("49"); // 49
        Assert.assertEquals("112018367367265601523375422877448795295446086",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1304815226239894396196262835990495078020481696818019151169614"); // 1304815226239894396196262835990495078020481696818019151169614
        BigInteger bigintB=BigInteger.fromString("1055170227260"); // 1055170227260
        Assert.assertEquals("1236592155967247961067402602246632951515563497247",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("61216201864408719"); // 61216201864408719
        BigInteger bigintB=BigInteger.fromString("21507266951945751040373985337995713196826292490829762649"); // 21507266951945751040373985337995713196826292490829762649
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("245600940369791811419244279"); // 245600940369791811419244279
        BigInteger bigintB=BigInteger.fromString("32694176830338537"); // 32694176830338537
        Assert.assertEquals("7512069860",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("277175140041014789865126596"); // 277175140041014789865126596
        BigInteger bigintB=BigInteger.fromString("14022737"); // 14022737
        Assert.assertEquals("19766122693523724353",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3053619256"); // 3053619256
        BigInteger bigintB=BigInteger.fromString("3889937837521292260495617573973091367792092965"); // 3889937837521292260495617573973091367792092965
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("204208703282090486218346087629203383460720141481594"); // 204208703282090486218346087629203383460720141481594
        BigInteger bigintB=BigInteger.fromString("23766596356185345390900268107922609137739394746791391142"); // 23766596356185345390900268107922609137739394746791391142
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16685925610033084101986098879899357728643187583642954"); // 16685925610033084101986098879899357728643187583642954
        BigInteger bigintB=BigInteger.fromString("572832119344782814944783483460695124143379694852174349596468"); // 572832119344782814944783483460695124143379694852174349596468
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("164556782107600230073627401458173173045524157651023449"); // 164556782107600230073627401458173173045524157651023449
        BigInteger bigintB=BigInteger.fromString("3085342947"); // 3085342947
        Assert.assertEquals("53335005195323665934640555683475913138262926",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16928071245381519977998069199125"); // 16928071245381519977998069199125
        BigInteger bigintB=BigInteger.fromString("63421"); // 63421
        Assert.assertEquals("266915867699681808517652972",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("962269509357"); // 962269509357
        BigInteger bigintB=BigInteger.fromString("231617485933624285531731923182575435174444133368453"); // 231617485933624285531731923182575435174444133368453
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("44116086910255644561326428360"); // 44116086910255644561326428360
        BigInteger bigintB=BigInteger.fromString("48728168862620742"); // 48728168862620742
        Assert.assertEquals("905350805088",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1890403843"); // 1890403843
        BigInteger bigintB=BigInteger.fromString("81674362710853403360654722184059339226719868040380993"); // 81674362710853403360654722184059339226719868040380993
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("166"); // 166
        BigInteger bigintB=BigInteger.fromString("11581"); // 11581
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("138220911734520"); // 138220911734520
        BigInteger bigintB=BigInteger.fromString("191522227"); // 191522227
        Assert.assertEquals("721696",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("299967455184638524372719051017954490944810391485145"); // 299967455184638524372719051017954490944810391485145
        BigInteger bigintB=BigInteger.fromString("12605309"); // 12605309
        Assert.assertEquals("23796914076809900048679413651656971752521924",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("177896759897476506724905"); // 177896759897476506724905
        BigInteger bigintB=BigInteger.fromString("9049743888490732773"); // 9049743888490732773
        Assert.assertEquals("19657",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16204662743622038244358501267218982906826037"); // 16204662743622038244358501267218982906826037
        BigInteger bigintB=BigInteger.fromString("256294070126835228616661750"); // 256294070126835228616661750
        Assert.assertEquals("63226834454666267",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1087129444057850022784241"); // 1087129444057850022784241
        BigInteger bigintB=BigInteger.fromString("11519845158815553519"); // 11519845158815553519
        Assert.assertEquals("94370",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("64019781034868016204309836127"); // 64019781034868016204309836127
        BigInteger bigintB=BigInteger.fromString("54353577181107177214940273061"); // 54353577181107177214940273061
        Assert.assertEquals("1",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("14964044"); // 14964044
        BigInteger bigintB=BigInteger.fromString("3896729121495131637250"); // 3896729121495131637250
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1382034461798072633537316701284782772479605295272"); // 1382034461798072633537316701284782772479605295272
        BigInteger bigintB=BigInteger.fromString("707825827247031335380266816652476171"); // 707825827247031335380266816652476171
        Assert.assertEquals("1952506405669",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("39392221555260244470260674"); // 39392221555260244470260674
        BigInteger bigintB=BigInteger.fromString("1011731241080562815110667730899171921332181099138"); // 1011731241080562815110667730899171921332181099138
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("61003"); // 61003
        BigInteger bigintB=BigInteger.fromString("2264144983434777263"); // 2264144983434777263
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1464185236"); // 1464185236
        BigInteger bigintB=BigInteger.fromString("10784287213235167088639372857684"); // 10784287213235167088639372857684
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("58"); // 58
        BigInteger bigintB=BigInteger.fromString("515463019908"); // 515463019908
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("4730074784891683701656684568121518524575642706850663820"); // 4730074784891683701656684568121518524575642706850663820
        BigInteger bigintB=BigInteger.fromString("22365555837090168774503502486953791428232710104102600"); // 22365555837090168774503502486953791428232710104102600
        Assert.assertEquals("211",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("18512334611210780936480117"); // 18512334611210780936480117
        BigInteger bigintB=BigInteger.fromString("116693703803"); // 116693703803
        Assert.assertEquals("158640389394640",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("524011356"); // 524011356
        BigInteger bigintB=BigInteger.fromString("38849854152087"); // 38849854152087
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("159593870660698583247765875927126222169773997800593"); // 159593870660698583247765875927126222169773997800593
        BigInteger bigintB=BigInteger.fromString("3008411173"); // 3008411173
        Assert.assertEquals("53049221493733158412174889209243813086241",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5066419054698449551449048540017723"); // 5066419054698449551449048540017723
        BigInteger bigintB=BigInteger.fromString("1567838722043577844434294594060549027726032709"); // 1567838722043577844434294594060549027726032709
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("357884610330882921418370231003354527575307580505849"); // 357884610330882921418370231003354527575307580505849
        BigInteger bigintB=BigInteger.fromString("1147048213197651214579552861732507623"); // 1147048213197651214579552861732507623
        Assert.assertEquals("312004854035908",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5004574530168275847144582210218869834404142093598213810365"); // 5004574530168275847144582210218869834404142093598213810365
        BigInteger bigintB=BigInteger.fromString("1110072785772751806957833482770401788000297432978"); // 1110072785772751806957833482770401788000297432978
        Assert.assertEquals("4508330079",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3245070931454459206394365754201511347771752005"); // 3245070931454459206394365754201511347771752005
        BigInteger bigintB=BigInteger.fromString("63080670652168107293266796512"); // 63080670652168107293266796512
        Assert.assertEquals("51443190091431357",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("175567438667021553427570459394505474824505333562065"); // 175567438667021553427570459394505474824505333562065
        BigInteger bigintB=BigInteger.fromString("32842706947203557"); // 32842706947203557
        Assert.assertEquals("5345705484911940673967925214174173",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("229951605017765730803083676441251598183482458154477"); // 229951605017765730803083676441251598183482458154477
        BigInteger bigintB=BigInteger.fromString("539719716586322"); // 539719716586322
        Assert.assertEquals("426057447877184601217786616277765390",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("343403752808197049773777654876884775507365208468026"); // 343403752808197049773777654876884775507365208468026
        BigInteger bigintB=BigInteger.fromString("332893212130196614984733757882935847890"); // 332893212130196614984733757882935847890
        Assert.assertEquals("1031573310283",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5511494026677341415634985968446724133322443473571925388"); // 5511494026677341415634985968446724133322443473571925388
        BigInteger bigintB=BigInteger.fromString("41497884604263396463662"); // 41497884604263396463662
        Assert.assertEquals("132813854952767960998764675332056",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2043040269366376689500009731196494770319189271"); // 2043040269366376689500009731196494770319189271
        BigInteger bigintB=BigInteger.fromString("43282360303688011726841411029"); // 43282360303688011726841411029
        Assert.assertEquals("47202607598834967",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("195"); // 195
        BigInteger bigintB=BigInteger.fromString("31516526691046280511984344266011567050887"); // 31516526691046280511984344266011567050887
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5309143939874190283091201386056576438813819857262420461530"); // 5309143939874190283091201386056576438813819857262420461530
        BigInteger bigintB=BigInteger.fromString("22378"); // 22378
        Assert.assertEquals("237248366246947461037233058631538852391358470697221398",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("22880124487350329541929923819902361028492776441100538645"); // 22880124487350329541929923819902361028492776441100538645
        BigInteger bigintB=BigInteger.fromString("796265645840592396307440"); // 796265645840592396307440
        Assert.assertEquals("28734285607909791896892206783750",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("74511762285418805611812941383187385949926"); // 74511762285418805611812941383187385949926
        BigInteger bigintB=BigInteger.fromString("15552892279479949242399895651963109320094753"); // 15552892279479949242399895651963109320094753
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("182403467740597149874815199886118601806979205515092110443678"); // 182403467740597149874815199886118601806979205515092110443678
        BigInteger bigintB=BigInteger.fromString("17315110863173076661706887244311"); // 17315110863173076661706887244311
        Assert.assertEquals("10534351710594294417108711424",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("37406241181912205961321377044663404419984814022404509968885"); // 37406241181912205961321377044663404419984814022404509968885
        BigInteger bigintB=BigInteger.fromString("13951977907188403732006102860528140701458742"); // 13951977907188403732006102860528140701458742
        Assert.assertEquals("2681070843915226",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("289142254712860864259668184"); // 289142254712860864259668184
        BigInteger bigintB=BigInteger.fromString("4783780951830151831891580563028784451761448015"); // 4783780951830151831891580563028784451761448015
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1093658236800674608065052369573393794957622009577005825367"); // 1093658236800674608065052369573393794957622009577005825367
        BigInteger bigintB=BigInteger.fromString("106909378721172"); // 106909378721172
        Assert.assertEquals("10229768892895922713510820298901827848610184",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("6082932288596298831298695362885685314725466"); // 6082932288596298831298695362885685314725466
        BigInteger bigintB=BigInteger.fromString("86327628482281986370455058"); // 86327628482281986370455058
        Assert.assertEquals("70463331329028331",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("322823922108126801243893093437388113759"); // 322823922108126801243893093437388113759
        BigInteger bigintB=BigInteger.fromString("19262954220460056"); // 19262954220460056
        Assert.assertEquals("16758796102273912611949",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1205732917158739278012416038290776464600373972154271452345327"); // 1205732917158739278012416038290776464600373972154271452345327
        BigInteger bigintB=BigInteger.fromString("9974728006095282298469743581658345978707"); // 9974728006095282298469743581658345978707
        Assert.assertEquals("120878776486130650523",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("19920076596243651490964612415048286847313610"); // 19920076596243651490964612415048286847313610
        BigInteger bigintB=BigInteger.fromString("65543416411"); // 65543416411
        Assert.assertEquals("303921853437296733637374879729906",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("250247722208725936472648447"); // 250247722208725936472648447
        BigInteger bigintB=BigInteger.fromString("20339869879647380228156694837669635607549643"); // 20339869879647380228156694837669635607549643
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3504829794151936084604053621170767976515713781"); // 3504829794151936084604053621170767976515713781
        BigInteger bigintB=BigInteger.fromString("1218027401962696150364744594516557507438851625534496964616608"); // 1218027401962696150364744594516557507438851625534496964616608
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3637971326"); // 3637971326
        BigInteger bigintB=BigInteger.fromString("68947129038936047967537874710427858"); // 68947129038936047967537874710427858
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3688941916124160617360001783931021726112105163027619731837"); // 3688941916124160617360001783931021726112105163027619731837
        BigInteger bigintB=BigInteger.fromString("1279772507360093850535833781219320344226875643393650386"); // 1279772507360093850535833781219320344226875643393650386
        Assert.assertEquals("2882",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("21071068983823519598981663736065226380734163755889602678"); // 21071068983823519598981663736065226380734163755889602678
        BigInteger bigintB=BigInteger.fromString("1506189038594464147908"); // 1506189038594464147908
        Assert.assertEquals("13989657635197295662000068985290550",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5607809409844058771"); // 5607809409844058771
        BigInteger bigintB=BigInteger.fromString("24957117297546576112138778022"); // 24957117297546576112138778022
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("166065118902182782817106713027003"); // 166065118902182782817106713027003
        BigInteger bigintB=BigInteger.fromString("13796818277980056243307326715243"); // 13796818277980056243307326715243
        Assert.assertEquals("12",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2762481558528298370484839373507590"); // 2762481558528298370484839373507590
        BigInteger bigintB=BigInteger.fromString("4200867735839020355580492231888845"); // 4200867735839020355580492231888845
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5816814194974102202"); // 5816814194974102202
        BigInteger bigintB=BigInteger.fromString("17932729764782702990538972279008"); // 17932729764782702990538972279008
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("6548364492226305374889935536268724842275"); // 6548364492226305374889935536268724842275
        BigInteger bigintB=BigInteger.fromString("4568966009106089672830"); // 4568966009106089672830
        Assert.assertEquals("1433226791176649966",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("544621867926128389441117335320905186334540182959"); // 544621867926128389441117335320905186334540182959
        BigInteger bigintB=BigInteger.fromString("186331248793243961871657"); // 186331248793243961871657
        Assert.assertEquals("2922869199092038743392689",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16443434728263632712"); // 16443434728263632712
        BigInteger bigintB=BigInteger.fromString("10497904745276107233552891021171"); // 10497904745276107233552891021171
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("61594581859832105420181386676151336975"); // 61594581859832105420181386676151336975
        BigInteger bigintB=BigInteger.fromString("89606722227973444108100730547875835926716315685712037"); // 89606722227973444108100730547875835926716315685712037
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1290968390359226535245364905020604257442226391644642402087438"); // 1290968390359226535245364905020604257442226391644642402087438
        BigInteger bigintB=BigInteger.fromString("53963239231565447688137107884253754424"); // 53963239231565447688137107884253754424
        Assert.assertEquals("23923107818258673524373",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("262951782155265278979813121426890806419"); // 262951782155265278979813121426890806419
        BigInteger bigintB=BigInteger.fromString("7725527324865430563281640998578094709677"); // 7725527324865430563281640998578094709677
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("124270127530164010733372152835112608197"); // 124270127530164010733372152835112608197
        BigInteger bigintB=BigInteger.fromString("59501056289953530800540363762104909363197"); // 59501056289953530800540363762104909363197
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("18213405451697318796263796268568426895806475740132546070"); // 18213405451697318796263796268568426895806475740132546070
        BigInteger bigintB=BigInteger.fromString("6838816734857859461"); // 6838816734857859461
        Assert.assertEquals("2663239293847793548890486685569655570",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("230041198043007648336262204592890887355"); // 230041198043007648336262204592890887355
        BigInteger bigintB=BigInteger.fromString("257769977626148"); // 257769977626148
        Assert.assertEquals("892428203476215981452473",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("780272345154781542054511498768842320"); // 780272345154781542054511498768842320
        BigInteger bigintB=BigInteger.fromString("106621312834085217926594755732736396099784943795660"); // 106621312834085217926594755732736396099784943795660
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("73094734063383887877752215"); // 73094734063383887877752215
        BigInteger bigintB=BigInteger.fromString("3759503583968112544189435528821"); // 3759503583968112544189435528821
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("21515"); // 21515
        BigInteger bigintB=BigInteger.fromString("2416815728450294363064932650738980251904021802956550126568"); // 2416815728450294363064932650738980251904021802956550126568
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("53482427370820035611948294701976605402522"); // 53482427370820035611948294701976605402522
        BigInteger bigintB=BigInteger.fromString("218278579009809230184466935104817148465"); // 218278579009809230184466935104817148465
        Assert.assertEquals("245",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("60706075568942735064459"); // 60706075568942735064459
        BigInteger bigintB=BigInteger.fromString("444714749921098463700783752758073350914167908"); // 444714749921098463700783752758073350914167908
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("12384484807864121163"); // 12384484807864121163
        BigInteger bigintB=BigInteger.fromString("14871094"); // 14871094
        Assert.assertEquals("832789087868",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("158266131267146125739561"); // 158266131267146125739561
        BigInteger bigintB=BigInteger.fromString("693762235698915116045349"); // 693762235698915116045349
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("262735215813"); // 262735215813
        BigInteger bigintB=BigInteger.fromString("36665857999579023660483633480301806204327"); // 36665857999579023660483633480301806204327
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1039512372418259111966894"); // 1039512372418259111966894
        BigInteger bigintB=BigInteger.fromString("1233064771694900261855686169087799917"); // 1233064771694900261855686169087799917
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("74181116339121465310060830453692695155403"); // 74181116339121465310060830453692695155403
        BigInteger bigintB=BigInteger.fromString("3000881334467585104903507074872625"); // 3000881334467585104903507074872625
        Assert.assertEquals("24719776",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2217994439722542513680067301273611"); // 2217994439722542513680067301273611
        BigInteger bigintB=BigInteger.fromString("74539166234094356577201652510639248985898432445751618"); // 74539166234094356577201652510639248985898432445751618
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3699701275589519267662661039312150"); // 3699701275589519267662661039312150
        BigInteger bigintB=BigInteger.fromString("13162"); // 13162
        Assert.assertEquals("281089596990542415108848278324",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("193207227672154269464941975408616993011037777096092899986917"); // 193207227672154269464941975408616993011037777096092899986917
        BigInteger bigintB=BigInteger.fromString("238303933570081987821640"); // 238303933570081987821640
        Assert.assertEquals("810759708317339283282832614851242275",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("38103327437663520383734285166"); // 38103327437663520383734285166
        BigInteger bigintB=BigInteger.fromString("584631583415"); // 584631583415
        Assert.assertEquals("65174938403243809",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("7108458778420752410513684518295357106611824125925455634"); // 7108458778420752410513684518295357106611824125925455634
        BigInteger bigintB=BigInteger.fromString("4669131397851771878584"); // 4669131397851771878584
        Assert.assertEquals("1522437081486139919311054854998998",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("150"); // 150
        BigInteger bigintB=BigInteger.fromString("19820224147804791149933840147198291114725321897995997472"); // 19820224147804791149933840147198291114725321897995997472
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1584059330689528374960896061303142347450970572031188533468"); // 1584059330689528374960896061303142347450970572031188533468
        BigInteger bigintB=BigInteger.fromString("316240774568376979362217713374489266768294026479801"); // 316240774568376979362217713374489266768294026479801
        Assert.assertEquals("5009029",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("16"); // 16
        BigInteger bigintB=BigInteger.fromString("1242131776085428008401940568162215196"); // 1242131776085428008401940568162215196
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("165"); // 165
        BigInteger bigintB=BigInteger.fromString("354557728201927727667615636827799653172394364815856133381903"); // 354557728201927727667615636827799653172394364815856133381903
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("62701"); // 62701
        BigInteger bigintB=BigInteger.fromString("136422999051250510942512797"); // 136422999051250510942512797
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("4247278008"); // 4247278008
        BigInteger bigintB=BigInteger.fromString("36176725731646"); // 36176725731646
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("23188108553628903734067831385859722107404378591291698"); // 23188108553628903734067831385859722107404378591291698
        BigInteger bigintB=BigInteger.fromString("4962785891194059333454994503525053856989264654"); // 4962785891194059333454994503525053856989264654
        Assert.assertEquals("4672397",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("54236425410213064"); // 54236425410213064
        BigInteger bigintB=BigInteger.fromString("202"); // 202
        Assert.assertEquals("268497155496104",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("50717103400241528726396964552806993945224758583903431"); // 50717103400241528726396964552806993945224758583903431
        BigInteger bigintB=BigInteger.fromString("13311828259023861054348234231469552707507025315180560251"); // 13311828259023861054348234231469552707507025315180560251
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("147"); // 147
        BigInteger bigintB=BigInteger.fromString("37390292104984063426177718588945870166243"); // 37390292104984063426177718588945870166243
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("18198638075943402532436393"); // 18198638075943402532436393
        BigInteger bigintB=BigInteger.fromString("2556057938873536746209061636977502418568"); // 2556057938873536746209061636977502418568
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("50381284620998611669193791272627528684700"); // 50381284620998611669193791272627528684700
        BigInteger bigintB=BigInteger.fromString("72176499123087641196043377127102097127014971787572647"); // 72176499123087641196043377127102097127014971787572647
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("736624634066"); // 736624634066
        BigInteger bigintB=BigInteger.fromString("141648605084997733904060156069071834786989639657089"); // 141648605084997733904060156069071834786989639657089
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("5455079434895491712317329176457250389288017010"); // 5455079434895491712317329176457250389288017010
        BigInteger bigintB=BigInteger.fromString("1369318065037287030552532646487548157153622511627696756255809"); // 1369318065037287030552532646487548157153622511627696756255809
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("939747741807419414225277232907066"); // 939747741807419414225277232907066
        BigInteger bigintB=BigInteger.fromString("148274948217320103555377396"); // 148274948217320103555377396
        Assert.assertEquals("6337872",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1216924945010075477328011172699448176"); // 1216924945010075477328011172699448176
        BigInteger bigintB=BigInteger.fromString("706277040130"); // 706277040130
        Assert.assertEquals("1723013599289711738923792",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("21079793429780053792015827524420051258191034"); // 21079793429780053792015827524420051258191034
        BigInteger bigintB=BigInteger.fromString("174642561428062"); // 174642561428062
        Assert.assertEquals("120702498047494282180214536053",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("433510731176239"); // 433510731176239
        BigInteger bigintB=BigInteger.fromString("5243016"); // 5243016
        Assert.assertEquals("82683465",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("8332990974204162410857915312967116178201633"); // 8332990974204162410857915312967116178201633
        BigInteger bigintB=BigInteger.fromString("758395858352018337836968"); // 758395858352018337836968
        Assert.assertEquals("10987653588076831055",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1335849467255659017211763554025702249129245733402"); // 1335849467255659017211763554025702249129245733402
        BigInteger bigintB=BigInteger.fromString("2082102965"); // 2082102965
        Assert.assertEquals("641586650473675790194056783366475946173",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("263097897148"); // 263097897148
        BigInteger bigintB=BigInteger.fromString("3017342185612940114"); // 3017342185612940114
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("318515663736576193890042035244882992876"); // 318515663736576193890042035244882992876
        BigInteger bigintB=BigInteger.fromString("24027000686358893516553461445"); // 24027000686358893516553461445
        Assert.assertEquals("13256571966",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("47535134382470652672115527243133682864"); // 47535134382470652672115527243133682864
        BigInteger bigintB=BigInteger.fromString("258729474066895878965497049610982623676"); // 258729474066895878965497049610982623676
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("4997468921208199435803128002541820691138220997990751697326"); // 4997468921208199435803128002541820691138220997990751697326
        BigInteger bigintB=BigInteger.fromString("860974446312167882416810563290472989"); // 860974446312167882416810563290472989
        Assert.assertEquals("5804433502775809121531",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("815387634192348456579590"); // 815387634192348456579590
        BigInteger bigintB=BigInteger.fromString("2656571903444917194357263381440033554899180975998999727897"); // 2656571903444917194357263381440033554899180975998999727897
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("42821561037937620075061165631280989283305099039555520"); // 42821561037937620075061165631280989283305099039555520
        BigInteger bigintB=BigInteger.fromString("41232206278118970747040089028818600464741357352005969"); // 41232206278118970747040089028818600464741357352005969
        Assert.assertEquals("1",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2772805862265801115465"); // 2772805862265801115465
        BigInteger bigintB=BigInteger.fromString("249"); // 249
        Assert.assertEquals("11135766515123699258",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("57815042337381104468285030899567347211993584760717"); // 57815042337381104468285030899567347211993584760717
        BigInteger bigintB=BigInteger.fromString("5450934179240667661951986212037231785231487441285446882745"); // 5450934179240667661951986212037231785231487441285446882745
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("962533147027"); // 962533147027
        BigInteger bigintB=BigInteger.fromString("745224029413"); // 745224029413
        Assert.assertEquals("1",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("253582124947043772567698873286238362251"); // 253582124947043772567698873286238362251
        BigInteger bigintB=BigInteger.fromString("67667115039229212560051051679318354628324703869359214"); // 67667115039229212560051051679318354628324703869359214
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("4086906440985531841632425776556359881457485072"); // 4086906440985531841632425776556359881457485072
        BigInteger bigintB=BigInteger.fromString("2569391220"); // 2569391220
        Assert.assertEquals("1590612752613645127047809315918951369",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("25782"); // 25782
        BigInteger bigintB=BigInteger.fromString("47918"); // 47918
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3842178352791645158093493624923294190726187909"); // 3842178352791645158093493624923294190726187909
        BigInteger bigintB=BigInteger.fromString("12561717627938022517577847992896370970305685715305408784"); // 12561717627938022517577847992896370970305685715305408784
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("3963142360949086042919654355783935651034728518349421792688"); // 3963142360949086042919654355783935651034728518349421792688
        BigInteger bigintB=BigInteger.fromString("12233485007024570625426476997451214533282843739492109697"); // 12233485007024570625426476997451214533282843739492109697
        Assert.assertEquals("323",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("1195512227779186966"); // 1195512227779186966
        BigInteger bigintB=BigInteger.fromString("14383756459207206"); // 14383756459207206
        Assert.assertEquals("83",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("224431084455069600136186869141845889069424051468"); // 224431084455069600136186869141845889069424051468
        BigInteger bigintB=BigInteger.fromString("63940683948719852546922096882335314796"); // 63940683948719852546922096882335314796
        Assert.assertEquals("3509988798",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("8837078107083731577"); // 8837078107083731577
        BigInteger bigintB=BigInteger.fromString("1719579539876606440390191501585"); // 1719579539876606440390191501585
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("2254206"); // 2254206
        BigInteger bigintB=BigInteger.fromString("1827195323043701861"); // 1827195323043701861
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("161"); // 161
        BigInteger bigintB=BigInteger.fromString("459637651387259174325713495091156981933296496959"); // 459637651387259174325713495091156981933296496959
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("27508829714570642"); // 27508829714570642
        BigInteger bigintB=BigInteger.fromString("3482771809"); // 3482771809
        Assert.assertEquals("7898544",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("249297765221448246579842718"); // 249297765221448246579842718
        BigInteger bigintB=BigInteger.fromString("38645"); // 38645
        Assert.assertEquals("6450970765207614091857",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("195324370181522"); // 195324370181522
        BigInteger bigintB=BigInteger.fromString("21229302527845005014112453746787451082638333"); // 21229302527845005014112453746787451082638333
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("81745922896545"); // 81745922896545
        BigInteger bigintB=BigInteger.fromString("22300084332340929930207968256768974504695278"); // 22300084332340929930207968256768974504695278
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("29002903688520124466719374346172465914"); // 29002903688520124466719374346172465914
        BigInteger bigintB=BigInteger.fromString("146104173348329689911175914"); // 146104173348329689911175914
        Assert.assertEquals("198508386337",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("416098167006230804796416973822128811395785"); // 416098167006230804796416973822128811395785
        BigInteger bigintB=BigInteger.fromString("303578832470555812257370066425577916588233886696314151577789"); // 303578832470555812257370066425577916588233886696314151577789
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("14339555"); // 14339555
        BigInteger bigintB=BigInteger.fromString("140783451070746485187740502"); // 140783451070746485187740502
        Assert.assertEquals("0",(bigintA.divide(bigintB)).toString()); }
      { BigInteger bigintA=BigInteger.fromString("20269435548453025769754906130865"); // 20269435548453025769754906130865
        BigInteger bigintB=BigInteger.fromString("181800878"); // 181800878
        Assert.assertEquals("111492506369815363431605",(bigintA.divide(bigintB)).toString()); }
    }
    
  }
