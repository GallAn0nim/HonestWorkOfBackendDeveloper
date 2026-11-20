FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src
COPY HonestWork.Api/HonestWork.Api.csproj HonestWork.Api/
RUN dotnet restore HonestWork.Api/HonestWork.Api.csproj
COPY HonestWork.Api/ HonestWork.Api/
RUN dotnet publish HonestWork.Api/HonestWork.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:11.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HonestWork.Api.dll"]
