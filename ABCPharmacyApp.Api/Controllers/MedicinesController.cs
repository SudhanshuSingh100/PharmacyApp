using Microsoft.AspNetCore.Mvc;
using ABCPharmacyApp.Api.Models;
using ABCPharmacyApp.Api.Services;

namespace ABCPharmacyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly MedicineService _service;
        public MedicinesController(MedicineService service) { _service = service; }
        
        /// <summary>
        /// Get the list of all medicine
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var medicines = _service.GetAllMedicines();
            return Ok(medicines);
        }
        
        /// <summary>
        /// Get the medicine by Id
        /// </summary>
        /// <param name="id">Medicine Id</param>
        /// <returns>Respone of the medicine details</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
             if (id == 0)
                 throw new KeyNotFoundException("Medicine not found");
            var medicine = _service.GetMedicineById(id);
            if (medicine == null) return NotFound();
            return Ok(medicine);
        }
        
        /// <summary>
        /// Add the medicine in the stock
        /// </summary>
        /// <param name="medicine">Medicine Details</param>
        /// <returns>Respone of the stored medicine details</returns>
        [HttpPost]
        public IActionResult Add(Medicine medicine)
        {
            var added = _service.AddMedicine(medicine);
            return Ok(added);
        }
        
        /// <summary>
        /// Update the medicine details by medicine id
        /// </summary>
        /// <param name="id">Medicine Id</param>
        /// <param name="updated">Medicine Details</param>
        /// <returns>Respone of the =updated medicine details</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, Medicine updated)
        {
            var result = _service.UpdateMedicine(id, updated);
            if (result == null) return NotFound();
            return Ok(result);
        }
        
        /// <summary>
        /// Delete the Medicine Details record by medicine id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             if (id == 0)
                 throw new KeyNotFoundException("Medicine not found");
   
            var success = _service.DeleteMedicine(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
