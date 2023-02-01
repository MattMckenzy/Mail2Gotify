#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime AS base
WORKDIR /app
EXPOSE 587

FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src
COPY ["Mail2Gotify.csproj", "."]
RUN dotnet restore "./Mail2Gotify.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Mail2Gotify.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mail2Gotify.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mail2Gotify.dll"]