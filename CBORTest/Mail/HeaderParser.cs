using System;

namespace PeterO.Mail {
  internal class HeaderParser {
    public static int ParseAddrSpec(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseLocalPart(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAddress(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseGroup(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAddressList(string str, int index, int endIndex, ITokener tokener) {
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
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = ParseAddress(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                int indexTemp4;
                indexTemp4 = ParseAddress(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseAngleAddr(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseObsRoute(str, index, endIndex, tokener);
        indexTemp = ParseAddrSpec(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAtext(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 33) || (str[index] == 35) || (str[index] == 36) || (str[index] == 37) || (str[index] == 38) || (str[index] == 39) || (str[index] == 42) || (str[index] == 43) || (str[index] == 45) || (str[index] == 47) || (str[index] == 61) || (str[index] == 63) || (str[index] == 94) || (str[index] == 95) || (str[index] == 96) || (str[index] == 123) || (str[index] == 124) || (str[index] == 125) || (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          ++indexTemp; break;
        }
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
            ++index;
          } else {
            break;
          }
          if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] == 126) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseAtom(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
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

    public static int ParseAuthresVersion(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
          ++index;
          while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
            ++index;
          }
        } else {
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

    public static int ParseAuthservId(string str, int index, int endIndex, ITokener tokener) {
      return ParseX400Value(str, index, endIndex, tokener);
    }

    public static int ParseCFWS(string str, int index, int endIndex, ITokener tokener) {
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
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        for (int i = 0;; ++i) {
          indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
        index = indexStart;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseCcontent(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          ++indexTemp; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
            ++index;
          } else {
            break;
          }
          if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127))) {
          ++indexTemp; break;
        }
        indexTemp2 = ParseComment(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] >= 93 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 91) || (str[index] >= 33 && str[index] <= 39))) {
          ++indexTemp; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseCharset(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] == 45) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] == 63))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] == 45) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] == 63))) {
            ++index;
          }
        } else {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseComment(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] == 40)) {
          ++index;
        } else {
          break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseCcontent(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 41)) {
          ++index;
        } else {
          index = indexStart; break;
        }
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

    public static int ParseDate(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseDay(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index + 2 < endIndex && (((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 70 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 66) || ((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 82) || ((str[index] & ~32) == 65 && (str[index + 1] & ~32) == 80 && (str[index + 2] & ~32) == 82) || ((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 89) || ((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 76) || ((str[index] & ~32) == 65 && (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 71) || ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 80) || ((str[index] & ~32) == 79 && (str[index + 1] & ~32) == 67 && (str[index + 2] & ~32) == 84) || ((str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 86) || ((str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 67))) {
          index += 3;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseYear(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDateTime(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = ParseDate(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = ParseTime(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDay(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 2; ++i) {
          if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
            ++index;
          } else if (i < 1) {
            index = indexStart; break;
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

    public static int ParseDayOfWeek(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 2 < endIndex && (((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 84 && (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 69) || ((str[index] & ~32) == 87 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 68) || ((str[index] & ~32) == 84 && (str[index + 1] & ~32) == 72 && (str[index + 2] & ~32) == 85) || ((str[index] & ~32) == 70 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73) || ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 84) || ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 78))) {
          index += 3;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDispNotParam(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        if (index + 8 < endIndex && (str[index] == 61) && (((str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 81 && (str[index + 4] & ~32) == 85 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 69 && (str[index + 8] & ~32) == 68) || ((str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 80 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 73 && (str[index + 5] & ~32) == 79 && (str[index + 6] & ~32) == 78 && (str[index + 7] & ~32) == 65 && (str[index + 8] & ~32) == 76))) {
          index += 9;
        } else {
          index = indexStart; break;
        }
        if (index < endIndex && (str[index] == 44)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseValue(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseValue(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseDisplayName(string str, int index, int endIndex, ITokener tokener) {
      return ParsePhrase(str, index, endIndex, tokener);
    }

    public static int ParseDispositionParm(string str, int index, int endIndex, ITokener tokener) {
      return ParseParameter(str, index, endIndex, tokener);
    }

    public static int ParseDispositionType(string str, int index, int endIndex, ITokener tokener) {
      return ParseToken(str, index, endIndex, tokener);
    }

    public static int ParseDomain(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          indexTemp2 = ParseAtom(str, index, endIndex, tokener);
          if (indexTemp2 == index) {
            indexTemp2 = indexStart2; break;
          } else {
            index = indexTemp2;
          }
          while (true) {
            int indexTemp3;
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str[index] == 46)) {
                ++index;
              } else {
                break;
              }
              indexTemp3 = ParseAtom(str, index, endIndex, tokener);
              if (indexTemp3 == index) {
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else if (tokener != null) {
              tokener.RestoreState(state3); break;
            } else {
              break;
            }
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(9, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseDomainLiteral(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 91)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseDtext(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 93)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDomainName(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        } else {
          break;
        }
        index = ParseLdhStr(str, index, endIndex, tokener);
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && (str[index] == 46) && ((str[index + 1] >= 48 && str[index + 1] <= 57) || (str[index + 1] >= 97 && str[index + 1] <= 122) || (str[index + 1] >= 65 && str[index + 1] <= 90))) {
              index += 2;
            } else {
              break;
            }
            index = ParseLdhStr(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDomainNoCfws(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] == 91)) {
            ++index;
          } else {
            break;
          }
          while (true) {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && ((str[index] >= 33 && str[index] <= 90) || (str[index] >= 94 && str[index] <= 126))) {
                ++indexTemp3; break;
              }
              int indexTemp4;
              indexTemp4 = index;
              do {
                int indexStart4 = index;
                if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                  ++indexTemp4; break;
                }
                int indexTemp5;
                indexTemp5 = index;
                do {
                  int indexStart5 = index;
                  if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
                    ++index;
                  } else {
                    break;
                  }
                  if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
                    ++index;
                  } else {
                    index = indexStart5; break;
                  }
                  indexTemp5 = index;
                  index = indexStart5;
                } while (false);
                if (indexTemp5 != index) {
                  indexTemp4 = indexTemp5; break;
                }
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              break;
            }
          }
          if (index < endIndex && (str[index] == 93)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDotAtom(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDotAtomText(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 46)) {
              ++index;
            } else {
              break;
            }
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseAtext(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseDtext(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 33 && str[index] <= 90) || (str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          ++indexTemp; break;
        }
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
            ++index;
          } else {
            break;
          }
          if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseExtOctet(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index + 2 < endIndex && (str[index] == 37) && (((str[index + 1] >= 65 && str[index + 1] <= 70) && (str[index + 2] >= 65 && str[index + 2] <= 70)) || ((str[index + 1] >= 97 && str[index + 1] <= 102) && (str[index + 2] >= 97 && str[index + 2] <= 102)) || ((str[index + 1] >= 48 && str[index + 1] <= 57) && (str[index + 2] >= 48 && str[index + 2] <= 57)))) {
          index += 3;
        } else {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseExtendedInitialName(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
            ++index;
          }
        } else {
          break;
        }
        if (index + 1 < endIndex && str[index] == 42 && str[index + 1] == 48) {
          index += 2;
        }
        if (index < endIndex && (str[index] == 42)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseExtendedInitialValue(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCharset(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 39)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseLanguageTag(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 39)) {
          ++index;
        } else {
          index = indexStart; break;
        }
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

    public static int ParseExtendedOtherNames(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
            ++index;
          }
        } else {
          break;
        }
        indexTemp = ParseOtherSections(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 42)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseExtendedOtherValues(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseFWS(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
              index += 2;
            }
            if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseGroup(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseDisplayName(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseGroupList(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(5, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseGroupList(string str, int index, int endIndex, ITokener tokener) {
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
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        for (int i = 0;; ++i) {
          indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
        index = indexStart;
        indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseObsGroupList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAuthenticationResults(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseAuthservId(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            indexTemp2 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = ParseAuthresVersion(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int indexTemp3 = ParseNoResult(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            for (int i2 = 0;; ++i2) {
              indexTemp3 = ParseResinfo(str, index, endIndex, tokener);
              if (indexTemp3 == index) { if (i2 < 1) {
                  indexTemp2 = indexStart2;
                } break;
              } else {
                index = indexTemp3;
              }
            }
            if (indexTemp3 != indexStart) {
              indexTemp2 = indexTemp3; break;
            }
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
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

    public static int ParseHeaderAutoSubmitted(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
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

    public static int ParseHeaderBase(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderContentBase(str, index, endIndex, tokener);
    }

    public static int ParseHeaderBcc(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderCc(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderContentBase(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentDisposition(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseDispositionType(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseDispositionParm(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseHeaderContentId(string str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderContentLocation(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentType(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseRestrictedName(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 47)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseRestrictedName(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseParameter(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseHeaderDate(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDeferredDelivery(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDeliveryDate(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDispositionNotificationOptions(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseDispNotParam(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseDispNotParam(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseHeaderDispositionNotificationTo(string str, int index, int endIndex, ITokener tokener) {
      return ParseMailboxList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderEncrypted(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseWord(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = ParseWord(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
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

    public static int ParseHeaderExpires(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderExpiryDate(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderFrom(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddressList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderInReplyTo(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 60)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = ParseIdLeft(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            if (index < endIndex && (str[index] == 64)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = ParseIdRight(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            if (index < endIndex && (str[index] == 62)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderKeywords(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParsePhrase(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParsePhrase(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseHeaderLanguage(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && (((str[index] >= 65 && str[index] <= 90) && (str[index + 1] >= 65 && str[index + 1] <= 90)) || ((str[index] >= 97 && str[index] <= 122) && (str[index + 1] >= 97 && str[index + 1] <= 122)))) {
              index += 2;
            } else {
              break;
            }
            while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122))) {
              ++index;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && (str[index] == 40)) {
                  ++index;
                } else {
                  break;
                }
                indexTemp3 = ParseLanguageDescription(str, index, endIndex, tokener);
                if (indexTemp3 == index) {
                  indexTemp3 = indexStart3; index = indexStart3; break;
                } else {
                  index = indexTemp3;
                }
                if (index < endIndex && (str[index] == 41)) {
                  ++index;
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderLatestDeliveryTime(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListArchive(string str, int index, int endIndex, ITokener tokener) {
      return ParseListHeaderUrlList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListHelp(string str, int index, int endIndex, ITokener tokener) {
      return ParseListHeaderUrlList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListId(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        if (index < endIndex && (str[index] == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 46)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                int indexTemp4;
                indexTemp4 = ParseDotAtomText(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index + 8 < endIndex && (str[index] & ~32) == 76 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 67 && (str[index + 3] & ~32) == 65 && (str[index + 4] & ~32) == 76 && (str[index + 5] & ~32) == 72 && (str[index + 6] & ~32) == 79 && (str[index + 7] & ~32) == 83 && (str[index + 8] & ~32) == 84) {
                  indexTemp3 += 9; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        if (index < endIndex && (str[index] == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderListOwner(string str, int index, int endIndex, ITokener tokener) {
      return ParseListHeaderUrlList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListPost(string str, int index, int endIndex, ITokener tokener) {
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
          if (index + 1 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79) {
            index += 2;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseListHeaderUrlList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderListSubscribe(string str, int index, int endIndex, ITokener tokener) {
      return ParseListHeaderUrlList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListUnsubscribe(string str, int index, int endIndex, ITokener tokener) {
      return ParseListHeaderUrlList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderMessageId(string str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderMmhsCopyPrecedence(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = ParsePrecedence(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsExemptedAddress(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = ParseAddressList(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsExtendedAuthorisationInfo(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = ParseDateTime(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsMessageType(string str, int index, int endIndex, ITokener tokener) {
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
              if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
                ++index;
              } else {
                break;
              }
              while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
                ++index;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str[index] == 48)) {
              ++indexTemp2; break;
            }
            if (index + 4 < endIndex && (str[index] & ~32) == 68 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) == 76 && (str[index + 4] & ~32) == 76) {
              indexTemp2 += 5; break;
            }
            if (index + 6 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 79 && (str[index + 3] & ~32) == 74 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 67 && (str[index + 6] & ~32) == 84) {
              indexTemp2 += 7; break;
            }
            if (index + 8 < endIndex && (str[index] & ~32) == 79 && (str[index + 1] & ~32) == 80 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 82 && (str[index + 4] & ~32) == 65 && (str[index + 5] & ~32) == 84 && (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 79 && (str[index + 8] & ~32) == 78) {
              indexTemp2 += 9; break;
            }
            if (index + 7 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 88 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 82 && (str[index + 4] & ~32) == 67 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 83 && (str[index + 7] & ~32) == 69) {
              indexTemp2 += 8; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseMessageTypeParam(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsPrimaryPrecedence(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = ParsePrecedence(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderObsoletes(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseMsgId(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseMsgId(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseHeaderOriginalFrom(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderFrom(str, index, endIndex, tokener);
    }

    public static int ParseHeaderOriginalMessageId(string str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderOriginalRecipient(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseAtom(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2 = ParseText(str, index, endIndex, tokener);
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

    public static int ParseHeaderReceived(string str, int index, int endIndex, ITokener tokener) {
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
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseDateTime(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderReceivedSpf(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 3 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) == 83) {
              indexTemp2 += 4; break;
            }
            if (index + 3 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) == 76) {
              indexTemp2 += 4; break;
            }
            if (index + 7 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 65 && (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 76) {
              indexTemp2 += 8; break;
            }
            if (index + 6 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 65 && (str[index + 6] & ~32) == 76) {
              indexTemp2 += 7; break;
            }
            if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) == 69) {
              indexTemp2 += 4; break;
            }
            if (index + 8 < endIndex && (str[index] & ~32) == 84 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) == 80 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index + 8] & ~32) == 82) {
              indexTemp2 += 9; break;
            }
            if (index + 8 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 82 && (str[index + 3] & ~32) == 77 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index + 8] & ~32) == 82) {
              indexTemp2 += 9; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            indexTemp2 = ParseComment(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseFWS(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        index = ParseKeyValueList(str, index, endIndex, tokener);
        if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderReferences(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderInReplyTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderReplyBy(string str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderReplyTo(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentBcc(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderBcc(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentCc(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentDate(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderDate(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentFrom(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderFrom(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentMessageId(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderMessageId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentReplyTo(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentSender(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderSender(str, index, endIndex, tokener);
    }

    public static int ParseHeaderResentTo(string str, int index, int endIndex, ITokener tokener) {
      return ParseHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderReturnPath(string str, int index, int endIndex, ITokener tokener) {
      return ParsePath(str, index, endIndex, tokener);
    }

    public static int ParseHeaderSender(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseGroup(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderTo(string str, int index, int endIndex, ITokener tokener) {
      return ParseAddressList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400Originator(string str, int index, int endIndex, ITokener tokener) {
      return ParseMailbox(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400Recipients(string str, int index, int endIndex, ITokener tokener) {
      return ParseMailboxList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXMittente(string str, int index, int endIndex, ITokener tokener) {
      return ParseMailbox(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXRiferimentoMessageId(string str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHour(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseIdLeft(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseLocalPart(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseIdRight(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseNoFoldLiteral(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseKey(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122))) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 95) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseKeyValueList(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseKeyValuePair(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseKeyValuePair(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseKeyValuePair(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseKey(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int indexTemp3 = ParseDotAtom(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseQuotedString(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseLanguageDescription(string str, int index, int endIndex, ITokener tokener) {
      return ParsePrintablestring(str, index, endIndex, tokener);
    }

    public static int ParseLanguageTag(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122))) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 45) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseLdhStr(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 45) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        }
        if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseListHeaderUrl(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) || (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        if (index < endIndex && (str[index] == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseListHeaderUrlList(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseListHeaderUrl(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseListHeaderUrl(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseLocalPart(string str, int index, int endIndex, ITokener tokener) {
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
            indexTemp2 = indexStart2; break;
          } else {
            index = indexTemp2;
          }
          while (true) {
            int indexTemp3;
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str[index] == 46)) {
                ++index;
              } else {
                break;
              }
              indexTemp3 = ParseWord(str, index, endIndex, tokener);
              if (indexTemp3 == index) {
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else if (tokener != null) {
              tokener.RestoreState(state3); break;
            } else {
              break;
            }
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(8, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseLocalPartNoCfws(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] == 34)) {
            ++index;
          } else {
            break;
          }
          while (true) {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                ++indexTemp3; break;
              }
              if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 91) || (str[index] >= 93 && str[index] <= 126))) {
                ++indexTemp3; break;
              }
              int indexTemp4;
              indexTemp4 = index;
              do {
                int indexStart4 = index;
                if (index < endIndex && (str[index] == 92)) {
                  ++index;
                }
                do {
                  int indexTemp5;
                  indexTemp5 = index;
                  do {
                    int indexStart5 = index;
                    if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                      ++indexTemp5; break;
                    }
                    int indexTemp6;
                    indexTemp6 = index;
                    do {
                      int indexStart6 = index;
                      if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
                        ++index;
                      } else {
                        break;
                      }
                      if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
                        ++index;
                      } else {
                        index = indexStart6; break;
                      }
                      indexTemp6 = index;
                      index = indexStart6;
                    } while (false);
                    if (indexTemp6 != index) {
                      indexTemp5 = indexTemp6; break;
                    }
                  } while (false);
                  if (indexTemp5 != index) {
                    index = indexTemp5;
                  } else {
                    index = indexStart4; break;
                  }
                } while (false);
                if (index == indexStart4) {
                  break;
                }
                indexTemp4 = index;
                index = indexStart4;
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
              indexTemp4 = index;
              do {
                int indexStart4 = index;
                if (index + 1 < endIndex && (str[index] == 92) && ((str[index + 1] == 9) || (str[index + 1] >= 32 && str[index + 1] <= 126))) {
                  index += 2;
                } else {
                  break;
                }
                indexTemp4 = index;
                index = indexStart4;
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              break;
            }
          }
          if (index < endIndex && (str[index] == 34)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMailbox(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(6, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseMailboxList(string str, int index, int endIndex, ITokener tokener) {
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
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                int indexTemp4;
                indexTemp4 = ParseMailbox(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseMessageTypeParam(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 9 < endIndex && (str[index] & ~32) == 73 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 78 && (str[index + 4] & ~32) == 84 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 70 && (str[index + 7] & ~32) == 73 && (str[index + 8] & ~32) == 69 && (str[index + 9] & ~32) == 82) {
          index += 10;
        } else {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = ParseQuotedMilitaryString(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMethod(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseLdhStr(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 47)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseMethodVersion(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMethodVersion(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
          ++index;
          while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
            ++index;
          }
        } else {
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

    public static int ParseMethodspec(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseMethod(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseResult(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMinute(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMsgId(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseIdLeft(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseIdRight(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNameAddr(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseDisplayName(str, index, endIndex, tokener);
        indexTemp = ParseAngleAddr(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNoFoldLiteral(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] == 91)) {
          ++index;
        } else {
          break;
        }
        while (true) {
          int indexTemp2 = ParseDtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        if (index < endIndex && (str[index] == 93)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNoResult(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) == 69) {
          index += 4;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsDomainList(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str[index] == 44)) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        if (index < endIndex && (str[index] == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && (str[index] == 64)) {
                  ++index;
                } else {
                  break;
                }
                indexTemp3 = ParseDomain(str, index, endIndex, tokener);
                if (indexTemp3 == index) {
                  indexTemp3 = indexStart3; index = indexStart3; break;
                } else {
                  index = indexTemp3;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseObsGroupList(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
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

    public static int ParseObsRoute(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseObsDomainList(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseOptParameterList(string str, int index, int endIndex, ITokener tokener) {
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
            if (index < endIndex && (str[index] == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseParameter(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseOtherSections(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index + 1 < endIndex && (str[index] == 42) && (str[index + 1] >= 49 && str[index + 1] <= 57)) {
          index += 2;
        } else {
          break;
        }
        while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseParameter(string str, int index, int endIndex, ITokener tokener) {
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
                indexTemp3 = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              if (index < endIndex && (str[index] == 61)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = ParseExtendedInitialValue(str, index, endIndex, tokener);
              if (indexTemp3 == index) {
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
            state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              indexTemp3 = ParseExtendedOtherNames(str, index, endIndex, tokener);
              if (indexTemp3 == index) {
                indexTemp3 = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              if (index < endIndex && (str[index] == 61)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
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
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
            indexTemp3 = ParseRegularParameter(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
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

    public static int ParsePath(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str[index] == 60)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str[index] == 62)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePhrase(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParsePhraseWord(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        while (true) {
          int indexTemp2 = ParsePhraseWordOrDot(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
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

    public static int ParsePhraseAtom(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122))) {
              ++indexTemp2; break;
            }
            if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 33) || (str[index] == 35) || (str[index] == 36) || (str[index] == 37) || (str[index] == 38) || (str[index] == 39) || (str[index] == 42) || (str[index] == 43) || (str[index] == 45) || (str[index] == 47) || (str[index] == 61) || (str[index] == 63) || (str[index] == 94) || (str[index] == 95) || (str[index] == 96) || (str[index] == 123) || (str[index] == 124) || (str[index] == 125) || (str[index] == 126))) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                ++indexTemp3; break;
              }
              int indexTemp4;
              indexTemp4 = index;
              do {
                int indexStart4 = index;
                if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
                  ++index;
                } else {
                  break;
                }
                if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
                  ++index;
                } else {
                  index = indexStart4; break;
                }
                indexTemp4 = index;
                index = indexStart4;
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
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

    public static int ParsePhraseAtomOrDot(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = index;
        for (int i = 0;; ++i) {
          indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
        index = indexStart;
        if (index < endIndex && (str[index] == 46)) {
          ++indexTemp; break;
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(4, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParsePhraseWord(string str, int index, int endIndex, ITokener tokener) {
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
            indexTemp2 = indexStart2; index = indexStart2; break;
          } else {
            index = indexTemp2;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePhraseWordOrDot(string str, int index, int endIndex, ITokener tokener) {
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
          indexTemp2 = ParsePhraseAtomOrDot(str, index, endIndex, tokener);
          if (indexTemp2 == index) {
            indexTemp2 = indexStart2; index = indexStart2; break;
          } else {
            index = indexTemp2;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePrecedence(string str, int index, int endIndex, ITokener tokener) {
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
              if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
                ++index;
              } else {
                break;
              }
              while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
                ++index;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str[index] == 48)) {
              ++indexTemp2; break;
            }
            if (index + 7 < endIndex && (str[index] & ~32) == 79 && (str[index + 1] & ~32) == 86 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 82 && (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 68 && (str[index + 7] & ~32) == 69) {
              indexTemp2 += 8; break;
            }
            if (index + 4 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 76 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) == 83 && (str[index + 4] & ~32) == 72) {
              indexTemp2 += 5; break;
            }
            if (index + 8 < endIndex && (str[index] & ~32) == 73 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) == 69 && (str[index + 4] & ~32) == 68 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 65 && (str[index + 7] & ~32) == 84 && (str[index + 8] & ~32) == 69) {
              indexTemp2 += 9; break;
            }
            if (index + 7 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) == 79 && (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 73 && (str[index + 6] & ~32) == 84 && (str[index + 7] & ~32) == 89) {
              indexTemp2 += 8; break;
            }
            if (index + 6 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 73 && (str[index + 5] & ~32) == 78 && (str[index + 6] & ~32) == 69) {
              indexTemp2 += 7; break;
            }
            if (index + 7 < endIndex && (str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) == 69 && (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 69 && (str[index + 7] & ~32) == 68) {
              indexTemp2 += 8; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
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

    public static int ParsePrintablestring(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122) || (str[index] == 32) || (str[index] == 39) || (str[index] >= 43 && str[index] <= 58) || (str[index] == 61) || (str[index] == 63))) {
              ++indexTemp2; break;
            }
            if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
              ++indexTemp2; break;
            }
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

    public static int ParseProperty(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseLdhStr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index + 5 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 67 && (str[index + 2] & ~32) == 80 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 84 && (str[index + 5] & ~32) == 79) {
          indexTemp += 6; break;
        }
        if (index + 7 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) == 76 && (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 79 && (str[index + 7] & ~32) == 77) {
          indexTemp += 8; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePropspec(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParsePtype(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 46)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = ParseProperty(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParsePvalue(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePsChar(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 122) || (str[index] == 32) || (str[index] == 39) || (str[index] >= 43 && str[index] <= 58) || (str[index] == 61) || (str[index] == 63))) {
          ++indexTemp; break;
        }
        if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParsePtype(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index + 3 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 84 && (str[index + 3] & ~32) == 80) {
          indexTemp += 4; break;
        }
        if (index + 5 < endIndex && (str[index] & ~32) == 72 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) == 68 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82) {
          indexTemp += 6; break;
        }
        if (index + 3 < endIndex && (str[index] & ~32) == 66 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 68 && (str[index + 3] & ~32) == 89) {
          indexTemp += 4; break;
        }
        if (index + 5 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 76 && (str[index + 3] & ~32) == 73 && (str[index + 4] & ~32) == 67 && (str[index + 5] & ~32) == 89) {
          indexTemp += 6; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParsePvalue(string str, int index, int endIndex, ITokener tokener) {
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
                  if (index < endIndex && (str[index] == 64)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else { break;
                }
              } while (false);
              indexTemp3 = ParseDomainName(str, index, endIndex, tokener);
              if (indexTemp3 == index) {
                indexTemp3 = indexStart3; index = indexStart3; break;
              } else {
                index = indexTemp3;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
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

    public static int ParseQcontent(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          ++indexTemp; break;
        }
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
            ++index;
          } else {
            break;
          }
          if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127))) {
          ++indexTemp; break;
        }
        indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str[index] >= 93 && str[index] <= 126) || (str[index] >= 35 && str[index] <= 91) || (str[index] == 33))) {
          ++indexTemp; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseQuotedMilitaryString(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] == 34)) {
          ++index;
        } else {
          break;
        }
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        if (index < endIndex && (str[index] == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseQuotedPair(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] == 92)) {
          ++index;
        } else {
          break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && ((str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127) || (str[index] == 0) || (str[index] == 10) || (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
                ++index;
              } else {
                break;
              }
              if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && ((str[index] == 13) || (str[index] >= 33 && str[index] <= 126) || (str[index] == 9) || (str[index] == 32))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseQuotedString(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = ParseQcontent(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(7, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseReasonspec(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 5 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) == 83 && (str[index + 4] & ~32) == 79 && (str[index + 5] & ~32) == 78) {
          index += 6;
        } else {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
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

    public static int ParseReceivedToken(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAtom(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseRegularParameter(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseRegularParameterName(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseValue(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseRegularParameterName(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] == 43) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126))) {
            ++index;
          }
        } else {
          break;
        }
        index = ParseSection(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseResinfo(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str[index] == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseMethodspec(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            indexTemp2 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = ParseReasonspec(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
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
              indexTemp2 = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = ParsePropspec(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseRestrictedName(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
          ++index;
        } else {
          break;
        }
        for (int i = 0; i < 126; ++i) {
          if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] == 38) || (str[index] >= 94 && str[index] <= 95) || (str[index] >= 45 && str[index] <= 46) || (str[index] == 43) || (str[index] >= 97 && str[index] <= 122) || (str[index] >= 65 && str[index] <= 90))) {
            ++index;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseResult(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index + 3 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) == 83) {
          indexTemp += 4; break;
        }
        if (index + 3 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) == 76) {
          indexTemp += 4; break;
        }
        if (index + 7 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 65 && (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 76) {
          indexTemp += 8; break;
        }
        if (index + 6 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 65 && (str[index + 6] & ~32) == 76) {
          indexTemp += 7; break;
        }
        if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) == 69) {
          indexTemp += 4; break;
        }
        if (index + 8 < endIndex && (str[index] & ~32) == 84 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) == 80 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index + 8] & ~32) == 82) {
          indexTemp += 9; break;
        }
        if (index + 8 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 82 && (str[index + 3] & ~32) == 77 && (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 && (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index + 8] & ~32) == 82) {
          indexTemp += 9; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseSecond(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseSection(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 1 < endIndex && str[index] == 42 && str[index + 1] == 48) {
          indexTemp += 2; break;
        }
        int indexTemp2 = ParseOtherSections(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseStdChar(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] >= 65 && str[index] <= 90)) {
          ++indexTemp; break;
        }
        if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
          ++indexTemp; break;
        }
        if (index < endIndex && ((str[index] == 32) || (str[index] == 39) || (str[index] >= 97 && str[index] <= 123) || (str[index] >= 48 && str[index] <= 58) || (str[index] == 125) || (str[index] == 63) || (str[index] >= 42 && str[index] <= 46))) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseStdPair(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str[index] == 36)) {
          ++index;
        } else {
          break;
        }
        indexTemp = ParsePsChar(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseStdPrintablestring(string str, int index, int endIndex, ITokener tokener) {
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
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseStdPair(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
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

    public static int ParseText(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] >= 1 && str[index] <= 9) || (str[index] == 11) || (str[index] == 12) || (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          ++indexTemp; break;
        }
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str[index] >= 55296 && str[index] <= 56319)) {
            ++index;
          } else {
            break;
          }
          if (index < endIndex && (str[index] >= 56320 && str[index] <= 57343)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && (str[index] >= 14 && str[index] <= 127)) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseTime(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseTimeOfDay(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = ParseZone(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseTimeOfDay(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        indexTemp = ParseHour(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; break;
        } else {
          index = indexTemp;
        }
        if (index < endIndex && (str[index] == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = ParseMinute(str, index, endIndex, tokener);
        if (indexTemp == index) {
          indexTemp = indexStart; index = indexStart; break;
        } else {
          index = indexTemp;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str[index] == 58)) {
              ++index;
            } else {
              break;
            }
            indexTemp2 = ParseSecond(str, index, endIndex, tokener);
            if (indexTemp2 == index) {
              indexTemp2 = indexStart2; index = indexStart2; break;
            } else {
              index = indexTemp2;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseToken(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
          ++index;
          while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
            ++index;
          }
        } else {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseValue(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 && str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 48 && str[index] <= 57) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
          ++indexTemp;
          while (indexTemp < endIndex && ((str[indexTemp] == 33) || (str[indexTemp] >= 35 && str[indexTemp] <= 36) || (str[indexTemp] >= 45 && str[indexTemp] <= 46) || (str[indexTemp] >= 65 && str[indexTemp] <= 90) || (str[indexTemp] >= 48 && str[indexTemp] <= 57) || (str[indexTemp] >= 94 && str[indexTemp] <= 126) || (str[indexTemp] >= 42 && str[indexTemp] <= 43) || (str[indexTemp] >= 38 && str[indexTemp] <= 39) || (str[indexTemp] == 63))) {
            ++indexTemp;
          }
          break;
        }
        int indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseWord(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAtom(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseX400Value(string str, int index, int endIndex, ITokener tokener) {
      return ParseStdPrintablestring(str, index, endIndex, tokener);
    }

    public static int ParseYear(string str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
          ++index;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseZone(string str, int index, int endIndex, ITokener tokener) {
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
            int indexTemp3 = ParseFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          if (index < endIndex && ((str[index] == 43) || (str[index] == 45))) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          if (index + 3 < endIndex && ((str[index] >= 48 && str[index] <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57) || (str[index + 2] >= 48 && str[index + 2] <= 57) || (str[index + 3] >= 48 && str[index + 3] <= 57))) {
            index += 4;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        if (index + 1 < endIndex && (str[index] & ~32) == 85 && (str[index + 1] & ~32) == 84) {
          indexTemp += 2; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 71 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
          indexTemp += 3; break;
        }
        if (index < endIndex && ((str[index] >= 65 && str[index] <= 73) || (str[index] >= 75 && str[index] <= 90) || (str[index] >= 97 && str[index] <= 105) || (str[index] >= 107 && str[index] <= 122))) {
          ++indexTemp; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }
  }
}
