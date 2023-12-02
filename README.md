# Auth.MicroService

This repository contains the source code for the Auth.MicroService, a microservice for handling user authentication in a .NET application.

[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-Auth.MicroService-blue)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Image Version (latest by date)](https://img.shields.io/docker/v/duartefernandes/auth-microservice?label=version)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Image Size (latest by date)](https://img.shields.io/docker/image-size/duartefernandes/auth-microservice?label=size)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Pulls](https://img.shields.io/docker/pulls/duartefernandes/auth-microservice)](https://hub.docker.com/r/duartefernandes/auth-microservice)

## Kubernetes Documentation

For detailed instructions on how to deploy the Auth.MicroService to a Kubernetes cluster, please refer to the [Kubernetes Documentation](KUBERNETES.md).


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

## Running the Application (via Docker)

To run the application via Docker, you need to have Docker and Docker Compose installed on your machine. You can then use the following command to start the services (on the repository root):

```bash
docker-compose up -d
```


This command will start the Auth.MicroService, SQL Server, Elasticsearch, and Kibana services. The Auth.MicroService will be accessible at `http://localhost:5001`.

## Viewing Logs in Kibana

You can view the logs in Kibana by navigating to `http://localhost:5601` in your web browser. From there, you can select the appropriate index pattern and use Kibana's Discover feature to view and analyze your logs.
