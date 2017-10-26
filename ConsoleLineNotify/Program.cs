using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isRock.LineNotify;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace ConsoleLineNotify
{
    class Program
    {

        private const int TIMEOUT = 1000;
        
        static void Main(string[] args)
        {
            GOGOGO();
        }

        private static void GOGOGO()
        {
            while (1 == 1)
            {
                //Console.Write("開始囉 \n");
                
                string strUrl_1 = @"https://www.costco.com.tw/Baby-Care-Kids-Toys/Toys/Infant-%26-Preschool/VTech-Sit-To-Stand-Ultimate-Alphabet-Train/p/105123";
                string strUrl_2 = @"https://www.ptt.cc/bbs/Stock/index.html";

                //LoadInformation(@"https://goo.gl/wVWSe8");

                GetUrlContentKeyword(strUrl_1, "已售完","Y","有貨了，快買!");
                GetUrlContentKeyword(strUrl_2, "chengwaye","Y", "chengwaye 有新文章了");
                
                //抓股票資料
                //getStockPrice();

                System.Threading.Thread.Sleep(5000);
                //Console.Write("下一輪GO \n");
            }
        }

        public static String LoadInformation(String url)
        {
            HttpWebRequest myWebRequest = null;
            HttpWebResponse myWebResponse = null;
            Stream receiveStream = null;
            Encoding encode = null;
            StreamReader readStream = null;
            string text = null;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                myWebRequest = HttpWebRequest.Create(url) as HttpWebRequest;

                myWebRequest.Timeout = TIMEOUT;
                myWebRequest.ReadWriteTimeout = TIMEOUT;

                myWebResponse = myWebRequest.GetResponse() as HttpWebResponse;
                receiveStream = myWebResponse.GetResponseStream();
                encode = System.Text.Encoding.GetEncoding("utf-8");
                readStream = new StreamReader(receiveStream, encode);
                text = readStream.ReadToEnd().ToLower();
                if (!text.Contains("已售完"))
                {
                    //透過LineNotSDK中的API，傳送
                    sendLineMSG(@"https://goo.gl/wVWSe8" + " COSTCO的車有貨了，快去買!!!");
                }
                if (readStream != null) readStream.Close();
                if (receiveStream != null) receiveStream.Close();
                if (myWebResponse != null) myWebResponse.Close();
            }
            catch (Exception ex)
            {

                //Do Something
            }
            finally
            {
                readStream = null;
                receiveStream = null;
                myWebResponse = null;
                myWebRequest = null;
            }
            return text;
        }

        public static String LoadInformation2(String url)
        {
            try
            {
                string strFoucsid = "chengwaye";
                var Loadurl = url;
                var web = new HtmlWeb();
                var doc = web.Load(Loadurl);

                if (doc.ParsedText.Contains(strFoucsid))
                {
                    //透過LineNotSDK中的API，傳送
                    sendLineMSG(@"https://www.ptt.cc/bbs/Stock/index.html  " + strFoucsid + "有新文章了!");
                }
            }
            catch (Exception ex)
            {
                //Do Something
            }
            finally
            {
            }
            return "";
        }

        public static String GetUrlContentKeyword(String strUrl,string strKeyword,string strHaveKeyword,string strShowMessage)
        {
            try
            {
                var Loadurl = strUrl;
                var web = new HtmlWeb();
                web.StreamBufferSize = 1024000;

                web.PreRequest += request =>
                {
                    request.CookieContainer = new System.Net.CookieContainer();
                    return true;
                };
                var doc = web.Load(Loadurl);

                if (strHaveKeyword == "Y")
                {
                    if (doc.ParsedText.Contains(strKeyword))
                    {
                        //透過LineNotSDK中的API，傳送
                        sendLineMSG(strUrl + " " + strShowMessage);
                        //var ret = Utility.SendNotify("yyz6EyX8qUUJGtQH6NJU1ylyLTNE2VsvGoYOYX0j3XO", @"https://www.ptt.cc/bbs/Stock/index.html  " + strFoucsid + "有新文章了!");
                        //var ret = Utility.SendNotify("yyz6EyX8qUUJGtQH6NJU1ylyLTNE2VsvGoYOYX0j3XO", strUrl + " " + strShowMessage);
                    }
                }
                else
                {
                    if (!doc.ParsedText.Contains(strKeyword))
                    {
                        //透過LineNotSDK中的API，傳送
                        sendLineMSG(@"https://www.ptt.cc/bbs/Stock/index.html  " + strShowMessage);
                         //var ret = Utility.SendNotify("yyz6EyX8qUUJGtQH6NJU1ylyLTNE2VsvGoYOYX0j3XO", @"https://www.ptt.cc/bbs/Stock/index.html  " + strShowMessage);
                         //   ret = Utility.SendNotify("1gf8RVEtKX8iewAw902nyefx87hTXoOKJzyG3wPqTGD", strUrl + " " +  strShowMessage);
                    }
                }

                //else
                //{
                //    //Console.Write("沒有新文章 \n");
                //}
            }
            catch (Exception ex)
            {
                //Do Something
            }
            finally
            {
            }
            return "";
        }

        private static void sendLineMSG(string strMSG)
        {
            var ret = Utility.SendNotify("yyz6EyX8qUUJGtQH6NJU1ylyLTNE2VsvGoYOYX0j3XO", strMSG);
        }

        private static void getStockPrice()
        {
            string strFinal = "";
            //指定來源網頁
            WebClient url = new WebClient();
            //將網頁來源資料暫存到記憶體內
            MemoryStream ms = new MemoryStream(url.DownloadData("http://tw.stock.yahoo.com/q/q?s=3008"));
            //以奇摩股市為例http://tw.stock.yahoo.com，3008以大立光為例

            // 使用預設編碼讀入 HTML
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);

            //XPath 來解讀它 /html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);

            // 取得個股標頭
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("./tr[1]/th");
            // 取得個股數值
            string[] txt = doc.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');

            int i = 0;

            // 輸出資料
            foreach (HtmlNode nodeHeader in htnode)
            {
                if (i<6)
                {
                    //將 "加到投資組合" 這個字串過濾掉
                    strFinal += (nodeHeader.InnerText + " " + txt[i].Trim().Replace("加到投資組合", ""));
                    //Response.Write(nodeHeader.InnerText + ":" + txt[i].Trim().Replace("加到投資組合", "") + "<br />");
                }
                i++;
            }

            //透過LineNotSDK中的API，傳送
            sendLineMSG(strFinal);

            //清除資料
            doc = null;
            url = null;
            ms.Close();

            //網頁更新
            //HtmlMeta meta = new HtmlMeta();
            //meta.Attributes.Add("http-equiv", "refresh");
            //設定秒數，5秒後執行頁面更新
            //meta.Content = "5";
            //this.Header.Controls.Add(meta);
        }
    }
}