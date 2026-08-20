using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Admin
{
    public class CreateArtistDto
    {
        public string Name { get; set; }
        public string? Bio { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}
