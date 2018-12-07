using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class T_WS
    {
        [Key]
        public string WSID { get; set; }
        public string WSCompany { get; set; }
        public string WSName { get; set; }
        public string ContactName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public int? IsValid { get; set; }
        public DateTime? researchBeginDate { get; set; }
        public DateTime? researchEndDate { get; set; }
        public DateTime? trainBeginDate { get; set; }
        public DateTime? trainEndDate { get; set; }
        public DateTime? goLiveDate { get; set; }
        public DateTime? localeBeginDate { get; set; }
        public DateTime? localeEndDate { get; set; }
        public string adminAccount { get; set; }
        public int? isDms { get; set; }
        public int? isFinZjgl { get; set; }
        public int? isFinZz { get; set; }
        public int? isSfa { get; set; }
        public int? isVehicle { get; set; }
        public int? isTran { get; set; }
        public int? Personnel1 { get; set; }
        [NotMapped]
        public string Personnel1Name { get; set; }
        [NotMapped]
        public string Personnel1Email { get; set; }
        [NotMapped]
        public string Personnel1Mobile { get; set; }
        public int? Personnel2 { get; set; }
        [NotMapped]
        public string Personnel2Name { get; set; }
        [NotMapped]
        public string Personnel2Email { get; set; }
        [NotMapped]
        public string Personnel2Mobile { get; set; }
        public int? status { get; set; }
        [NotMapped]
        public string statusName { get; set; }
        [NotMapped]
        public int sszStatus { get; set; }
        [NotMapped]
        public int ysxStatus { get; set; }
        [NotMapped]
        public int wsxStatus { get; set; }
        [NotMapped]
        public int planStatus { get; set; }
        [NotMapped]
        public int custCount { get; set; }
        [NotMapped]
        public int xsCount { get; set; }
        [NotMapped]
        public string yearName { get; set; }
        [NotMapped]
        public string monName { get; set; }
        [NotMapped]
        public int gzrCount { get; set; }
        [NotMapped]
        public int xsCount1 { get; set; }
        [NotMapped]
        public int xsCount2 { get; set; }
        [NotMapped]
        public int xsCount3 { get; set; }
        [NotMapped]
        public int xsCount4 { get; set; }
        [NotMapped]
        public int xsCount5 { get; set; }
        [NotMapped]
        public int xsCount6 { get; set; }
        [NotMapped]
        public int xsCount7 { get; set; }
        [NotMapped]
        public int xsCount8 { get; set; }
        [NotMapped]
        public int xsCount9 { get; set; }
        [NotMapped]
        public int xsCount10 { get; set; }
        [NotMapped]
        public int xsCount11 { get; set; }
        [NotMapped]
        public int xsCount12 { get; set; }
        [NotMapped]
        public string xsCount1Name { get; set; }
        [NotMapped]
        public string xsCount2Name { get; set; }
        [NotMapped]
        public string xsCount3Name { get; set; }
        [NotMapped]
        public string xsCount4Name { get; set; }
        [NotMapped]
        public string xsCount5Name { get; set; }
        [NotMapped]
        public string xsCount6Name { get; set; }
        [NotMapped]
        public string xsCount7Name { get; set; }
        [NotMapped]
        public string xsCount8Name { get; set; }
        [NotMapped]
        public string xsCount9Name { get; set; }
        [NotMapped]
        public string xsCount10Name { get; set; }
        [NotMapped]
        public string xsCount11Name { get; set; }
        [NotMapped]
        public string xsCount12Name { get; set; }
        public string remark { get; set; }
    }
}