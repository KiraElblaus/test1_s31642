using APBD_TEST_TEMPLATE.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_TEST_TEMPLATE.Repositories;

public class MakersRepository : IMakersRepository
{
    private readonly string _connectionString;

    public MakersRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing 'Default' connection string.");
    }

    public async Task<List<MakersProductResponse>> GetMakersAsync(string? name)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var makers = new Dictionary<int, MakersProductResponse>();
        var products = new Dictionary<int, ProductVendrosResponse>();

        var query = @"
            SELECT  m.maker_id,
                    m.name AS maker_name,
                    p.product_id,
                    p.name AS product_name,
                    p.description,
                    p.stricker_price,
                    pt.product_type_id,
                    pt.name AS product_type_name,
                    v.vendor_code,
                    v.name AS vendor_name,
                    pv.amount,
                    pv.price_per_unit
            FROM    Makers m
            LEFT JOIN Product p ON p.maker_id = m.maker_id
            LEFT JOIN Product_Type pt ON pt.product_type_id = p.product_type_id
            LEFT JOIN Product_Vendor pv ON pv.product_id = p.product_id
            LEFT JOIN Vendor v ON v.vendor_code = pv.vendor_code";

        if (!string.IsNullOrWhiteSpace(name))
        {
            query += " WHERE m.name LIKE @name";
        }

        query += " ORDER BY m.maker_id, p.product_id, v.vendor_code";

        await using var command = new SqlCommand(query, connection);
        if (!string.IsNullOrWhiteSpace(name))
        {
            command.Parameters.AddWithValue("@name", $"%{name}%");
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var makerId = reader.GetInt32(0);

            if (!makers.TryGetValue(makerId, out var maker))
            {
                maker = new MakersProductResponse
                {
                    id = makerId,
                    name = reader.GetString(1),
                    products = new List<MakersProductResponse>()
                };
                makers.Add(makerId, maker);
            }

            if (!reader.IsDBNull(2))
            {
                var productId = reader.GetInt32(2);

                if (!products.TryGetValue(productId, out var product))
                {
                    product = new ProductVendrosResponse
                    {
                        id = productId,
                        name = reader.GetString(3),
                        description = reader.GetString(4),
                        strickerPrice = reader.GetDecimal(5),
                        productType = new ProductTypeResponse
                        {
                            id = reader.GetInt32(6),
                            name = reader.GetString(7)
                        },
                        vendors = new List<VendorsResponse>()
                    };
                    products.Add(productId, product);
                }

                if (!reader.IsDBNull(8))
                {
                    product.vendors.Add(new VendorsResponse
                    {
                        code = reader.GetString(8),
                        name = reader.GetString(9),
                        amount = reader.GetInt32(10),
                        pricePerUnit = reader.GetDecimal(11)
                    });
                }
            }
        }

        return makers.Values.ToList();
    }

    public async Task CreateMakerAsync(CreateMakerRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            int makerId;
            await using (var makerCommand = new SqlCommand(
                @"INSERT INTO Makers (name) VALUES (@name);
                  SELECT CAST(SCOPE_IDENTITY() as int);",
                connection,
                transaction))
            {
                makerCommand.Parameters.AddWithValue("@name", request.name);
                makerId = (int)(await makerCommand.ExecuteScalarAsync())!;
            }

            foreach (var product in request.products)
            {
                int productTypeId;
                await using (var typeCommand = new SqlCommand(
                    @"SELECT product_type_id FROM Product_Type WHERE name = @name;",
                    connection,
                    transaction))
                {
                    typeCommand.Parameters.AddWithValue("@name", product.type);
                    var result = await typeCommand.ExecuteScalarAsync();
                    
                    if (result == null)
                    {
                        await using var insertTypeCommand = new SqlCommand(
                            @"INSERT INTO Product_Type (name) VALUES (@name);
                              SELECT CAST(SCOPE_IDENTITY() as int);",
                            connection,
                            transaction);
                        insertTypeCommand.Parameters.AddWithValue("@name", product.type);
                        productTypeId = (int)(await insertTypeCommand.ExecuteScalarAsync())!;
                    }
                    else
                    {
                        productTypeId = (int)result;
                    }
                }

                await using var productCommand = new SqlCommand(
                    @"INSERT INTO Product (name, description, stricker_price, maker_id, product_type_id)
                      VALUES (@name, @description, @strickerPrice, @makerId, @productTypeId);",
                    connection,
                    transaction);
                productCommand.Parameters.AddWithValue("@name", product.name);
                productCommand.Parameters.AddWithValue("@description", product.description);
                productCommand.Parameters.AddWithValue("@strickerPrice", product.strickerPrice);
                productCommand.Parameters.AddWithValue("@makerId", makerId);
                productCommand.Parameters.AddWithValue("@productTypeId", productTypeId);

                await productCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}