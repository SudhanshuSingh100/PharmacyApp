const API_URL = "http://localhost:5000/api";

export async function getMedicines() {
  const res = await fetch(`${API_URL}/medicines`);
  return res.json();
}

export async function addMedicine(medicine) {
  const res = await fetch(`${API_URL}/medicines`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(medicine),
  });
  return res.json();
}

export async function recordSale(sale) {
  const res = await fetch(`${API_URL}/sales`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(sale),
  });
  return res.json();
}


// Get sales this function
export async function getSales() {
  const res = await fetch(`${API_URL}/sales`);
  return res.json(); }


// update medicine list
  export async function updateMedicine(id, updatedData) {
  const res = await fetch(`${API_URL}/medicines/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(updatedData),
  });
  return res.json();
}

// delete the record
export async function deleteMedicine(id) {
  const res = await fetch(`http://localhost:5000/api/medicines/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete medicine");
  return res.json();
}


//develped by sudhanshu @copyright