using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterStore_Domain.EntityModels
{
    public class LoggedUserInfo : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        public DateTime? LastLogin { get; set; }

        [Required]
        public int LoginStatus { get; set; }
    }
}
