using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
namespace Visualiser.Pages;

public class IndexModel(IDownstreamApi api) : PageModel
{
    public void OnGet() { }

    public async Task<IActionResult> OnPostNetwork([FromBody] object data)
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await api.CallApiForUserAsync("DMS", 
                opts =>
                {
                    opts.RelativePath = "/Visualisation/Save";
                    opts.HttpMethod = HttpMethod.Post.Method;
                },
                content: new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        }
        catch (Exception ex) when (ex.InnerException is MsalUiRequiredException { ErrorCode: MsalError.UserNullError })
        {
            return Unauthorized();
        }

        return response switch
        {
            { IsSuccessStatusCode: true } => Content(string.Empty),
            { StatusCode: HttpStatusCode.Unauthorized } => StatusCode((int)HttpStatusCode.Unauthorized, "Not authorized to perform this action"),
            _ => StatusCode((int)HttpStatusCode.Forbidden, $"An unexpected error occurred")
        };
    }

    public async Task<IActionResult> OnGetDetails(
    [Required, RegularExpression("^[0-9][A-Z]{3}[0-9]{4}[A-Z]$", ErrorMessage = "Invalid format for {0}")] string upci)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            return BadRequest(string.Join(", ", errors));
        }


        HttpResponseMessage? response = null;

        try
        {
            response = await api.CallApiForUserAsync("DMS", opts => opts.RelativePath = $"Visualisation/{upci}/Details");
        }
        catch (Exception ex) when (ex.InnerException is MsalUiRequiredException { ErrorCode: MsalError.UserNullError })
        {
            return Unauthorized();
        }

        return response switch
        {
            { IsSuccessStatusCode: true } => Content(await response.Content.ReadAsStringAsync(), "application/json"),
            { StatusCode: HttpStatusCode.NotFound } => NotFound("Cluster not found"),
            { StatusCode: HttpStatusCode.Forbidden } => StatusCode((int)HttpStatusCode.Forbidden, "Not authorized to perform this action"),
            _ => StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };
    }

    public async Task<IActionResult> OnGetRandom()
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await api.CallApiForUserAsync("DMS", opts => opts.RelativePath = "Visualisation/Generate" );
        }
        catch (Exception ex) when (ex.InnerException is MsalUiRequiredException { ErrorCode: MsalError.UserNullError })
        {
            return Unauthorized();
        }

        return response switch
        {
            { IsSuccessStatusCode: true } => Content(await response.Content.ReadAsStringAsync(), "application/json"),
            { StatusCode: HttpStatusCode.NotFound } => NotFound("No empty clusters found"),
            { StatusCode: HttpStatusCode.Forbidden } => StatusCode((int)HttpStatusCode.Forbidden, "Not authorized to perform this action"),
            _ => StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }

}
