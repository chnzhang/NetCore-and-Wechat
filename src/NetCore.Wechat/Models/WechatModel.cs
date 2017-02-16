
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Wechat.Models
{
    public class WechatModel {             

       public string AppId { get; set; }
       public string AppSecret { get; set; }
       public string access_token { get; set; }
       public DateTime createtime { get; set; }
       public int ExportTime { get; set; }
    }
}
