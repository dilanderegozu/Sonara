using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.AuthDto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceIdentifier { get; set; }
        public string? DeviceName { get; set; }
    }

}
