using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamina.BL.Models
{
    public class VersionBo
    {
        public Guid Id { get; set; }
        
        public Int32 VersionNumber { get; set; }

        public Int64 VersionSize { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
