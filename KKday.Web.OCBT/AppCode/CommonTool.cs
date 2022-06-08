using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Net.Http;


namespace KKday.Web.OCBT.AppCode
{

    public class CommonTool {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Nearlies the equal.
        /// </summary>
        /// <returns><c>true</c>, if equal was nearlyed, <c>false</c> otherwise.</returns>
        /// <param name="a">第一個值</param>
        /// <param name="b">第二個值</param>
        /// <param name="epsilon">容忍差異,eg. 0.1, 當a=1.2, b=1.3時回傳 true</param>
        public static bool NearlyEqual(double a, double b, double epsilon) {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b) { // shortcut, handles infinities
                return true;
            } else if (a == 0 || b == 0 || diff < Double.Epsilon) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            } else { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
    }
    public static class ExtensionMethod
    {
        //Object深層複製
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }


    public static class MD5Tool
    {
        public static string GetMD5(string str)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }

    ////////////////////////////////

    class StrTool
    {
        public static string Null2Empty(string s)
        {
            if (s == null) { return ""; } else { return s; }
        }

        public static string Null2EmptyWithTrim(string s)
        {
            return Null2Empty(s).Trim();
        }

        public static string LoadFileToStr(string FilePath)
        {
            //StreamReader sr = new StreamReader(FilePath);
            //StringBuilder szContent = new StringBuilder();

            //while (sr.Peek() != -1)
            //{
            //    szContent.AppendLine(sr.ReadLine());
            //}
            //sr.Close();

            //return szContent.ToString();
            //return szContent.ToString().Substring(0, szContent.ToString().LastIndexOf(Environment.NewLine));
            return File.ReadAllText(FilePath);
        }

        public static string ReplaceEmptyLine(string s)
        {
            string sNewLine = Environment.NewLine; //換行字元
            string sNewLine2 = sNewLine + sNewLine; //連續換行(會產生空白行)

            //先取代連續兩個換行符號的
            while (true)
            {
                if (s.IndexOf(sNewLine2) != -1)
                {
                    s = s.Replace(sNewLine2, sNewLine); //換成一個
                }
                else { break; }
            }

            //再取代結尾有換行的
            if (s.Substring(s.Length - sNewLine.Length) == sNewLine)
            {
                s = s.Substring(0, s.Length - sNewLine.Length);
            }

            return s;
        }
    }

    ////////////////////////////////


