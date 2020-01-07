FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["dotnetCoreWebTest/dotnetCoreWebTest.csproj", "dotnetCoreWebTest/"]
RUN dotnet restore "dotnetCoreWebTest/dotnetCoreWebTest.csproj"
COPY . .
WORKDIR "/src/dotnetCoreWebTest"
RUN dotnet build "dotnetCoreWebTest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "dotnetCoreWebTest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "dotnetCoreWebTest.dll"]