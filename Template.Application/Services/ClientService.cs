using AutoMapper;
using FluentResults;
using Sasquat.Infra.Alerts;
using Template.Domain.DTO;
using Template.Domain.Entities;
using Template.Infra.Data.Repository.Interface;

namespace Template.Application.Services;

public class ClientService (IClientRepository _clientRepository, IMapper _mapper, EmailAlert _emailAlert, StripeService _stripeService)
{
    public async Task<Result<CreateClientDto>> CreateClient(CreateClientDto client)
    {
        Client clientEntity = _mapper.Map<Client>(client);
        
        var result = await _clientRepository.Create(clientEntity);
        await _clientRepository.SaveChanges();

        var checkoutUrl = _stripeService.CreateStripeCheckoutSession(client.AuthInfo.Email, clientEntity.Plan);
        
        await _emailAlert.OnboardingAlertAsync(clientEntity.AuthInfo.Email, checkoutUrl);
        
        return _mapper.Map<CreateClientDto>(result);
    }

    public async Task<Result<ClientDto>> GetClient(int id)
    {
        var result = await _clientRepository.GetById(id);
        return _mapper.Map<ClientDto>(result);
    }

    public Result<ClientDto> UpdateClient(ClientDto client)
    {
        Client clientEntity = _mapper.Map<Client>(client);
        
        clientEntity.UpdatedAt = DateTime.UtcNow;
        
        var result = _clientRepository.Update(clientEntity);
        _clientRepository.SaveChanges();
        
        return _mapper.Map<ClientDto>(result);
    }

    public Result<ClientDto> InactivateClient(string email)
    {
        var clientEntity = _clientRepository.SearchClientByAuthEmail(email);
        
        if (clientEntity == null)
            return Result.Fail<ClientDto>("Client not found.");
        
        clientEntity.IsActive = false;
        clientEntity.UpdatedAt = DateTime.UtcNow;
        
        var result = _clientRepository.Update(clientEntity);
        _clientRepository.SaveChanges();
        
        return _mapper.Map<ClientDto>(result);
    }
    
    public Result<ClientDto> ActivateClient(string email)
    {
        var clientEntity = _clientRepository.SearchClientByAuthEmail(email);
        
        if (clientEntity == null)
            return Result.Fail<ClientDto>("Client not found.");
        
        clientEntity.IsActive = true;
        clientEntity.UpdatedAt = DateTime.UtcNow;
        
        var result = _clientRepository.Update(clientEntity);
        _clientRepository.SaveChanges();
        
        return _mapper.Map<ClientDto>(result);
    }
    
    public async Task<Result<bool>> UpdatePassword(string email, string code, string newPassword)
    {
        bool isValidCode = AuthCodeStoreService.Validate(email, code);
        if (!isValidCode)
            return Result.Fail<bool>("Invalid or expired authentication code.");

        var clientEntity = _clientRepository.SearchClientByAuthEmail(email);
            
        if (clientEntity == null)
            return Result.Fail<bool>("Client not found.");

        clientEntity.AuthInfo.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        clientEntity.UpdatedAt = DateTime.UtcNow;
            
        _clientRepository.Update(clientEntity);
        await _clientRepository.SaveChanges();

        return Result.Ok(true);
    }
    
}