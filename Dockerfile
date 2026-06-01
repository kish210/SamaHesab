# SQL Server 2022 for SamaHesab ERP Testing
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=SamaHesab@2024
ENV MSSQL_PID=Developer
