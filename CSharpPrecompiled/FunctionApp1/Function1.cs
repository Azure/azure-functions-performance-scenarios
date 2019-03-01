using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("CSharpPingPrecompiled")]
        public static string Run([HttpTrigger()] HttpRequestMessage req) => "Pong";
    }
}
