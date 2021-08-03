FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Development
ENV SONAR_SCANNER_VERSION 4.5.0.2216
ENV DOCKER_BUILDKIT=1
SHELL ["/bin/bash", "-c"]

ENV DOCKER_BUILDKIT=1

ENV PIP_CACHE_DIR=/var/cache/buildkit/pip
RUN mkdir -p $PIP_CACHE_DIR
RUN rm -f /etc/apt/apt.conf.d/docker-clean

RUN --mount=type=cache,target=/var/cache/apt \
    apt-get update -y &&\
    apt-get install -yqq --no-install-recommends openjdk-8-jdk \
    maven \
    nodejs \
    wget \
    unzip \
    apt-transport-https &&\
    wget https://binaries.sonarsource.com/Distribution/sonar-scanner-cli/sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux.zip &&\
    unzip sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux &&\
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb  &&\
    dpkg -i packages-microsoft-prod.deb &&\
    apt-get update

RUN  apt-get install -yq --no-install-recommends dotnet-sdk-3.1
ENV JAVA_HOME="/usr/lib/jvm/java-8-openjdk-amd64"
ENV PATH="$PATH:$JAVA_HOME/jre/bin/java:/root/.dotnet/tools:/sonar-scanner-${SONAR_SCANNER_VERSION}-linux/bin:/opt/gradle/gradle-5.0/bin"

RUN dotnet tool install --global dotnet-sonarscanner --version 4.8.0

WORKDIR /app
COPY ./src/App/App.csproj ./
RUN dotnet restore "App.csproj"
COPY ./src/App/ ./
RUN dotnet build "App.csproj" -c Release -o /app/build
RUN dotnet publish "App.csproj" -c Release -o /app/publish  --no-restore
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "App.dll"]
