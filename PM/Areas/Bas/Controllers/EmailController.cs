using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PM.Models;

namespace PM.Areas.Bas.Controllers
{
    public class EmailController : Controller
    {
        // GET: Bas/Email
        public ActionResult Index()
        {
            var str = GetEmailData(false);
            ViewBag.Message = str;
            return View();
        }

        public ActionResult Data()
        {
            var str = GetEmailData(true);
            ViewBag.Message = str;
            return null;
        }

        private string GetEmailData(bool IsSend)
        {
            var hour = Convert.ToInt16(Request.QueryString["hour"] == null ? "0" : Request.QueryString["hour"].ToString());

            if ((DateTime.Now.Hour == hour && DateTime.Now.Hour != 0) || IsSend == false)
            {
                try
                {
                    string toMails = ConfigurationManager.AppSettings["toMails"].ToString();
                    string smtpEmail = ConfigurationManager.AppSettings["smtpEmail"].ToString();
                    string fromEmail = ConfigurationManager.AppSettings["fromEmail"].ToString();
                    string fromEmailPassword = ConfigurationManager.AppSettings["fromEmailPassword"].ToString();
                    EmailParameterSet model = new EmailParameterSet();
                    model.SendEmail = fromEmail;
                    model.SendPwd = fromEmailPassword;//密码
                    model.SendSetSmtp = smtpEmail;//发送的SMTP服务地址 ，每个邮箱的是不一样的。。根据发件人的邮箱来定
                    model.ConsigneeAddress = toMails;
                    model.ConsigneeTheme = "EPM-U+经销商使用情况";
                    model.ConsigneeHand = "实施部门";
                    model.ConsigneeName = "邹智勇";
                    string thStr = @"<th style='width:50px;'>序号</th>
                                    <th style='width:60px;'>经销商ID</th>
                                    <th style='width:250px;'>经销商名称</th>
                                    <th style='width:55px;'>客户数量</th>
                                    <th style='width:48px;'>采购订单数量</th>
                                    <th style='width:60px;'>采购退货单数量</th>
                                    <th style='width:55px;'>销售订单<br>(未发货)</th>
                                    <th style='width:100px;'>销售订单数量<br>(有发货)</th>
                                    <th style='width:55px;'>销售退货单数量</th>
                                    <th style='width:75px;'>上线日期</th>
                                    <th style='width:100px;'>状态</th>
                                    <th style='width:48px;'>顾问负责人</th>
                                    <th style='width:200px;'>顾问邮箱</th>
                                    <th style='width:85px;'>顾问手机号</th>";
                    string labelStr = @"<div style=""font-family:'Microsoft Yahei';font-size:12px;text-align:left;"">当天订单数量如下：</div>";
                    string strStyle = @"<style>table{border-collapse:collapse;font-family:'Microsoft Yahei';font-size:12px;}table,th,td{border:1px solid #eee;}th{text-align:center;}th,td{padding:3px;} thead{background-color:#350a4d;color:#fff;}tr:nth-child(even){background-color:#eaf1fb;}</style>";
                    string headStr = @"<div style=""width:100%;overflow-x:auto"">
                                            <table style=""width:1241px;"">
                                              <thead>
                                                    <tr>
                                                        {0}
                                                    </tr>
                                                </thead>
                                            </table>
                                         </div>";
                    headStr = String.Format(headStr.ToString(), thStr);
                    string bodyStr = @"<div style=""width:100%;overflow-x:auto"">
                                            <table style=""width:1241px;"">
                                                <thead>
                                                    <tr>
                                                        {0}
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                        {1}
                                                </tbody>
                                            </table>
                                         </div>";
                    

                    TestDBContext testContext = new TestDBContext();
                    var wsList = testContext.Database.SqlQuery<wsinfo>(@"SELECT 
                                                                            a.wsid as wsid, 
                                                                            a.WSCompany as WSCompany,
                                                                            CONVERT(varchar(100), a.goLiveDate, 23) goLiveDate,
                                                                            a.wsname as wsname,
                                                                            b.T_Name as gwname1, 
                                                                            b.T_Email as gwemail1, 
                                                                            b.T_Mobile as gwmobile1,
		                                                                    c.T_Name as gwname2, 
                                                                            c.T_Email as gwemail2, 
                                                                            c.T_Mobile as gwmobile2,
		                                                                    d.name as status
                                                                            from t_ws a 
		                                                                    left join T_ImplementPersonnel b on a.Personnel1 = b.T_Id and ISNULL(a.IsValid, 1) = 1
		                                                                    left join T_ImplementPersonnel c on a.Personnel2 = c.T_Id and ISNULL(a.IsValid, 1) = 1
		                                                                    left join t_status d on a.status=d.id");
                    List<string> list = new List<string>();
                    foreach (var item in wsList)
                    {
                        list.Add(item.wsid);
                    }
                    var where = string.Join(", ", list.ToArray());


                    StringBuilder whereStr = new StringBuilder();
                    whereStr.Append(where);
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"
                            SELECT 
                            wsid as wsid,
                            WSName as wsname,
                            (select count(0) from Bas_Customer where a.WSID=WSID and isnull(isvalid,1)=1) as custsl,
                            (select count(0) from DMS_pur_bill where a.WSID=WSID and PurType='41' and DateDiff(dd,purdate,getdate())=0 and status>1) as cgddsl,
                            (select count(0) from DMS_pur_bill where a.WSID=WSID and PurType='43' and DateDiff(dd,purdate,getdate())=0 and status>1) as cgddsl,
                            (select count(0) from SFA_Order_Header where a.WSID=WSID and OrderType='51' and DateDiff(dd,OrderDate,getdate())=0 and (status=1 or status=2)) as xsddwfh,
							(select count(0) from SFA_Order_Header where a.WSID=WSID and OrderType='51' and DateDiff(dd,OrderDate,getdate())=0 and (status=3 or status=4)) as xsddsl,
                            (select count(0) from SFA_Order_Header where a.WSID=WSID and OrderType='53' and DateDiff(dd,OrderDate,getdate())=0 and (status=3 or status=4)) as xsthsl
                            FROM Bas_WS a
                            where a.wsid in ({0})
                           ");
                    string str = String.Format(strSql.ToString(), whereStr.ToString());
                    StuInfoDBContext stuContext = new StuInfoDBContext();
                    var wsinfoTemp = stuContext.Database.SqlQuery<wsinfo>(str).ToList();
                    //将上线日期赋值后再排序
                    foreach(var item in wsinfoTemp)
                    {
                        var wsItem = wsList.Where(w => w.wsid == item.wsid).FirstOrDefault();
                        item.goLiveDate= (wsItem != null ? wsItem.goLiveDate : "");
                    }
                    var wsinfo = wsinfoTemp.OrderBy(t => t.goLiveDate).ToList();

                    //生成表格内容
                    StringBuilder tempStr = new StringBuilder();
                    for (int i = 0; i < wsinfo.Count; i++)
                    {
                        var wsItem = wsList.Where(w => w.wsid == wsinfo[i].wsid).FirstOrDefault();

                        string _str = @"<tr>
                                            <td style='text-align:center;'>{0}</td>
                                            <td style='text-align:left;'>{1}</td>
                                            <td style='text-align:left;'>{2}</td>
                                            <td style='text-align:center;'>{3}</td>
                                            <td style='text-align:center;'>{4}</td>
                                            <td style='text-align:center;'>{5}</td>
                                            <td style='text-align:center;background-color:#d9d9d8;'>{6}</td>
                                            <td style='text-align:center;background-color:{8};color:#fff;'>{7}</td>
                                            <td style='text-align:center;'>{9}</td>
                                            <td style='text-align:left;'>{10}</td>
                                            <td style='text-align:left;'>{11}</td>
                                            <td style='text-align:left;'>{12}</td>
                                            <td style='text-align:left;'>{13}</td>
                                            <td style='text-align:left;'>{14}</td>
                                        </tr>";
                        string wsid = wsinfo[i].wsid;
                        string wsname = wsList.Where(w => w.WSCompany== wsItem.WSCompany).Count()>1? wsinfo[i].wsname+"<br>【"+ wsItem.WSCompany + "】" : wsinfo[i].wsname;
                        string custsl = wsinfo[i].custsl == 0 ? "" : wsinfo[i].custsl.ToString();
                        string cgddsl = wsinfo[i].cgddsl == 0 ? "" : wsinfo[i].cgddsl.ToString();
                        string cgthsl = wsinfo[i].cgthsl == 0 ? "" : wsinfo[i].cgthsl.ToString();
                        string xsddwfh = wsinfo[i].xsddwfh == 0 ? "" : wsinfo[i].xsddwfh.ToString();
                        string xsddsl = wsinfo[i].xsddsl == 0 ? "" : wsinfo[i].xsddsl.ToString();
                        string xsthsl = wsinfo[i].xsthsl == 0 ? "" : wsinfo[i].xsthsl.ToString();
                        string bgcolor = wsinfo[i].xsddsl == 0 ? "red" : "#1d6c09";
                        string goLiveDate = (wsItem != null ? wsItem.goLiveDate : "");
                        string status = (wsItem != null ? wsItem.status : "");
                        string gwname1 = (wsItem != null ? wsItem.gwname1 : "");
                        string gwemail1 = (wsItem != null ? wsItem.gwemail1 : "");
                        string gwmobile1 = (wsItem != null ? wsItem.gwmobile1 : "");
                        string gwname2 = (wsItem != null ? wsItem.gwname2 : "");
                        string gwemail2 = (wsItem != null ? wsItem.gwemail2 : "");
                        string gwmobile2 = (wsItem != null ? wsItem.gwmobile2 : "");

                        string temp = String.Format(_str.ToString(), i + 1, wsid, wsname, custsl, cgddsl, cgthsl, xsddwfh, xsddsl, bgcolor, xsthsl, goLiveDate, status, gwname1+"<br>"+ gwname2, gwemail1 + "<br>" + gwemail2, gwmobile1 + "<br>" + gwmobile2);
                        tempStr.Append(temp);
                    }

                    
                    StringBuilder sqlQueryStr = new StringBuilder();
                    sqlQueryStr.Append(strStyle);
                    sqlQueryStr.Append(labelStr);
                    //sqlQueryStr.Append(headStr);
                    bodyStr = String.Format(bodyStr.ToString(), thStr, tempStr);
                    sqlQueryStr.Append(bodyStr);


                    //是否发送
                    if (IsSend == true)
                    {
                        model.SendContent = sqlQueryStr.ToString();
                        MailSend(model, true);
                    }

                    return sqlQueryStr.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return null;
            }
        }

        public bool MailSend(EmailParameterSet EPSModel, bool IsBodyHtml)
        {
            try
            {
                //确定smtp服务器端的地址，实列化一个客户端smtp 
                System.Net.Mail.SmtpClient sendSmtpClient = new System.Net.Mail.SmtpClient(EPSModel.SendSetSmtp);//发件人的邮件服务器地址
                                                                                                                 //构造一个发件的人的地址
                                                                                                                 //System.Net.Mail.MailAddress sendMailAddress = new MailAddress(EPSModel.SendEmail, EPSModel.ConsigneeHand, Encoding.UTF8);//发件人的邮件地址和收件人的标题、编码

                //构造一个收件的人的地址
                //System.Net.Mail.MailAddress consigneeMailAddress = new MailAddress(EPSModel.ConsigneeAddress, EPSModel.ConsigneeName, Encoding.UTF8);//收件人的邮件地址和收件人的名称 和编码

                //构造一个Email对象
                System.Net.Mail.MailMessage mailMessage = new MailMessage();//发件地址和收件地址
                mailMessage.From = new MailAddress(EPSModel.SendEmail, EPSModel.ConsigneeHand);
                string[] list = EPSModel.ConsigneeAddress.Split(';'); //多个收件人 ,隔开
                foreach (var item in list)
                {
                    mailMessage.To.Add(item);
                }
                mailMessage.Subject = EPSModel.ConsigneeTheme;//邮件的主题
                mailMessage.BodyEncoding = Encoding.UTF8;//编码
                mailMessage.SubjectEncoding = Encoding.UTF8;//编码
                mailMessage.Body = EPSModel.SendContent;//发件内容
                mailMessage.IsBodyHtml = IsBodyHtml;//获取或者设置指定邮件正文是否为html

                //设置邮件信息 (指定如何处理待发的电子邮件)
                sendSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定如何发邮件 是以网络来发
                sendSmtpClient.EnableSsl = false;//服务器支持安全接连，安全则为true

                sendSmtpClient.UseDefaultCredentials = false;//是否随着请求一起发

                //用户登录信息
                NetworkCredential myCredential = new NetworkCredential(EPSModel.SendEmail, EPSModel.SendPwd);
                sendSmtpClient.Credentials = myCredential;//登录
                sendSmtpClient.Send(mailMessage);//发邮件
                return true;//发送成功
            }
            catch (Exception)
            {
                return false;//发送失败
            }
        }
    }

    public class EmailParameterSet
    {
        /// <summary>
        /// 收件人的邮件地址 
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 收件人的名称
        /// </summary>
        public string ConsigneeName { get; set; }

        /// <summary>
        /// 收件人标题
        /// </summary>
        public string ConsigneeHand { get; set; }

        /// <summary>
        /// 收件人的主题
        /// </summary>
        public string ConsigneeTheme { get; set; }

        /// <summary>
        /// 发件邮件服务器的Smtp设置
        /// </summary>
        public string SendSetSmtp { get; set; }

        /// <summary>
        /// 发件人的邮件
        /// </summary>
        public string SendEmail { get; set; }

        /// <summary>
        /// 发件人的邮件密码
        /// </summary>
        public string SendPwd { get; set; }
        /// <summary>
        /// 发件内容
        /// </summary>
        public string SendContent { get; set; }
    }

    public class wsinfo
    {
        public string wsid { get; set; }
        public string WSCompany { get; set; }
        public string wsname { get; set; }
        public int custsl { get; set; }
        public int cgddsl { get; set; }
        public int cgthsl { get; set; }
        public int xsddwfh { get; set; }
        public int xsddsl { get; set; }
        public int xsthsl { get; set; }
        public string wslxrname { get; set; }
        public string wslxremial { get; set; }
        public string wslxrmobile { get; set; }
        public string gwname1 { get; set; }
        public string gwemail1 { get; set; }
        public string gwmobile1 { get; set; }
        public string gwname2 { get; set; }
        public string gwemail2 { get; set; }
        public string gwmobile2 { get; set; }
        public string status { get; set; }
        public string goLiveDate { get; set; }
    }
}