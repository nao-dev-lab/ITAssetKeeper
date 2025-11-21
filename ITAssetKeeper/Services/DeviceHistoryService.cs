using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Services;

public class DeviceHistoryService : IDeviceHistoryService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceDiffService _deviceDiffService;
    private readonly IDeviceHistorySequenceService _sequenceService;

    public DeviceHistoryService(
        ITAssetKeeperDbContext context,
        IDeviceDiffService deviceDiffService,
        IDeviceHistorySequenceService sequenceService)
    {
        _context = context;
        _deviceDiffService = deviceDiffService;
        _sequenceService = sequenceService;
    }

    // Index用 統合メソッド
    public async Task<DeviceHistoryViewModel> SearchHistoriesAsync(DeviceHistoryViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        IQueryable<DeviceHistory> query = _context.DeviceHistories;

        // フィルタリング実施
        query = FilterHistories(query, condition); 

        // 並び替え
        query = SortHistories(query, condition.SortKeyValue, condition.SortOrderValue);

        // ページング前の全件数を取得
        condition.TotalCount = query.Count();

        // ページング
        query = PagingHistories(query, condition.PageNumber, condition.PageSize);

        // SQLを実行
        var devices = await query.ToListAsync();

        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices);
    }

    // フィルタリング (条件に応じて IQueryable<DeviceHistory> を返す)
    private IQueryable<DeviceHistory> FilterHistories(IQueryable<DeviceHistory> query, DeviceHistoryViewModel condition)
    {
        // 部分一致
        if (!string.IsNullOrWhiteSpace(condition.HistoryId))
        {
            query = query.Where(x => x.HistoryId.Contains(condition.HistoryId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ManagementId))
        {
            query = query.Where(x => x.ManagementId.Contains(condition.ManagementId));
        }

        if (!string.IsNullOrWhiteSpace(condition.BeforeValue))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.BeforeValue != null && x.BeforeValue.Contains(condition.BeforeValue));
        }

        if (!string.IsNullOrWhiteSpace(condition.AfterValue))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.AfterValue != null && x.AfterValue.Contains(condition.AfterValue));
        }

        if (!string.IsNullOrWhiteSpace(condition.UpdatedBy))
        {
            query = query.Where(x => x.UpdatedBy.Contains(condition.UpdatedBy));
        }


        // 完全一致
        if (!string.IsNullOrWhiteSpace(condition.SelectedChangeField))
        {
            query = query.Where(x => x.ChangeField == condition.SelectedChangeField);
        }


        // 日付範囲：更新日
        if (condition.UpdatedDateFrom != null || condition.UpdatedDateTo != null)
        {
            // 検索日付の丸め（Date にし、時刻 00:00:00 起点にする）
            var from = condition.UpdatedDateFrom?.Date;
            var to = condition.UpdatedDateTo?.Date;

            // From のみ指定されている場合
            if (from != null && to == null)
            {
                // from ～ 今日の翌日 00:00 まで
                query = query.Where(x =>
                    x.UpdatedAt >= from.Value &&
                    x.UpdatedAt < DateTime.Now.Date.AddDays(1));
            }
            // To のみ指定されている場合
            else if (from == null && to != null)
            {
                // ～ to の翌日 00:00 まで
                query = query.Where(x =>
                    x.UpdatedAt < to.Value.AddDays(1));
            }
            // From と To 両方指定されている場合
            else if (from != null && to != null)
            {
                if (from == to)
                {
                    // 日付一致 → 当日の 00:00 ～ 翌日 00:00
                    query = query.Where(x =>
                        x.UpdatedAt >= from.Value &&
                        x.UpdatedAt < from.Value.AddDays(1));
                }
                else
                {
                    // from ～ to(包含) → 翌日 00:00 まで
                    query = query.Where(x =>
                        x.UpdatedAt >= from.Value &&
                        x.UpdatedAt < to.Value.AddDays(1));
                }
            }
        }

        return query;
    }

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    private IQueryable<DeviceHistory> SortHistories(IQueryable<DeviceHistory> query, DeviceHistoryColumns sortKey, SortOrders sortOrder)
    {
        // 指定されたSortKeyを基準に、
        // 指定されたSortOrderに応じて、昇順 or 降順でソートする
        if (sortOrder == SortOrders.Asc)
        {
            query = query.OrderBy(DeviceHistoryConstants.SORT_KEY_SELECTORS[sortKey]);
        }
        else
        {
            query = query.OrderByDescending(DeviceHistoryConstants.SORT_KEY_SELECTORS[sortKey]);
        }

        return query;
    }

    // ページング (Skip/Take の適用)
    private IQueryable<DeviceHistory> PagingHistories(IQueryable<DeviceHistory> query, int pageNumber, int pageSize)
    {
        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        return query;
    }

    // ViewModel 変換(結果を DeviceHistoryViewModel に詰める)
    private DeviceHistoryViewModel ToViewModel(DeviceHistoryViewModel condition, List<DeviceHistory> histories)
    {
        // プルダウン用のデータを定数から取得
        EnumDisplayHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceHistoryColumns>(condition, selectList => condition.SortKeyList = selectList);

        EnumDisplayHelper.SetEnumSelectList<DeviceColumns>(condition, selectList => condition.ChangeFieldItems = selectList);

        // DeviceColumns は UpdatedAt を除外して取得する
        EnumDisplayHelper.SetEnumSelectList<DeviceColumns>(condition, selectList =>
            condition.ChangeFieldItems = new SelectList(EnumDisplayHelper.EnumToDictionary(DeviceColumns.UpdatedAt), "Key", "Value"));


        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.DeviceHistories = histories
            .Select(x => new DeviceHistoryDto
            {
                // ChangeField,BeforeValue,AfterValue は Helperを使って日本語表示名に変換
                // BeforeValue,AfterValue は、nullなら"-"に変換される
                Id = x.Id,
                HistoryId = x.HistoryId,
                ManagementId = x.ManagementId,
                ChangeField = EnumDisplayHelper.ResolveDisplayName<DeviceColumns>(x.ChangeField),
                BeforeValue = EnumDisplayHelper.ResolveHistoryDisplay(x.ChangeField, x.BeforeValue),
                AfterValue = EnumDisplayHelper.ResolveHistoryDisplay(x.ChangeField, x.AfterValue),
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();

        return condition;
    }


    //////////////////////////////////////////
    // --- CreateHistory ---

    // 新規登録時の履歴作成
    public async Task AddCreateHistoryAsync(Device created, string userName)
    {
        // 履歴の Entity 作成
        var history = new DeviceHistory
        {
            HistoryId = await GenerateHistoryIdAsync(),    // 新しい履歴IDを生成して設定
            ManagementId = created.ManagementId,
            ChangeField = "Created",
            BeforeValue = null,
            AfterValue = null,
            UpdatedBy = userName,
            UpdatedAt = created.UpdatedAt
        };

        // 履歴テーブルにレコードを追加
        _context.DeviceHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    // 更新時の履歴作成
    // DeviceDiffService で取得した差分データを元に履歴データを作成する
    public async Task AddUpdateHistoryAsync(Device before, Device after, string userName)
    {
        // 更新箇所のみ取得
        var changes = _deviceDiffService.GetChanges(before, after);

        // 更新箇所を履歴に反映させる
        // 更新箇所1つにつき、1レコード生成
        foreach (var change in changes)
        {
            // 履歴の Entity 作成
            var history = new DeviceHistory
            {
                // 複数行のレコード追加時でもGenerateHistoryIdAsync()でID競合を回避
                HistoryId = await GenerateHistoryIdAsync(),
                ManagementId = before.ManagementId,
                ChangeField = change.FieldName,
                BeforeValue = change.BeforeValue,
                AfterValue = change.AfterValue,
                UpdatedBy = userName,
                UpdatedAt = after.UpdatedAt
            };

            // 履歴テーブルにレコードを追加
            _context.DeviceHistories.Add(history);
        }
        // まとめて保存
        await _context.SaveChangesAsync();
    }

    // 削除時の履歴作成
    public async Task AddDeleteHistoryAsync(Device before, Device after, string userName)
    {
        // 履歴の Entity 作成
        var history = new DeviceHistory
        {
            HistoryId = await GenerateHistoryIdAsync(),    // 新しい履歴IDを生成して設定
            ManagementId = before.ManagementId,
            ChangeField = "Deleted",
            BeforeValue = null,
            AfterValue = null,
            UpdatedBy = userName,
            UpdatedAt = after.DeletedAt == null ? after.UpdatedAt : after.DeletedAt.Value
        };

        // 履歴テーブルにレコードを追加
        _context.DeviceHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    // HistoryId を生成して返す
    // 採番テーブルを使って、HistoryId が競合しないようにする
    private async Task<string> GenerateHistoryIdAsync()
    {
        var seq = await _sequenceService.GetNextHistoryIdAsync();

        return DeviceHistoryConstants.HISTORY_ID_PREFIX +
               seq.ToString($"D{DeviceHistoryConstants.HISTORY_ID_NUM_DIGIT_COUNT}");
    }

    // HistoryIdの自動採番を履歴テーブル内の最大 HistoryId からの連番になるよう同期
    // ダミーデータ追加時などの整合性の担保
    public async Task SyncHistorySequenceAsync()
    {
        // 履歴テーブル内の最大 HistoryId を取得
        var maxHistoryId = await _context.DeviceHistories
            .Select(h => h.HistoryId)
            .ToListAsync();

        // "UH000001" → 数字部分 "000001" → 1 に変換
        int maxNum = maxHistoryId
            .Select(id => int.Parse(id.Substring(DeviceHistoryConstants.HISTORY_ID_PREFIX.Length)))
            .DefaultIfEmpty(0)
            .Max();

        // 採番テーブルを最新の値に同期
        // 最大値をプレースホルダーで渡す
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE DeviceHistorySequences SET LastUsedNumber = @p0 WHERE Id = 1", maxNum);
    }



    //////////////////////////////////////////
    // --- Details ---
    // 履歴情報を ID から取得し、DTO型で返す
    public async Task<DeviceHistoryDto?> GetHistoryDetailsByIdAsync(int id)
    {
        // DeviceHistoriesテーブルから指定のIdのデータを取得する
        var history = await _context.DeviceHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        // 見つからなかった場合は null を返す
        if (history == null)
        {
            return null;
        }

        // 取得したデータをDTOの項目に詰める
        var dto = new DeviceHistoryDto
        {
            // ChangeField,BeforeValue,AfterValue は Helperを使って日本語表示名に変換
            // BeforeValue,AfterValue は、nullなら"-"に変換される
            Id = history.Id,
            HistoryId = history.HistoryId,
            ManagementId = history.ManagementId,
            ChangeField = EnumDisplayHelper.ResolveDisplayName<DeviceColumns>(history.ChangeField),
            BeforeValue = EnumDisplayHelper.ResolveHistoryDisplay(history.ChangeField, history.BeforeValue),
            AfterValue = EnumDisplayHelper.ResolveHistoryDisplay(history.ChangeField, history.AfterValue),
            UpdatedBy = history.UpdatedBy,
            UpdatedAt = history.UpdatedAt
        };

        // DTO型を返す
        return dto;
    }
}
