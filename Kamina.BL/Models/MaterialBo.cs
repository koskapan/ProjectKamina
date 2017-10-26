using System;
using System.Collections;
using System.Collections.Generic;

namespace Kamina.BL.Models
{
    public class MaterialBo
    {
        public Guid Id { get; set; }

        public CategoryBo Category { get; set; }

        public  String Name { get; set; }
        
        public IList<VersionBo> Versions { get; set; }
    }
}
