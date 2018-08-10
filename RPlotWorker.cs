using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZipfChartNet.Pages;
using Newtonsoft.Json;
using System.Diagnostics;


namespace ZipfChartNet
{
    public class RPlotWorker
    {
        public RZipfFunctionPrams Prams {get { return _prams; }}
        RZipfFunctionPrams _prams;
        private readonly ILogger _logger;
        private string _root = Path.Combine("wwwroot", "req");
        private string _workingDir;
        public RPlotWorker(RZipfFunctionPrams prams, ILogger logger)
        {            
            _logger = logger;
            _workingDir = Path.Combine(_root,prams.id);
            _prams = prams;
        }

        public Task<int> StartTask()
        {
            return RunScript();
        }        
        private Task<int> RunScript()
        {
            var tcs = new TaskCompletionSource<int>();

            var x = new Process();
            x.StartInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("rPath"), Path.Combine(Environment.CurrentDirectory, _workingDir,"zipf.R"));
            
            x.Exited += (s,e) =>
            {
                tcs.SetResult(x.ExitCode);
                _logger.LogInformation("Running job {0}: End",_prams.id);
                x.Dispose();
            };
           
            x.Start();
            _logger.LogInformation("Running job {0}: Start",_prams.id);

           
            return tcs.Task;
        }



    }
}