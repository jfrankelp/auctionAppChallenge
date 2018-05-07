using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Repositories;

namespace AuctionApp.Models
{
    public class Auction : IAuditableEntity
    {
        [Key]
        public int AuctionId { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required, MaxLength(256)]
        [Display(Name = "Description")]
        public string ShortDescription { get; set; }

        [Display(Name ="Starting Bid")]
        public float StartAmount { get; set; }
        public List<Bid> Bids { get; set; }

        [MaxLength(50)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = string.Empty;
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }

        [MaxLength(50)]
        [Display(Name ="Last Modified By")]
        public string ModifiedBy { get; set; } = string.Empty;
        [Display(Name = "Last Modified")]
        public DateTime? ModifiedDate { get; set; }
    }
}
