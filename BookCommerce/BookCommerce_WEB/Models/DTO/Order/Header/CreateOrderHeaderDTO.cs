﻿using BookCommerce_WEB.Models.DTO.Order.Details;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCommerce_WEB.Models.DTO.Order.Header
{
	public class CreateOrderHeaderDTO
    {
        [NotMapped]
        public string UserName { get; set; }
        public string ApplicationUserId { get; set; }
        [Column(TypeName = "Date")]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? ShippingDate { get; set; }
        public double? OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? PaymentDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? PaymentDueDate { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<CreateOrderDetailsDTO> CreateOrderDetails { get; set; }
    }
}