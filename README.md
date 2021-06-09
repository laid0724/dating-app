# Description

A simple desktop-only dating app built with .NET Core 5, Angular 10 and Bootstrap 4. 

Built just for fun & practice.

# Setup

1. Get a cloudinary account, and update the fields in `CloudinarySettings` your `appsettings.json` with your credentials.
2. `cd` to `API` and run `dotnet restore` and `dotnet ef database update`
3. run `dotnet watch run` to spin up APIs
4. `cd` to `client` and run `npm i`
5. run `ng serve` to spin up angular project

# Dotnet API

`cd` to `API`

Generate Initial Migrations:

`dotnet ef migrations add ${MigrationName} -o Data/Migrations`

Generate Migrations:

`dotnet ef migrations add ${MigrationName}`

Remove Migrations:

`dotnet ef migrations remove`

Apply Migrations/Create Database:

`dotnet ef database update`

Drop Db:

`dotnet ef database drop`

Run Project:

`dotnet watch run`

Note: seed data are generated via: https://www.json-generator.com/

To quickly convert the models to typescript interfaces, use the JSON to TS extension in VSCode and paste the returned values from the API there.

---

# Angular

`npm i`

Run Project:

`ng serve`