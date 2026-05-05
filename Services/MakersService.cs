using APBD_TEST_TEMPLATE.DTOs;
using APBD_TEST_TEMPLATE.Repositories;

namespace APBD_TEST_TEMPLATE.Services;

public class MakersService : IMakerService
{
    private readonly IMakersRepository _makersRepository;
    
    public MakersService(IMakersRepository makersRepository)
    {
        _makersRepository = makersRepository;
    }
    
    public Task<List<MakersProductResponse>> GetMakersAsync(string? name)
    {
        return _makersRepository.GetMakersAsync(name);
    }
    
    public Task CreateMakerAsync(CreateMakerRequest request)
    {
        return _makersRepository.CreateMakerAsync(request);
    }
}