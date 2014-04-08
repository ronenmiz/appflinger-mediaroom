using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace AppFlinger
{
    public interface IAppFlingerCB
    {
        bool load(string url);
        bool cancelLoad();
        bool pause();
        bool play();
        bool seek(float time);
        bool getPaused(out bool paused);
        bool getSeeking(out bool seeking);
        bool getDuration(out float duration);
        bool getCurrentTime(out float time);
        bool getNetworkState(out int networkState);
        bool getReadyState(out int readyState);
        bool getMaxTimeSeekable(out float maxTimeSeekable);
        bool setRect(uint x, uint y, uint width, uint height);
        bool setVisible(bool visible);
    }

    public class AppFlinger
    {
        private IAppFlingerCB appFlingerCb;
        private Thread t;
        private volatile bool _shouldStop;
        private string _host;
        private string _sessionId;

        // Network state constants
        public const int APPFLINGER_NETWORK_STATE_EMPTY = 0;
        public const int APPFLINGER_NETWORK_STATE_IDLE = 1;
        public const int APPFLINGER_NETWORK_STATE_LOADING = 2;
        public const int APPFLINGER_NETWORK_STATE_LOADED = 3;
        public const int APPFLINGER_NETWORK_STATE_FORMAT_ERROR = 4;
        public const int APPFLINGER_NETWORK_STATE_NETWORK_ERROR = 5;
        public const int APPFLINGER_NETWORK_STATE_DECODE_ERROR = 6;

        // Ready state constants
        public const int APPFLINGER_READY_STATE_HAVE_NOTHING = 0;
        public const int APPFLINGER_READY_STATE_HAVE_METADATA = 1;
        public const int APPFLINGER_READY_STATE_HAVE_CURRENT_DATA = 2;
        public const int APPFLINGER_READY_STATE_HAVE_FUTURE_DATA = 3;
        public const int APPFLINGER_READY_STATE_HAVE_ENOUGH_DATA = 4;

        public static void Log(string msg)
        {
            Debug.WriteLine(msg);
        }

        private string processRPCRequest(Dictionary<string, object> req)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string errMsg = null;
            bool success;

            string service = req["service"].ToString();
            if (service == "load")
            {
                string url = req["URL"].ToString();
                Log(string.Format("service: {0} -- {1}", service,url));
                success = appFlingerCb.load(url);
            }
            else if (service == "cancelLoad")
            {
                Log("service: " + service);
                success = appFlingerCb.cancelLoad();
            }
            else if (service == "play")
            {
                Log("service: " + service);
                success = appFlingerCb.play();
            }
            else if (service == "pause")
            {
                Log("service: " + service);
                success = appFlingerCb.pause();
            }
            else if (service == "seek")
            {
                try
                {
                    float time = float.Parse(req["time"].ToString());
                    Log(string.Format("service: {0} -- {1}", service, time));
                    success = appFlingerCb.seek(time);
                }
                catch (Exception ex)
                {
                    errMsg = string.Format("Failed to parse arguments of service: {0}, reason: {1}", service, ex.Message);
                    Log(errMsg);
                    success = false;
                }
            }
            else if (service == "getPaused")
            {
                bool paused;
                Log("service: " + service);
                success = appFlingerCb.getPaused(out paused);
                if (success)
                    result.Add("paused", paused ? "1" : "0");
            }
            else if (service == "getSeeking")
            {
                bool seeking;
                Log("service: " + service);
                success = appFlingerCb.getSeeking(out seeking);
                if (success)
                    result.Add("seeking", seeking ? "1" : "0");
            }
            else if (service == "getDuration")
            {
                float duration;
                Log("service: " + service);
                success = appFlingerCb.getDuration(out duration);
                if (success)
                    result.Add("duration", duration.ToString());
            }
            else if (service == "getCurrentTime")
            {
                float time;
                Log("service: " + service);
                success = appFlingerCb.getCurrentTime(out time);
                if (success)
                    result.Add("currentTime", time.ToString());
            }
            else if (service == "getMaxTimeSeekable")
            {
                float time;
                Log("service: " + service);
                success = appFlingerCb.getMaxTimeSeekable(out time);
                if (success)
                    result.Add("maxTimeSeekable", time.ToString());
            }
            else if (service == "getNetworkState")
            {
                int state;
                Log("service: " + service);
                success = appFlingerCb.getNetworkState(out state);
                if (success)
                    result.Add("networkState", state.ToString());
            }
            else if (service == "getReadyState")
            {
                int state;
                Log("service: " + service);
                success = appFlingerCb.getReadyState(out state);
                if (success)
                    result.Add("readyState", state.ToString());
            }
            else if (service == "setRect")
            {
                uint x, y, width, height;
                try
                {
                    x = uint.Parse(req["x"].ToString());
                    y = uint.Parse(req["y"].ToString());
                    width = uint.Parse(req["width"].ToString());
                    height = uint.Parse(req["height"].ToString());
                    Log(string.Format("service: {0} -- {1}, {2}, {3}, {4}", service, x, y, width, height));
                    success = appFlingerCb.setRect(x, y, width, height);
                }
                catch (Exception ex)
                {
                    errMsg = string.Format("Failed to parse arguments of service: {0}, reason: {1}", service, ex.Message);
                    Log(errMsg);
                    success = false;
                }
            }
            else if (service == "setVisible")
            {
                string visible = req["visible"].ToString();
                Log(string.Format("service: {0} -- {1}", service, visible));
                success = appFlingerCb.setVisible(visible == "true" || visible == "yes" || visible == "1");
            }
            else
            {
                errMsg = "Unknown service: " + service;
                Log(errMsg);
                success = false;
            }

            if (success)
            {
                result.Add("result", "OK");
                result.Add("message", "");
            }
            else
            {
                result.Add("result", "ERROR");
                result.Add("message", (errMsg != null ? errMsg : ""));
            }

             // Encode to JSON
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(result);
        }

        private void ControlChannelThreadProc()
        {
            bool isFirst = true;
            string postMessage = null;

            while (!_shouldStop)
            {
                string url = string.Format("http://{0}/osb/session/control?session_id={1}", _host, HttpUtility.UrlEncode(_sessionId));
                if (isFirst)
                {
                    url += "&reset=1";
                    isFirst = false;
                }

                HttpWebResponse response = null;

                try
                {
                    // Make a long polling request to the control channel in order to process RPC requests (JSON formatted):
                    // - the first invocation has &reset=1 and no payload
                    // - subsequent invocation do not have &reset=1 and do have a payload (the response to the previously received RPC request)
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    if (postMessage != null)
                    {
                        request.ContentType = "test/json";
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] bytes = encoding.GetBytes(postMessage);
                        request.ContentLength = bytes.Length;

                        Stream requestStream = request.GetRequestStream();
                        if (requestStream != null)
                        {
                            requestStream.Write(bytes, 0, bytes.Length);
                            requestStream.Close();
                        }
                    }
                    else
                    {
                        request.ContentLength = 0;
                    }
                    response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK || response.ContentType != "text/json")
                    {
                        Log("Failed to connect to session, make sure the session exists");
                        response.Close();
                        return;
                    }
                }
                catch (WebException)
                {
                    Log("Failed to connect to server");
                    if (response != null)
                        response.Close();
                    return;
                }

                string json = null;

                try
                {
                    // Read the response
                    Stream responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);

                        // This will block till an actual response from the server is made
                        // TODO add timeout after few minutes to avoid half-open connection issues
                        json = readStream.ReadToEnd();
                        readStream.Close();
                    }
                    response.Close();
                }
                catch (IOException)
                {
                    Log("Failed to read response");
                    if (response != null)
                        response.Close();
                    return;
                }
                catch (WebException)
                {
                    // This is most likely a timeout
                    if (response != null)
                        response.Close();
                    postMessage = null;
                    isFirst = true;
                    continue;
                }

                Dictionary<string, object> result;

                try
                {
                    // Parse the response (JSON) into a dictionary
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    result = (Dictionary<string, object>)serializer.DeserializeObject(json);
                }
                catch (ArgumentException)
                {
                    Log(String.Format("Failed to parse json: {0}", json));
                    return;
                }

                try
                {
                    // Process the received RPC request and return the result via the next HTTP long polling request
                    postMessage = processRPCRequest(result);
                }
                catch (KeyNotFoundException)
                {
                    Log(String.Format("Failed to parse json: {0}", json));
                    return;
                }
            }
        }

        public AppFlinger()
        {
            t = null;
            appFlingerCb = null;
            _shouldStop = false;
            _host = null;
            _sessionId = null;
        }

        public void AppFlingerStart(string host, string sessionId, IAppFlingerCB cb)
        {
            _shouldStop = false;
            appFlingerCb = cb;
            _host = host;
            _sessionId = sessionId;
            t = new Thread(new ThreadStart(ControlChannelThreadProc));
            t.IsBackground = true;
            t.Start();
        }

        public void AppFlingerStop()
        {
            _shouldStop = true;
            t.Join();
        }
    }
}