    public class DateTimeTool
    {
        public static DateTime yyyyMMdd2DateTime(string yyyyMMdd)
        {
            try
            {
                int yyyy = Convert.ToInt32(yyyyMMdd.Substring(0, 4));
                int MM = Convert.ToInt32(yyyyMMdd.Substring(4, 2));
                int dd = Convert.ToInt32(yyyyMMdd.Substring(6, 2));

                return new DateTime(yyyy, MM, dd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static String yyyy_mm_dd(string yyyyMMdd)
        {
            try
            {
                string yyyy = yyyyMMdd.Substring(0, 4);
                string MM = yyyyMMdd.Substring(4, 2);
                string dd = yyyyMMdd.Substring(6, 2);

                return string.Format("{0}-{1}-{2}", yyyy, MM, dd);
            }
            catch(Exception ex)
            {
                throw ex;
            }


        }

        public static DateTime yyyyMMddHHmmss2DateTime(string yyyyMMddHHmmss)
        {
            try
            {
                int yyyy = Convert.ToInt32(yyyyMMddHHmmss.Substring(0, 4));
                int MM = Convert.ToInt32(yyyyMMddHHmmss.Substring(4, 2));
                int dd = Convert.ToInt32(yyyyMMddHHmmss.Substring(6, 2));
                int HH = Convert.ToInt32(yyyyMMddHHmmss.Substring(8, 2));
                int mm = Convert.ToInt32(yyyyMMddHHmmss.Substring(10, 2));
                int ss = Convert.ToInt32(yyyyMMddHHmmss.Substring(12, 2));

                return new DateTime(yyyy, MM, dd, HH, mm, ss);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime yyyyMMddHHmmssfff2DateTime(string yyyyMMddHHmmssfff)
        {
            try
            {
                int yyyy = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(0, 4));
                int MM = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(4, 2));
                int dd = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(6, 2));
                int HH = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(8, 2));
                int mm = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(10, 2));
                int ss = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(12, 2));
                int fff = Convert.ToInt32(yyyyMMddHHmmssfff.Substring(14, 3));

                return new DateTime(yyyy, MM, dd, HH, mm, ss, fff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //日期驗證(YYYYMMDD)
        public static bool CheckIsYYYYMMDD(string str)
        {
            try
            {
                DateTime.ParseExact(str, "yyyyMMdd", null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //時間格式化
        public static string TimeFormat(string mmss)
        {
            return String.Format("{0}:{1}", mmss.Substring(0, 2), mmss.Substring(2, 2));
        }

        //判斷飛行日在使用星期內
        public static bool CheckDateInWeeks(DateTime Date, string Weeks)
        {
            if (Weeks == null || Weeks.Length != 7)
            {
                return false;
            }
            else if (Weeks.ToCharArray()[(int)Date.DayOfWeek] == '1')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //判斷日期是否在區間內
        public static bool CheckDateBetween(DateTime CurDate, DateTime? SDate, DateTime? EDate)
        {
            if ((SDate ?? new DateTime(1900, 1, 1)) <= CurDate && CurDate <= (EDate ?? new DateTime(9999, 12, 31)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //轉換星期
        public static string GetWeeksChinese(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "一";
                case DayOfWeek.Tuesday:
                    return "二";
                case DayOfWeek.Wednesday:
                    return "三";
                case DayOfWeek.Thursday:
                    return "四";
                case DayOfWeek.Friday:
                    return "五";
                case DayOfWeek.Saturday:
                    return "六";
                case DayOfWeek.Sunday:
                    return "日";
                default:
                    return "";
            }
        }

        public static DateTime ToDateTime(string date, string format)
        {
            try
            {
                return DateTime.ParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture, 
                            System.Globalization.DateTimeStyles.None);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    ////////////////////////////////


    //public class JsonTool
    //{
    //    //反序列化
    //    public static object JsonDeSerialize(string JssStr, Type Type)
    //    {
    //        try
    //        {
    //            JavaScriptSerializer jss = new JavaScriptSerializer();
    //            return jss.Deserialize(JssStr, Type);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    //序列化
    //    public static string JsonSerialize(object obj)
    //    {
    //        try
    //        {
    //            JavaScriptSerializer jss = new JavaScriptSerializer();
    //            return jss.Serialize(obj);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }
    //}


    ////////////////////////////////


    public class XMLTool
    {
        public static string StripHTML(string strHtml)
        {
            string[] aryReg = { @"<script[^>]*?>.*?</script>", @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+", @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);", @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);", @"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n" };
            string[] aryRep = { "", "", "", "\"", "&", "<", ">", " "
 		        ,"\xa1"//chr(161)
 		        ,"\xa2"//chr(162)
 		        ,"\xa3"//chr(163)
 		        ,"\xa9"//chr(169)
 		        ,"", "\r\n", "" 
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }

            strOutput = strOutput.Replace("&", "&amp;");
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");
            //strOutput = strOutput.Replace("'", "&apos;");
            //strOutput = strOutput.Replace("\"", "&quot;");            
            strOutput = strOutput.Replace("\r\n", "");

            return strOutput;
        }

        public static string StripXML(string strXml)
        {
            string strOutput = Regex.Replace(strXml, @"<p>|</p>|<br>|<br />", "\r\n", RegexOptions.IgnoreCase);

            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");
            strOutput = strOutput.Replace("'", "&apos;");
            strOutput = strOutput.Replace("\"", "&quot;");
            strOutput = strOutput.Replace("'", "&apos;");

            return strOutput;
        }

        //反序列化
        public static object XMLDeSerialize(string XmlStr, string TypeName)
        {
            try
            {
                XmlSerializer Des = new XmlSerializer(Type.GetType(TypeName));
                System.IO.StringReader Sr = new System.IO.StringReader(XmlStr);
                return Des.Deserialize(Sr);
            }
            catch (Exception)
            {
                //throw ex;
                return null;
            }
        }

        //序列化
        public static string XMLSerialize(object obj)
        {
            try
            {
                XmlSerializer s = new XmlSerializer(obj.GetType());
                System.IO.StringWriter Sw = new System.IO.StringWriter();
                s.Serialize(Sw, obj);
                return Sw.ToString();
            }
            catch (Exception)
            {
                //throw ex;
                return null;
            }
        }
    }


    ////////////////////////////////


    //public class MMCTool
    //{
    //    //將 Object 存入
    //    public static void Object_Save(string MMCKey, int CacheMinute, object obj)
    //    {
    //        //Object_Remove(MMCKey);
    //        DistCache.Add(MMCKey, obj, 60 * 1000 * CacheMinute);

    //        //SaveToMMCList(MMCKey, CacheMinute, obj);
    //    }

    //    //取 Object , 不存在將回傳 null
    //    public static object Object_Get(string MMCKey)
    //    {
    //        return DistCache.Get(MMCKey);
    //    }

    //    //移除 Object
    //    public static bool Object_Remove(string MMCKey)
    //    {
    //        //RemoveMMCList(MMCKey);
    //        return (bool)DistCache.Remove(MMCKey);
    //    }       
    //}


    // Supported MMC Object
    [Serializable]
    public class MMCObject
    {
        public string Key { get; set; }
        public DateTime SaveTime { get; set; }
        public int CacheMinute { get; set; }
        public string ClassName { get; set; }
    }


    ////////////////////////////////


    class BinaryTool11
    {
        /// <summary>
        /// Function to save object to external file
        /// </summary>
        /// <param name="_Object">object to save</param>
        /// <param name="_FileName">File name to save object</param>
        /// <returns>Return true if object save successfully, if not return false</returns>
        public static bool ObjectToFile(object _Object, string _FileName)
        {
            try
            {
                // create new memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // Serializes an object, or graph of connected objects, to the given stream.
                _BinaryFormatter.Serialize(_MemoryStream, _Object);

                // convert stream to byte array
                byte[] _ByteArray = _MemoryStream.ToArray();

                // Open file for writing
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray.ToArray(), 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                // cleanup
                _MemoryStream.Close();
                _MemoryStream.Dispose();
                _MemoryStream = null;
                _ByteArray = null;

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // Error occured, return null
            return false;
        }


        /// <summary>
        /// Function to get object from external file
        /// </summary>
        /// <param name="_FileName">File name to get object</param>
        /// <returns>object</returns>
        public static object FileToObject(string _FileName)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;

                // read entire file into buffer
                byte[] _ByteArray = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                // close file reader and do some cleanup
                _FileStream.Close();
                _FileStream.Dispose();
                _FileStream = null;
                _BinaryReader.Close();

                // convert byte array to memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_ByteArray);

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // set memory stream position to starting point
                _MemoryStream.Position = 0;

                // Deserializes a stream into an object graph and return as a object.
                return _BinaryFormatter.Deserialize(_MemoryStream);
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // Error occured, return null
            return null;
        }


    }


    ////////////////////////////


    public class TaiwanIdHelper
    {
        public static bool Verify(string user_pid) //檢查身分證字號
        {
            bool IsValid = true;

            int[] uid = new int[10]; //數字陣列存放身分證字號用
            int chkTotal; //計算總和用

            if (user_pid.Length == 10) //檢查長度
            {
                user_pid = user_pid.ToUpper(); //將身分證字號英文改為大寫

                //將輸入的值存入陣列中
                for (int i = 1; i < user_pid.Length; i++)
                {
                    uid[i] = Convert.ToInt32(user_pid.Substring(i, 1));
                }
                //將開頭字母轉換為對應的數值
                switch (user_pid.Substring(0, 1).ToUpper())
                {
                    case "A": uid[0] = 10; break;
                    case "B": uid[0] = 11; break;
                    case "C": uid[0] = 12; break;
                    case "D": uid[0] = 13; break;
                    case "E": uid[0] = 14; break;
                    case "F": uid[0] = 15; break;
                    case "G": uid[0] = 16; break;
                    case "H": uid[0] = 17; break;
                    case "I": uid[0] = 34; break;
                    case "J": uid[0] = 18; break;
                    case "K": uid[0] = 19; break;
                    case "L": uid[0] = 20; break;
                    case "M": uid[0] = 21; break;
                    case "N": uid[0] = 22; break;
                    case "O": uid[0] = 35; break;
                    case "P": uid[0] = 23; break;
                    case "Q": uid[0] = 24; break;
                    case "R": uid[0] = 25; break;
                    case "S": uid[0] = 26; break;
                    case "T": uid[0] = 27; break;
                    case "U": uid[0] = 28; break;
                    case "V": uid[0] = 29; break;
                    case "W": uid[0] = 32; break;
                    case "X": uid[0] = 30; break;
                    case "Y": uid[0] = 31; break;
                    case "Z": uid[0] = 33; break;
                }
                //檢查第一個數值是否為1.2(判斷性別)
                if (uid[1] == 1 || uid[1] == 2)
                {
                    chkTotal = (uid[0] / 10 * 1) + (uid[0] % 10 * 9);

                    int k = 8;
                    for (int j = 1; j < 9; j++)
                    {
                        chkTotal += uid[j] * k;
                        k--;
                    }

                    chkTotal += uid[9];

                    if (chkTotal % 10 != 0) IsValid = false; // 身分證字號錯誤                    
                }
                else IsValid = false; // 身分證字號錯誤                
            }
            else IsValid = false; // 身分證字號長度錯誤            

            return IsValid;
        }
    }


    ////////////////////////////

    public class StreamBinaryTool
    {
        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }

    ////////////////////////////

    public static class UrlTool
    {
        public static string Replace(string url, string key, string value)
        {
            return Regex.Replace(
                url,
                @"([?&]" + key + ")=[^?&]+",
                "$1=" + value);
        }

    }

    ////////////////////////////

    public static class LocaleTool
    {
        public static string Convert(string locale)
        {
            // KKday-Locale 轉換對應到 RFC 4646 
            var _culture = "zh-TW";
            switch (locale)
            {
                case "en":
                case "th":
                case "vi":
                case "ja":
                case "ko":
                    _culture = "en-US"; break;
                default: break; // 預設是繁體中文
            }
            return _culture;
        }
        /// <summary>
        /// RFC 4646 轉換對應到 KKday-Locale
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static string RfcToKKday(string locale)
        {
            string _culture = "zh-tw";
            switch (locale)
            {
                case "en-us": _culture = "en"; break;
                default: break;
            }
            return _culture;
        }

        public static string CultureToDisplay(string culture)
        {
            var display = "";

            switch (culture)
            {
                case "en-US": display = "English"; break; 
                case "zh-TW": display = "繁體中文"; break;
                default: break; // 預設是繁體中文
            }
            return display;
        }
    }

    ////////////////////////////

}