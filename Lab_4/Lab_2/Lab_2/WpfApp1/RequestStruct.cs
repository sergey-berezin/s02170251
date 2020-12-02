using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class RequestStruct
    {
        public string pixels;

        public string filename;

        public RequestStruct()
        { }

        public RequestStruct(string filename)
        {
            this.filename = Path.GetFileNameWithoutExtension(filename);
            pixels = Convert.ToBase64String(File.ReadAllBytes(filename));
        }
    }
}
