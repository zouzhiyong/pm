<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="PM.Areas.Report.Report" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

</head>
<body style="margin: 0; overflow: hidden;">

    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer
                ID="reportViewer1"
                runat="server"
                Font-Names="Verdana"
                Font-Size="8pt"
                WaitMessageFont-Names="Verdana"
                AsyncRendering="False"
                SizeToReportContent="True"
                WaitMessageFont-Size="12px"
                Height="100%"
                ZoomMode="Percent"
                KeepSessionAlive="True" ShowRefreshButton="False" ProcessingMode="Remote">
            </rsweb:ReportViewer>
        </div>
    </form>
    <script type="text/javascript" src="../../Scripts/jquery-2.2.4.min.js"></script>
    <script>
        Sys.Application.add_load(function () {
            $find("reportViewer1").add_propertyChanged(viewerPropertyChanged);
        });
        function viewerPropertyChanged(sender, e) {
            $("#reportViewer1_ctl04").width($(window).width());
            $("#reportViewer1_ctl05").width($(window).width());
            $("#reportViewer1_ToggleParam").width($(window).width());
            $("#reportViewer1_ctl09").width($(window).width());
            $("#reportViewer1_ctl09").height($(window).height() - $("#reportViewer1_ctl09").offset().top-5);
            $("#reportViewer1_ctl09").css("overflow", "auto");
            $(window).resize(function () {
                $("#reportViewer1_ctl04").width($(window).width());
                $("#reportViewer1_ctl05").width($(window).width());
                $("#reportViewer1_ToggleParam").width($(window).width());
                $("#reportViewer1_ctl09").width($(window).width());
                $("#reportViewer1_ctl09").height($(window).height() - $("#reportViewer1_ctl09").offset().top - 5);
                $("#reportViewer1_ctl09").css("overflow", "auto");
            });
        }
        //$(function () {
        //    //reportViewer1_ctl04
        //    //reportViewer1_ctl05
        //    //reportViewer1_ToggleParam

        //    $("#reportViewer1_ctl04").width($(window).width());
        //    $("#reportViewer1_ctl05").width($(window).width());
        //    $("#reportViewer1_ToggleParam").width($(window).width());
        //    $(window).resize(function () {
        //        $("#reportViewer1_ctl04").width($(window).width());
        //        $("#reportViewer1_ctl05").width($(window).width());
        //        $("#reportViewer1_ToggleParam").width($(window).width());
        //    });
        //})
    </script>
</body>
</html>
