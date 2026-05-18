FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Empen/Empen.csproj", "Empen/"]
COPY ["SharedData/SharedData.csproj", "SharedData/"]
COPY ["UtilService/UtilService.csproj", "UtilService/"]

RUN dotnet restore "Empen/Empen.csproj"

COPY . .
WORKDIR "/src/Empen"
RUN dotnet build "Empen.csproj" -c Release -o /app/build
RUN dotnet publish "Empen.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Empen.dll"]