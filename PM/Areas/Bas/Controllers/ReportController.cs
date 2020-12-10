using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using PM.Common;
using PM.Models;

namespace PM.Areas.Bas.Controllers
{
    public class ReportController : Controller
    {
        public JsonResult FindInitData0(HR_report1 model)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report1>();
            if (!string.IsNullOrEmpty(model.beginDate))
            {
                expression= expression.And(t => String.Compare(t.t_year_month, model.beginDate, StringComparison.Ordinal) >= 0);
            }

            if (!string.IsNullOrEmpty(model.endDate))
            {
                expression = expression.And(t => String.Compare(t.t_year_month, model.endDate, StringComparison.Ordinal) <= 0);
            }

            List<HR_report1> list = new List<HR_report1>();         
            if (!string.IsNullOrEmpty(model.sortName))
            {
                if (model.sortOrder == "asc")
                {
                    list = testContext.HR_report1.Where(expression).OrderBy(model.sortName).ToList();
                }else
                {
                    list = testContext.HR_report1.Where(expression).OrderByDescending(model.sortName).ToList();
                }
            }else
            {
                list = testContext.HR_report1.Where(expression).ToList();
            }

            List<HR_report1> rows = new List<HR_report1>();
            rows = list.Skip(model.pagesize * (model.pagenumber - 1)).Take(model.pagesize).ToList();// Skip(每个页面显示个数 * 页数).Take(每个页面显示个数)

            HR_report1 _footerData = new HR_report1();
            _footerData.t_custmer_number = list.Sum(t => t.t_custmer_number);
            _footerData.t_order_number = list.Sum(t => t.t_order_number);
            _footerData.t_sales_je = list.Sum(t => t.t_sales_je);
            _footerData.t_ws_number = list.Sum(t => t.t_ws_number);

            var obj = new { rows = rows, pagenumber = model.pagenumber, pagesize = model.pagesize, total = list.Count(), footerData = _footerData };
            return Json(obj);
        }


        public JsonResult FindInitData1(HR_report2 model)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report2>();
            if (!string.IsNullOrEmpty(model.t_year_month))
            {
                expression = expression.And(t => t.t_year_month== model.t_year_month);
            }

            List<HR_report2> list = new List<HR_report2>();
            if (!string.IsNullOrEmpty(model.sortName))
            {
                if (model.sortOrder == "asc")
                {
                    list = testContext.HR_report2.Where(expression).OrderBy(model.sortName).ToList();
                }
                else
                {
                    list = testContext.HR_report2.Where(expression).OrderByDescending(model.sortName).ToList();
                }
            }
            else
            {
                list = testContext.HR_report2.Where(expression).ToList();
            }

            List<HR_report2> rows = new List<HR_report2>();
            rows = list.Skip(model.pagesize * (model.pagenumber - 1)).Take(model.pagesize).ToList();// Skip(每个页面显示个数 * 页数).Take(每个页面显示个数)

            HR_report2 _footerData = new HR_report2();
            _footerData.t_sales_number = list.Sum(t => t.t_sales_number);
            _footerData.t_sales_je = list.Sum(t => t.t_sales_je);

            var obj = new { rows = rows, pagenumber = model.pagenumber, pagesize = model.pagesize, total = list.Count(), footerData = _footerData };
            return Json(obj);
        }


        public JsonResult FindInitData2(HR_report3 model)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report3>();
            if (!string.IsNullOrEmpty(model.t_year_month))
            {
                expression = expression.And(t => t.t_year_month == model.t_year_month && t.t_ws == model.t_ws);
            }           

            List<HR_report3> list = new List<HR_report3>();
            if (!string.IsNullOrEmpty(model.sortName))
            {
                if (model.sortOrder == "asc")
                {
                    list = testContext.HR_report3.Where(expression).OrderBy(model.sortName).ToList();
                }
                else
                {
                    list = testContext.HR_report3.Where(expression).OrderByDescending(model.sortName).ToList();
                }
            }
            else
            {
                list = testContext.HR_report3.Where(expression).ToList();
            }


            List<HR_report3> rows = new List<HR_report3>();
            rows = list.Skip(model.pagesize * (model.pagenumber - 1)).Take(model.pagesize).ToList();// Skip(每个页面显示个数 * 页数).Take(每个页面显示个数)

            HR_report3 _footerData = new HR_report3();
            _footerData.t_sales_number = list.Sum(t => t.t_sales_number);
            _footerData.t_sales_je = list.Sum(t => t.t_sales_je);

            var obj = new { rows = rows, pagenumber = model.pagenumber, pagesize = model.pagesize, total = list.Count(), footerData = _footerData };
            return Json(obj);
        }



        public JsonResult FindDataSelect(string type)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report_select>();
            if (!string.IsNullOrEmpty(type))
            {
                expression= expression.And(t => t.t_type == type);
            }

            var list = testContext.HR_report_select.Where(expression).Select(s =>
            new
            {
                id = s.t_select_value,
                text = s.t_select_name
            }).ToList();

            var obj = new { list = list };
            return Json(obj);
        }

        public JsonResult FindDataPopSelect(HR_report_poptype model)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report_poptype>();
            if (!string.IsNullOrEmpty(model.CodeOrName))
            {
                expression = expression.And(t => t.t_name.Contains(model.CodeOrName));
            }
            var _list = testContext.HR_report_poptype.Where(expression).AsQueryable();
            var list = _list.Where(t=>t.t_type=="&").Select(s =>
            new
            {
                id = s.t_value,
                text = s.t_name,
                children = _list.Where(t => t.t_type == s.t_value).Select(_s =>
                 new
                 {
                     id = _s.t_value,
                     text = _s.t_name
                 }).ToList()
            }).ToList();           

            var obj = new { rows = list };
            return Json(obj);
        }


        public JsonResult FindInitList(HR_report_list model)
        {
            TestDBContext testContext = new TestDBContext();
            var expression = ExtLinq.True<HR_report_list>();
            if (!string.IsNullOrEmpty(model.CodeOrName))
            {
                expression = expression.And(t => t.t_name.Contains(model.CodeOrName));
            }

            var list = testContext.HR_report_list.Where(expression).ToList();

            List<HR_report_list> rows = new List<HR_report_list>();
            rows = list.Skip(model.pagesize * (model.pagenumber - 1)).Take(model.pagesize).ToList();// Skip(每个页面显示个数 * 页数).Take(每个页面显示个数)

            var obj = new { rows = rows, pagenumber = model.pagenumber, pagesize = model.pagesize, total = list.Count() };
            return Json(obj);
        }
    }
}
