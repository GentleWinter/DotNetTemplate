using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Infra.Contexts;
using Template.Infra.Data.Repository.Interface;

namespace Template.Infra.Data.Repository;

public class ClientRepository : IClientRepository
{
    private readonly TemplateContext _dbContext;
    private readonly DbSet<Client> _dbSet;

    public ClientRepository(TemplateContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Client>();
    }

    public async Task<Client?> Create(Client? client)
    {
        var result = await _dbSet.AddAsync(client!);
        return result.Entity;
    }
    
    public Client? SearchClientByAuthInfos(string email, string password)
    {
        var result = _dbSet
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault(c => c.AuthInfo.Email == email);

        if (result == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, result.AuthInfo.Password))
            throw new InvalidOperationException("Incorrect auth infos.");

        return result;
    }
    
    public Client? SearchClientByAuthEmail(string email)
    {
        var result = _dbSet
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault(c => c.AuthInfo.Email == email);

        return result;
    }

    public Client Update(Client client)
        => _dbSet.Update(client).Entity!;

    public async Task<Client?> GetById(Guid id) 
        => await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
    
    public async Task SaveChanges() 
        => await _dbContext.SaveChangesAsync();
}