using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace KKday.Web.OCBT.AppCode
{
    public class SlackHelper {
        HttpStatusCode code = HttpStatusCode.OK;

        public string Post(SlackMessageQuery query) {
            string strUrl = $"https://hooks.slack.com/services/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";
            string strResult = this.CallAPI(strUrl, "POST", JsonConvert.SerializeObject(query), out code);
            return strResult;
        }


        protected string CallAPI(string strUrl, string strHttpMethod, string strPostContent, out HttpStatusCode code) {
            HttpWebRequest request = HttpWebRequest.Create(strUrl) as HttpWebRequest;
            request.Method = strHttpMethod;
            code = HttpStatusCode.OK;

            if (strPostContent != "" && strPostContent != string.Empty) {
                request.KeepAlive = true;
                request.ContentType = "application/json";

                byte[] bs = Encoding.UTF8.GetBytes(strPostContent);
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                    
            }

            string strReturn = "";
            try {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (var respStream = response.GetResponseStream())
                {
                    strReturn = new StreamReader(respStream).ReadToEnd();
                    respStream.Close();
                }

            } catch (Exception e) {
                strReturn = e.Message;
                code = HttpStatusCode.NotFound;
            }

            return strReturn;
        }

        public string SlackPost(string SlackText)
        {
            SlackMessageQuery q = new SlackMessageQuery();
            q.text = SlackText;
            string strUrl = $"https://hooks.slack.com/services/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";
            string strResult = this.CallAPI(strUrl, "POST", JsonConvert.SerializeObject(q), out code);
            return strResult;
        }

        //緊急發送使用
        public string SlackPost(string request_uuid, string method, string call_method, string slack_title, string slack_text)
        {
            //function_type  b2d-一般使用, billing-額度用，
            SlackMessageObj obj = new SlackMessageObj();
            List<SlackMessageBlocks> blackList = new List<SlackMessageBlocks>();

            //header
            SlackMessageBlocks header = new SlackMessageBlocks();
            header.type = "header";
            SlackMessageText headerTest = new SlackMessageText();
            headerTest.text = slack_title;
            header.text = headerTest;

            //section1
            SlackMessageBlocks section1 = new SlackMessageBlocks();
            section1.type = "section";
            List<SlackMessageFields> filedsLst = new List<SlackMessageFields>();
            SlackMessageFields fields_1 = new SlackMessageFields();
            fields_1.text = "*env:*\n" + Website.Instance.Configuration["app_env"];
            filedsLst.Add(fields_1);
            SlackMessageFields fields_2 = new SlackMessageFields();
            fields_2.text = "*project:*\n" + Assembly.GetCallingAssembly().GetName().Name;
            filedsLst.Add(fields_2);
            section1.fields = filedsLst;

            //setction2
            SlackMessageBlocks section2 = new SlackMessageBlocks();
            section2.type = "section";
            filedsLst = new List<SlackMessageFields>();
            fields_1 = new SlackMessageFields();
            fields_1.text = "*method:*\n" + method;
            filedsLst.Add(fields_1);
            fields_2 = new SlackMessageFields();
            fields_2.text = "*call_method:*\n" + call_method;
            filedsLst.Add(fields_2);
            section2.fields = filedsLst;

            //setction3
            SlackMessageBlocks section3 = new SlackMessageBlocks();
            section3.type = "section";
            filedsLst = new List<SlackMessageFields>();
            fields_1 = new SlackMessageFields();
            fields_1.text = "*request_uuid:*\n" + request_uuid;
            filedsLst.Add(fields_1);
            section3.fields = filedsLst;

            //setction4
            SlackMessageBlocks section4 = new SlackMessageBlocks();
            section4.type = "section";
            SlackMessageText contentText = new SlackMessageText();
            contentText.text = slack_text;
            section4.text = contentText;

            //filedsLst = new List<SlackMessageFields>();
            //fields_1 = new SlackMessageFields();
            //fields_1.text = "*message:*\n" + slack_text;
            //filedsLst.Add(fields_1);
            //section4.fields = filedsLst;

            blackList.Add(header);
            blackList.Add(section1);
            blackList.Add(section2);
            blackList.Add(section3);
            blackList.Add(section4);

            obj.blocks = blackList;

            string strUrl =  $"https://hooks.slack.com/services/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";
            string strResult = this.CallAPI(strUrl, "POST", JsonConvert.SerializeObject(obj,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), out code);
            return strResult;
        }

        //Bililng發送使用
        public string SlackPostforBilling(string request_uuid, string action, string slack_title, string slack_text)
        {
            //function_type  b2d-一般使用, billing-額度用，
            SlackMessageObj obj = new SlackMessageObj();
            List<SlackMessageBlocks> blackList = new List<SlackMessageBlocks>();

            //header
            SlackMessageBlocks header = new SlackMessageBlocks();
            header.type = "header";
            SlackMessageText headerTest = new SlackMessageText();
            headerTest.text = slack_title;
            header.text = headerTest;

            //section1
            SlackMessageBlocks section1 = new SlackMessageBlocks();
            section1.type = "section";
            List<SlackMessageFields> filedsLst = new List<SlackMessageFields>();
            SlackMessageFields fields_1 = new SlackMessageFields();
            fields_1.text = "*env:*\n" + Website.Instance.Configuration["app_env"];
            filedsLst.Add(fields_1);
            SlackMessageFields fields_2 = new SlackMessageFields();
            fields_2.text = "*action:*\n" + action;
            filedsLst.Add(fields_2);
            section1.fields = filedsLst;

            //setction2
            //SlackMessageBlocks section2 = new SlackMessageBlocks();
            //section2.type = "section";
            //filedsLst = new List<SlackMessageFields>();
            //fields_1 = new SlackMessageFields();
            //fields_1.text = "*method:*\n" + method;
            //filedsLst.Add(fields_1);
            //fields_2 = new SlackMessageFields();
            //fields_2.text = "*call_method:*\n" + call_method;
            //filedsLst.Add(fields_2);
            //section2.fields = filedsLst;

            //setction3
            SlackMessageBlocks section3 = new SlackMessageBlocks();
            section3.type = "section";
            filedsLst = new List<SlackMessageFields>();
            fields_1 = new SlackMessageFields();
            fields_1.text = "*request_uuid:*\n" + request_uuid;
            filedsLst.Add(fields_1);
            section3.fields = filedsLst;

            //setction4
            SlackMessageBlocks section4 = new SlackMessageBlocks();
            section4.type = "section";
            SlackMessageText contentText = new SlackMessageText();
            contentText.text = slack_text;
            section4.text = contentText;

            //filedsLst = new List<SlackMessageFields>();
            //fields_1 = new SlackMessageFields();
            //fields_1.text = "*message:*\n" + slack_text;
            //filedsLst.Add(fields_1);
            //section4.fields = filedsLst;

            blackList.Add(header);
            blackList.Add(section1);
            //blackList.Add(section2);
            blackList.Add(section3);
            blackList.Add(section4);

            obj.blocks = blackList;

            string strUrl = $"https://hooks.slack.com/services/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";
            string strResult = this.CallAPI(strUrl, "POST", JsonConvert.SerializeObject(obj,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }), out code);
            return strResult;
        }
    }

    public class SlackMessageQuery {
        public string text { get; set; }
    }

    public class SlackMessageObj
    {
        public List<SlackMessageBlocks> blocks { get; set; }
    }

    public class SlackMessageBlocks
    {
        public string type { get; set; }
        public SlackMessageText text { get; set; }
        public List<SlackMessageFields> fields { get; set; }
    }

    public class SlackMessageText
    {
        public string type { get; set; } = "plain_text";
        public string text { get; set; }
        public Boolean emoji { get; set; } = true;


    }
    public class SlackMessageFields
    {
        public string type { get; set; } = "mrkdwn";
        public string text { get; set; }
    }
}
