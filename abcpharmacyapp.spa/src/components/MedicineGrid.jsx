import React, { useEffect, useState } from "react";
import { getMedicines, recordSale, updateMedicine, deleteMedicine } from "../services/api";
import "bootstrap/dist/css/bootstrap.min.css";
import "../App.css";

function MedicineGrid({ searchQuery }) {
  const [medicines, setMedicines] = useState([]);
  const [editingMedicine, setEditingMedicine] = useState(null);
  const [deletingMedicine, setDeletingMedicine] = useState(null);

  useEffect(() => {
    loadMedicines();
  }, []);

  const loadMedicines = async () => {
    const data = await getMedicines();
    setMedicines(data);
  };

  const handleSell = async (medicineId, qty = 1) => {
    const sale = {
      medicineId,
      quantitySold: qty,
      saleDate: new Date().toISOString(),
    };
    await recordSale(sale);
    loadMedicines();
  };

  const handleUpdate = (medicine) => {
    setEditingMedicine({ ...medicine });
  };

  const handleSaveUpdate = async () => {
    await updateMedicine(editingMedicine.id, editingMedicine);
    setEditingMedicine(null);
    loadMedicines();
  };

  const handleDeleteConfirm = async () => {
    await deleteMedicine(deletingMedicine.id);
    setDeletingMedicine(null);
    loadMedicines();
  };

  const filtered = medicines.filter((m) =>
    (m.fullName || "").toLowerCase().includes((searchQuery || "").toLowerCase())
  );

  return (
    <div className="card mt-4 shadow-sm">
      <div className="card-body">
        <h5 className="card-title mb-3">Medicine Inventory</h5>
        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th>Name</th>
              <th>Expiry Date</th>
              <th>Quantity</th>
              <th>Price</th>
              <th>Brand</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((m) => {
              const daysToExpiry = m.expiryDate
                ? (new Date(m.expiryDate) - new Date()) / (1000 * 60 * 60 * 24)
                : null;

              let rowClass = "";
              if (daysToExpiry !== null && daysToExpiry < 30) rowClass = "table-danger";
              else if (m.quantity < 10) rowClass = "table-warning";

              return (
                <tr key={m.id} className={rowClass}>
                  <td>{m.fullName || "Unknown"}</td>
                  <td>{m.expiryDate ? new Date(m.expiryDate).toLocaleDateString() : "—"}</td>
                  <td>{m.quantity ?? "—"}</td>
                  <td>{m.price != null ? Number(m.price).toFixed(2) : "—"}</td>
                  <td>{m.brand || "—"}</td>
                  <td>
                    <div className="btn-group">
                      <button className="btn btn-sm btn-outline-primary" onClick={() => handleUpdate(m)}>
                        Update
                      </button>
                      <button className="btn btn-sm btn-outline-danger" onClick={() => setDeletingMedicine(m)}>
                        Remove
                      </button>
                     
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Delete Confirmation Modal */}
      {deletingMedicine && (
        <div className="modal show d-block" tabIndex="-1" role="dialog">
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Confirm Delete</h5>
                <button type="button" className="btn-close" onClick={() => setDeletingMedicine(null)}></button>
              </div>
              <div className="modal-body">
                <p>
                  Are you sure you want to delete <strong>{deletingMedicine.fullName}</strong>?
                </p>
              </div>
              <div className="modal-footer">
                <button className="btn btn-danger" onClick={handleDeleteConfirm}>Yes, Delete</button>
                <button className="btn btn-secondary" onClick={() => setDeletingMedicine(null)}>No, Cancel</button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Update Modal */}
           {editingMedicine && (
        <div className="modal show d-block" tabIndex="-1" role="dialog">
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Edit Medicine</h5>
                <button type="button" className="btn-close" onClick={() => setEditingMedicine(null)}></button>
              </div>
              <div className="modal-body">
                <div className="mb-2">
                  <label className="form-label">Name</label>
                  <input
                    className="form-control"
                    value={editingMedicine.fullName}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, fullName: e.target.value })}
                  />
                </div>
                <div className="mb-2">
                  <label className="form-label">Expiry Date</label>
                  <input
                    type="date"
                    className="form-control"
                    value={editingMedicine.expiryDate?.split("T")[0] || ""}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, expiryDate: e.target.value })}
                  />
                </div>
                <div className="mb-2">
                  <label className="form-label">Quantity</label>
                  <input
                    type="number"
                    className="form-control"
                    value={editingMedicine.quantity}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, quantity: parseInt(e.target.value, 10) })}
                  />
                </div>
                <div className="mb-2">
                  <label className="form-label">Price</label>
                  <input
                    type="number"
                    step="0.01"
                    className="form-control"
                    value={editingMedicine.price}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, price: parseFloat(e.target.value) })}
                  />
                </div>
                <div className="mb-2">
                  <label className="form-label">Brand</label>
                  <input
                    className="form-control"
                    value={editingMedicine.brand}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, brand: e.target.value })}
                  />
                </div>
                <div className="mb-2">
                  <label className="form-label">Notes</label>
                  <input
                    className="form-control"
                    value={editingMedicine.notes || ""}
                    onChange={(e) => setEditingMedicine({ ...editingMedicine, notes: e.target.value })}
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-success" onClick={handleSaveUpdate}>Save</button>
                <button className="btn btn-secondary" onClick={() => setEditingMedicine(null)}>Cancel</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}


export default MedicineGrid;
