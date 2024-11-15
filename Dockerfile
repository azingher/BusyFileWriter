# Use a lightweight .NET SDK base image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY BusyFileWriter.csproj .
RUN dotnet restore "BusyFileWriter.csproj"

# Copy the remaining files and build the application
COPY . .
RUN dotnet publish "BusyFileWriter.csproj" -c Release -o /publish
RUN ls -l

# Use a smaller runtime image for production
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

# Set a volume for the log directory
VOLUME /log

# Optional: List files in the image
RUN echo "Listing files in the image:" && find /app

# Set the entry point to your program's executable
ENTRYPOINT ["./BusyFileWriter"]
