# rc2

Simulating web-based loan calculator, written in C# using ASP.NET Core 7 and Blazor.


## Example

Try out the application at [rc2.rickebo.com](https://rc2.rickebo.com). Note that the calculator was primarily designed for
the Swedish housing market, and therefore contains features that may not be relevant in other countries. Specifying values
such as the loan amount accurately may also be difficult in other currencies.


## Setup

The application can be started either by running the project directly or by using Docker. The following sections
describe how to do both.


### Running the project directly

1. Install the [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) and [ASP.NET Core Runtime 7](https://dotnet.microsoft.com/download/dotnet/7.0).
2. Clone this repository and navigate to its root directory in a CLI.
3. Run ``dotnet run --project rc2 --urls=http://+:8080/`` to start the application. The port number is just an example 8080 can be changed if desired.

### Docker

1. Install and set up Docker.
2. Run `docker run --name rc2 -p 8080:80 ghcr.io/rickebo/rc2` from a CLI in the same directory as the [docker-compose.yml](docker-compose.yml) file to
   start the application.
3. Access the application at `http://localhost:8080`. Note that changing the exposed port in step 2 will change the port you need to access the
   application at.


### Docker Compose

1. Install and set up Docker as well as Docker Compose.
2. Configure the [docker-compose.yml](docker-compose.yml) to your liking.
3. Run `docker-compose up -d` from a CLI in the same directory as the [docker-compose.yml](docker-compose.yml) file to start the application.
4. Access the application at `http://localhost:8080`. Note that the port may be different if you changed it in the [docker-compose.yml](docker-compose.yml) file.
