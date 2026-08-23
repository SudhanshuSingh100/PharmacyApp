using System.Text.Json;
using ABCPharmacyApp.Api.Models;

namespace ABCPharmacyApp.Api.Services
{
    public class JsonStorageService
    {
        private readonly string _medicinesFile = "Data/medicines.json";
        private readonly string _salesFile = "Data/sales.json";

        // public List<Medicine> LoadMedicines()
        // {
        //     if (!File.Exists(_medicinesFile)) return new List<Medicine>();
        //     var json = File.ReadAllText(_medicinesFile);
        //     return JsonSerializer.Deserialize<List<Medicine>>(json) ?? new List<Medicine>();
        // }
        public List<Medicine> LoadMedicines()
        {
            if (!File.Exists(_medicinesFile)) return new List<Medicine>();
            var json = File.ReadAllText(_medicinesFile);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Medicine>();

            return JsonSerializer.Deserialize<List<Medicine>>(json) ?? new List<Medicine>();
        }
        public void SaveMedicines(List<Medicine> medicines)
        {
            var json = JsonSerializer.Serialize(medicines, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_medicinesFile, json);
        }

        public List<SaleRecord> LoadSales()
        {
            if (!File.Exists(_salesFile)) return new List<SaleRecord>();
            var json = File.ReadAllText(_salesFile);
            return JsonSerializer.Deserialize<List<SaleRecord>>(json) ?? new List<SaleRecord>();
        }

        public void SaveSales(List<SaleRecord> sales)
        {
            var json = JsonSerializer.Serialize(sales, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_salesFile, json);
        }
    }
}
