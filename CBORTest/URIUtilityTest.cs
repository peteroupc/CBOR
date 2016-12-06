using System;
using NUnit.Framework;
using PeterO.Cbor;

namespace Test {
  [TestFixture]
  public class URIUtilityTest {
    public static string CborNamespace() {
      return typeof(CBORObject).Namespace;
    }

    private static readonly string URIUtilityName =
      CborNamespace() + ".URIUtility";

    private static void assertIdempotency(string s) {
      var cond = (bool)Reflect.InvokeStatic(
    URIUtilityName,
    "isValidIRI",
    s);
      Assert.IsTrue(cond);
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          0);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 0),
          0);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          1);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 1),
          1);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          2);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 2),
          2);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          3);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 3),
          3);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
    }

    private static void assertIdempotencyNeg(string s) {
      Assert.IsTrue(!((
    bool)Reflect.InvokeStatic(
    URIUtilityName,
    "isValidIRI",
    s)));
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          0);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 0),
          0);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
             URIUtilityName,
             "escapeURI",
             s,
             1);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 1),
          1);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          2);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 2),
          2);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          s,
          3);
        var stringTemp2 = (string)Reflect.InvokeStatic(
          URIUtilityName,
          "escapeURI",
          (string)Reflect.InvokeStatic(URIUtilityName, "escapeURI", s, 3),
          3);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
    }

    private static void assertResolve(String src, String baseuri, String dest) {
      assertIdempotency(src);
      assertIdempotency(baseuri);
      assertIdempotency(dest);
      var res = (string)Reflect.InvokeStatic(
    URIUtilityName,
    "relativeResolve",
    src,
    baseuri);
      assertIdempotency(res);
      Assert.AreEqual(dest, res);
    }

    [Test]
    [Timeout(5000)]
    public void testRelativeResolve() {
      assertResolve(
    "index.html",
    "http://example.com",
    "http://example.com/index.html");
      assertResolve(
          "./.x",
          "http://example.com/a/b/c/d/e.f",
          "http://example.com/a/b/c/d/.x");
      assertResolve(
          ".x",
          "http://example.com/a/b/c/d/e.f",
          "http://example.com/a/b/c/d/.x");
      assertResolve(
          "../.x",
          "http://example.com/a/b/c/d/e.f",
          "http://example.com/a/b/c/.x");
      assertResolve(
          "../..../../../.../.x",
          "http://example.com/a/b/c/d/e.f",
          "http://example.com/a/b/.../.x");
    }

    [Test]
    [Timeout(5000)]
    public void testUri() {
      assertIdempotency(String.Empty);
      assertIdempotency("e");
      assertIdempotency("e:x");
      assertIdempotency("e://x:@y");
      assertIdempotency("e://x/y");
      assertIdempotency("e://x//y");
      assertIdempotency("a://x:@y/z");
      assertIdempotency("a://x:/y");
      assertIdempotency("aa:/w/x");
      assertIdempotency("01/w/x");
      assertIdempotency("e://x");
      assertIdempotency("e://x/:/");
      assertIdempotency("x/:/");
      assertIdempotency("/:/");
      assertIdempotency("///");
      assertIdempotency("e://x:");
      assertIdempotency("e://x:%30");
      assertIdempotency("a://x:?x");
      assertIdempotency("a://x#x");
      assertIdempotency("a://x?x");
      assertIdempotency("a://x:#x");
      assertIdempotency("e://x@x");
      assertIdempotency("e://x@:");
      assertIdempotency("e://x@:?");
      assertIdempotency("e://x@:#");
      assertIdempotency("//x@x");
      assertIdempotency("//x@:");
      assertIdempotency("//x@:?");
      assertIdempotency("//x@:#");
      assertIdempotency("//x@:?x");
      assertIdempotency("e://x@:#x");
      assertIdempotency("a://x:?x");
      assertIdempotency("a://x#x");
      assertIdempotency("a://x?x");
      assertIdempotency("a://x:#x");
      assertIdempotencyNeg("e://^//y");
      assertIdempotencyNeg("e^");
      assertIdempotencyNeg("e://x:a");
      assertIdempotencyNeg("a://x::/y");
      assertIdempotency("x@yz");
      assertIdempotencyNeg("x@y:z");
      assertIdempotencyNeg("01:/w/x");
      assertIdempotencyNeg("e://x:%30/");
      assertIdempotencyNeg("a://xxx@[");
      assertIdempotencyNeg("a://[");
      assertIdempotency("a://[va.a]");
      assertIdempotency("a://[v0.0]");
      assertIdempotency("a://x:/");
      assertIdempotency("a://[va.a]:/");
      assertIdempotencyNeg("a://x%/");
      assertIdempotencyNeg("a://x%xy/");
      assertIdempotencyNeg("a://x%x%/");
      assertIdempotencyNeg("a://x%%x/");
      assertIdempotency("a://x%20/");
      assertIdempotency("a://[v0.0]/");
      assertIdempotencyNeg("a://[wa.a]");
      assertIdempotencyNeg("a://[w0.0]");
      assertIdempotencyNeg("a://[va.a/");
      assertIdempotencyNeg("a://[v.a]");
      assertIdempotencyNeg("a://[va.]");
      assertIPv6("a:a:a:a:a:a:100.100.100.100");
      assertIPv6("::a:a:a:a:a:100.100.100.100");
      assertIPv6("::a:a:a:a:a:99.255.240.10");
      assertIPv6("::a:a:a:a:99.255.240.10");
      assertIPv6("::99.255.240.10");
      assertIPv6Neg("99.255.240.10");
      assertIPv6("a:a:a:a:a::99.255.240.10");
      assertIPv6("a:a:a:a:a:a:a:a");
      assertIPv6("aaa:a:a:a:a:a:a:a");
      assertIPv6("aaaa:a:a:a:a:a:a:a");
      assertIPv6("::a:a:a:a:a:a:a");
      assertIPv6("a::a:a:a:a:a:a");
      assertIPv6("a:a::a:a:a:a:a");
      assertIPv6("a:a:a::a:a:a:a");
      assertIPv6("a:a:a:a::a:a:a");
      assertIPv6("a:a:a:a:a::a:a");
      assertIPv6("a:a:a:a:a:a::a");
      assertIPv6("a::a");
      assertIPv6("::a");
      assertIPv6("::a:a");
      assertIPv6("::");
      assertIPv6("a:a:a:a:a:a:a::");
      assertIPv6("a:a:a:a:a:a::");
      assertIPv6("a:a:a:a:a::");
      assertIPv6("a:a:a:a::");
      assertIPv6("a:a::");
      assertIPv6Neg("a:a::a:a:a:a:a:a");
      assertIPv6Neg("aaaaa:a:a:a:a:a:a:a");
      assertIPv6Neg("a:a:a:a:a:a:a:100.100.100.100");
      assertIPv6Neg("a:a:a:a:a:a::99.255.240.10");
      assertIPv6Neg(":::a:a:a:a:a:a:a");
      assertIPv6Neg("a:a:a:a:a:a:::a");
      assertIPv6Neg("a:a:a:a:a:a:a:::");
      assertIPv6Neg("::a:a:a:a:a:a:a:");
      assertIPv6Neg("::a:a:a:a:a:a:a:a");
      assertIPv6Neg("a:a");
      assertIdempotency("e://[va.a]");
      assertIdempotency("e://[v0.0]");
      assertIdempotencyNeg("e://[wa.a]");
      assertIdempotencyNeg("e://[va.^]");
      assertIdempotencyNeg("e://[va.]");
      assertIdempotencyNeg("e://[v.a]");
    }

    private static void assertIPv6Neg(string str) {
      assertIdempotencyNeg("e://[" + str + "]");
      assertIdempotencyNeg("e://[" + str + "NANA]");
      assertIdempotencyNeg("e://[" + str + "%25]");
      assertIdempotencyNeg("e://[" + str + "%NANA]");
      assertIdempotencyNeg("e://[" + str + "%25NANA]");
      assertIdempotencyNeg("e://[" + str + "%52NANA]");
      assertIdempotencyNeg("e://[" + str + "%25NA<>NA]");
      assertIdempotencyNeg("e://[" + str + "%25NA%E2NA]");
      assertIdempotencyNeg("e://[" + str + "%25NA%2ENA]");
    }

    private static void assertIPv6(string str) {
      assertIdempotency("e://[" + str + "]");
      assertIdempotencyNeg("e://[" + str + "NANA]");
      assertIdempotencyNeg("e://[" + str + "%25]");
      assertIdempotencyNeg("e://[" + str + "%NANA]");
      assertIdempotency("e://[" + str + "%25NANA]");
      assertIdempotencyNeg("e://[" + str + "%52NANA]");
      assertIdempotencyNeg("e://[" + str + "%25NA<>NA]");
      assertIdempotency("e://[" + str + "%25NA%E2NA]");
      assertIdempotency("e://[" + str + "%25NA%2ENA]");
    }
  }
}
