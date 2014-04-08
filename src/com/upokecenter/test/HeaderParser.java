package com.upokecenter.test; import com.upokecenter.util.*;

class HeaderParser {
public static int ParseAddrSpec(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseLocalPart(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseDomain(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseAddress(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseGroup(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseAddressList(String str, int index, int endIndex, ITokener tokener) {
 return ParseObsAddrList(str, index, endIndex, tokener);
}

public static int ParseAngleAddr(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseObsRoute(str, index, endIndex, tokener);
 indexTemp = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseAtom(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseAuthresVersion(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
 ++index;
 while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseAuthservId(String str, int index, int endIndex, ITokener tokener) {
 return ParseX400Value(str, index, endIndex, tokener);
}

public static int ParseCFWS(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 for (int i2 = 0;; ++i2) {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp3 = ParseComment(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
  if (i2 < 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
  { indexTemp2 = indexStart2;
} break; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
for (int i = 0;; ++i) {
  indexTemp2 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp2 == index) {
  { if (i < 1) {
  { indexTemp = indexStart;
}
} } break;
  } else {
 index = indexTemp2;
}
}
if (indexTemp2 != indexStart) {
  { indexTemp = indexTemp2;
} break; }
 index = indexStart;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseCcontent(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseComment(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
if (index < endIndex && (str.charAt(index) >= 1 && str.charAt(index) <= 8)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 11 && str.charAt(index) <= 12)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 14 && str.charAt(index) <= 31)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 127)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 128 && str.charAt(index) <= 65535)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 93 && str.charAt(index) <= 126)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 42 && str.charAt(index) <= 91)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <= 39)) {
  { indexTemp++;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseCharset(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) == 63))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) == 63))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseComment(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) == 40)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseCcontent(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 41)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null) {
  if (indexTemp == indexStart) {
 tokener.RestoreState(state);
  } else {
 tokener.Commit(1, indexStart, indexTemp);
}
 }
 return indexTemp;
}

public static int ParseDate(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseDay(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index + 2 < endIndex && (((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 78) || ((str.charAt(index)&~32) == 70 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 66) || ((str.charAt(index)&~32) == 77 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 82) || ((str.charAt(index)&~32) == 65 && (str.charAt(index + 1)&~32) == 80 && (str.charAt(index + 2)&~32) == 82) || ((str.charAt(index)&~32) == 77 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 89) || ((str.charAt(index)&~32) == 74 && (str.charAt(index + 1)&~32) == 85 && (str.charAt(index + 2)&~32) == 78) || ((str.charAt(index)&~32) == 74 && (str.charAt(index + 1)&~32) == 85 && (str.charAt(index + 2)&~32) == 76) || ((str.charAt(index)&~32) == 65 && (str.charAt(index + 1)&~32) == 85 && (str.charAt(index + 2)&~32) == 71) || ((str.charAt(index)&~32) == 83 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 80) || ((str.charAt(index)&~32) == 79 && (str.charAt(index + 1)&~32) == 67 && (str.charAt(index + 2)&~32) == 84) || ((str.charAt(index)&~32) == 78 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 86) || ((str.charAt(index)&~32) == 68 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 67))) { index+=3;
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseYear(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDateTime(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseDayOfWeek(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
 indexTemp = ParseDate(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 indexTemp = ParseTime(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDay(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
for (int i = 0; i < 2; ++i) {
 if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
  ++index;
 } else if (i < 1) {
  indexTemp = indexStart; index = indexStart; break;
 } else {
 break;
}
}
if (index == indexStart) {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDayOfWeek(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && (((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 78) || ((str.charAt(index)&~32) == 84 && (str.charAt(index + 1)&~32) == 85 && (str.charAt(index + 2)&~32) == 69) || ((str.charAt(index)&~32) == 87 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 68) || ((str.charAt(index)&~32) == 84 && (str.charAt(index + 1)&~32) == 72 && (str.charAt(index + 2)&~32) == 85) || ((str.charAt(index)&~32) == 70 && (str.charAt(index + 1)&~32) == 82 && (str.charAt(index + 2)&~32) == 73) || ((str.charAt(index)&~32) == 83 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 84) || ((str.charAt(index)&~32) == 83 && (str.charAt(index + 1)&~32) == 85 && (str.charAt(index + 2)&~32) == 78))) { index+=3;
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDispNotParam(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
if (index + 8 < endIndex && (str.charAt(index) == 61) && (((str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2)&~32) == 69 && (str.charAt(index + 3)&~32) == 81 && (str.charAt(index + 4)&~32) == 85 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 82 && (str.charAt(index + 7)&~32) == 69 && (str.charAt(index + 8)&~32) == 68) || ((str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 80 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 73 && (str.charAt(index + 5)&~32) == 79 && (str.charAt(index + 6)&~32) == 78 && (str.charAt(index + 7)&~32) == 65 && (str.charAt(index + 8)&~32) == 76))) { index+=9;
  } else { indexTemp = indexStart; index = indexStart; break; }
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseValue(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseValue(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDisplayName(String str, int index, int endIndex, ITokener tokener) {
 return ParsePhrase(str, index, endIndex, tokener);
}

public static int ParseDispositionParm(String str, int index, int endIndex, ITokener tokener) {
 return ParseParameter(str, index, endIndex, tokener);
}

public static int ParseDispositionType(String str, int index, int endIndex, ITokener tokener) {
 return ParseToken(str, index, endIndex, tokener);
}

public static int ParseDomain(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseDotAtom(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseObsDomain(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDomainLiteral(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 91)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseDtext(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 93)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDomainName(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseLdhStr(str, index, endIndex, tokener);
 for (int i = 0;; ++i) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index + 1 < endIndex && (str.charAt(index) == 46) && ((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90))) {
  { index += 2;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseLdhStr(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDotAtom(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDotAtomText(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
}
} else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseDtext(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <= 90)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 1 && str.charAt(index) <= 8)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 11 && str.charAt(index) <= 12)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 14 && str.charAt(index) <= 31)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 127)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 94 && str.charAt(index) <= 126)) {
  { indexTemp++;
} break; }
  int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
if (index < endIndex && (str.charAt(index) >= 128 && str.charAt(index) <= 65535)) {
  { indexTemp++;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseExtOctet(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index + 2 < endIndex && (str.charAt(index) == 37) && (((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) && (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70)) || ((str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102) && (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102)) || ((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) && (str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57)))) { index += 3;
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseExtendedInitialName(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
if (index + 1 < endIndex && str.charAt(index) == 42 && str.charAt(index + 1)==48) {
  { index += 2;
} }
if (index < endIndex && (str.charAt(index) == 42)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseExtendedInitialValue(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCharset(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 39)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseLanguageTag(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 39)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2 = ParseExtendedOtherValues(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseExtendedOtherNames(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseOtherSections(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 42)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseExtendedOtherValues(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseExtOctet(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
  { indexTemp2++;
} break; }
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseFWS(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
 for (int i = 0;; ++i) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1)==10) {
  { index += 2;
} }
if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseGroup(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseDisplayName(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 58)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseGroupList(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseGroupList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 for (int i2 = 0;; ++i2) {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp3 = ParseComment(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
  if (i2 < 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
  { indexTemp2 = indexStart2;
} break; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
for (int i = 0;; ++i) {
  indexTemp2 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp2 == index) {
  { if (i < 1) {
  { indexTemp = indexStart;
}
} } break;
  } else {
 index = indexTemp2;
}
}
if (indexTemp2 != indexStart) {
  { indexTemp = indexTemp2;
} break; }
 index = indexStart;
  indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseObsGroupList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderAuthenticationResults(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseAuthservId(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 indexTemp2 = ParseAuthresVersion(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseNoResult(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
for (int i2 = 0;; ++i2) {
  indexTemp3 = ParseResinfo(str, index, endIndex, tokener);
  if (indexTemp3 == index) {
  { if (i2 < 1) {
  { indexTemp2 = indexStart2;
}
} } break;
  } else {
 index = indexTemp3;
}
}
if (indexTemp3 != indexStart) {
  { indexTemp2 = indexTemp3;
} break; }
 index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderAutoSubmitted(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2 = ParseOptParameterList(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderBase(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderContentBase(str, index, endIndex, tokener);
}

public static int ParseHeaderBcc(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseAddressList(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
  indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderCc(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderContentBase(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderContentDisposition(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseDispositionType(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseDispositionParm(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderContentId(String str, int index, int endIndex, ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHeaderContentLocation(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderContentType(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseRestrictedName(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 47)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseRestrictedName(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseParameter(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderDate(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderDeferredDelivery(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderDeliveryDate(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderDispositionNotificationOptions(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseDispNotParam(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseDispNotParam(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderDispositionNotificationTo(String str, int index, int endIndex, ITokener tokener) {
 return ParseMailboxList(str, index, endIndex, tokener);
}

public static int ParseHeaderEncrypted(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseWord(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseWord(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderExpires(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderExpiryDate(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderFrom(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAddressList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderInReplyTo(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 for (int i = 0;; ++i) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseIdLeft(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseIdRight(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderKeywords(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParsePhrase(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParsePhrase(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderLanguage(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index + 1 < endIndex && (((str.charAt(index) >= 65 && str.charAt(index) <= 90) && (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90)) || ((str.charAt(index) >= 97 && str.charAt(index) <= 122) && (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122)))) { index += 2;
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
  { index++;
} }
do {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) == 40)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 indexTemp3 = ParseLanguageDescription(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
if (index < endIndex && (str.charAt(index) == 41)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
 break;
}
} while (false);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderLatestDeliveryTime(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderListArchive(String str, int index, int endIndex, ITokener tokener) {
 return ParseListHeaderUrlList(str, index, endIndex, tokener);
}

public static int ParseHeaderListHelp(String str, int index, int endIndex, ITokener tokener) {
 return ParseListHeaderUrlList(str, index, endIndex, tokener);
}

public static int ParseHeaderListId(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParsePhrase(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
  indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseFWS(str, index, endIndex, tokener);
do {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
  int indexTemp4;
  indexTemp4 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp4 != index) {
  { indexTemp3 = indexTemp4;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 76 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 67 && (str.charAt(index + 3)&~32) == 65 && (str.charAt(index + 4)&~32) == 76 && (str.charAt(index + 5)&~32) == 72 && (str.charAt(index + 6)&~32) == 79 && (str.charAt(index + 7)&~32) == 83 && (str.charAt(index + 8)&~32) == 84) {
  { indexTemp3 += 9;
} break; }
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else { index = indexStart2; break; }
} while (false);
 if (index == indexStart2) {
  { indexTemp2 = indexStart2;
} break; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderListOwner(String str, int index, int endIndex, ITokener tokener) {
 return ParseListHeaderUrlList(str, index, endIndex, tokener);
}

public static int ParseHeaderListPost(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 79) {
  { index += 2;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
  indexTemp2 = ParseListHeaderUrlList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderListSubscribe(String str, int index, int endIndex, ITokener tokener) {
 return ParseListHeaderUrlList(str, index, endIndex, tokener);
}

public static int ParseHeaderListUnsubscribe(String str, int index, int endIndex, ITokener tokener) {
 return ParseListHeaderUrlList(str, index, endIndex, tokener);
}

public static int ParseHeaderMessageId(String str, int index, int endIndex, ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHeaderMmhsCopyPrecedence(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp = ParsePrecedence(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderMmhsExemptedAddress(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp = ParseAddressList(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderMmhsExtendedAuthorisationInfo(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp = ParseDateTime(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderMmhsMessageType(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 int indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
  { index++;
} }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
if (index < endIndex && (str.charAt(index) == 48)) {
  { indexTemp2++;
} break; }
if (index + 4 < endIndex && (str.charAt(index) & ~32) == 68 && (str.charAt(index + 1)&~32) == 82 && (str.charAt(index + 2)&~32) == 73 && (str.charAt(index + 3)&~32) == 76 && (str.charAt(index + 4)&~32) == 76) {
  { indexTemp2 += 5;
} break; }
if (index + 6 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 82 && (str.charAt(index + 2)&~32) == 79 && (str.charAt(index + 3)&~32) == 74 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 67 && (str.charAt(index + 6)&~32) == 84) {
  { indexTemp2 += 7;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 79 && (str.charAt(index + 1)&~32) == 80 && (str.charAt(index + 2)&~32) == 69 && (str.charAt(index + 3)&~32) == 82 && (str.charAt(index + 4)&~32) == 65 && (str.charAt(index + 5)&~32) == 84 && (str.charAt(index + 6)&~32) == 73 && (str.charAt(index + 7)&~32) == 79 && (str.charAt(index + 8)&~32) == 78) {
  { indexTemp2 += 9;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1)&~32) == 88 && (str.charAt(index + 2)&~32) == 69 && (str.charAt(index + 3)&~32) == 82 && (str.charAt(index + 4)&~32) == 67 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 83 && (str.charAt(index + 7)&~32) == 69) {
  { indexTemp2 += 8;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseMessageTypeParam(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderMmhsPrimaryPrecedence(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp = ParsePrecedence(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderObsoletes(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseMsgId(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseMsgId(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderOriginalFrom(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderFrom(str, index, endIndex, tokener);
}

public static int ParseHeaderOriginalMessageId(String str, int index, int endIndex, ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHeaderOriginalRecipient(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseAtom(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
while (index < endIndex && ((str.charAt(index) >= 1 && str.charAt(index) <= 9) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 65535))) {
  { index++;
} }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderReceived(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2 = ParseReceivedToken(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseDateTime(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderReceivedSpf(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 83 && (str.charAt(index + 3)&~32) == 83) {
  { indexTemp2 += 4;
} break; }
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 73 && (str.charAt(index + 3)&~32) == 76) {
  { indexTemp2 += 4;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 70 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 70 && (str.charAt(index + 5)&~32) == 65 && (str.charAt(index + 6)&~32) == 73 && (str.charAt(index + 7)&~32) == 76) {
  { indexTemp2 += 8;
} break; }
if (index + 6 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 85 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 82 && (str.charAt(index + 5)&~32) == 65 && (str.charAt(index + 6)&~32) == 76) {
  { indexTemp2 += 7;
} break; }
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 78 && (str.charAt(index + 3)&~32) == 69) {
  { indexTemp2 += 4;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 84 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 77 && (str.charAt(index + 3)&~32) == 80 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 82 && (str.charAt(index + 7)&~32) == 79 && (str.charAt(index + 8)&~32) == 82) {
  { indexTemp2 += 9;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 82 && (str.charAt(index + 3)&~32) == 77 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 82 && (str.charAt(index + 7)&~32) == 79 && (str.charAt(index + 8)&~32) == 82) {
  { indexTemp2 += 9;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 for (int i = 0;; ++i) {
  int indexTemp2 = ParseFWS(str, index, endIndex, tokener);  // SEQUENCE FWS 1 -1
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseComment(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 for (int i2 = 0;; ++i2) {
  int indexTemp3 = ParseFWS(str, index, endIndex, tokener);  // SEQUENCE FWS 1 -1
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
  if (i2 < 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
  { indexTemp2 = indexStart2;
} break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
 index = ParseKeyValueList(str, index, endIndex, tokener);
if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1)==10) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderReferences(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderInReplyTo(str, index, endIndex, tokener);
}

public static int ParseHeaderReplyBy(String str, int index, int endIndex, ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderReplyTo(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderResentBcc(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderBcc(str, index, endIndex, tokener);
}

public static int ParseHeaderResentCc(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderResentDate(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderDate(str, index, endIndex, tokener);
}

public static int ParseHeaderResentFrom(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderFrom(str, index, endIndex, tokener);
}

public static int ParseHeaderResentMessageId(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderMessageId(str, index, endIndex, tokener);
}

public static int ParseHeaderResentReplyTo(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderResentSender(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderSender(str, index, endIndex, tokener);
}

public static int ParseHeaderResentTo(String str, int index, int endIndex, ITokener tokener) {
 return ParseHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderReturnPath(String str, int index, int endIndex, ITokener tokener) {
 return ParsePath(str, index, endIndex, tokener);
}

public static int ParseHeaderSender(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseGroup(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderTo(String str, int index, int endIndex, ITokener tokener) {
 return ParseAddressList(str, index, endIndex, tokener);
}

public static int ParseHeaderX400Originator(String str, int index, int endIndex, ITokener tokener) {
 return ParseMailbox(str, index, endIndex, tokener);
}

public static int ParseHeaderX400Recipients(String str, int index, int endIndex, ITokener tokener) {
 return ParseMailboxList(str, index, endIndex, tokener);
}

public static int ParseHeaderXMittente(String str, int index, int endIndex, ITokener tokener) {
 return ParseMailbox(str, index, endIndex, tokener);
}

public static int ParseHeaderXRiferimentoMessageId(String str, int index, int endIndex, ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHour(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseIdLeft(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseWord(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 while (true) {
  int indexTemp3;
  int state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 indexTemp3 = ParseWord(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
 index = indexTemp3;
  } else if (tokener != null) {
  { tokener.RestoreState(state3);
} break;
  } else {
 break;
}
 }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
  indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseIdRight(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDotAtom(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseObsDomain(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseNoFoldLiteral(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseKey(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 95) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
} }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseKeyValueList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseKeyValuePair(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseKeyValuePair(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
} }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseKeyValuePair(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseKey(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseDotAtom(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
  indexTemp3 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseLanguageDescription(String str, int index, int endIndex, ITokener tokener) {
 return ParsePrintablestring(str, index, endIndex, tokener);
}

public static int ParseLanguageTag(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
} }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseLdhStr(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
} }
if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseListHeaderUrl(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseListHeaderUrlList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseListHeaderUrl(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseListHeaderUrl(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseLocalPart(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseWord(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 while (true) {
  int indexTemp3;
  int state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 indexTemp3 = ParseWord(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
 index = indexTemp3;
  } else if (tokener != null) {
  { tokener.RestoreState(state3);
} break;
  } else {
 break;
}
 }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
  indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMailbox(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMailboxList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 indexTemp = ParseMailbox(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
do {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
  int indexTemp4;
  indexTemp4 = ParseMailbox(str, index, endIndex, tokener);
 if (indexTemp4 != index) {
  { indexTemp3 = indexTemp4;
} break; }
  indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp4 != index) {
  { indexTemp3 = indexTemp4;
} break; }
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
 break;
}
} while (false);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMessageTypeParam(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index + 9 < endIndex && (str.charAt(index) & ~32) == 73 && (str.charAt(index + 1)&~32) == 68 && (str.charAt(index + 2)&~32) == 69 && (str.charAt(index + 3)&~32) == 78 && (str.charAt(index + 4)&~32) == 84 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 70 && (str.charAt(index + 7)&~32) == 73 && (str.charAt(index + 8)&~32) == 69 && (str.charAt(index + 9)&~32) == 82) {
  { index += 10;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp = ParseQuotedMilitaryString(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMethod(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseLdhStr(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 47)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseMethodVersion(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMethodVersion(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
 ++index;
 while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMethodspec(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseMethod(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseResult(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMinute(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseMsgId(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseIdLeft(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseIdRight(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseNameAddr(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseDisplayName(str, index, endIndex, tokener);
 indexTemp = ParseAngleAddr(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseNoFoldLiteral(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) == 91)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2 = ParseDtext(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
if (index < endIndex && (str.charAt(index) == 93)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseNoResult(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 78 && (str.charAt(index + 3)&~32) == 69) {
  { index += 4;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseObsAddrList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 indexTemp = ParseAddress(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
do {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
  int indexTemp4;
  indexTemp4 = ParseAddress(str, index, endIndex, tokener);
 if (indexTemp4 != index) {
  { indexTemp3 = indexTemp4;
} break; }
  indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp4 != index) {
  { indexTemp3 = indexTemp4;
} break; }
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
 break;
}
} while (false);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseObsDomain(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseAtom(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseAtom(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseObsDomainList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
if (index < endIndex && (str.charAt(index) == 44)) {
  { indexTemp2++;
} break; }
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseDomain(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 indexTemp3 = ParseDomain(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
 break;
}
} while (false);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseObsGroupList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 for (int i = 0;; ++i) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 44)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseObsRoute(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseObsDomainList(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 58)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseOptParameterList(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseParameter(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseOtherSections(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index + 1 < endIndex && (str.charAt(index) == 42) && (str.charAt(index + 1) >= 49 && str.charAt(index + 1) <= 57)) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
  { index++;
} }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseParameter(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 int indexTemp3;
  int state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 int indexStart3 = index;
 indexTemp3 = ParseExtendedInitialName(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 indexTemp3 = ParseExtendedInitialValue(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state3);
}
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 int indexStart3 = index;
 indexTemp3 = ParseExtendedOtherNames(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
 while (true) {
  int indexTemp4;
  indexTemp4 = ParseExtendedOtherValues(str, index, endIndex, tokener);
  if (indexTemp4 != index) {
 index = indexTemp4;
  } else {
 break;
}
 }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state3);
}
  indexTemp3 = ParseRegularParameter(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePath(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 60)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 62)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePhrase(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParsePhraseWord(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParsePhraseWord(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
if (index < endIndex && (str.charAt(index) == 46)) {
  { indexTemp2++;
} break; }
  indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null) {
  if (indexTemp == indexStart) {
 tokener.RestoreState(state);
  } else {
 tokener.Commit(2, indexStart, indexTemp);
}
 }
 return indexTemp;
}

public static int ParsePhraseAtom(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) >= 128 && str.charAt(index) <= 65535) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null) {
  if (indexTemp == indexStart) {
 tokener.RestoreState(state);
  } else {
 tokener.Commit(3, indexStart, indexTemp);
}
 }
 return indexTemp;
}

public static int ParsePhraseWord(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp2 = ParsePhraseAtom(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
  indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePrecedence(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 int indexTemp3 = index;
 do {
 int indexStart3 = index;
if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
  { index++;
}
  } else { indexTemp3 = indexStart3; index = indexStart3; break; }
while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
  { index++;
} }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
if (index < endIndex && (str.charAt(index) == 48)) {
  { indexTemp2++;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 79 && (str.charAt(index + 1)&~32) == 86 && (str.charAt(index + 2)&~32) == 69 && (str.charAt(index + 3)&~32) == 82 && (str.charAt(index + 4)&~32) == 82 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 68 && (str.charAt(index + 7)&~32) == 69) {
  { indexTemp2 += 8;
} break; }
if (index + 4 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1)&~32) == 76 && (str.charAt(index + 2)&~32) == 65 && (str.charAt(index + 3)&~32) == 83 && (str.charAt(index + 4)&~32) == 72) {
  { indexTemp2 += 5;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 73 && (str.charAt(index + 1)&~32) == 77 && (str.charAt(index + 2)&~32) == 77 && (str.charAt(index + 3)&~32) == 69 && (str.charAt(index + 4)&~32) == 68 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 65 && (str.charAt(index + 7)&~32) == 84 && (str.charAt(index + 8)&~32) == 69) {
  { indexTemp2 += 9;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 82 && (str.charAt(index + 2)&~32) == 73 && (str.charAt(index + 3)&~32) == 79 && (str.charAt(index + 4)&~32) == 82 && (str.charAt(index + 5)&~32) == 73 && (str.charAt(index + 6)&~32) == 84 && (str.charAt(index + 7)&~32) == 89) {
  { indexTemp2 += 8;
} break; }
if (index + 6 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 85 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 73 && (str.charAt(index + 5)&~32) == 78 && (str.charAt(index + 6)&~32) == 69) {
  { indexTemp2 += 7;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 68 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 70 && (str.charAt(index + 3)&~32) == 69 && (str.charAt(index + 4)&~32) == 82 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 69 && (str.charAt(index + 7)&~32) == 68) {
  { indexTemp2 += 8;
} break; }
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePrintablestring(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) >= 65 && str.charAt(index) <= 90)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) >= 97 && str.charAt(index) <= 122)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) == 32)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) == 39)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) >= 43 && str.charAt(index) <= 58)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) == 61)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) == 63)) {
  { indexTemp2++;
} break; }
if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
  { indexTemp2++;
} break; }
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseProperty(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseLdhStr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
if (index + 5 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1)&~32) == 67 && (str.charAt(index + 2)&~32) == 80 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 84 && (str.charAt(index + 5)&~32) == 79) {
  { indexTemp += 6;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 73 && (str.charAt(index + 3)&~32) == 76 && (str.charAt(index + 4)&~32) == 70 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 79 && (str.charAt(index + 7)&~32) == 77) {
  { indexTemp += 8;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePropspec(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParsePtype(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 46)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 indexTemp = ParseProperty(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParsePvalue(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePsChar(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) >= 65 && str.charAt(index) <= 90)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 97 && str.charAt(index) <= 122)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 32)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 39)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 43 && str.charAt(index) <= 58)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 61)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 63)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
  { indexTemp++;
} break; }
 } while (false);
 return indexTemp;
}

public static int ParsePtype(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1)&~32) == 77 && (str.charAt(index + 2)&~32) == 84 && (str.charAt(index + 3)&~32) == 80) {
  { indexTemp += 4;
} break; }
if (index + 5 < endIndex && (str.charAt(index) & ~32) == 72 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 65 && (str.charAt(index + 3)&~32) == 68 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 82) {
  { indexTemp += 6;
} break; }
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 66 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 68 && (str.charAt(index + 3)&~32) == 89) {
  { indexTemp += 4;
} break; }
if (index + 5 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 76 && (str.charAt(index + 3)&~32) == 73 && (str.charAt(index + 4)&~32) == 67 && (str.charAt(index + 5)&~32) == 89) {
  { indexTemp += 6;
} break; }
 } while (false);
 return indexTemp;
}

public static int ParsePvalue(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 int indexTemp3 = index;
 // Unlimited production in choice
 index = indexStart2;
  int state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 int indexStart3 = index;
do {
  int indexTemp4;
 indexTemp4 = index;
 do {
 int indexStart4 = index;
 index = ParseLocalPart(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 64)) {
  { index++;
}
  } else { indexTemp4 = indexStart4; index = indexStart4; break; }
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
  if (indexTemp4 != index) {
  { index = indexTemp4;
}
  } else {
 break;
}
} while (false);
 indexTemp3 = ParseDomainName(str, index, endIndex, tokener);
 if (indexTemp3 == index) {
  { indexTemp3 = indexStart3;
} index = indexStart3; break;
  } else { index = indexTemp3; }
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state3);
}
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else { index = indexStart; break; }
} while (false);
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseQcontent(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
if (index < endIndex && (str.charAt(index) >= 1 && str.charAt(index) <= 8)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 11 && str.charAt(index) <= 12)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 14 && str.charAt(index) <= 31)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 127)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 128 && str.charAt(index) <= 65535)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 93 && str.charAt(index) <= 126)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 35 && str.charAt(index) <= 91)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 33)) {
  { indexTemp++;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseQuotedMilitaryString(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) == 34)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 for (int i = 0; i < 69; ++i) {
  int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);  // CHOICE ps-char 1 69
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
  if (i < 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
  { indexTemp = indexStart;
} break; }
if (index < endIndex && (str.charAt(index) == 34)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseQuotedPair(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index + 1 < endIndex && (str.charAt(index) == 92) && (str.charAt(index + 1) >= 0 && str.charAt(index + 1) <= 65535)) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseQuotedString(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 34)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 indexTemp2 = ParseQcontent(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 34)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseReasonspec(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index + 5 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 65 && (str.charAt(index + 3)&~32) == 83 && (str.charAt(index + 4)&~32) == 79 && (str.charAt(index + 5)&~32) == 78) {
  { index += 6;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
 while (true) {
  int indexTemp2 = ParseStdPrintablestring(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseReceivedToken(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDotAtom(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseObsDomain(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseAtom(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseRegularParameter(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseRegularParameterName(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 61)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseValue(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseRegularParameterName(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseSection(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseResinfo(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str.charAt(index) == 59)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseMethodspec(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 indexTemp2 = ParseReasonspec(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 indexTemp2 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
 indexTemp2 = ParsePropspec(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseRestrictedName(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
for (int i = 0; i < 126; ++i) {
 if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) >= 94 && str.charAt(index) <= 95) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 43) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
  ++index;
 } else {
 break;
}
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseResult(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 83 && (str.charAt(index + 3)&~32) == 83) {
  { indexTemp += 4;
} break; }
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1)&~32) == 65 && (str.charAt(index + 2)&~32) == 73 && (str.charAt(index + 3)&~32) == 76) {
  { indexTemp += 4;
} break; }
if (index + 7 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 70 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 70 && (str.charAt(index + 5)&~32) == 65 && (str.charAt(index + 6)&~32) == 73 && (str.charAt(index + 7)&~32) == 76) {
  { indexTemp += 8;
} break; }
if (index + 6 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 85 && (str.charAt(index + 3)&~32) == 84 && (str.charAt(index + 4)&~32) == 82 && (str.charAt(index + 5)&~32) == 65 && (str.charAt(index + 6)&~32) == 76) {
  { indexTemp += 7;
} break; }
if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1)&~32) == 79 && (str.charAt(index + 2)&~32) == 78 && (str.charAt(index + 3)&~32) == 69) {
  { indexTemp += 4;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 84 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 77 && (str.charAt(index + 3)&~32) == 80 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 82 && (str.charAt(index + 7)&~32) == 79 && (str.charAt(index + 8)&~32) == 82) {
  { indexTemp += 9;
} break; }
if (index + 8 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 69 && (str.charAt(index + 2)&~32) == 82 && (str.charAt(index + 3)&~32) == 77 && (str.charAt(index + 4)&~32) == 69 && (str.charAt(index + 5)&~32) == 82 && (str.charAt(index + 6)&~32) == 82 && (str.charAt(index + 7)&~32) == 79 && (str.charAt(index + 8)&~32) == 82) {
  { indexTemp += 9;
} break; }
 } while (false);
 return indexTemp;
}

public static int ParseSecond(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseSection(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index + 1 < endIndex && str.charAt(index) == 42 && str.charAt(index + 1)==48) {
  { indexTemp += 2;
} break; }
  int indexTemp2 = ParseOtherSections(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseStdChar(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) >= 65 && str.charAt(index) <= 90)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 32)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 39)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 97 && str.charAt(index) <= 123)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 58)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 125)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) == 63)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 42 && str.charAt(index) <= 46)) {
  { indexTemp++;
} break; }
 } while (false);
 return indexTemp;
}

public static int ParseStdPair(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && (str.charAt(index) == 36)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParsePsChar(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseStdPrintablestring(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 while (true) {
  int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
  int indexTemp3 = ParseStdChar(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
  indexTemp3 = ParseStdPair(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
  { indexTemp2 = indexTemp3;
} break; }
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
  } else if (tokener != null) {
  { tokener.RestoreState(state2);
} break;
  } else {
 break;
}
 }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseTime(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseTimeOfDay(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
 indexTemp = ParseZone(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseTimeOfDay(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 indexTemp = ParseHour(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
if (index < endIndex && (str.charAt(index) == 58)) {
  { index++;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
 indexTemp = ParseMinute(str, index, endIndex, tokener);
 if (indexTemp == index) {
  { indexTemp = indexStart;
} index = indexStart; break;
  } else { index = indexTemp; }
do {
  int indexTemp2 = index;
 do {
 int indexStart2 = index;
if (index < endIndex && (str.charAt(index) == 58)) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
 indexTemp2 = ParseSecond(str, index, endIndex, tokener);
 if (indexTemp2 == index) {
  { indexTemp2 = indexStart2;
} index = indexStart2; break;
  } else { index = indexTemp2; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
  { index = indexTemp2;
}
  } else {
 break;
}
} while (false);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseToken(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
 ++index;
 while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
 ++index;
}
} else { indexTemp = indexStart; index = indexStart; break; }
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseValue(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
 ++indexTemp;
 while (indexTemp < endIndex && ((str.charAt(indexTemp) == 33) || (str.charAt(indexTemp) >= 35 && str.charAt(indexTemp) <= 36) || (str.charAt(indexTemp) >= 45 && str.charAt(indexTemp) <= 46) || (str.charAt(indexTemp) >= 65 && str.charAt(indexTemp) <= 90) || (str.charAt(indexTemp) >= 48 && str.charAt(indexTemp) <= 57) || (str.charAt(indexTemp) >= 94 && str.charAt(indexTemp) <= 126) || (str.charAt(indexTemp) >= 42 && str.charAt(indexTemp) <= 43) || (str.charAt(indexTemp) >= 38 && str.charAt(indexTemp) <= 39) || (str.charAt(indexTemp) == 63))) {
 ++indexTemp;
}
 break;
}
  int indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseWord(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
  int indexTemp2 = ParseAtom(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
  indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseX400Value(String str, int index, int endIndex, ITokener tokener) {
 return ParseStdPrintablestring(str, index, endIndex, tokener);
}

public static int ParseYear(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
  { index += 2;
}
  } else { indexTemp = indexStart; index = indexStart; break; }
while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
  { index++;
} }
 index = ParseCFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseZone(String str, int index, int endIndex, ITokener tokener) {
int indexStart = index;
 int state = (tokener != null) ? tokener.GetState() : 0;
 int indexTemp = index;
 do {
 int indexTemp2;
  int state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 int indexStart2 = index;
 for (int i2 = 0;; ++i2) {
  int indexTemp3 = ParseFWS(str, index, endIndex, tokener);  // SEQUENCE FWS 1 -1
  if (indexTemp3 != index) {
  { index = indexTemp3;
}
  } else {
  if (i2 < 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
  { indexTemp2 = indexStart2;
} break; }
if (index < endIndex && ((str.charAt(index) == 43) || (str.charAt(index) == 45))) {
  { index++;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
if (index + 3 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 3) >= 48 && str.charAt(index + 3) <= 57))) {
  { index += 4;
}
  } else { indexTemp2 = indexStart2; index = indexStart2; break; }
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
  { indexTemp = indexTemp2;
} break; }
 if (tokener != null) {
 tokener.RestoreState(state2);
}
if (index + 1 < endIndex && (str.charAt(index) & ~32) == 85 && (str.charAt(index + 1)&~32) == 84) {
  { indexTemp += 2;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 71 && (str.charAt(index + 1)&~32) == 77 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1)&~32) == 83 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1)&~32) == 68 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 && (str.charAt(index + 1)&~32) == 83 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 && (str.charAt(index + 1)&~32) == 68 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1)&~32) == 83 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1)&~32) == 68 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 83 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1)&~32) == 68 && (str.charAt(index + 2)&~32) == 84) {
  { indexTemp += 3;
} break; }
if (index < endIndex && (str.charAt(index) >= 65 && str.charAt(index) <= 73)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 75 && str.charAt(index) <= 90)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 97 && str.charAt(index) <= 105)) {
  { indexTemp++;
} break; }
if (index < endIndex && (str.charAt(index) >= 107 && str.charAt(index) <= 122)) {
  { indexTemp++;
} break; }
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}
}
