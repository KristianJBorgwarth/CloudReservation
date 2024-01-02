FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CloudReservation.Service/CloudReservation.Service.csproj", "CloudReservation.Service/"]
COPY ["CloudReservation.DAL/CloudReservation.DAL.csproj", "CloudReservation.DAL/"]
RUN dotnet restore "CloudReservation.Service/CloudReservation.Service.csproj"
COPY . .
WORKDIR "/src/CloudReservation.Service"
RUN dotnet build "CloudReservation.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudReservation.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["./wait-for-it.sh", "/wait-for-it.sh"]
RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "mariadb:3306", "--", "dotnet", "CloudReservation.Service.dll"]

