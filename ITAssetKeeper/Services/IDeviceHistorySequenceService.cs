namespace ITAssetKeeper.Services;

// HistoryId自動採番用
public interface IDeviceHistorySequenceService
{
    Task<int> GetNextAsync();
}
