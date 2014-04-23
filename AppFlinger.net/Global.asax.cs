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
    public class Global : System.Web.HttpApplication
    {
        private static volatile string _videoURL = null;
        private static volatile bool _videoStateChanged = false;
        private static volatile string _videoState = "";
        private static volatile float _duration = -1;
        private static bool _loaded = false;
        private static bool _paused = true;
        private static AppFlinger _appflinger = null;
        private static AppFlingerCB _appFlingerCB;

        public static bool VideoStateChanged
        {
            get
            {
                return _videoStateChanged;
            }
            set
            {
                // TODO should be made thread safe
                _videoStateChanged = value;
            }
        }

        public static string VideoState
        {
            get
            {
                return _videoState;
            }
        }

        public static string VideoURL
        {
            get
            {
                return _videoURL;
            }
        }

        public static float VideoDuration
        {
            get
            {
                return _duration;
            }
            set
            {
                // TODO should be made thread safe
                _duration = value;
            }
        }

        public class AppFlingerCB : IAppFlingerCB
        {
            public bool load(string url)
            {
                _loaded = true;
                _videoURL = url;
                _videoState = "load";
                _videoStateChanged = true;
                _duration = -1;
                return true;
            }

            public bool cancelLoad()
            {
                 _loaded = false;
                _videoURL = null;
                _videoState = "unload";
                _videoStateChanged = true;
                _duration = -1;
                return true;
            }

            public bool pause()
            {
                if (_loaded)
                {
                    _paused = true;
                    _videoState = "pause";
                    _videoStateChanged = true;
                    return true;
                }
                else
                    return false;
            }

            public bool play()
            {
                if (_loaded)
                {
                    _paused = false;
                    _videoState = "play";
                    _videoStateChanged = true;
                     return true;
                }
                else
                    return false;
            }

            public bool seek(float time)
            {
                if (_loaded)
                {
                    // TODO implement
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool getPaused(out bool paused)
            {
                if (_loaded)
                {
                    paused = _paused;
                    return true;
                }
                else
                {
                    paused = false;
                    return false;
                }
            }

            public bool getSeeking(out bool seeking)
            {
                seeking = false;
                if (_loaded)
                {
                    return true;
                }
                else
                    return false;
            }

            public bool getDuration(out float duration)
            {
                float d = _duration;
                if (_loaded &&  d >= 0)
                {
                    duration = d;
                    return true;
                }
                else
                {
                    duration = 0;
                    return false;
                }
            }

            public bool getCurrentTime(out float time)
            {
                time = 0;
                if (_loaded)
                {
                    // TODO implement
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool getNetworkState(out int networkState)
            {
                networkState = _loaded ? AppFlinger.APPFLINGER_NETWORK_STATE_LOADED : AppFlinger.APPFLINGER_NETWORK_STATE_EMPTY;
                return true;
            }

            public bool getReadyState(out int readyState)
            {
                readyState = _loaded ? AppFlinger.APPFLINGER_READY_STATE_HAVE_ENOUGH_DATA : AppFlinger.APPFLINGER_READY_STATE_HAVE_NOTHING;
                return true;
            }

            public bool getMaxTimeSeekable(out float maxTimeSeekable)
            {
                float d = _duration;
                if (_loaded && d >= 0)
                {
                    maxTimeSeekable = d;
                    return true;
                }
                else
                {
                    maxTimeSeekable = 0;
                    return false;
                }
            }

            public bool setRect(uint x, uint y, uint width, uint height)
            {
                // TODO implement
                return true;
            }

            public bool setVisible(bool visible)
            {
                // TODO implement
                return true;
            }
        }

        public static void AppFlingerStart(string host, string sessionId)
        {
            // Reset the state variables
            _videoURL = null;
            _videoStateChanged = false;
            _videoState = "";
            _duration = -1;
            _loaded = false;
            _paused = true;

            if (_appflinger == null)
            {
                _appflinger = new AppFlinger();
                _appFlingerCB = new AppFlingerCB();
                _appflinger.AppFlingerStart(host, sessionId, _appFlingerCB);
            }
            else
            {
                _appflinger.AppFlingerStopAsync();
                _appflinger.AppFlingerStart(host, sessionId, _appFlingerCB);
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
