using AutoMapper;
using Template.Domain.DTO;

namespace Template.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Client, ClientDto>();
        CreateMap<ClientDto, Client>();
    }
}