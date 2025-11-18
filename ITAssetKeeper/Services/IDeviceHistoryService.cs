using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;

namespace ITAssetKeeper.Services;

public interface IDeviceHistoryService
{
    //////////////////////////////////////////
    // --- Index ---

    // Index用 統合メソッド
    Task<DeviceHistoryViewModel> SearchHistoriesAsync(DeviceHistoryViewModel condition);

    // フィルタリング (条件に応じて IQueryable<DeviceHistory> を返す)
    IQueryable<DeviceHistory> FilterHistories(IQueryable<DeviceHistory> query, DeviceHistoryViewModel condition);

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    IQueryable<DeviceHistory> SortHistories(IQueryable<DeviceHistory> query, DeviceHistoryColumns sortKey, SortOrders sortOrder);

    // ページング (Skip/Take の適用)
    IQueryable<DeviceHistory> PagingHistories(IQueryable<DeviceHistory> query, int pageNumber, int pageSize);

    // ViewModel 変換(結果を DeviceHistoryViewModel に詰める)
    DeviceHistoryViewModel ToViewModel(DeviceHistoryViewModel condition, List<DeviceHistory> histories);


}
