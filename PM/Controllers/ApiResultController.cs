using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;

namespace PM.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiResultController : ApiController
    {
        [NonAction]
        public static HttpResponseMessage Json(Object obj, bool @new = true)
        {
            if (@new)
                obj = new { list = obj };
            var str = JsonConvert.SerializeObject(obj);
            var result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            //var LoginToken = "";
            //var F_ShortName = "";
            //if (OperatorProvider.Provider.GetCurrent() != null)
            //{
            //    F_ShortName = OperatorProvider.Provider.GetCurrent().F_ShortName;
            //    LoginToken = OperatorProvider.Provider.GetCurrent().LoginToken;
            //}
            //result.Headers.Add("Token", LoginToken);
            //result.Headers.Add("Name", HttpUtility.UrlEncode(F_ShortName).Replace("+", "%20"));
            return result;
        }
        [NonAction]
        public HttpResponseMessage Json(bool result, string message)
        {
            var obj = new { result = result, message = message };
            return Json(obj, false);
        }
        [NonAction]
        public HttpResponseMessage Json(bool result, string message, object data)
        {
            var obj = new { result = result, message = message, data = data };
            return Json(obj, false);
        }
        [NonAction]
        public HttpResponseMessage Json(object data, int pagenumber, int pagesize, int? total)
        {
            var obj = new { rows = data, pagenumber = pagenumber, pagesize = pagesize, total = total };
            return Json(obj, false);
        }
    }
}
