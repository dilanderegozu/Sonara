using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.AuthDto
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
