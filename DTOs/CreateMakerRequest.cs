namespace APBD_TEST_TEMPLATE.DTOs;

public class CreateMakerRequest
{
    public string name { get; set; } = null!;
    public List<CreateProductRequest> products { get; set; } = new();
}