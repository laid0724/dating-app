# Description

A simple desktop-only dating app built with .NET Core 5, Angular 10 and Bootstrap 4. 

Built just for fun & practice.

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

---

# Angular

`npm i`

Run Project:

`ng serve`