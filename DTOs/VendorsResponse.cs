namespace APBD_TEST_TEMPLATE.DTOs;

public class VendorsResponse
{
    public string code { get; set; } = null!;
    public string name { get; set; } = null!;
    public int amount { get; set; }
    public decimal PricePerUnit { get; set; }
}