using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ZipfChartNet
{
    [Route("[controller]")]
    public class OutputController : Controller
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            string file = Path.Combine("wwwroot", "req",id.Replace(".png",""),id);
            if(!Directory.Exists(Path.Combine("wwwroot", "req",id.Replace(".png",""))))
            {
                return StatusCode(201, "Job not found");
            } 
            else
            {
                if(System.IO.File.Exists(file))
                {
                    dynamic image = "";
                    try
                    {
                        image = System.IO.File.OpenRead(file);

                    } catch (System.IO.IOException e)
                    {
                        if (e.Message.Contains("because it is being used by another process"))
                        {               
                            return StatusCode(201, "Please wait, your file is being processed");
                        }
                    }                
                    return File(image, "image/png");        
                } 
                else
                {
                    return StatusCode(201, "This file is yet to be processed");
                }
            }
        }        
    }
}