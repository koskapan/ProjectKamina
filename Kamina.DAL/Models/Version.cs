using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamina.DAL.Models
{
    public class MaterialVersion
    {
        [Key, Column("id")]
        public Guid Id { get; set; }

        [Column("material_id")]
        public Guid MaterialId { get; set; }

        [Column("version_number")]
        public Int32 VersionNumber { get; set; }

        [Column("version_size")]
        public Int64 VersionSize { get; set; }
        
        [Column("create_date")]
        public DateTime CreateDate { get; set; }
    }
}
