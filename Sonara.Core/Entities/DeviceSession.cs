using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class DeviceSession
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string DeviceIdentifier { get; set; }   // client'tan gelen benzersiz cihaz Id'si
        public string? DeviceName { get; set; }          
        public DateTime LoginDate { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}
