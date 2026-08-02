FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["RentalPipeline/RentalPipeline.csproj", "RentalPipeline/"]
COPY ["RentalPipeline.Tests/RentalPipeline.Tests.csproj", "RentalPipeline.Tests/"]
RUN dotnet restore "RentalPipeline/RentalPipeline.csproj"

COPY . .
WORKDIR "/src/RentalPipeline"

RUN dotnet build "RentalPipeline.csproj" -c Release -o /app/build
RUN dotnet publish "RentalPipeline.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RentalPipeline.dll"]