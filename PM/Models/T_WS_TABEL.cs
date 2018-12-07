using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class T_WS_TABEL
    {
        [Key]
        public string WSID { get; set; }
        public string WSCompany { get; set; }
        public string WSName { get; set; }
        public string ContactName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public int IsValid { get; set; }
        public DateTime researchBeginDate { get; set; }
        public DateTime researchEndDate { get; set; }
        public DateTime trainBeginDate { get; set; }
        public DateTime trainEndDate { get; set; }
        public DateTime goLiveDate { get; set; }
        public DateTime localeBeginDate { get; set; }
        public DateTime localeEndDate { get; set; }
        public string adminAccount { get; set; }
        public int isDms { get; set; }
        public int isFinZjgl { get; set; }
        public int isFinZz { get; set; }
        public int isSfa { get; set; }
        public int isVehicle { get; set; }
        public int isTran { get; set; }
        public int Personnel1 { get; set; }
        public int Personnel2 { get; set; }
        public int status { get; set; }
        public string remark { get; set; }
    }
}