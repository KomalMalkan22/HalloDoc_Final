namespace HalloDocMVC.DBEntity.ViewModels
{
    public class AdminDashboardList
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? RequestorPhoneNumber { get; set; }        
        public string? Notes { get; set; }
        public int? RequestId { get; set; }
        public int? RequestTypeId { get; set; }
        public string? Address { get; set; }
        public int? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? Region { get; set; }
        public string Status { get; set; }
    }
}
