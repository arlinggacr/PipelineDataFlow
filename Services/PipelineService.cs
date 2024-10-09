using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PipelineDataFlow.Data;
using PipelineDataFlow.Models;
using PipelineDataFlow.Utils.Handler;

namespace PipelineDataFlow.Services
{
    public class PipelineService
    {
        private readonly AppDbContext _context1;
        private readonly Local2AppDbContext _context2;

        public PipelineService(
            AppDbContext applicationDbContext,
            Local2AppDbContext local2AppDbContext
        )
        {
            _context1 = applicationDbContext;
            _context2 = local2AppDbContext;
        }

        public async Task<ResponseHandler?> TransferDataAsync(
            string sourceTableName,
            string targetTableName
        )
        {
            try
            {
                if (string.IsNullOrEmpty(sourceTableName) || string.IsNullOrEmpty(targetTableName))
                {
                    throw new ArgumentException("Invalid table name");
                }

                var query = $"SELECT * FROM {sourceTableName}";
                DataTable dataTable = new DataTable();
                using (var connection = (NpgsqlConnection)_context1.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    var command = new NpgsqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    dataTable.Load(reader);
                }

                var columnDefinitions = dataTable
                    .Columns
                    .Cast<DataColumn>()
                    .Select(c => $"{c.ColumnName} {GetPostgresType(c.DataType)}")
                    .ToArray();
                var createTableQuery =
                    $"CREATE TABLE {targetTableName} ({string.Join(", ", columnDefinitions)})";

                using (var connection = (NpgsqlConnection)_context2.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(createTableQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    // Insert data into the target table
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var columnNames = string.Join(
                            ", ",
                            dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)
                        );
                        var parameterNames = string.Join(
                            ", ",
                            dataTable.Columns.Cast<DataColumn>().Select(c => "@" + c.ColumnName)
                        );

                        var insertQuery =
                            $"INSERT INTO {targetTableName} ({columnNames}) VALUES ({parameterNames})";
                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                command
                                    .Parameters
                                    .AddWithValue("@" + column.ColumnName, row[column.ColumnName]);
                            }
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                return dataTable;
            }
            catch (System.Exception)
            {
                Console.WriteLine($"Error Service: {ex}");
                throw;
            }
        }

        private static string GetPostgresType(Type type)
        {
            if (type == typeof(string))
                return "TEXT";
            if (type == typeof(int))
                return "INTEGER";
            if (type == typeof(long))
                return "BIGINT";
            if (type == typeof(bool))
                return "BOOLEAN";
            if (type == typeof(DateTime))
                return "TIMESTAMP";
            if (type == typeof(float))
                return "REAL";
            if (type == typeof(double))
                return "DOUBLE PRECISION";
            if (type == typeof(decimal))
                return "NUMERIC";
            throw new NotSupportedException($"Type '{type.Name}' is not supported");
        }
    }
}
