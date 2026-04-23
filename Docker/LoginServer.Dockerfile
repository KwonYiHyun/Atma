FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["LoginServer/LoginServer.csproj", "LoginServer/"]
COPY ["SharedData/SharedData.csproj", "SharedData/"]
COPY ["UtilService/UtilService.csproj", "UtilService/"]

RUN dotnet restore "LoginServer/LoginServer.csproj"

COPY . .
WORKDIR "/src/LoginServer"
RUN dotnet build "LoginServer.csproj" -c Release -o /app/build
RUN dotnet publish "LoginServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "LoginServer.dll"]