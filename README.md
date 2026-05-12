# Endless

This is a YouTube-example web application for video streaming, content discovery, and user interaction, with high-performance REST API built in ASP.NET Core. 

### **Why this project matters**

This project demonstrates real-world backend architecture including:

- scalable video processing pipeline using FFmpeg (HLS / m3u8 generation) and background workers
- event-driven job processing using RabbitMQ (producer/consumer model)
- modular Clean Architecture design with separation of concerns
- integration of multiple data storage systems (PostgreSQL, Elasticsearch)
- recommendation system using vector-based similarity for personalized video ranking
- production-like Dockerized environment for full system orchestration
- user interaction system for videos (likes, comments, saves, etc.)

### **Features**
- JWT Authentication (Token Rotation, Rate Limiting, Role-based Authorization)
- Pagination, Filtering, Cookies, Logging
- Repository Pattern, CQRS, MediatR
- RabbitMQ messaging system
- xUnit Tests, SignalR
- Open API with Scalar

## Tech Stack

- Backend: `ASP.NET Core (.NET)`
- Database: `PostgreSQL`, `Dapper`, `Entity Framework Core`
- Messaging: `RabbitMQ`
- Search: `Elasticsearch`
- Testing: `xUnit`
- DevOps: `Docker`, `Docker Compose`

## How to run

#### **1. Clone repo:**
```
git clone https://github.com/woolction/endless
```
#### **2. Create .env:**
``` .env
DB_USER=postgres
DB_PASSWORD=password
DB_NAME=endless

SecretKey=64 bytes of random numbers
Issuer=Endless
Audience=EndlessUsers
ExpireMinutes=30
```
#### **3. Start Docker compose:**
``` powershell
docker compose up --build
```
#### **4. Open API**
```
- api docs: http://localhost:5000/scalar/v1
- upload video: http://localhost:5000/upload.html
- watch videos: http://localhost:5000/watch.html
```

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
├── docker-compose.yml
├── .gitignore
├── README.md
└── Endless.slnx
```

---
## License & Copyright

This project is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0). 
For more details, see the [LICENSE](LICENSE) file.

***Commercial licensing is available upon request. [Woolction@gmail.com]***

### Contributing
By contributing to this project, you agree to sign our [Contributor License Agreement (CLA)](https://gist.github.com/Woolction/7ead628e8c36751793ed60536c2bfc9f). 
All pull requests require a signed CLA before they can be merged.


#### ***Copyright © 2026 Woolction.***
