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
    public partial class GetVideoState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool videoStateChanged = Global.VideoStateChanged;

            if (videoStateChanged)
                Global.VideoStateChanged = false;

            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <result>" + (!videoStateChanged ? "" : Global.VideoState) + "</result>");
        }
    }
}