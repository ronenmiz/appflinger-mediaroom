using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace AppFlinger
{
    public partial class GetVideoURL : System.Web.UI.Page
    {
        private string EscapeXML(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            string returnString = s;
            returnString = returnString.Replace("'", "&apos;");
            returnString = returnString.Replace("\"", "&quot;");
            returnString = returnString.Replace(">", "&gt;");
            returnString = returnString.Replace("<", "&lt;");
            returnString = returnString.Replace("&", "&amp;");

            return returnString;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string videoURL = Global.VideoURL;

            if (videoURL != null && !videoURL.EndsWith(".mp4"))
            {
                videoURL += (videoURL.Contains("?") ? "&" : "?") + "_ext=vid.mp4";
            }

            // For now we are proxying the video and not providing the original URL because for some unknown reason mediaroom does not play the YouTube video URLs but
            // plays those videos fine when proxied
            // TODO Do not proxy once mediaroom can handle those URLs
            if (videoURL != null)
            {
                videoURL = string.Format("http://{0}:{1}/GetVideo.aspx?guid={2}&name=vid.mp4", Request.Url.Host, Request.Url.Port, System.Guid.NewGuid());
            }

            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <result>" + (videoURL == null ? "tune:-1" : EscapeXML(videoURL)) + "</result>");
        }
    }
}