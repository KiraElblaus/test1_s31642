namespace APBD_TEST_TEMPLATE.DTOs;

public class ProductVendrosResponse
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public string description { get; set; } = null!;
    public decimal strickerPrice { get; set; }
    public ProductTypeResponse productType { get; set; } = null!;
    public List<VendorsResponse> vendors { get; set; } 

}