namespace HairSalonDemo.ViewModels
{
    public class ReportsViewModel
    {
        public decimal DailySales { get; set; }
        public decimal WeeklySales { get; set; }
        public decimal MonthlySales { get; set; }
        public string TopStaffMember { get; set; } = "N/A";
        public decimal TopStaffSales { get; set; }
        public string MostPopularService { get; set; } = "N/A";
        public int MostPopularServiceCount { get; set; }
        public List<StaffCommissionRow> StaffCommissions { get; set; } = new();
    }

    public class StaffCommissionRow
    {
        public string StaffName { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
        public decimal ServiceSalesTotal { get; set; }
        public decimal CommissionAmount { get; set; }
    }
}
