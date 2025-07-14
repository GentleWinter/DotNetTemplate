using AutoMapper;
using Template.Domain.DTO;
using Template.Domain.Entities;

namespace Template.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Client, ClientDto>().ReverseMap();
        CreateMap<Client, CreateClientDto>().ReverseMap();
    }
}