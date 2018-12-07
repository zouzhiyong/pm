using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class T_ImplementPersonnel
    {
        [Key]
        public int T_Id { get; set; }
        public string T_Code { get; set; }
        public string T_Name { get; set; }
        public string T_Email { get; set; }
        public string T_Mobile { get; set; }
        public string T_Remark { get; set; }
        public int IsValid { get; set; }
    }
}