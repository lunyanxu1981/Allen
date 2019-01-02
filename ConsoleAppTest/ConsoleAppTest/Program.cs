using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Diagnostics;

namespace ConsoleAppTest
{
    public class AbcClass
    {
        public bool IsYes { get; set; }
        public int Hit { get; set; }

        public static explicit operator AbcClass(XyzClass source)
        {
            if (source != null)
            {
                return new AbcClass
                {
                    IsYes = source.IsYes,
                    Hit = source.Hit
                };
            }
            else
            {
                return null;
            }
            
        }
    }

    public class XyzClass
    {
        public bool IsYes { get; set; }
        public int Hit { get; set; }
        public XyzClass()
        {
            IsYes = true;
            Hit = 100;
        }
        
        public void CallStatic() 
        {
            ExtTest.TestStatic();
        }
    }

    public static class ABCStatic
    {
        static ABCStatic()
        {
            throw new Exception();
        }

        public static string GetLine()
        {
            return "test";
        }
    }

    public abstract class BaseClass
    {
        public virtual int DoSomething()
        {
            return -1;
        }
    }

    public class InheritClassOne : BaseClass
    {
        public override int DoSomething()
        {
            int result = base.DoSomething();
            return 5 + result;
        }
    }

    class Program
    {
        private static void ArgumentModifier(XyzClass obj)
        {
            ArgumentModifier2(out obj);
        }
        private static void ArgumentModifier2(out XyzClass obj)
        {
            obj = new XyzClass();
            obj.Hit = 1000;
        }

        static void Main(string[] args)
        {
            string str = string.Empty;
            string upper = str.ToUpper();

            Console.ReadLine();
        }
        
        public static void TestSubString()
        {
            var str = TestMD5Calc();
            if (str.IndexOf("--", StringComparison.OrdinalIgnoreCase) <= 0)
            {
                // sometimes people mess up and will not use double dash, see if they did use single dash
                int index = str.IndexOf(" - ", StringComparison.OrdinalIgnoreCase);
                if (index >= 0 && index <= 20)
                {
                    str= str.Substring(0, index).Trim();
                }
                
            }
        }

        public static string GetPrivateKeyCertId()
        {
            string certPath = "D:\\Work\\Document\\LocalDeals\\CUP\\FDMS\\Production\\Production\\UPOPe SDK for Merchant_V2.2\\UPOPe SDK for Merchant_V2.2\\Cert\\Testing\\Private cert (Signature)\\acp_test_sign.pfx";
            X509Certificate2 pc = new X509Certificate2(certPath, "000000");
            return BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
        }

        public static string GetPublicKeyCertId()
        {
            string certPath = "D:\\Work\\Document\\LocalDeals\\CUP\\FDMS\\Production\\Production\\UPOPe SDK for Merchant_V2.2\\UPOPe SDK for Merchant_V2.2\\Cert\\Testing\\Public cert (Verify Signature)\\acp_test_verify_sign_new.cer";
            X509Certificate2 pc = new X509Certificate2(certPath);
            return BigInteger.Parse(pc.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
        }


        public static string TestMD5Calc()
        {
            //Set v_md5info from HMAC digest of v_moneytype,v_ymd,v_amount,v_rcvname,v_oid,v_mid,v_url
            return getHMAC_MD5("75cdf8e01fa354774b1fd7bfa969ff2f", "020171208178.001409020171208-14090-1588621714090https://www.travelzoo.com/cn/purchase/payeasesuccesscallback/");
        }

        public static string getHMAC_MD5(string key, string data)
        {
            byte[] bKey, bData, bRet;
            string ret = "";
            UTF8Encoding encoder = new UTF8Encoding();
            bKey = encoder.GetBytes(key);
            bData = encoder.GetBytes(data);

            HMACMD5 c = new HMACMD5(bKey);
            bRet = c.ComputeHash(bData);
            foreach (byte b in bRet)
            {
                ret += String.Format("{0:x2}", b);
            }

            return ret;
        }
    }

    public static class ExtTest
    {
        public static string HtmlDecode(this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }
        public static void TestStatic()
        {
            StackTrace stackTrace = new StackTrace();
            var obj = stackTrace.GetFrame(1);
        }
    }

    public abstract class SuperClassA
    {
        public abstract string TestMethod1(string input1, string input2);
    }

    public class DerivedClassB:SuperClassA
    {
        public override string TestMethod1(string input3, string input4)
        {
            return string.Format("Input1: {0} Input2: {1}", input3, input4);
        }
    }
}
