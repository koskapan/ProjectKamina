using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamina.DAL.Models
{
    public class Material
    {
        [Key, Column("id")]
        public  Guid Id { get; set; }

        [Column("category_id")]
        public  Guid CategoryId { get; set; }

        [Column("name")]
        public String Name { get; set; }
    }
}
