using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using NetCore.Wechat.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCore.Wechat.Controllers
{
    public class MenuController : Controller    {
    
        private IOptions<WechatModel> option;

        public MenuController(IOptions<WechatModel> options)
        {
            option = options;           
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.acctoken = new WechatHelper(option).GetAccess_Token();
            return View();
        }

        /// <summary>
        /// 页面请求设置菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SetWechatMenu()
        {
            bool flag = await new WechatHelper(option).SetWechatMenu();
            return Json(flag);
        }

    }
}
