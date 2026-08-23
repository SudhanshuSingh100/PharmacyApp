import React, { useState } from "react";
import MedicineGrid from "./components/MedicineGrid";
import AddMedicineForm from "./components/AddMedicineForm";
import SearchBar from "./components/SearchBar";
import SalesHistory from "./components/SalesHistory";
import Footer from "./components/Footer"; // ✅ new footer component

function App() {
  const [searchQuery, setSearchQuery] = useState("");

  return (
    <div className="d-flex flex-column min-vh-100">
      <div className="container py-4 flex-grow-1">
        {/* Elegant Heading */}
        <h1
          className="text-center fw-bold mb-4"
          style={{ fontFamily: "Segoe UI, sans-serif", color: "#0d6efd" }}
        >
          🏥 ABC Pharmacy
        </h1>

        {/* Search Bar */}
        <div className="d-flex justify-content-center mb-4">
          <div className="input-group shadow-sm" style={{ maxWidth: "400px" }}>
            <input
              type="text"
              className="form-control"
              placeholder="🔍 Search medicine..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button
              className="btn btn-primary"
              onClick={() => setSearchQuery(searchQuery)}
            >
              Search
            </button>
          </div>
        </div>

        {/* Add Medicine Form */}
        <AddMedicineForm />

        {/* Medicine Grid */}
        <MedicineGrid searchQuery={searchQuery} />

        {/* Sales History (optional panel) */}
        <SalesHistory />
      </div>

      {/* Footer */}
      <Footer />
    </div>
  );
}

export default App;






// return (
  //   <div style={{ padding: "20px" }}>
  //     <h1>ABC Pharmacy</h1>
  //     <SearchBar onSearch={setSearchQuery} />
  //     <AddMedicineForm />
  //     <MedicineGrid searchQuery={searchQuery} />
  //   </div>
  // );