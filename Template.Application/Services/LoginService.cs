using FluentResults;
using Sasquat.Infra.Alerts;
using Template.Domain.Entities;
using Template.Infra.Data.Repository.Interface;

namespace Template.Application.Services;

public class LoginService (IClientRepository clientRepository, TokenService tokenService, EmailAlert emailAlert)
{
    public Task<string> AuthClient(AuthInfo client)
    {
        var clientFounded = clientRepository.SearchClientByAuthInfos(client.Email, client.Password);

        if (clientFounded == null)
            throw new InvalidOperationException("Client not found.");

        return tokenService.GenerateToken(clientFounded);
    }
        
    public Task<Result<bool>> ChangePasswordAuth(string email, string authCode)
    {
        bool isValid = AuthCodeStoreService.Validate(email, authCode);

        if (isValid)
            return Task.FromResult(Result.Ok(true));

        return Task.FromResult(Result.Fail<bool>("Invalid or expired auth code."));
    }
        
    public async Task<Result<bool>> GenerateAuthCode(string email)
    {
        var clientFounded = clientRepository.SearchClientByAuthEmail(email);

        if (clientFounded == null)
            return Result.Fail<bool>("Client not found.");

        var authCode = AuthCodeStoreService.GenerateNumericAuthCode(6);

        AuthCodeStoreService.Save(email, authCode, TimeSpan.FromMinutes(10));

        var result = await emailAlert.AuthCode(email, authCode);

        if (result)
            return Result.Ok(true);

        return Result.Fail<bool>("Email could not be sent");
    }
}