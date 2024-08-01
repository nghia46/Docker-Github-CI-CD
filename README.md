# .NET Core Web API CI/CD with Docker

This repository demonstrates a CI/CD pipeline for a .NET Core Web API project using GitHub Actions and Docker. The pipeline automatically builds the project, creates a Docker image, and pushes it to Docker Hub whenever code is pushed to the `main` branch.

## Prerequisites

Before using this repository, make sure you have the following set up:

1. A **Docker Hub** account.
2. **GitHub repository secrets** for Docker Hub credentials:
   - `DOCKER_USERNAME`: Your Docker Hub username.
   - `DOCKER_PASSWORD`: Your Docker Hub password.

## CI/CD Workflow

The CI/CD pipeline is configured using GitHub Actions. The workflow file is located at `.github/workflows/ci-cd-docker.yml`. It is triggered on every push or pull request to the `main` branch.

### Workflow Steps

1. **Checkout Code:**
   - The latest code is checked out from the repository.

2. **Set Up .NET:**
   - The specified version of .NET is installed on the runner.

3. **Build Project:**
   - The .NET Core Web API project is built in `Release` mode.

4. **Publish Project:**
   - The project is published, and the output is saved to the `./publish` directory.

5. **Log in to Docker Hub:**
   - The Docker Hub login credentials are used to authenticate the Docker CLI.

6. **Build Docker Image:**
   - A Docker image is built using the Dockerfile located at `Docker-Ci-Di/Dockerfile`.
   - The image is tagged as `your-dockerhub-username/dockercicd:latest`.

7. **Push Docker Image to Docker Hub:**
   - The Docker image is pushed to Docker Hub under your account.

## Dockerfile

The `Dockerfile` is used to containerize the .NET Core Web API application. It is located in the `Docker-Ci-Di` directory. The Dockerfile uses multi-stage builds to minimize the final image size.

```dockerfile
# Step 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["YourProjectName.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Step 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
