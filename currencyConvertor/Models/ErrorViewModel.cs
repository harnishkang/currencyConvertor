using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace currencyConvertor.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class currencyList
    {

        [Required(ErrorMessage = "Please enter amount")]
        public double conversionAmount { get; set; } = 1;

        public string? conversionDate { get; set; }
        public string? CurrencyCode { get; set; }
        public List<SelectListItem>? ListofCurrency { get; set; }
        
    }
  
}