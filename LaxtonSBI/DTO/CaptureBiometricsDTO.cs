using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaxtonSBI.DTO
{

    public class CaptureBiometricsDTO
    {
        public string specVersion { get; set; }
        public string data { get; set; }
        public string hash { get; set; }
        public ErrorMsg error { get; set; }
    }
}
