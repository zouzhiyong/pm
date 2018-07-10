using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class useInfo
    {
        public string AreaName { get; set; }
        public string WSID { get; set; }
        public string WSName { get; set; }
        public DateTime? onlineDate { get; set; }
        public DateTime? noLoginWebDay { get; set; }
        public DateTime? noLoginAppDay { get; set; }
        public DateTime? RecTimestamp { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? noLoginWebDayNums { get; set; }
        public int? noLoginAppDayNums { get; set; }
        public int userCount { get; set; }
        public int vehCount { get; set; }
        public int xsdbCount { get; set; }
        public int xszgCount { get; set; }
        public int xsjlCount { get; set; }
        public int xszjCount { get; set; }
        public int cxywyCount { get; set; }
        public int sjCount { get; set; }
        public int qtCount { get; set; }
        public int? noInputFyDayNums { get; set; }
        public int? noBfDayNums { get; set; }
        public int? noOrderDayNums { get; set; }
    }
    public class moduleInfo
    {
        public int ApplicationType { get; set; }
        public string ApplicationTypeName
        {
            get
            {
                if (ApplicationType == 0)
                {
                    return "框架部分";
                }
                if (ApplicationType == 1)
                {
                    return "DMS";
                }
                if (ApplicationType == 2)
                {
                    return "SFA";
                }
                if (ApplicationType == 3)
                {
                    return "TMS";
                }
                if (ApplicationType == 4)
                {
                    return "FIN";
                }
                if (ApplicationType == 5)
                {
                    return "ws-WeChat";
                }
                if (ApplicationType == 6)
                {
                    return "poc-WeChat";
                }
                if (ApplicationType == 9)
                {
                    return "公共基础数据";
                }
                return "";
            }
        }
        public int moduleCount { get; set; }
    }
}