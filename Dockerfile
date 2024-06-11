# Use the official .NET 7 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory
WORKDIR /source

# Copy the solution file and restore dependencies
COPY HalloDoc.sln .
COPY HalloDoc/HalloDoc.csproj HalloDoc/
COPY HaaloDoc_BAL/HalloDoc_BAL.csproj HaaloDoc_BAL/
COPY HalloDoc_DAL/HalloDoc_DAL.csproj HalloDoc_DAL/
RUN dotnet restore

# Copy the remaining source code and build the project
COPY . .
RUN dotnet publish HalloDoc/HalloDoc.csproj -o /app/publish

# Use the official .NET runtime image as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published output from the build environment
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "HalloDoc.dll"]
