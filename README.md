# Meow Lib backend

Backend for [frontend version](https://github.com/ProjectMeowInc/meowlib.frontend.remaster)

## Requirements

- [Git](https://git-scm.com/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [ef-tools](https://www.nuget.org/packages/dotnet-ef)

## Start up

### Clone repository

```
git clone https://github.com/ProjectMeowInc/MeowLib.Backend.git

cd MeowLib.Backend
```

### Start migrations

EF tools:

```
dotnet ef database update --startup-project ./src/MeowLib.WebApi
```

On Windows also:

```
./.tools/startMigrations.bat
```

Also migrations auto-applied in DEV environments:

```csharp
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}
```

### Start

Develop version:

```
dotnet run --project ./src/MeowLib.WebApi
```

Production version:

```
dotnet run --project ./src/MeowLib.WebApi --configuration Release
```

### Usage

In develop environments available swagger: http://localhost:5000/swagger/index.html

You can use this API with [frontend version](https://github.com/ProjectMeowInc/meowlib.frontend.remaster).

Or create your own