using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class useinfo
    {
        public string AreaName { get; set; }
        public string WSID { get;set;}
        public string WSName { get; set; }
        public DateTime? onlineDate { get; set; }
        public DateTime? noLoginWebDay { get; set; }
        public DateTime? noLoginAppDay { get; set; }
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
        public int noBfDayNums { get; set; }        
    }

}