# Auth.MicroService

[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-Auth.MicroService-blue)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Image Version (latest by date)](https://img.shields.io/docker/v/duartefernandes/auth-microservice?label=version)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Image Size (latest by date)](https://img.shields.io/docker/image-size/duartefernandes/auth-microservice?label=size)](https://hub.docker.com/r/duartefernandes/auth-microservice)
[![Docker Pulls](https://img.shields.io/docker/pulls/duartefernandes/auth-microservice)](https://hub.docker.com/r/duartefernandes/auth-microservice)

This repository contains the source code for the Auth.MicroService, a microservice for handling user authentication/authorization and user management in a .NET application, using a SQL Server database and Elasticsearch + Kibana for audit logs.

## Kubernetes Documentation

For detailed instructions on how to deploy the Auth.MicroService to a Kubernetes cluster, please refer to the [Kubernetes Documentation](/kubernetes/README.md).


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you can run this project, you need to have the following installed on your machine:

- .NET 8.0 or later
- Docker
- Docker Compose

### Installing

1. Clone the repository to your local machine.
2. Open the solution in Visual Studio or another .NET IDE.
3. Update the connection string in the `appsettings.json` file to point to your SQL Server instance.
4. Build the solution.
5. Run the `docker-compose.yml` file to start the services.

## Running the Application (via Docker)


To run the application via Docker, you need to have Docker and Docker Compose installed on your machine. 

1. Create a file named `.env` in the same directory as your `docker-compose.yml` file.
2. Open the `.env` file in a text editor.
3. Add the following line to the file, replacing `your_smtp_password` with your actual SMTP password:

    ```
    SMTP_PASSWORD=your_smtp_password
    ```

4. Save and close the `.env` file.

You can then use the following command to start the services (on the repository root):

```bash
docker-compose up -d
```

Replace `your_smtp_password` with the actual SMTP password.

This command will start the Auth.MicroService, SQL Server, Elasticsearch, and Kibana services. The Auth.MicroService will be accessible at `http://localhost:5001`.

## Viewing Logs in Kibana

You can view the logs in Kibana by navigating to `http://localhost:5601` in your web browser. From there, you can select the appropriate index pattern and use Kibana's Discover feature to view and analyze your logs.
