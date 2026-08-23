using System;
using System.ComponentModel.DataAnnotations;

namespace ABCPharmacyApp.Api.Models
{
    public class SaleRecord
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be non-negative.")]
        public int? QuantitySold { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; }
    }
}
