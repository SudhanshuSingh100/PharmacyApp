import React, { useState } from "react";
import { addMedicine } from "../services/api";
import "bootstrap/dist/css/bootstrap.min.css";

function AddMedicineForm() {
  const [name, setName] = useState("");
  const [stock, setStock] = useState("");
  const [price, setPrice] = useState("");
  const [expiryDate, setExpiryDate] = useState("");
  const [brand, setBrand] = useState("");
  const [notes, setNotes] = useState("");
  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};
    if (!name.trim()) newErrors.name = "Medicine name is required.";
    if (!brand.trim()) newErrors.brand = "Brand is required.";
    if (!expiryDate) newErrors.expiryDate = "Expiry date is required.";
    else if (new Date(expiryDate) < new Date())
      newErrors.expiryDate = "Expiry date must be in the future.";
    if (!stock || parseInt(stock, 10) < 0)
      newErrors.stock = "Quantity must be non-negative.";
    if (!price || parseFloat(price) <= 0)
      newErrors.price = "Price must be greater than 0.";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    await addMedicine({
      fullName: name,
      quantity: parseInt(stock, 10),
      price: parseFloat(price).toFixed(2),
      expiryDate,
      brand,
      notes,
    });

    clearForm();
  };

  const clearForm = () => {
    setName("");
    setStock("");
    setPrice("");
    setExpiryDate("");
    setBrand("");
    setNotes("");
    setErrors({});
  };

  return (
    <div className="card mt-4 shadow-sm" style={{ maxWidth: "600px", margin: "0 auto" }}>
      <div className="card-body">
        <h5 className="card-title mb-3 text-center">Add Medicine</h5>
        <form onSubmit={handleSubmit}>
          {/* Name */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Name</label>
            <div className="col-sm-8">
              <input
                className="form-control"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
              {errors.name && <div className="text-danger">{errors.name}</div>}
            </div>
          </div>

          {/* Stock */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Stock</label>
            <div className="col-sm-8">
              <input
                type="number"
                min="0"
                className="form-control"
                value={stock}
                onChange={(e) => setStock(e.target.value)}
              />
              {errors.stock && <div className="text-danger">{errors.stock}</div>}
            </div>
          </div>

          {/* Price */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Price</label>
            <div className="col-sm-8">
              <input
                type="number"
                step="0.01"
                min="0.01"
                className="form-control"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
              />
              {errors.price && <div className="text-danger">{errors.price}</div>}
            </div>
          </div>

          {/* Expiry Date */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Expiry Date</label>
            <div className="col-sm-8">
              <input
                type="date"
                className="form-control"
                value={expiryDate}
                onChange={(e) => setExpiryDate(e.target.value)}
              />
              {errors.expiryDate && <div className="text-danger">{errors.expiryDate}</div>}
            </div>
          </div>

          {/* Brand */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Brand</label>
            <div className="col-sm-8">
              <input
                className="form-control"
                value={brand}
                onChange={(e) => setBrand(e.target.value)}
              />
              {errors.brand && <div className="text-danger">{errors.brand}</div>}
            </div>
          </div>

          {/* Notes */}
          <div className="row mb-3">
            <label className="col-sm-4 col-form-label">Notes</label>
            <div className="col-sm-8">
              <input
                className="form-control"
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
              />
            </div>
          </div>

          {/* Buttons */}
          <div className="d-flex justify-content-between">
            <button type="submit" className="btn btn-primary w-50 me-2">
              Add Medicine
            </button>
            <button type="button" className="btn btn-secondary w-50" onClick={clearForm}>
              Clear
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AddMedicineForm;
