using Microsoft.AspNetCore.Mvc;
using ABCPharmacyApp.Api.Models;
using ABCPharmacyApp.Api.Services;
using ABCPharmacyApp.Api.Services.IServices;

namespace ABCPharmacyApp.Api.Controllers
{
    /// <summary>
    /// SalesController define the status of sales stock in the inventory.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly MedicineService _medicineService;
        private readonly JsonStorageService _storage;

        public SalesController(MedicineService medicineService, JsonStorageService storage, ILogger<SalesController> logger)
        {   
            _logger = logger;
            _medicineService = medicineService;
            _storage = storage;
        }
        
        /// <summary>
        /// List of all Sold medicines
        /// </summary>
        /// <returns>List Response </returns>
        [HttpGet]
        public IActionResult GetAllSales()
        {
             _logger.LogInformation("HTTP Get /GetAllSales received");

            var sales = _storage.LoadSales();
            return Ok(sales);
        }
         
        /// <summary>
        /// Records a new sale for a medicine.
        /// </summary>
        /// <param name="sale">Sale details including medicineId and quantitySold.</param>
        /// <returns>Returns the recorded sale entry.</returns>
        [HttpPost]
        public IActionResult RecordSale(SaleRecord sale)
        {
            var medicine = _medicineService.GetMedicineById(sale.MedicineId);
            if (medicine == null) return NotFound("Medicine not found");

            var success = _medicineService.ReduceStock(sale.MedicineId, sale.QuantitySold);
            if (!success) return BadRequest("Not enough stock");

            var sales = _storage.LoadSales();
            sale.Id = sales.Count > 0 ? sales.Max(s => s.Id) + 1 : 1;
            sale.SaleDate = DateTime.Now;
            sales.Add(sale);
            _storage.SaveSales(sales);

            return Ok(sale);
        }
        
        [HttpGet("downstream")]
        public async Task<IActionResult> CallDownstream(
            [FromServices] IHttpClientFactory factory,
            [FromServices] ICorrelationIdAccessor accessor)
        {
            _logger.LogInformation("Calling downstream with correlation {CorrelationId}", accessor.CorrelationId);

            var client = factory.CreateClient("downstream");
            var response = await client.GetAsync("api/ping");   // X-Correlation-ID is added automatically

            return StatusCode((int)response.StatusCode);
        }
    }
}
