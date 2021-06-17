# Description

A simple dating app built with .NET Core 5, Angular 10 and Bootstrap 4. Currently desktop only. 

# Setup

1. get a cloudinary account, and update the fields in `CloudinarySettings` of your `appsettings.json` with your credentials.
2. `cd` to `API` and run `dotnet restore`
3. run `dotnet watch run` to setup db, seed data, and spin up APIs
4. `cd` to `client` and run `npm i`
5. run `ng serve` to spin up angular project
6. either register with a new user or login to existing usernames with dev only password `1234`, or login as admin with username `admin` and password `1234` to use the app.
7. have fun!

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

---

TODO - Features to be implemented / Bugs to be fixed, in no particular order:

- photo upload progress bar not reflective of actual progress; gets stuck when file too big
- impl unread messages badges via signalr
- RWD
- swiping left/right like tinder
- block/report users, admin can read/respond to reports and ban users
- transfer db to dockerized pgsql
- separate admin angular project with statistics displayed via ngx charts, manage users and content, etc.
- events / news / announcement center
- user location / google map api integration
- forget password / confirm email functionality via asp core identity
- cooler homepage with banners and animations, needs design
- like notification / badge
- pet / owner photo - pet dating app maybe?
