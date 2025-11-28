using ITAssetKeeper.Models.Enums;

namespace ITAssetKeeper.Models.ViewModels.Shared;

// Sidebar用ViewModel
public class SidebarViewModel
{
    public Roles? Role { get; set; }
    public string? UserName { get; set; }
}
