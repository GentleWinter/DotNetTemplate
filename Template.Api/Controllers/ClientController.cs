using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Services;
using Template.Domain.DTO;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(ClientService clientService) 
    : ControllerBase
{
        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDto client)
        {
            try
            {
                  var result = await clientService.CreateClient(client);
                  if (result.IsFailed)
                      return BadRequest(result.Errors);
                  
                  return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the client: {ex.Message}");
            }
        }

        [HttpPut("UpdateClient")]
        [Authorize]
        public IActionResult UpdateClient([FromBody] ClientDto client)
        {
            try
            {
                var result = clientService.UpdateClient(client);
             
                if (result.IsFailed)
                    return BadRequest(result.Errors);
                  
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the client: {ex.Message}");
            }
        }
        
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto passwordDto)
        {
            try
            {
                var result = await clientService.UpdatePassword(passwordDto.Email, passwordDto.Code, passwordDto.Password);

                if (result.IsFailed)
                    return BadRequest(result.Errors);

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the client: {ex.Message}");
            }
        }


        [HttpPut("InactivateClient")]
        [Authorize(Roles = "Admin")]
        public IActionResult InactivateClient([FromBody] string clientEmail)
        {
            try
            {
                var result = clientService.InactivateClient(clientEmail);
                
                if (result.IsFailed)
                    return BadRequest(result.Errors);
                  
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deactivating the client: {ex.Message}");
            }
        }

        [HttpPut("ActivateClient")]
        [Authorize(Roles = "Admin")]
        public IActionResult ActivateClient([FromBody] string clientEmail)
        {
            try
            {
                var result = clientService.ActivateClient(clientEmail);
                
                if (result.IsFailed)
                    return BadRequest(result.Errors);
                  
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deactivating the client: {ex.Message}");
            }
        }
        
        [HttpGet("SearchClient")]
        [Authorize]
        public async Task<IActionResult> SearchClient([FromQuery] ClientDto client)
        {
            try
            {
                var result = await clientService.GetClient(client.Id);

                if (result.IsFailed)
                    return BadRequest(result.Errors);
                  
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while searching for the client: {ex.Message}");
            }
        }
}