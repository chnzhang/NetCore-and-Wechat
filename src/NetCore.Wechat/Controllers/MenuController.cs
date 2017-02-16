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
    public class MenuController : Controller
    {
        private IMemoryCache _memoryCache;
        private IOptions<WechatModel> option;

        public MenuController(IOptions<WechatModel> options, IMemoryCache memoryCache)
        {
            option = options;
            _memoryCache = memoryCache;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.acctoken = new WechatHelper(option, _memoryCache).GetAccess_Token();
            return View();
        }
    }
}
