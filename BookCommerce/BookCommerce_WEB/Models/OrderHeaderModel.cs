﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCommerce_WEB.Models
{
    public class OrderHeaderModel
    {
        [Key]
        public int OrderHeaderId { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUserModel ApplicationUser { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? PaymentDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? PaymentDueDate { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public OrderDetailsModel OrderDetails { get; set; }
    }
}
