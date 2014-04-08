appflinger-mediaroom
====================

appflinger-mediaroom is an AppFlinger client for the Mediaroom IPTV platform. It comes with appflinger.cs - the C# client SDK for AppFlinger (www.appflinger.com).

It was tested with Mediaroom Application Development Kit (ADK) 3.4 and .Net 4.0.

There are also a Javascript client SDK (see http://github.com/ronenmiz/appflinger-js), a C client SDK (available in binary form only), and a full client for the Raspberry Pi (please contact us to gain access to it). 

The server side code is closed source and is available for licensing from TVersity, please contact us for more information.

### What is AppFlinger?

AppFlinger (www.appflinger.com) is an HTML5 browser (based on Chromium) running in the cloud and delivered to client devices as a video stream,. It is also a full solution for aggregation, monetization and delivery of TV apps (based on HTML5) to set-top boxes and smart TVs. AppFlinger utilizes the cloud for running the HTML5 TV apps and delivers them as a video stream to target devices with unprecedented quality and responsiveness.

AppFlinger makes it possible to run HTML5 TV apps on any device and in the same time makes the full power of desktop-grade HTML5 browsers available to TV app developers.

AppFlinger is available out of the box with many premium TV apps.

If you are interested in AppFlinger, please contact us.

If you are the developer of a TV App in HTML5 and would like to reach the existing deployment base of AppFlinger, please contact us as well.

### AppFlinger Uniqueness

AppFlinger is unique in its ability to deliver desktop grade HTML5 browser experience with just having a solid video player on the client and very low bandwidth and CPU requirement on the server (this is unlike cloud gaming where CPU and bandwidth requirements on the server are typically cost-prohibitive).

This is achieved by breaking the browser experience to two video streams. The UI video stream is created in real-time by the server (by encoding the content of the browser window) and delivered to the client for low-latency rendering. The other video stream is the actual video played via the HTML5 media element (aka HTML5 video tag).

This approach allows HTML5 TV apps, like the one at www.youtube.com/tv, to run in the cloud and be delivered to any client device (including ultra low-end and legacy devices).

### Notes about the Mediaroom client

This implementation is designed to run on in basically any Mediaroom device and given that Mediaroom does not support low latency video playback, the UI is rendered as a sequence of JPEG images. Adapting the client to render the UI as a low-latency video stream would be trivial, as long as Mediaroom supports low-latency video playback.

Additionally, the current Mediaroom client implementation has the following limitations:
- It lacks support for video seeking.
- It provides the duration of videos in resolution of minutes (due to Mediaroom limitations).
- It proxies videos as opposed to playing them directly from the source. This is because, at least for www.youtube.com/tv, the Mediaroom simulator (included in ADK 3.4) refused to play those URLs.
- When running in the Mediaroom simulator, it has to be online or else it won't play mp4 videos.
- The Mediaroom simulator has A/V synch issues with MP4 files and those will be evident when the HTML5 TV app utilizes MP4 (most do, including YouTube).
- The current client implementation is intended for demonstration purposes and as such it supports a single user only.
- The URL to put into the simulator is as follows:
  - page:http://<host:port of Mediaroom app>/Default.aspx?host=<host:port of AppFlinger server>&session_id=<session id>&fps=<frames per second>
  - Where:
  - The host:port of the Mediaroom app should be replaced with the IIS server host and port.
  - The host:port of AppFlinger server should be replaced with the cloud server information provided by TVersity.
  - The session id should be the session created for the given user.
  - The fps should be a number between 1 to 5, depending on what the network and set-top box can handle.