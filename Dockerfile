FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ExpressionEvaluator.Core/ExpressionEvaluator.Core.csproj src/ExpressionEvaluator.Core/
COPY src/ExpressionEvaluator.Api/ExpressionEvaluator.Api.csproj src/ExpressionEvaluator.Api/
RUN dotnet restore src/ExpressionEvaluator.Api/ExpressionEvaluator.Api.csproj

COPY src/ExpressionEvaluator.Core/ src/ExpressionEvaluator.Core/
COPY src/ExpressionEvaluator.Api/ src/ExpressionEvaluator.Api/
RUN dotnet publish src/ExpressionEvaluator.Api/ExpressionEvaluator.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ExpressionEvaluator.Api.dll"]
