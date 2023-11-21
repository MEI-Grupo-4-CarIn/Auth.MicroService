# Auth.MicroService

This repository contains the source code for the Auth.MicroService, a microservice for handling user authentication in a .NET application.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you can run this project, you need to have the following installed on your machine:

- .NET 7.0 or later
- Docker
- Docker Compose

### Installing

1. Clone the repository to your local machine.
2. Open the solution in Visual Studio or another .NET IDE.
3. Update the connection string in the `appsettings.json` file to point to your SQL Server instance.
4. Build the solution.
5. Run the `docker-compose.yml` file to start the services.

## Running the Application

To run the application, you need to have Docker and Docker Compose installed on your machine. You can then use the following command to start the services:

```bash
docker-compose up
```


This command will start the Auth.MicroService, SQL Server, Elasticsearch, and Kibana services. The Auth.MicroService will be accessible at `http://localhost:5000`.

## Viewing Logs in Kibana

You can view the logs in Kibana by navigating to `http://localhost:5601` in your web browser. From there, you can select the appropriate index pattern and use Kibana's Discover feature to view and analyze your logs.

## Building the Docker Image

You can build the Docker image for the Auth.MicroService by running the following command:

```bash
docker build -t duartefernandes/auth-microservice .
```

You can then push the Docker image to Docker Hub by running the following command:

```bash
docker push duartefernandes/auth-microservice
```

You can then pull the Docker image from Docker Hub by running the following command:

```bash
docker pull your-dockerhub-username/auth-microservice
```