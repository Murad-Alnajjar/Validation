using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Validation.MyLibrary
{
    public class ResultModule
    {
        public int errors = 0;
        public string type = "";
        public string table = "";
        public int totalNumberOfErrors = 0;
        public List<string> totalForEachTypeError = new List<string>();
    }
}
