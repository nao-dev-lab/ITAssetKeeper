namespace ITAssetKeeper.Services;

// ManagementId自動採番用
public interface IDeviceSequenceService
{
    Task<int> GetNextManagementIdAsync();
}
