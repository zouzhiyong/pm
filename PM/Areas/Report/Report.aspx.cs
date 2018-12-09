using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace PM.Areas.Report
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            reportViewer1.ServerReport.ReportServerUrl= new Uri("http://192.168.1.206/ReportServer");
            reportViewer1.ServerReport.ReportPath = "/rdlc/pm";
        }
    }
}