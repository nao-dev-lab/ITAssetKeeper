using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Services;

public class DeviceHistoryService : IDeviceHistoryService
{
    private readonly ITAssetKeeperDbContext _context;

    public DeviceHistoryService(ITAssetKeeperDbContext context)
    {
        _context = context;
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
    public IQueryable<DeviceHistory> FilterHistories(IQueryable<DeviceHistory> query, DeviceHistoryViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        //query = _context.DeviceHistories;

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
    public IQueryable<DeviceHistory> SortHistories(IQueryable<DeviceHistory> query, DeviceHistoryColumns sortKey, SortOrders sortOrder)
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
    public IQueryable<DeviceHistory> PagingHistories(IQueryable<DeviceHistory> query, int pageNumber, int pageSize)
    {
        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        return query;
    }

    // ViewModel 変換(結果を DeviceHistoryViewModel に詰める)
    public DeviceHistoryViewModel ToViewModel(DeviceHistoryViewModel condition, List<DeviceHistory> histories)
    {
        // プルダウン用のデータを定数から取得
        SelectListHelper.SetEnumSelectList<SortOrders>(condition, selectList => condition.SortOrderList = selectList);
        SelectListHelper.SetEnumSelectList<DeviceHistoryColumns>(condition, selectList => condition.SortKeyList = selectList);

        SelectListHelper.SetEnumSelectList<DeviceColumns>(condition, selectList => condition.ChangeFieldItems = selectList);

        // DeviceColumns は UpdatedAt を除外して取得する
        SelectListHelper.SetEnumSelectList<DeviceColumns>(condition, selectList =>
            condition.ChangeFieldItems = new SelectList(SelectListHelper.ToDictionary(DeviceColumns.UpdatedAt), "Key", "Value"));


        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.DeviceHistories = histories
            .Select(x => new DeviceHistoryDto
            {
                Id = x.Id,
                HistoryId = x.HistoryId,
                ManagementId = x.ManagementId,
                ChangeField = x.ChangeField,
                BeforeValue = x.BeforeValue,
                AfterValue = x.AfterValue,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();

        return condition;
    }
}
