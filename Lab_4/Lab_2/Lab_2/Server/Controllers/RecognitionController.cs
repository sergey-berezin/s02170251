using DigitRecognition;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WpfApp1;

namespace Server
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecognitionController :ControllerBase
    {
        [HttpPut]
        public async Task<ImageInfo> RecognizeImageAsync([FromBody] object arg)
        {
            Console.WriteLine("Image recognition request");

            RequestStruct rs = JsonConvert.DeserializeObject<RequestStruct>(arg.ToString());
            byte[] bytes = Convert.FromBase64String(rs.pixels);
            string filename = rs.filename;

            Console.WriteLine("Input desserialized");

            ImageInfo result = new ImageInfo();
            using (AppContext context = new AppContext())
            {
                bool found = false;
                foreach (var p in context.Pictures)
                {
                    if (p.Filename == filename)
                    {
                        if (((IStructuralEquatable)bytes).Equals((IStructuralEquatable)p.Blob.Pixels))
                        {
                            result = new ImageInfo() { filename = p.Filename, res_digit = p.Label };
                            found = true;
                            break;
                        }
                    }
                }
                
                if (!found)
                {
                    Console.WriteLine("starting recognition");
                    var recognizer = new Recognizer(new MyOutput());
                    result = recognizer.GetResults(new Tuple<byte[], string>(bytes, filename));
                    Console.WriteLine("recognition ok");
                }
            }

            return result;                       
        }
    }
}
