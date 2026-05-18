## Project structure

``` 
endless/
│
├── src/
│   ├── API/                
│   │   ├── Controllers/
│   │   ├── Extensions/       
│   │   ├── Middlewares/
│   │   ├── Properties/       
│   │   ├── wwwroot/          
│   │   ├── appsettings.json
│   │   ├── API.csproj
│   │   └── Program.cs
│   │
│   ├── Application/        
│   │   ├── Contents/
│   │   │   ├── Choose/
│   │   │   │   ├── Query.cs
│   │   │   └── Create/
│   │   │       ├── Command.cs
│   │   │       ├── Handler.cs
│   │   │       ├── Request.cs
│   │   │       └── Publisher.cs
│   │   ├── .../
│   │   ├── Utilities/
│   │   ├── Application.csproj
│   │   ├── AppMaker.cs
│   │   └── Result.cs
│   │ 
│   ├── Domain/         
│   │   ├── Common/
│   │   │   ├── Enums/
│   │   │   └── Interfaces/
│   │   ├── Entities/
│   │   ├── Rows/
│   │   └── Domain.csproj
│   │
│   └── Infrastructure/      
│       ├── Connector/
│       ├── Contexts/
│       ├── Managers/
│       ├── Repositories/
│       ├── Services/
│       │   ├── Background/
│       │   └── RabbitConsumers/
│       └── Infrastructure.csproj
│
├── tests/
│   ├── test-files...
│   └── tests.csproj
│
├── docker/
│   └── Dockerfile
│
├── .github/
│   ├── workflows/
│   └── FUNDING.yml
│
├── docker-compose.yml
├── .gitignore
├── CONTRIBUTING.md
├── PROMOTION.md
├── LICENSE.md
├── README.md
└── Endless.slnx
```