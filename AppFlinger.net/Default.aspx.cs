using System;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.TV.TVControls;
using Microsoft.TV.TVControls.Events;

namespace AppFlinger
{
    public partial class Default : System.Web.UI.Page
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["host"] == null || Request.QueryString["session_id"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.SuppressContent = true;
                Response.StatusDescription = "Missing query string argument(s) -- host and/or session_id";
                return;
            }

            string host = Request.QueryString["host"].ToString();
            string sessionId = Request.QueryString["session_id"].ToString();

            // Set the image URL to be the AppFlinger session snapshot URL (JPEG)
            TVImage img = (TVImage) this.Page.FindControl("TVImage1");
            img.Url = string.Format("http://{0}/osb/session/snapshot?session_id={1}", host, HttpUtility.UrlEncode(sessionId));

            // Start the control channel
            Global.AppFlingerStart(host, sessionId);
        }
    }
}
