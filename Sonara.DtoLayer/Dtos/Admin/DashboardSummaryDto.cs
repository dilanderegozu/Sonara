using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DtoLayer.Dtos.Admin
{
    public class DashboardSummaryDto
    {
        public int TodayRegistrations { get; set; }
        public int TodayPurchases { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
