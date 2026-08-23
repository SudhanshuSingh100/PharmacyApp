using ABCPharmacyApp.Api.Models;
using System.Diagnostics;

namespace ABCPharmacyApp.Api.Services
{
    public class MedicineService
    {
        private readonly JsonStorageService _storage;
        private static readonly ActivitySource ActivitySource = new("ABCPharmacyApp.Api");
        public MedicineService(JsonStorageService storage)
        {
            _storage = storage;
        }

       
        /// <summary>
        /// Get all medicines
        /// </summary>
        /// <returns></returns>
        public List<Medicine> GetAllMedicines()
        {
            return _storage.LoadMedicines();
        }

       
        /// <summary>
        /// Get medicine by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Medicine? GetMedicineById(int id)
        {
            var medicines = _storage.LoadMedicines();
            return medicines.FirstOrDefault(m => m.Id == id);
        }

       
        /// <summary>
        ///  Add new medicine
        /// </summary>
        /// <param name="medicine">object of medicine</param>
        /// <returns></returns>
        public Medicine AddMedicine(Medicine medicine)
        {
            using var activity = ActivitySource.StartActivity("AddMedicine");
            var medicines = _storage.LoadMedicines();
            medicine.Id = medicines.Count > 0 ? medicines.Max(m => m.Id) + 1 : 1;
            medicines.Add(medicine);
            _storage.SaveMedicines(medicines);
            return medicine;
        }

        
        /// <summary>
        /// Update existing medicine
        /// </summary>
        /// <param name="id">medicine id</param>
        /// <param name="updated">medicine object</param>
        /// <returns></returns>
        public Medicine? UpdateMedicine(int id, Medicine updated)
        {
            var medicines = _storage.LoadMedicines();
            var medicine = medicines.FirstOrDefault(m => m.Id == id);
            if (medicine == null) return null;

            medicine.FullName = updated.FullName;
            medicine.Notes = updated.Notes;
            medicine.ExpiryDate = updated.ExpiryDate;
            medicine.Quantity = updated.Quantity;
            medicine.Price = updated.Price;
            medicine.Brand = updated.Brand;

            _storage.SaveMedicines(medicines);
            return medicine;
        }

        
        /// <summary>
        ///  Delete medicine by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteMedicine(int id)
        {
            var medicines = _storage.LoadMedicines();
            var medicine = medicines.FirstOrDefault(m => m.Id == id);
            if (medicine == null) return false;

            medicines.Remove(medicine);
            _storage.SaveMedicines(medicines);
            return true;
        }

        // Reduce stock when a sale is recorded
        public bool ReduceStock(int medicineId, int? quantitySold)
        {
            var medicines = _storage.LoadMedicines();
            var medicine = medicines.FirstOrDefault(m => m.Id == medicineId);
            if (medicine == null || medicine.Quantity < quantitySold) return false;

            medicine.Quantity -= quantitySold;
            _storage.SaveMedicines(medicines);
            return true;
        }

        // Business rule: check if medicine is expired
        public bool IsExpired(Medicine medicine)
        {
            return medicine.ExpiryDate <= DateTime.Now;
        }

        // Business rule: check if stock is low
        public bool IsLowStock(Medicine medicine)
        {
            return medicine.Quantity < 10;
        }
    }
}
