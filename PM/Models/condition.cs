using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PM.Models
{    
    public class condition
    {
        [NotMapped]
        public string beginDate { get; set; }
        [NotMapped]
        public string endDate { get; set; }
        [NotMapped]
        public int pagenumber { get; set; }
        [NotMapped]
        public int pagesize { get; set; }
        [NotMapped]
        public int total { get; set; }
        [NotMapped]
        public string CodeOrName { get; set; }
        [NotMapped]
        public string sortName { get; set; }
        [NotMapped]
        public string sortOrder { get; set; }
    }
}