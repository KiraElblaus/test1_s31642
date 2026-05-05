namespace APBD_TEST_TEMPLATE.DTOs;

public class MakersProductResponse
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public List<MakersProductResponse> products { get; set; } = new();
}