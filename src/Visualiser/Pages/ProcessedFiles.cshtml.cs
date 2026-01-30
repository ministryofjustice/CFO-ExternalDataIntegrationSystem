using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;

namespace Visualiser.Pages
{
    public class ProcessedFilesModel(IDownstreamApi api) : PageModel
    {

        // Bindable property for the view
        public ProcessedFileDto[] ProcessedFiles { get; private set; } = Array.Empty<ProcessedFileDto>();

        public ProcessedFileDto[] OffLocFiles => ProcessedFiles
                        .Where(c => c.FileName.StartsWith("cfoextract", StringComparison.CurrentCultureIgnoreCase) == false)
                        .OrderByDescending(c => c.ValidFrom)
                        .ToArray();

        public ProcessedFileDto[] DeliusFiles => ProcessedFiles
                        .Where(c => c.FileName.StartsWith("cfoextract", StringComparison.CurrentCultureIgnoreCase))
                        .OrderByDescending(c => c.ValidFrom)
                        .ToArray();

        public async Task<IActionResult> OnGetAsync()
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await api.CallApiForUserAsync("DMS", opts => opts.RelativePath = "Visualisation/processedfiles");
            }
            catch (Exception ex) when (ex.InnerException is MsalUiRequiredException { ErrorCode: MsalError.UserNullError })
            {
                return Challenge();
            }

            if (response is null)
            {
                ModelState.AddModelError(string.Empty, "No response");
                return Page();
            }

            if (response.IsSuccessStatusCode)
            {

                try
                    {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };


                    var json = await response.Content.ReadAsStringAsync();
                    ProcessedFiles = JsonSerializer.Deserialize<ProcessedFileDto[]>(json, options)
                        ?? Array.Empty<ProcessedFileDto>();      
                }
                catch(JsonException)
                {
                    ModelState.AddModelError(string.Empty, $"Failed to parse API response");
                }

            }

            return Page();
        }
    }

    public class ProcessedFileDto
    {
        public required string FileName { get; set; }
        public DateTime ValidFrom { get; set; }
        public required string Status { get; set; }
    }
}