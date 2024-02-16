FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 587

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
ARG TARGETARCH
ARG CONFIG
COPY . ./app
WORKDIR /app
RUN dotnet restore "Mail2Gotify.csproj" -a $TARGETARCH
RUN dotnet publish "Mail2Gotify.csproj" --self-contained -a $TARGETARCH -c $CONFIG -o publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mail2Gotify.dll"]