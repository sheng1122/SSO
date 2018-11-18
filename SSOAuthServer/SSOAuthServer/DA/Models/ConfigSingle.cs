using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("Config")]
    public class ConfigSingle
    {
        [Key]
        public Guid Config_Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
