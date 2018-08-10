using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ZipfChartNet.Services;
using ZipfChartNet;
using Newtonsoft.Json;
using System.IO;

namespace ZipfChartNet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApplicationLifetime _appLifetime;
        private readonly ILogger _logger;
        public IBackgroundTaskQueue Queue { get; }
        public RZipfFunctionPrams requestItem { get; set; }
        private string _workingDir;

        public IndexModel(IBackgroundTaskQueue queue, IApplicationLifetime appLifetime, ILogger<IndexModel> logger)
        {
            Queue = queue;
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public void OnGet()
        {
            requestItem = new RZipfFunctionPrams();
        }
        [HttpPost("UploadFiles")]
        public IActionResult OnPost(RZipfFunctionPrams requestItem)
        {
            // Determine that a file has been uploaded and the settings are valid
            if (requestItem.file.Length > 0 & string.IsNullOrWhiteSpace(requestItem.Validate()))
            {
                // Create the working directory and move the relevent files to it
                _workingDir = Path.Combine("wwwroot", "req", requestItem.id);
                CreateDir();
                CopyReqFiles(requestItem);

                // Queue the RPlotWorker as a background task
                Queue.QueueBackgroundWorkItem(async token =>
                {
                    var worker = new RPlotWorker(requestItem, _logger);
                    await worker.StartTask();
                });

                // Redirect the user to the output
                string imageURL = Request.Scheme + "://" + Request.Host + Request.PathBase + "/output/" + requestItem.id + ".png";
                return Redirect(imageURL);
            }
            else
            {
                return StatusCode(201, "please upload a file or enter valid inputs");
            }
        }


        private void CreateDir()
        {
            // Create the working directory
            if (!Directory.Exists(_workingDir))
            {
                Directory.CreateDirectory(_workingDir);
            }
        }
        private void CopyReqFiles(RZipfFunctionPrams _prams)
        {
            //Copy the uploaded file to the working directory
            using (var stream = new FileStream(Path.Combine(_workingDir, "data.txt"), FileMode.Create))
            {
                _prams.file.CopyTo(stream);
            }
            //Copy the R script to the working directory
            string fromFile = Path.Combine(Directory.GetCurrentDirectory(), "r", "zipf.R");
            string toFile = Path.Combine(_workingDir, "zipf.R");
            System.IO.File.Copy(fromFile, toFile, true);
            System.IO.File.SetAttributes(toFile, FileAttributes.Normal);
            //Save the selected options to the working directory
            using (var stream = new StreamWriter(Path.Combine(_workingDir, "settings.json"), false))
            {
                stream.Write(JsonConvert.SerializeObject(_prams));
            }
        }
    }
}
