using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCore.Wechat.Models
{
    public class WechatHelper
    {
        /// <summary>
        /// 目录分割符
        /// </summary>
        private static string DirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// 应用程序目录绝对路径
        /// </summary>
        private static string _ContentRootPath = DI.ServiceProvider.GetRequiredService<IHostingEnvironment>().ContentRootPath;



        public WechatHelper(IOptions<WechatModel> option, IMemoryCache memoryCache)
        {
            config = option.Value;
            _memoryCache = memoryCache;
        }

        //缓存对象
        private IMemoryCache _memoryCache;
        //系统配置的微信信息
        WechatModel config;




       /// <summary>
       /// 获得令牌
       /// </summary>
       /// <param name="AppId"></param>
       /// <param name="AppSecret"></param>
       /// <returns></returns>
        public string GetAccess_Token(string AppId=null, string AppSecret=null)
        {
            string cacheKey = "access_token";
            string result;

            /*如果未输入则使用系统默认配置的帐户*/
            if (string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(AppSecret))
            {
                AppId = config.AppId;
                AppSecret = config.AppSecret;
            }

            //如果缓存中没有access_token 则向微信发请求获取
            if (!_memoryCache.TryGetValue(cacheKey, out result))
            {
                //判断是否需要向微信发起请求
                var file_result = GetAccess_TokenByJsonFile();
                if (string.IsNullOrEmpty(file_result.Item1))
                {

                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppId, AppSecret);
                    HttpClient httpClient = new HttpClient();
                    var t = httpClient.GetStringAsync(url);
                    t.Wait();
                    result = t.Result;

                    //解析json
                    Hashtable ht = JsonConvert.DeserializeObject<Hashtable>(result);
                    string acctoken = ht["access_token"].ToString();
                    //写入缓存
                    _memoryCache.Set(cacheKey, acctoken, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(7000)));


                    //写入json文件
                    WechatModel model = new WechatModel();
                    model.access_token = acctoken;
                    model.ExportTime = 7000;
                    model.createtime = DateTime.Now;
                    model.AppId = AppId;
                    model.AppSecret = AppSecret;
                    //生成json
                    string writejson=JsonConvert.SerializeObject(model);

                    //写入json文件
                    WriteFile(writejson);

                    return acctoken;

                }
                else
                {
                    //将文件中的令牌设置到缓存中
                    _memoryCache.Set(cacheKey, file_result.Item1, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(file_result.Item2)));
                    //返回文件中的令牌
                    return file_result.Item1;
                }
               
            }
            else
            {
                //从缓存中获取access_token
                return _memoryCache.Get<string>("access_token");
            }

        }


        /// <summary>
        /// 读取JsonFile
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, long> GetAccess_TokenByJsonFile()
        {
            Tuple<string, long> result;
            //判断文件中是否存在
            string patch = MapPath("access_token.json");
            if (!File.Exists(patch))
            {
                //不存在 则创建该文件
                File.Create(patch);
                result = new Tuple<string, long>(null, 0);
                return result;
            }
            else
            {
                //读取文件中 access_token是否过期
                //FileStream stream = new FileStream(patch,FileMode.Open);
                //StreamReader sr = new StreamReader(stream);               
                //string json = sr.ReadToEnd().ToString();
                //sr.Dispose();
                //stream.Dispose();
                string json= File.ReadAllText(patch);

                if (!string.IsNullOrEmpty(json))
                {
                    //解析json
                    Hashtable ht = JsonConvert.DeserializeObject<Hashtable>(json);
                    string acctoken = ht["access_token"].ToString();
                    DateTime dt = Convert.ToDateTime(ht["createtime"]);

                    if (dt.AddSeconds(7000) > DateTime.Now)
                    {
                        //还可使用 未过期
                        long Seconds = GetTimeSeconds(dt.AddSeconds(7000) ,DateTime.Now);
                        result = new Tuple<string, long>(acctoken, Seconds);
                        return result;
                    }
                    else {
                        //已过期 不能使用
                        result = new Tuple<string, long>(null, 0);
                        return result;
                    }
                    
                }
                else
                {
                    //无json信息
                    result = new Tuple<string, long>(null, 0);
                    return result;
                }
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool WriteFile(string content)
        {
            try
            {
                string patch = MapPath("access_token.json");
                if (!File.Exists(patch))
                {
                    //不存在 则创建该文件
                    File.Create(patch);
                }

                File.WriteAllText(patch, content);                            

                //FileStream stream = new FileStream(patch, FileMode.Open);
                //StreamWriter wr = new StreamWriter(stream);
                //wr.Write(content);
                //wr.Flush();
                //wr.Dispose();
                //stream.Dispose();

                return true;
            }
            catch (Exception e)
            {
                throw e;               
            }
        }

        
        /// <summary>
        /// 获得两个时间之间相差的秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static long GetTimeSeconds(DateTime dt, DateTime dt2)
        {
            long time1 =dt.Ticks;
            long time2 = dt2.Ticks;
            long min = (time1 - time2) / 10000000;    //min=90
            return min;
        }

        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
       /// <param name="path">文件路径</param>
       /// <returns></returns>
        public static string MapPath(string path)
        {
            return IsAbsolute(path) ? path : Path.Combine(_ContentRootPath, path.TrimStart('~', '/').Replace("/", DirectorySeparatorChar));
        }

        /// <summary>
        /// 是否是绝对路径
        /// windows下判断 路径是否包含 ":"
        /// Mac OS、Linux下判断 路径是否包含 "\"
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool IsAbsolute(string path)
        {
            return Path.VolumeSeparatorChar == ':' ? path.IndexOf(Path.VolumeSeparatorChar) > 0 : path.IndexOf('\\') > 0;
        }
    }
}
