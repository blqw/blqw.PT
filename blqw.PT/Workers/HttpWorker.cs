using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT.Workers
{
    public sealed class HttpWorker : WorkerBase
    {
        public HttpWorker()
        {
            AddProperty("URL", "需要测试的超链接地址,需要带 http://");
            AddProperty("KeepAlive", "(true/false) 是否设置请求头 Connection=KeepAlive");
            AddProperty("Timeout", "设置请求的超时时间");
            KeepAlive = true;
            Timeout = 5000;
        }

        public string URL
        {
            get
            {
                return this["URL"];
            }
            set
            {
                this["URL"] = value;
            }
        }

        public bool KeepAlive
        {
            get
            {
                return string.Equals("true", this["KeepAlive"], StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                this["KeepAlive"] = value.ToString();
            }
        }

        public int Timeout
        {
            get
            {
                int timeout;
                int.TryParse(this["Timeout"], out timeout);
                if (timeout <= 0)
                {
                    timeout = 1000;
                }
                return timeout;
            }
            set
            {
                this["Timeout"] = value.ToString();
            }
        }

        public override void Initialize()
        {
            url = URL;
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("URL");
            keep = KeepAlive;
            timeout = Timeout;
        }

        string url;
        bool keep;
        int timeout;

        public override Exception Testing()
        {
            var www = (HttpWebRequest)WebRequest.Create(url);

            www.Timeout = timeout;
            www.KeepAlive = keep;
            using (var resp = (HttpWebResponse)www.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK
                    && resp.StatusCode != HttpStatusCode.Redirect)
                {
                    throw new HttpListenerException((int)resp.StatusCode, $"响应状态为:{(int)resp.StatusCode}{resp.StatusCode.ToString()}");
                }
                using (var stream = resp.GetResponseStream())
                {

                }
            }
            return null;
        }
    }
}
