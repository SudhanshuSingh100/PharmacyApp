Steps you can follow to set up and execute your ASP.NET Core + React ABCPharmacy App on a machine.

Install Prerequisites: 
   1. Ensure your machine has the required tools before running the app.
   2. Install .NET SDK (8.0 or latest) from Microsoft site.  → dotnet --version should confirm
   3. Install Node.js (LTS) which includes npm 
       >>  node -v   # should print something like v24.x.x
       >>  npm -v    # should print npm version
   4. Install Visual Studio 2022 or VS Code with C# and React extensions

Set up the ASP.NET Core API project.

  1. Open ABCPharmacyApp.Api.csproj in Visual Studio or VS Code >> cd ../abcpharmacyapp.spa
  2. Run "dotnet restore" to install dependencies.
  3. Run "dotnet build" to compile
  4. Run "dotnet run" to execute backend API project in Runtime
  5. Verify it runs on http://localhost:5000 
  6. Test endpoints via Swagger UI at http://localhost:5000/swagger/index.html
  
 
Set up the React app for the pharmacy dashboard.
   1. Navigate to ABCPharmacyApp.React folder 
   2. Run npm install to install dependencies -> npm install -> npm start
   3. Check .env or config file for API base URL (http://localhost:5000/api) 
   5. Verify it runs on http://localhost:5173
   
 Stop and Restart: 
   1. Know how to stop and restart services.
   2. Stop backend: press Ctrl+C in API terminal
   3. Stop frontend: press Ctrl+C in React terminal
   4. Restart by re‑running "dotnet run" and "npm start"  


In case , npm not install properly, try this
# Clean old installs
Remove-Item -Recurse -Force node_modules
Remove-Item -Force package-lock.json
# Clear npm cache
npm cache clean --force
# Reinstall dependencies from package.json
npm install
# Ensure Vite + React plugin are installed
npm install vite@latest @vitejs/plugin-react --save-dev
# Start the dev server
npm run dev


Steps to Run the Project
Open Terminal:Run the project

PS dir C:\Users\<systemName>\ABCPharmacyApp
PS >>cd abcpharmacyapp.api
PS >>C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api> dotnet build         
PS >>C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api> dotnet run         

Open New Terminal:

PS>> dir C:\Users\<systemName>\ABCPharmacyApp

PS>> C:\Users\mr.sudhann\ABCPharmacyApp> cd abcpharmacyapp.spa
PS>> C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.spa> npm run dev  


-----------------------------------------------------------------------------------

**************************************************************************************************************
PS C:\Users\mr.sudhann\ABCPharmacyApp> cd abcpharmacyapp.api
PS C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api> dotnet build         
  Determining projects to restore...
  All projects are up-to-date for restore.
  ABCPharmacyApp.Api -> C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api\bin\Debug\net8.0
  \ABCPharmacyApp.Api.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:08.70
PS C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api> dotnet run           
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.api
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.

************************************************************************************

PS C:\Users\mr.sudhann\ABCPharmacyApp> cd abcpharmacyapp.spa
PS C:\Users\mr.sudhann\ABCPharmacyApp\abcpharmacyapp.spa> npm run dev          

> abcpharmacyapp.spa@0.1.0 dev
> vite

(!) Your Vite config uses features that are unsupported by `configLoader: 'native'`, which is planned to become the default in a future major version of Vite:
  - ESM syntax in a file loaded as CommonJS (vite.config.js+:1:1). Use a `.mjs` extension or set `"type": "module"` in the closest package.json
Set `VITE_CONFIG_NATIVE_IGNORE_WARNING=true` to suppress this warning.

  VITE v8.2.1  ready in 722 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
  
  *******************************************************************************************************