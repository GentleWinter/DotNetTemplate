using Template.Domain.Entities;

namespace Template.Infra.Data.Repository.Interface;

public interface IClientRepository
{
    Task<Client?> Create(Client? client);
    Client Update(Client client);
    Client? SearchClientByAuthInfos(string email, string password);
    Client? SearchClientByAuthEmail(string email);
    Task<Client?> GetById(int id);
    Task SaveChanges();
}