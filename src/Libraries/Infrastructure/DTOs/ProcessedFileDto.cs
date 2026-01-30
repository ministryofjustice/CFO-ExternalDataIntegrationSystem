using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs;

public class ProcessedFileDto
{
    public required string FileName {get;set;}
    public DateTime ValidFrom {get;set;}
    public required string Status {get;set;}
}