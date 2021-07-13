FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

EXPOSE 5000
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Development

SHELL ["/bin/bash", "-c"]

WORKDIR /app
COPY ./src/App/App.csproj ./
RUN dotnet restore "App.csproj"
COPY ./src/App/ ./
RUN dotnet build "App.csproj" -c Release -o /app/build
RUN dotnet publish "App.csproj" -c Release -o /app/publish  --no-restore
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "App.dll"]
