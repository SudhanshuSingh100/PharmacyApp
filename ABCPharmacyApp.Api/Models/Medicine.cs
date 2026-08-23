using System;
using System.ComponentModel.DataAnnotations;
//using ABCPharmacyApp.Api.Utilities;

namespace ABCPharmacyApp.Api.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Full name is mandatory.")]
        public string? FullName { get; set; }
       
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Medicine expiry date is required.")]
    //  [FutureDate(ErrorMessage = "Expiry date must be in the future.")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
       
        [Range(1,int.MaxValue, ErrorMessage = "Quantity must be an integer.")]
        public int? Quantity { get; set; }
       
        [Range(0,double.MaxValue,  ErrorMessage = "Price must be non-negative.")]
        public decimal? Price { get; set; }
        
        public string? Brand { get; set; }
    }


}
