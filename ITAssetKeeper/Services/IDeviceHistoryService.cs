using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public interface IDeviceHistoryService
{
    //////////////////////////////////////////
    // --- Index ---

    // Index用 統合メソッド：検索、ソート、ページング処理を実施
    Task<DeviceHistoryListViewModel> SearchHistoriesAsync(DeviceHistoryListViewModel condition, ClaimsPrincipal user);


    //////////////////////////////////////////
    // --- 履歴データ作成 ---

    // 新規登録時の履歴作成
    Task AddCreateHistoryAsync(Device created, string userName);

    // 更新時の履歴作成
    // DeviceDiffService で取得した差分データを元に履歴データを作成する
    Task AddUpdateHistoryAsync(Device before, Device after, string userName);

    // 削除時の履歴作成
    Task AddDeleteHistoryAsync(Device before, Device after, string userName);

    // HistoryIdの自動採番を履歴テーブル内の最大 HistoryId からの連番になるよう同期
    // ダミーデータ追加時などの整合性の担保
    Task SyncHistorySequenceAsync();


    //////////////////////////////////////////
    // --- Details ---

    // 履歴情報を ID から取得し、DTO型で返す
    Task<DeviceHistoryDetailViewModel> BuildHistoryDetailAsync(int id);
}
