using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamina.BL.Models
{
    public class FileStreamInfo
    {
        public String FileName { get; set; }

        public  Stream FileStream { get; set; }
    }
}
