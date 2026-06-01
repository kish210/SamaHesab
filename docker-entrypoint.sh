#!/bin/bash

# Start SQL Server
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "⏳ Waiting for SQL Server to start..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" 2>/dev/null; do
    sleep 2
done

echo "✅ SQL Server started successfully!"

# Run initialization scripts
echo "📝 Running database initialization scripts..."

# Create database
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /scripts/01_CreateDatabase.sql

# Create tables
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/02_CreateTables.sql

# Create indexes
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/03_CreateIndexes.sql

# Create views
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/04_CreateViews.sql

# Create stored procedures
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/05_StoredProcedures.sql

# Seed data
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/06_SeedData.sql

# Create chart of accounts
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d SamaHesab -i /scripts/07_DefaultChartOfAccounts.sql

echo "✅ Database initialization completed!"
echo ""
echo "════════════════════════════════════════════════════════════"
echo "🎉 SamaHesab Database is Ready!"
echo "════════════════════════════════════════════════════════════"
echo ""
echo "📊 Connection Details:"
echo "   Server: localhost,1433"
echo "   Username: sa"
echo "   Password: SamaHesab@2024"
echo "   Database: SamaHesab"
echo ""
echo "🔌 Connection String:"
echo "   Server=localhost,1433;Database=SamaHesab;User Id=sa;Password=SamaHesab@2024;TrustServerCertificate=True;"
echo ""
echo "════════════════════════════════════════════════════════════"
echo ""

# Keep container running
wait
