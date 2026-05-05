namespace APBD_TEST_TEMPLATE.Services;
using APBD_TEST_TEMPLATE.DTOs;

public interface IMakerService
{
    Task<List<MakersProductResponse>> GetMakersAsync(string? name);
    
    Task CreateMakerAsync(CreateMakerRequest request);
}