# SQL Server Docker Setup for SamaHesab ERP
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=SamaHesab@2024

# Create directory for scripts
RUN mkdir -p /scripts

# Copy SQL scripts
COPY database/*.sql /scripts/

# Create entrypoint script
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Health check
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
    CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -Q "SELECT 1" || exit 1

ENTRYPOINT ["/docker-entrypoint.sh"]
