FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorLeaflet.Demo/BlazorLeaflet.Demo.csproj", "BlazorLeaflet.Demo/"]
RUN dotnet restore "BlazorLeaflet.Demo/BlazorLeaflet.Demo.csproj"
COPY . .
WORKDIR "/src/BlazorLeaflet.Demo"
RUN dotnet build "BlazorLeaflet.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorLeaflet.Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorLeaflet.Demo.dll"]
