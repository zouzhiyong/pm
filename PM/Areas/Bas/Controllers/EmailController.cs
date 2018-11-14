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
            if (DateTime.Now.Hour == 17)
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
                    string str = @"<style>table{border-collapse:collapse;font-family:'Microsoft YaHei UI';font-size:12px;}table,th,td{border:1px solid #eee;}th{text-align:center;} thead{background-color:#350a4d;color:#fff;}</style>
                                <div style='font-family:'Microsoft YaHei UI';font-size:12px;'>当天订单数量如下：</div>
                                 <table>
                                      <thead>
                                            <tr>
                                                <th style='width:60px;'>序号</th>
                                                <th style='width:120px;'>经销商编码</th>
                                                <th style='width:250px;'>经销商名称</th>
                                                <th style='width:80px;'>客户数量</th>
                                                <th style='width:80px;'>采购订单数量</th>
                                                <th style='width:100px;'>采购退货单数量</th>
                                                <th style='width:80px;'>销售订单数量</th>
                                                <th style='width:100px;'>销售退货单数量</th>
                                            </tr>
                                        </thead>";
                    StringBuilder sqlQuery = new StringBuilder();
                    sqlQuery.Append(str);

                    string where = ConfigurationManager.AppSettings["ws"].ToString();
                    StringBuilder whereStr = new StringBuilder();
                    whereStr.Append(where);
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"
                            SELECT 
                            WSCode as code,
                            WSName as name,
                            (select count(0) from Bas_Customer where a.WSID=WSID and isnull(isvalid,1)=1) as custsl,
                            (select count(0) from DMS_pur_bill where a.WSID=WSID and PurType='41' and DateDiff(dd,purdate,getdate())=0 and status>1) as cgddsl,
                            (select count(0) from DMS_pur_bill where a.WSID=WSID and PurType='43' and DateDiff(dd,purdate,getdate())=0 and status>1) as cgddsl,
                            (select count(0) from SFA_Order_Header where a.WSID=WSID and OrderType='51' and DateDiff(dd,OrderDate,getdate())=0 and status>1) as xsddsl,
                            (select count(0) from SFA_Order_Header where a.WSID=WSID and OrderType='53' and DateDiff(dd,OrderDate,getdate())=0 and status>1) as xsthsl
                            FROM Bas_WS a
                            where a.wsid in ({0})
                           ");
                    str = String.Format(strSql.ToString(), whereStr.ToString());
                    StuInfoDBContext stuContext = new StuInfoDBContext();
                    var wsinfo = stuContext.Database.SqlQuery<wsinfo>(str).ToList();
                    sqlQuery.Append("<tbody>");
                    for (int i = 0; i < wsinfo.Count; i++)
                    {
                        string _str = @"<tr>
                                    <td style='text-align:center;'>{0}</td>
                                    <td style='text-align:left;'>{1}</td>
                                    <td style='text-align:left;'>{2}</td>
                                    <td style='text-align:right;'>{3}</td>
                                    <td style='text-align:right;'>{4}</td>
                                    <td style='text-align:right;'>{5}</td>
                                    <td style='text-align:right;background-color:#ff7901;color:#fff;'>{6}</td>
                                    <td style='text-align:right;'>{7}</td>
                                </tr>";
                        string temp = String.Format(_str.ToString(), i + 1, wsinfo[i].code, wsinfo[i].name, wsinfo[i].custsl, wsinfo[i].cgddsl, wsinfo[i].cgthsl, wsinfo[i].xsddsl, wsinfo[i].xsthsl);
                        sqlQuery.Append(temp);
                    }
                    sqlQuery.Append("</tbody>");
                    sqlQuery.Append("</table>");
                    model.SendContent = sqlQuery.ToString();

                    MailSend(model, true);
                    ViewBag.Message = "发送成功!";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }
            }else
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
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int custsl { get; set; }
        public int cgddsl { get; set; }
        public int cgthsl { get; set; }
        public int xsddsl { get; set; }
        public int xsthsl { get; set; }
    }
}