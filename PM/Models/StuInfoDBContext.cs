using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PM.Models
{
    public class StuInfoDBContext : DbContext
    {
        public StuInfoDBContext()
            : base("DefaultConnection")
        {
        }        
    }

    public class TestDBContext : DbContext
    {
        public TestDBContext()
            : base("TestConnection")
        {
        }
    }
}