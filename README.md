Pharmacy Finder System 🏥💊
A web app connecting pharmacies with customers for real-time medicine inventory management and discovery.

✨ Features
3 User Roles: Admin, Pharmacy Owner, Customer

Pharmacy Registration: License verification & admin approval

Stock Management: Real-time inventory tracking

Prescription Upload: Image processing & medicine extraction

Smart Search: Find medicines by location, price, availability

Admin Dashboard: Manage users, pharmacies, and medicines

🛠️ Tech Stack
Frontend: Angular 17, Bootstrap 5, RxJS
Backend: .NET Core 8, Entity Framework Core
Database: SQL Server 2022
Auth: JWT Tokens
API Docs: Swagger/OpenAPI

🚀 Quick Start
1. Clone & Setup
bash
git clone https://github.com/tsedeysolomons/pharmacy-finder.git
cd pharmacy-finder
2. Backend Setup
bash
cd Backend/PharmacyFinder.API
dotnet restore
dotnet ef database update
dotnet run
# API: https://localhost:7207/swagger
3. Frontend Setup
bash
cd frontend/pharmacy-finder-frontend
npm install
ng serve
# App: http://localhost:4200
📁 Database Schema
text
Users → Pharmacies (1-to-Many)
Pharmacies ↔ Medicines (Many-to-Many via PharmacyMedicines)
Users → Prescriptions (1-to-Many)
Key Tables: Users, Pharmacies, Medicines, PharmacyMedicines, Prescriptions

🔑 Default Admin Login
text
Email: admin@pharmacyfinder.com
Password: Admin@123
