using System;
using System.IO;
using System.Data;
using System.Net;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace AppFlinger
{
    public partial class GetVideo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string videoURL = Global.VideoURL;

            if (videoURL == null)
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(videoURL);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    AppFlinger.Log("Failed to connect to session, make sure the session exists");
                    resp.Close();
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                Response.ContentType = resp.ContentType;
                Response.AddHeader("Content-Length",  resp.ContentLength.ToString());

                Stream responseStream = resp.GetResponseStream();
                if (responseStream != null)
                {
                    int count = 0;
                    while (true)
                    {
                        int data = responseStream.ReadByte();
                        if (data < 0)
                            break;

                        Response.OutputStream.WriteByte((byte)data);
                        count++;
                        if (count > 4096)
                        {
                            Response.Flush();
                            count = 0;
                        }
                    }
                    responseStream.Close();
                }
                resp.Close();
            }
            catch (Exception ex)
            {
                AppFlinger.Log("Failed to proxy content -- " + ex.Message);
            }
        }
    }
}