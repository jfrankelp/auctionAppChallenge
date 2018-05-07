using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuctionApp.Repositories;

namespace AuctionApp.Models
{
    public class Bid : ICreationAuditableEntity 
    {
        [Key]
        public int BidId { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public float Amount { get; set; }

        [ForeignKey("Auction")]
        public int AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }
    }
}