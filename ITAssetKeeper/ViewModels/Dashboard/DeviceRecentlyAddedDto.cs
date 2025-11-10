using System.ComponentModel.DataAnnotations;

namespace ITAssetKeeper.ViewModels.Dashboard;

// DTO:
// Admin用Dashboard：最近追加された機器一覧(5件)
public class DeviceRecentlyAddedDto
{
    public string DisplayDate { get; set; }
    public string ManagementId { get; set; }
    public string Category { get; set; }
    public string ModelNumber { get; set; }
    public string HostName { get; set; }
    public string Location { get; set; }
    public string UserName { get; set; }
    public string Status { get; set; }
}
