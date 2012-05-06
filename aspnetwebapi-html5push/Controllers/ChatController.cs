using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using Strathweb.HTML5push.Models;

namespace Strathweb.HTML5push.Controllers
{
     public class ChatController : ApiController
    {
         private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
           //request.Headers.AcceptEncoding.Clear();
           HttpResponseMessage response = request.CreateResponse();
           response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");
           return response;
        }

        public void Post(Message m)
        {
            m.dt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            MessageCallback(m);
        }

        public static void OnStreamAvailable(Stream stream, HttpContentHeaders headers, TransportContext context)
        {
            StreamWriter streamwriter = new StreamWriter(stream);
            _streammessage.Enqueue(streamwriter);
        }

        private static void MessageCallback(Message m)
        {
            foreach (var subscriber in _streammessage)
            {
                subscriber.WriteLine("data:" + JsonConvert.SerializeObject(m) + "\n");
                subscriber.Flush();
            }
        }
  }
}