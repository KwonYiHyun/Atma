FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AdminTool/AdminTool.csproj", "AdminTool/"]
COPY ["SharedData/SharedData.csproj", "SharedData/"]
COPY ["UtilService/UtilService.csproj", "UtilService/"]

RUN dotnet restore "AdminTool/AdminTool.csproj"

COPY . .
WORKDIR "/src/AdminTool"
RUN dotnet build "AdminTool.csproj" -c Release -o /app/build
RUN dotnet publish "AdminTool.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AdminTool.dll"]