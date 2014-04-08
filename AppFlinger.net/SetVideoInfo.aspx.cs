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
    public partial class SetVideoInfo : System.Web.UI.Page
    {
        // Parse string of the form "x days y hours z minutes"
        private float durationToSeconds(string duration)
        {
            string[] tags = { "days", "hours", "minutes" };
            int tagIx = 0;
            float rv = 0, num = -1;

            string[] subs = duration.Split(' ');
            for (int j=0; j<subs.Length; j++)
            {
                string s = subs[j];

                // We exepct a number at even positions
                if (j % 2 == 0)
                {
                    try
                    {
                        num = float.Parse(s);
                    }
                    catch (Exception ex)
                    {
                        AppFlinger.Log(string.Format("Failed to parse duration: {0}, {1}", duration, ex.Message));
                        return 0;
                    }
                    continue;
                }

                // We expect a tag at odd positions and we should have seen a number by now

                if (num < 0)
                {
                    AppFlinger.Log(string.Format("Failed to parse duration: {0}", duration));
                    return 0;
                }

                bool found = false;

                // Look for a tag
                for (int i = tagIx; i < tags.Length; i++)
                {
                    if (s == tags[i])
                    {
                        found = true;
                        tagIx = i;
                        break;
                    }
                }
                if (!found)
                {
                    AppFlinger.Log(string.Format("Failed to parse duration: {0}", duration));
                    return 0;
                }

                // Add the num based on its units

                if (s == "days")
                    rv += num * 24 * 60 * 60;
                else if (s == "hours")
                    rv += num * 60 * 60;
                else if (s == "minutes")
                    rv += num * 60;
                else
                {
                    AppFlinger.Log(string.Format("Failed to parse duration: {0}", duration));
                    return 0;
                }
                num = -1;
            }
            return rv;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["duration"] != null)
            {
                // TODO unfortunately mediaroom does not return seconds so the duration we have is in resolution of minutes
                Global.VideoDuration = durationToSeconds(Request.QueryString["duration"]);
            }
        }
    }
}