using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace ZipfChartNet
{
    public class RZipfFunctionPrams
    {
        public string id {get;set;}
        public int numCount {get;set;} = 50;
        public bool removePunctuation {get;set;} = true;
        public bool caseSensitive {get;set;} = false;
        public string chartTitle {get;set;} = "Actual frequency vs Zipf frequency";
        public string chartSubTitle {get;set;} = "Isn't this Zipf-y?";

        [JsonIgnore]
        public IFormFile file {get;set;}

        public bool valid {get{
            return _valid;
        }}
        private bool _valid;

        
        public RZipfFunctionPrams()
        {
            id = Guid.NewGuid().ToString().Replace("-","");
            _valid = false;
        }
        public string Validate()
        {
            string output = "";

            id = Guid.NewGuid().ToString().Replace("-","");

            if(string.IsNullOrWhiteSpace(chartTitle))
            {
                output += "Enter a chart title. ";
            }
            if(string.IsNullOrWhiteSpace(chartSubTitle))
            {
                output += "Enter a chart sub title. ";                
            }
            if (numCount > 100 )
            {
                output += "Number of elements cannot be greater than 100. ";
            }
            if (numCount < 5 )
            {
                output += "Number of elements cannot be less than 5. ";
            }            
            return output;
        }
    }
}