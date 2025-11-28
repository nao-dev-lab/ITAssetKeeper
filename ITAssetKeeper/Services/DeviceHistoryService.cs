using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace ITAssetKeeper.Services;

public class DeviceHistoryService : IDeviceHistoryService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceHistorySequenceService _sequenceService;
    private readonly IUserRoleService _userRoleService;

    public DeviceHistoryService(
        ITAssetKeeperDbContext context,
        IDeviceHistorySequenceService sequenceService,
        IUserRoleService userRoleService)
    {
        _context = context;
        _sequenceService = sequenceService;
        _userRoleService = userRoleService;
    }

    ///////////////////////////////////////////////////
    // -- Index --
    // 検索 & 一覧表示
    public async Task<DeviceHistoryListViewModel> SearchHistoriesAsync(DeviceHistoryListViewModel condition, ClaimsPrincipal user)
    {
        // Deviceテーブルから全てのデータを取得する
        IQueryable<DeviceHistory> query = _context.DeviceHistories;

        // ユーザーのRoleを取得
        var role = await _userRoleService.GetUserRoleAsync(user);

        // Roleに応じてDeviceテーブルから取得する内容を変更
        // Admin:すべて表示
        // それ以外:インフラに関わる機器は除外して表示
        if (role != Roles.Admin)
        {
            // Deviceテーブルからインフラに関わる機器は除外して取得
            var hideCategoryList = DeviceConstants.HIDE_CATEGORIES.Select(c => c.ToString()).ToList();
            query = query.Where(x => x.CategoryAtHistory != null && !hideCategoryList.Contains(x.CategoryAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.FreeText))
        {
            // フリーワードに入力があれば、フリーワード検索
            query = FilterHistoryByFreeText(query, condition);
        }
        else
        {
            // なければ、詳細検索
            query = FilterHistories(query, condition);
        }

        // 並び替え
        query = SortHistories(query, condition.SortKeyValue, condition.SortOrderValue);

        // ページング前の全件数を取得
        condition.TotalCount = query.Count();

        // ページング
        query = PagingHistories(query, condition.PageNumber, condition.PageSize);

        // SQLを実行
        var devices = await query.ToListAsync();

        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices, role);
    }

    // フリーワード検索用フィルタリング
    private IQueryable<DeviceHistory> FilterHistoryByFreeText(IQueryable<DeviceHistory> query, DeviceHistoryListViewModel condition)
    {
        var free = condition.FreeText;

        // フリーワードが日付として解釈できるかチェック
        DateTime dt;
        bool isDate = DateTime.TryParse(free, out dt);

        // フリーワードが含まれる表示名に対応する生値を取得
        var categoryValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DeviceCategory>(free);
        var purposeValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DevicePurpose>(free);
        var statusValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<DeviceStatus>(free);
        var changeTypeValues = EnumDisplayHelper.GetRawOfDisplayNameContainsText<HistoryChangeType>(free);

        // BeforeValue, AfterValue用に、Category, Purpose,Statusのリストを結合する
        var values = categoryValues.Union(purposeValues).Union(statusValues);

        // すべての項目から部分一致で検索
        query = query.Where(x =>
            x.HistoryId.Contains(free)
            || x.ManagementIdAtHistory.Contains(free)
            || x.ModelNumberAtHistory != null && x.ModelNumberAtHistory.Contains(free)
            || x.SerialNumberAtHistory != null && x.SerialNumberAtHistory.Contains(free)
            || x.HostNameAtHistory != null && x.HostNameAtHistory.Contains(free)
            || x.LocationAtHistory != null && x.LocationAtHistory.Contains(free)
            || x.UserNameAtHistory != null && x.UserNameAtHistory.Contains(free)
            || x.MemoAtHistory != null && x.MemoAtHistory.Contains(free)
            || x.UpdatedBy.Contains(free)
            || x.ChangeType != null && changeTypeValues.Contains(x.ChangeType)
            || x.CategoryAtHistory != null && categoryValues.Contains(x.CategoryAtHistory)
            || x.PurposeAtHistory != null && purposeValues.Contains(x.PurposeAtHistory)
            || x.StatusAtHistory != null && statusValues.Contains(x.StatusAtHistory)
            || (isDate && x.UpdatedAt.Date == dt.Date)
            || (isDate && x.PurchaseDateAtHistory != null && x.PurchaseDateAtHistory.Value.Date == dt.Date)
            || (isDate && x.CreatedAtHistory != null && x.CreatedAtHistory.Value.Date == dt.Date)
            );

        return query;
    }

    // フィルタリング (条件に応じて IQueryable<DeviceHistory> を返す)
    private IQueryable<DeviceHistory> FilterHistories(IQueryable<DeviceHistory> query, DeviceHistoryListViewModel condition)
    {
        // --- 部分一致 ---
        if (!string.IsNullOrWhiteSpace(condition.HistoryId))
        {
            query = query.Where(x => x.HistoryId.Contains(condition.HistoryId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ManagementId))
        {
            query = query.Where(x => x.ManagementIdAtHistory.Contains(condition.ManagementId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ModelNumberAtHistory))
        {
            query = query.Where(x => x.ModelNumberAtHistory != null && x.ModelNumberAtHistory.Contains(condition.ModelNumberAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.SerialNumberAtHistory))
        {
            query = query.Where(x => x.SerialNumberAtHistory != null && x.SerialNumberAtHistory.Contains(condition.SerialNumberAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.HostNameAtHistory))
        {
            query = query.Where(x => x.HostNameAtHistory != null && x.HostNameAtHistory.Contains(condition.HostNameAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.LocationAtHistory))
        {
            query = query.Where(x => x.LocationAtHistory != null && x.LocationAtHistory.Contains(condition.LocationAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.UserNameAtHistory))
        {
            query = query.Where(x => x.UserNameAtHistory != null && x.UserNameAtHistory.Contains(condition.UserNameAtHistory));
        }

        if (!string.IsNullOrWhiteSpace(condition.UpdatedBy))
        {
            query = query.Where(x => x.UpdatedBy.Contains(condition.UpdatedBy));
        }


        // --- 完全一致 ---
        if (!string.IsNullOrWhiteSpace(condition.SelectedChangeType))
        {
            query = query.Where(x => x.ChangeType == condition.SelectedChangeType);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedCategoryAtHistory))
        {
            query = query.Where(x => x.CategoryAtHistory == condition.SelectedCategoryAtHistory);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedPurposeAtHistory))
        {
            query = query.Where(x => x.PurposeAtHistory == condition.SelectedPurposeAtHistory);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedStatusAtHistory))
        {
            query = query.Where(x => x.StatusAtHistory == condition.SelectedStatusAtHistory);
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

        // 日付範囲：購入日
        if (condition.PurchaseDateFrom != null || condition.PurchaseDateTo != null)
        {
            // 検索日付の丸め（Date にし、時刻 00:00:00 起点にする）
            var from = condition.PurchaseDateFrom?.Date;
            var to = condition.PurchaseDateTo?.Date;

            // From のみ指定されている場合
            if (from != null && to == null)
            {
                // from ～ 今日の翌日 00:00 まで
                query = query.Where(x =>
                    x.PurchaseDateAtHistory >= from.Value &&
                    x.PurchaseDateAtHistory < DateTime.Today.AddDays(1));
            }
            // To のみ指定されている場合
            else if (from == null && to != null)
            {
                // ～ to の翌日 00:00 まで
                query = query.Where(x =>
                    x.PurchaseDateAtHistory < to.Value.AddDays(1));
            }
            // From と To 両方指定されている場合
            else if (from != null && to != null)
            {
                if (from == to)
                {
                    // 日付一致 → 当日の 00:00 ～ 翌日 00:00
                    query = query.Where(x =>
                        x.PurchaseDateAtHistory >= from.Value &&
                        x.PurchaseDateAtHistory < from.Value.AddDays(1));
                }
                else
                {
                    // from ～ to(包含) → 翌日 00:00 まで
                    query = query.Where(x =>
                        x.PurchaseDateAtHistory >= from.Value &&
                        x.PurchaseDateAtHistory < to.Value.AddDays(1));
                }
            }
        }

        // 日付範囲：登録日
        if (condition.CreatedDateFrom != null || condition.CreatedDateTo != null)
        {
            // 検索日付の丸め（Date にし、時刻 00:00:00 起点にする）
            var from = condition.CreatedDateFrom?.Date;
            var to = condition.CreatedDateTo?.Date;

            // From のみ指定されている場合
            if (from != null && to == null)
            {
                // from ～ 今日の翌日 00:00 まで
                query = query.Where(x =>
                    x.CreatedAtHistory >= from.Value &&
                    x.CreatedAtHistory < DateTime.Today.AddDays(1));
            }
            // To のみ指定されている場合
            else if (from == null && to != null)
            {
                // ～ to の翌日 00:00 まで
                query = query.Where(x =>
                    x.CreatedAtHistory < to.Value.AddDays(1));
            }
            // From と To 両方指定されている場合
            else if (from != null && to != null)
            {
                if (from == to)
                {
                    // 日付一致 → 当日の 00:00 ～ 翌日 00:00
                    query = query.Where(x =>
                        x.CreatedAtHistory >= from.Value &&
                        x.CreatedAtHistory < from.Value.AddDays(1));
                }
                else
                {
                    // from ～ to(包含) → 翌日 00:00 まで
                    query = query.Where(x =>
                        x.CreatedAtHistory >= from.Value &&
                        x.CreatedAtHistory < to.Value.AddDays(1));
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

    // ViewModel 変換(結果を DeviceHistoryListViewModel に詰める)
    private DeviceHistoryListViewModel ToViewModel(DeviceHistoryListViewModel condition, List<DeviceHistory> histories, Roles? role)
    {
        // プルダウン用のデータを定数から取得
        EnumDisplayHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceHistoryColumns>(condition, selectList => condition.SortKeyList = selectList);
        EnumDisplayHelper.SetEnumSelectList<HistoryChangeType>(condition, selectList => condition.ChangeTypeItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.PurposeAtHistoryItems = selectList);
        EnumDisplayHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => condition.StatusAtHistoryItems = selectList);

        // DeviceCategoryのみ、Role別でプルダウン用データを変更
        // Admin：すべて、それ以外：インフラに関わる機器を除外したもの
        if (role == Roles.Admin)
        {
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.CategoryAtHistoryItems = selectList);
        }
        else
        {
            EnumDisplayHelper.SetEnumSelectList<DeviceCategory>(condition, selectList =>
            condition.CategoryAtHistoryItems = new SelectList(
                EnumDisplayHelper.EnumToDictionary(
                    false, (DeviceConstants.HIDE_CATEGORIES)), "Key", "Value"));
        }

        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.Histories = histories
            .Select(x => new DeviceHistoryListItemDto
            {
                // ChangeField,BeforeValue,AfterValue は Helperを使って日本語表示名に変換
                // BeforeValue,AfterValue は、nullなら"-"に変換される
                Id = x.Id,
                HistoryId = x.HistoryId,
                ManagementId = x.ManagementIdAtHistory,
                ChangeType = EnumDisplayHelper.ResolveDisplayName<HistoryChangeType>(x.ChangeType),
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                CategoryAtHistory = x.CategoryAtHistory != null ? EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(x.CategoryAtHistory) : "-",
                PurposeAtHistory = x.PurposeAtHistory != null ? EnumDisplayHelper.ResolveDisplayName<DevicePurpose>(x.PurposeAtHistory) : "-",
                ModelNumberAtHistory = x.ModelNumberAtHistory ?? "-",
                SerialNumberAtHistory = x.SerialNumberAtHistory ?? "-",
                HostNameAtHistory = x.HostNameAtHistory ?? "-",
                LocationAtHistory = x.LocationAtHistory ?? "-",
                UserNameAtHistory = x.UserNameAtHistory ?? "-",
                StatusAtHistory = x.StatusAtHistory != null ? EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(x.StatusAtHistory) : "-",
                MemoAtHistory = x.MemoAtHistory ?? "-",
                PurchaseDateAtHistory = x.PurchaseDateAtHistory.Value,
                CreatedAtHistory = x.CreatedAtHistory.Value,
            })
            .ToList();

        return condition;
    }


    //////////////////////////////////////////
    // --- CreateHistory ---

    // =====================================================
    // 新規登録時の履歴作成
    public async Task AddCreateHistoryAsync(Device created, string userName)
    {
        // 履歴の Entity 作成
        var history = new DeviceHistory
        {
            HistoryId = await GenerateHistoryIdAsync(),    // 新しい履歴IDを生成して設定
            ChangeType = HistoryChangeType.Created.ToString(),
            UpdatedBy = userName,
            UpdatedAt = created.CreatedAt,

            ManagementIdAtHistory = created.ManagementId,
            CategoryAtHistory = created.Category,
            PurposeAtHistory = created.Purpose,
            ModelNumberAtHistory = created.ModelNumber,
            SerialNumberAtHistory = created.SerialNumber,
            HostNameAtHistory = created.HostName,
            LocationAtHistory = created.Location,
            UserNameAtHistory = created.UserName,
            StatusAtHistory = created.Status,
            MemoAtHistory = created.Memo,
            PurchaseDateAtHistory = created.PurchaseDate,
            CreatedAtHistory = created.CreatedAt,

            IsDeletedAtHistory = false,
            DeletedAtHistory = null,
            DeletedByAtHistory = null
        };

        // 履歴テーブルにレコードを追加
        _context.DeviceHistories.Add(history);
    }

    // =====================================================
    // 更新時の履歴作成
    // DeviceDiffService で取得した差分データを元に履歴データを作成する
    public async Task AddUpdateHistoryAsync(Device before, Device after, string userName)
    {
        // 変更前・変更後のスナップショットの取得
        var beforeSnapshot = MapSnapshot(before);
        var afterSnapshot = MapSnapshot(after);

        // 差分リストの取得
        var diff = BuildDiffList(beforeSnapshot, afterSnapshot);

        // 変更がなければ履歴作成をせずに抜ける
        if (!diff.Any(x => x.IsChanged))
        {
            return;
        }

        // 履歴の Entity 作成
        var history = new DeviceHistory
        {
            HistoryId = await GenerateHistoryIdAsync(),
            ChangeType = HistoryChangeType.Updated.ToString(),
            UpdatedBy = userName,
            UpdatedAt = after.UpdatedAt,

            ManagementIdAtHistory = after.ManagementId,
            CategoryAtHistory = after.Category,
            PurposeAtHistory = after.Purpose,
            ModelNumberAtHistory = after.ModelNumber,
            SerialNumberAtHistory = after.SerialNumber,
            HostNameAtHistory = after.HostName,
            LocationAtHistory = after.Location,
            UserNameAtHistory = after.UserName,
            StatusAtHistory = after.Status,
            MemoAtHistory = after.Memo,
            PurchaseDateAtHistory = after.PurchaseDate,
            CreatedAtHistory = after.CreatedAt,

            IsDeletedAtHistory = after.IsDeleted,
            DeletedAtHistory = after.DeletedAt,
            DeletedByAtHistory = after.DeletedBy
        };

        // 履歴テーブルにレコードを追加
        _context.DeviceHistories.Add(history);
    }

    // =====================================================
    // 削除時の履歴作成
    public async Task AddDeleteHistoryAsync(Device before, Device after, string userName)
    {
        // 履歴の Entity 作成
        var history = new DeviceHistory
        {
            HistoryId = await GenerateHistoryIdAsync(),
            ChangeType = HistoryChangeType.Deleted.ToString(),
            UpdatedBy = userName,
            UpdatedAt = after.DeletedAt == null ? after.UpdatedAt : after.DeletedAt.Value,

            ManagementIdAtHistory = before.ManagementId,
            CategoryAtHistory = before.Category,
            PurposeAtHistory = before.Purpose,
            ModelNumberAtHistory = before.ModelNumber,
            SerialNumberAtHistory = before.SerialNumber,
            HostNameAtHistory = before.HostName,
            LocationAtHistory = before.Location,
            UserNameAtHistory = before.UserName,
            StatusAtHistory = before.Status,
            MemoAtHistory = before.Memo,
            PurchaseDateAtHistory = before.PurchaseDate,
            CreatedAtHistory = before.CreatedAt,

            IsDeletedAtHistory = true,
            DeletedAtHistory = after.DeletedAt == null ? after.UpdatedAt : after.DeletedAt.Value,
            DeletedByAtHistory = userName
        };

        // 履歴テーブルにレコードを追加
        _context.DeviceHistories.Add(history);
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
        // 履歴テーブルから全ての HistoryId を取得
        var historyIdList = await _context.DeviceHistories
            .Select(h => h.HistoryId)
            .ToListAsync();

        // HistoryId の数字部分の最大値を取得
        // "UH000001" → 数字部分 "000001" → 1 に変換
        int maxNum = historyIdList
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

    // =====================================================
    // 履歴詳細(差分を取得)
    public async Task<DeviceHistoryDetailViewModel> BuildHistoryDetailAsync(int id)
    {
        // 対象IDの最新の履歴を取得
        var current = await _context.DeviceHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (current == null)
        {
            return null;
        }

        // 対象IDの直前の履歴を取得
        var previous = await _context.DeviceHistories
            .AsNoTracking()
            .Where(x => x.ManagementIdAtHistory == current.ManagementIdAtHistory)
            .Where(x => x.UpdatedAt < current.UpdatedAt)
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync();

        // 最新の履歴をスナップショット化
        var after = CreateSnapshot(current);

        // 直前の履歴をスナップショット化
        // 直前の履歴がなければ null
        DeviceSnapshotDto? before = previous != null ? CreateSnapshot(previous) : null;

        // 新規登録(Created) / 削除(Deleted) → 差分なし
        if (current.ChangeType == "Created" || current.ChangeType == "Deleted")
        {
            // 差分リストは作らずに返す
            return new DeviceHistoryDetailViewModel
            {
                HistoryId = current.HistoryId,
                ChangeType = EnumDisplayHelper.ResolveDisplayName<HistoryChangeType>(current.ChangeType),
                UpdatedBy = current.UpdatedBy,
                UpdatedAt = current.UpdatedAt,
                AfterSnapshot = after,
                BeforeSnapshot = before,
                Fields = new List<DeviceHistoryFieldDiffDto>()
            };
        }

        // Updated → 差分あり
        // before, after の全項目を比較した差分リストを取得
        var diffList = BuildDiffList(before!, after);

        // 差分リストを含めて返す
        return new DeviceHistoryDetailViewModel
        {
            HistoryId = current.HistoryId,
            ChangeType = EnumDisplayHelper.ResolveDisplayName<HistoryChangeType>(current.ChangeType),
            UpdatedBy = current.UpdatedBy,
            UpdatedAt = current.UpdatedAt,
            AfterSnapshot = after,
            BeforeSnapshot = before,
            Fields = diffList
        };
    }


    //////////////////////////////////////////
    // --- Helper ---

    // =====================================================
    // EntityからDTOに変換し、Snapshotを取得
    private DeviceSnapshotDto CreateSnapshot(DeviceHistory entity)
    {
        return new DeviceSnapshotDto
        {
            ManagementId = entity.ManagementIdAtHistory,
            Category = EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(entity.CategoryAtHistory),
            Purpose = EnumDisplayHelper.ResolveDisplayName<DevicePurpose>(entity.PurposeAtHistory),
            ModelNumber = entity.ModelNumberAtHistory ?? "-",
            SerialNumber = entity.SerialNumberAtHistory ?? "-",
            HostName = entity.HostNameAtHistory ?? "-",
            Location = entity.LocationAtHistory ?? "-",
            UserName = entity.UserNameAtHistory ?? "-",
            Status = EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(entity.StatusAtHistory),
            Memo = entity.MemoAtHistory ?? "-",
            PurchaseDate = entity.PurchaseDateAtHistory,
            CreatedAt = entity.CreatedAtHistory,
            IsDeleted = entity.IsDeletedAtHistory,
            DeletedAt = entity.DeletedAtHistory,
            DeletedBy = entity.DeletedByAtHistory ?? "-"
        };
    }

    // =====================================================
    // Entity から Snapshot DTO に変換し、SnapShotを取得
    private DeviceSnapshotDto MapSnapshot(Device device)
    {
        return new DeviceSnapshotDto
        {
            ManagementId = device.ManagementId,
            Category = EnumDisplayHelper.ResolveDisplayName<DeviceCategory>(device.Category),
            Purpose = EnumDisplayHelper.ResolveDisplayName<DevicePurpose>(device.Purpose),
            ModelNumber = device.ModelNumber ?? "-",
            SerialNumber = device.SerialNumber ?? "-",
            HostName = device.HostName ?? "-",
            Location = device.Location ?? "-",
            UserName = device.UserName ?? "-",
            Status = EnumDisplayHelper.ResolveDisplayName<DeviceStatus>(device.Status),
            Memo = device.Memo ?? "-",
            PurchaseDate = device.PurchaseDate,
            CreatedAt = device.CreatedAt,
            IsDeleted = device.IsDeleted,
            DeletedAt = device.DeletedAt,
            DeletedBy = device.DeletedBy ?? "-"
        };
    }

    // =====================================================
    // Diff(全項目比較)
    // Snapshotの before, after の全項目を比較し、差分リストを作成して返す
    private List<DeviceHistoryFieldDiffDto> BuildDiffList(DeviceSnapshotDto before, DeviceSnapshotDto after)
    {
        // 返す為のリストを用意
        var list = new List<DeviceHistoryFieldDiffDto>();

        // 各フィールドを比較し、差分リストに追加
        foreach (var field in DeviceHistoryConstants.SNAPSHOT_TARGET_COLUMNS)
        {
            list.Add(BuildDiffItem(before, after, field));
        }

        // 差分リストを返す
        return list;
    }

    // =====================================================
    // Diffアイテム作成
    private DeviceHistoryFieldDiffDto BuildDiffItem(object beforeDto, object afterDto, string fieldName)
    {
        // DTOの型情報を取得
        var dtoType = beforeDto.GetType();

        // 更新前、更新後の DTO からプロパティ名から値を取得し、
        // 表示用にフォーマットした文字列を取得
        var beforeValue = FormatSnapshotValue(dtoType.GetProperty(fieldName)?.GetValue(beforeDto), fieldName);
        var afterValue = FormatSnapshotValue(dtoType.GetProperty(fieldName)?.GetValue(afterDto), fieldName);

        // プロパティに付与された DisplayAttribute の Name を取得
        var displayName = GetDisplayNameByProperty(dtoType, fieldName);

        // 差分アイテムを作成して返す
        return new DeviceHistoryFieldDiffDto
        {
            FieldName = fieldName,
            DisplayFieldName = displayName,
            BeforeValue = beforeValue,
            AfterValue = afterValue,
            IsChanged = beforeValue != afterValue
        };
    }

    // =====================================================
    // プロパティに付与された DisplayAttribute の Name を取得する
    private string GetDisplayNameByProperty(Type dtoType,string fieldName)
    {
        // Dto のプロパティ情報を取得
        var prop = dtoType.GetProperty(fieldName);

        // プロパティが見つからない場合はそのまま返す
        if (prop == null)
        {
            return fieldName;
        }

        // DisplayAttribute を取得し、Name プロパティを返す
        var attr = prop.GetCustomAttributes(typeof(DisplayAttribute), false)
                       .OfType<DisplayAttribute>()
                       .FirstOrDefault();

        // DisplayAttribute がなければプロパティ名を返す
        if (attr?.Name == null)
        {
            return prop.Name;
        }

        // DisplayAttribute の Name を返す
        return attr.Name;
    }

    // =====================================================
    // スナップショット用の値フォーマット
    private string FormatSnapshotValue(object? value, string fieldName)
    {
        if (value == null)
        {
            return "-";
        }

        // 型ごとにフォーマットを分ける
        return value switch
        {
            // CreatedAt と DeletedAt は時刻までで変換
            DateTime dt 
                when fieldName == nameof(DeviceSnapshotDto.CreatedAt) 
                || fieldName == nameof(DeviceSnapshotDto.DeletedAt)
                    => dt.ToString("yyyy/MM/dd HH:mm:ss"),

            // PurchaseDate は日付のみに変換
            DateTime dt => dt.ToString("yyyy/MM/dd"),

            // それ以外はそのまま文字列変換
            _ => value.ToString() ?? ""
        };
    }
}
