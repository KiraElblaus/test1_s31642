namespace APBD_TEST_TEMPLATE.DTOs;

public class CreateProductRequest
{
    public string name { get; set; } = null!;
    public string description { get; set; } = null!;
    public decimal strickerPrice { get; set; }
    public string type { get; set; } = null!;
}