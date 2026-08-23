import React, { useEffect, useState } from "react";
import { getSales, getMedicines } from "../services/api";
import "../App.css";

function SalesHistory() {
  const [sales, setSales] = useState([]);
  const [medicines, setMedicines] = useState([]);

  useEffect(() => {
    loadSales();
    loadMedicines();
  }, []);

  const loadSales = async () => {
    const data = await getSales();
    setSales(data);
  };

  const loadMedicines = async () => {
    const data = await getMedicines();
    setMedicines(data);
  };

  const getMedicineName = (id) => {
    const med = medicines.find((m) => m.id === id);
    return med ? med.fullName : "Unknown";
  };

  return (
    <div style={{ marginTop: "30px" }}>
      <h2>Sales History</h2>
      <table border="1" width="100%">
        <thead>
          <tr>
            <th>Medicine</th>
            <th>Quantity Sold</th>
            <th>Sale Date</th>
          </tr>
        </thead>
        <tbody>
          {sales.map((s) => (
            <tr key={s.id}>
              <td>{getMedicineName(s.medicineId)}</td>
              <td>{s.quantitySold}</td>
              <td>{new Date(s.saleDate).toLocaleString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default SalesHistory;
