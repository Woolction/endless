# Endless

[![CI/CD](https://github.com/Woolction/endless/actions/workflows/dotnet.yaml/badge.svg)](https://github.com/Woolction/endless/actions/workflows/dotnet.yaml)
![License](https://img.shields.io/github/license/Woolction/Endless)
[![CLA assistant](https://cla-assistant.io/readme/badge/Woolction/Endless)](https://cla-assistant.io/Woolction/Endless)
![Issues](https://img.shields.io/github/issues/Woolction/Endless)

Endless is an open-source distributed video hosting platform focused on high-performance and fairness for content creators and viewers, providing an easy environment for building an audience and privacy for all users of this platform. [Learn more about Endless.](https://github.com/Woolction/endless/wiki)

### Why Endless?

YouTube has become a corporate monolith focused on advertising and data collection. Endless was created to make it easy for viewers to watch what they love and for content creators to grow. We are returning power to the creators and the viewers. Endless is built on transparency, fairness, and performance.
​
- **No Shadowbans:** Forget the fear of being "silenced." We don’t use hidden suppression. Whether you edit or re-upload your content, your reach remains organic and predictable: [Learn more about Banning Policy.](https://github.com/Woolction/endless/wiki/Banning-Policy)
- **Opt-in Advertising:** We respect your time. No forced ads that ruin the viewing experience. If a promotion isn't relevant to you, skip it instantly: [Learn more about Advertising Policy.](https://github.com/Woolction/endless/wiki/Advertising-Policy)
- **Privacy by Design:** Your data is not a product. We collect only what is strictly necessary for the platform to function. We never sell your information to third-party brokers: [Learn more about Privacy Policy.](https://github.com/Woolction/endless/wiki/Privacy-Policy)
- **Creator-First Revenue:** We believe creators deserve the lion's share. Endless offers a disruptive 50% to 90% revenue share for creators, compared to the industry standard: [Learn more about Monetization.](https://github.com/Woolction/endless/wiki/Monetization-in-Endless)
- **Community-Led Evolution:** Endless is open to the world. Any developer can contribute to the core, and every user has a voice in reporting issues or suggesting changes: [Learn more about contributing.](CONTRIBUTING.md)
- **Radical Transparency** ([The Endless Wiki](https://github.com/Woolction/endless/wiki)): No "black box" algorithms. Our Wikipedia-style documentation allows anyone to see how our systems work-from Vector Embeddings to ranking logic.


### Core Infrastructure

Endless is engineered for high availability and low latency, utilizing a modern distributed stack:

*   **Asynchronous Video Processing:** Leveraging FFmpeg and RabbitMQ to handle multi-resolution encoding (HLS/m3u8) without blocking the main API.
*   **Hybrid Data Layer:** High-performance data access using Dapper for read-heavy operations and Entity Framework Core for complex domain logic.
*   **Recommendation:** A personalized video ranking system based on vector similarity using custom Preference Engine
*   **Searching:** Advanced content filtering and users/channels discovery using Elasticsearch (edge-ngram, ngram).
*   **Real-time Engagement:** Instant notifications and live interactions powered by SignalR.

## Tech Stack

- Backend: `ASP.NET Core (.NET)`
- Database: `PostgreSQL`, `Elasticsearch`
- Messaging: `RabbitMQ`
- Media: `FFmpeg` 
- Testing: `xUnit`
- DevOps: `CI`, `Docker Compose`

## How to run

#### 1. Clone repo:
```bash
git clone https://github.com/woolction/endless

cd endless
```
#### 2. Create .env:
```.env
DB_USER=postgres
DB_PASSWORD=password
DB_NAME=endless

SecretKey=64 bytes of random numbers
Issuer=Endless
Audience=EndlessUsers
ExpireMinutes=30
```
#### 3. Obtain a license:
obtain a license (community) from [SixLabors](https://sixlabors.com/pricing) for ImageSharp or change version to 3.1.11 in Domain.csproj
```.csproj
from => 
  <PackageReference Include="SixLabors.ImageSharp" Version="4.0.0" />
to =>
  <PackageReference Include="SixLabors.ImageSharp" Version="3.1.11" />
```
#### 4. Start Docker compose:
```bash
docker compose up --build
```
#### 5. Open API
```
- api docs: http://localhost:5000/scalar/v1

- main video: http://localhost:5000/main.html
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

---

## Supporting

Endless is built and maintained int the free time of creator and contributors. The most effective way to support the project is to share your time and experience.

### The primary ways to help are to:

1. ***`Contributing:`*** Contribute features and fixing bugs, please read the [Contributing Rules](CONTRIBUTING.md) and how best to contribute to development before making your first pull request.
2. ***`Promotion:`*** Want to help us grow? Check out our [Promotion Guide](PROMOTION.md) We need people who can create hype around the project.
3. ***`Issues:`*** Find bugs and report them as described in [Contributing Rules](CONTRIBUTING.md) or come up with new features

***If you want to support the project,  you can do so via [GitHub Sponsors](https://github.com/sponsors/Woolction) or [Ko-fi](https://ko-fi.com/woolction) or [DonationAlerts](https://www.donationalerts.com/r/solderey) we would be very happy with even a very small payment***

## License

This project is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0). 
For more details, see the [LICENSE](LICENSE) file.

#### ***Copyright © 2026 Woolction.***
