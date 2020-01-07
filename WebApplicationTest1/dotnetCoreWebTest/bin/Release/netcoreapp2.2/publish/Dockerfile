FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
RUN mkdir app
COPY netcoreweb/* /app/
EXPOSE 80
ENTRYPOINT ["dotnet", "/app/dotnetCoreWebTest.dll"]