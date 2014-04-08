<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AppFlinger.Default" %>

<%@ Register Assembly="TVControls" Namespace="Microsoft.TV.TVControls.Actions" TagPrefix="mrml" %>
<%@ Register Assembly="TVControls" Namespace="Microsoft.TV.TVControls" TagPrefix="mrml" %>

<mrml:TVPage ID="TVPage1" runat="server" style="position:absolute; width: 1280px; height: 720px;" PageLayout="WideScreen" OnEnter="doRefreshImg doRefreshData">
    
    <mrml:TVScript ID="TVScript1" runat="server">
        <Script>
            function getVideoDuration()
            {
                var duration = getProperty("TVText2", "text");
                if (duration != "")
                    invokeAction(null, "sendDurartion", "SetVideoInfo.aspx?duration=" + encodeURIComponent(duration));
            }

            function checkVideoState()
            {
                var videoState = getProperty("TVText1", "text");
                if (videoState != "")
                {
                    if (videoState == "load")
                    {
                        invokeAction("TVXmlDataSourceVideo", "refreshVideo");
                    }
                    if (videoState == "unload")
                    {
                        invokeAction("TVXmlDataSourceVideo", "refreshVideo");
                        setProperty("TVImage1", "alpha", 1.0); // no video is loaded so image is opaque
                    }
                    else if (videoState == "pause")
                    {
                        setProperty("TVImage1", "alpha", 1.0); // video is paused so image is opaque
                        invokeAction("TVVideo1", "pauseVideo");
                    }
                    else if (videoState == "play") {
                        setProperty("TVImage1", "alpha", 0.5); // video is playing so image is partially transparent
                        invokeAction("TVVideo1", "playVideo");
                    }
                    else
                    {
                        alert("ERROR- Unknown video state");
                        return;
                    }
                }
            }
        </Script>
    </mrml:TVScript>

    <mrml:TVXmlDataSource runat="server" ID="TVXmlDataSourceData" Url="GetVideoState.aspx">
    </mrml:TVXmlDataSource>
    <mrml:TVText ID="TVText1" runat=server DataBinder="XmlDataBinder,TVXmlDataSourceData,result" Text="0" OnChange="CheckVideoState" IsVisible="false">
    </mrml:TVText>

    <mrml:TVFullScreenDataSource ID="TVFullScreenDataSource1" runat="server">
    </mrml:TVFullScreenDataSource>
    <mrml:TVText ID="TVText2" runat="server" DataBinder="FullScreenDataBinder,TVFullScreenDataSource1,duration" OnChange="GetVideoDuration" IsVisible="false">
    </mrml:TVText>

    <mrml:TVXmlDataSource runat="server" ID="TVXmlDataSourceDuration" Url="SetVideoInfo.aspx" AutoLoad="false">
    </mrml:TVXmlDataSource>

    <mrml:TVXmlDataSource runat="server" ID="TVXmlDataSourceVideo" Url="GetVideoURL.aspx">
    </mrml:TVXmlDataSource>
    <mrml:TVVideo ID="TVVideo1" runat=server style="position:absolute; top: 0px; left: 0px; width: 1280px; height: 720px;" TuneURL="tune:-1" DataBinder="XmlDataBinder,TVXmlDataSourceVideo,result" IsPip="false">
    </mrml:TVVideo>

    <mrml:TVImage ID="TVImage1" runat="server" UseCache="false" style="position: absolute; top: 0px; left: 0px; width: 1280px; height: 720px;"  Alpha="1.0">
    </mrml:TVImage>

    <mrml:TVActions ID="TVActions1" runat="server" Version="5">
        <mrml:CompositeAction Name="doRefreshImg" X="0" Y="0">
            <mrml:RefreshAction Name="refreshImg" Target="TVImage1" X="0" Y="0" />
            <mrml:TimeoutAction Action="doRefreshImg" Delay="250" Name="timeout" X="0" Y="0" />
        </mrml:CompositeAction>
        <mrml:CompositeAction Name="doRefreshData" X="0" Y="0">
            <mrml:RefreshAction Name="refreshData" Target="TVXmlDataSourceData" X="0" Y="0" />
            <mrml:TimeoutAction Action="doRefreshData" Delay="1000" Name="timeout" X="0" Y="0" />
        </mrml:CompositeAction>
        <mrml:ScriptAction Function="checkVideoState" Name="CheckVideoState" X="0" Y="0" />
        <mrml:RefreshAction Name="refreshVideo" Target="TVXmlDataSourceVideo" X="0" Y="0" />
        <mrml:AVPlayAction Name="pauseVideo" PlaybackSpeed="0" Target="TVVideo1" X="0" Y="0" />
        <mrml:AVPlayAction Name="playVideo" PlaybackSpeed="1" Target="TVVideo1" X="0" Y="0" />
        <mrml:ScriptAction Function="getVideoDuration" Name="GetVideoDuration" X="0" Y="0" />
        <mrml:RefreshAction Name="sendDurartion" Target="TVXmlDataSourceDuration" X="0" Y="0" />
    </mrml:TVActions>

</mrml:TVPage>
