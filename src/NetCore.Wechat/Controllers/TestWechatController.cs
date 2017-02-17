using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Wechat.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCore.Wechat.Controllers
{
    public class TestWechatController : Controller
    {
      
        private IOptions<WechatModel> option;

        public TestWechatController(IOptions<WechatModel> options)
        {
            option = options;        
        }

      

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(option.Value);
        }

        [HttpPost]
        public IActionResult Index(WechatModel model)
        {
            ViewBag.acctoken =  new WechatHelper(option).GetAccess_Token(model.AppId, model.AppSecret);
            return View();
        }


    }
}

        //public IActionResult Index()
        //{
        //    string cacheKey = "access_token";
        //    string result;
        //    if (!_memoryCache.TryGetValue(cacheKey, out result))
        //    {
        //        result = $"LineZero{DateTime.Now}"; _memoryCache.Set(cacheKey, result);
        //        ////设置相对过期时间2分钟
        //        //_memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));
        //        //设置绝对过期时间2分钟
        //        _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(7000)));
        //        //移除缓存_memoryCache.Remove(cacheKey);//缓存优先级 （程序压力大时，会根据优先级自动回收）_memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions()    .SetPriority(CacheItemPriority.NeverRemove));//缓存回调 10秒过期会回调_memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions()    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))    .RegisterPostEvictionCallback((key, value, reason, substate) =>    {Console.WriteLine($"键{key}值{value}改变，因为{reason}");    }));//缓存回调 根据Token过期var cts = new CancellationTokenSource();_memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions()    .AddExpirationToken(new CancellationChangeToken(cts.Token))    .RegisterPostEvictionCallback((key, value, reason, substate) =>    {Console.WriteLine($"键{key}值{value}改变，因为{reason}");    }));cts.Cancel();    }    ViewBag.Cache = result;    return View();}
        //    }
        //}
