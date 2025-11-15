using ITAssetKeeper.Constants;
using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ITAssetKeeper.Services;

public class DeviceService : IDeviceService
{
    private readonly ITAssetKeeperDbContext _context;

    public DeviceService(ITAssetKeeperDbContext context)
    {
        _context = context;
    }

    // 検索 & 一覧表示
    public async Task<DeviceListViewModel> SearchDevicesAsync(DeviceListViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        IQueryable<Device> query = _context.Devices;

        // フィルタリング実施
        query = FilterDevices(query, condition);

        // 並び替え
        query = SortDevices(query, condition.SortKeyValue, condition.SortOrderValue);

        // ページング前の全件数を取得
        condition.TotalCount = query.Count();

        // ページング
        query = PagingDevices(query, condition.PageNumber, condition.PageSize);

        // SQLを実行
        var devices = await query.ToListAsync();

        // ビューモデルに詰めて、呼び出し元に返す
        return ToViewModel(condition, devices);
    }

    // フィルタリング (条件に応じて IQueryable<Device> を返す)
    public IQueryable<Device> FilterDevices(IQueryable<Device> query, DeviceListViewModel condition)
    {
        // Deviceテーブルから全てのデータを取得する
        query = _context.Devices;

        // 部分一致
        if (!string.IsNullOrWhiteSpace(condition.ManagementId))
        {
            query = query.Where(x => x.ManagementId.Contains(condition.ManagementId));
        }

        if (!string.IsNullOrWhiteSpace(condition.ModelNumber))
        {
            query = query.Where(x => x.ModelNumber.Contains(condition.ModelNumber));
        }

        if (!string.IsNullOrWhiteSpace(condition.SerialNumber))
        {
            query = query.Where(x => x.SerialNumber.Contains(condition.SerialNumber));
        }

        if (!string.IsNullOrWhiteSpace(condition.HostName))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.HostName != null && x.HostName.Contains(condition.HostName));
        }

        if (!string.IsNullOrWhiteSpace(condition.Location))
        {
            query = query.Where(x => x.Location.Contains(condition.Location));
        }

        if (!string.IsNullOrWhiteSpace(condition.UserName))
        {
            // 実データ側のnullチェックをいれる
            query = query.Where(x => x.UserName != null && x.UserName.Contains(condition.UserName));
        }

        // 完全一致
        if (!string.IsNullOrWhiteSpace(condition.SelectedCategory))
        {
            query = query.Where(x => x.Category == condition.SelectedCategory);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedPurpose))
        {
            query = query.Where(x => x.Purpose == condition.SelectedPurpose);
        }

        if (!string.IsNullOrWhiteSpace(condition.SelectedStatus))
        {
            query = query.Where(x => x.Status == condition.SelectedStatus);
        }

        // 範囲
        if (condition.PurchaseDateFrom != null && condition.PurchaseDateTo == null)
        {
            // PurchaseDateFrom ～ 今日まで
            query = query.Where(x => x.PurchaseDate >= condition.PurchaseDateFrom && x.PurchaseDate <= DateTime.Now);
        }
        else if (condition.PurchaseDateFrom == null && condition.PurchaseDateTo != null)
        {
            // PurchaseDateTo 以前のものすべて
            query = query.Where(x => x.PurchaseDate <= condition.PurchaseDateTo);
        }
        else if (condition.PurchaseDateFrom != null && condition.PurchaseDateTo != null)
        {
            if (condition.PurchaseDateFrom == condition.PurchaseDateTo)
            {
                // 同一の日付が入力されている場合は、PurchaseDateFromで指定された日付
                query = query.Where(x => x.PurchaseDate == condition.PurchaseDateFrom);
            }
            else
            {
                // PurchaseDateFrom ～ PurchaseDateToの範囲
                query = query.Where(x => x.PurchaseDate >= condition.PurchaseDateFrom && x.PurchaseDate <= condition.PurchaseDateTo);
            }
        }

        return query;
    }

    // ソート (フィルタ済み IQueryable を昇順 / 降順に並べ替える)
    public IQueryable<Device> SortDevices(IQueryable<Device> query, SortKeyColums sortKey, SortOrder sortOrder)
    {
        // 指定されたSortKeyを基準に、
        // 指定されたSortOrderに応じて、昇順 or 降順でソートする
        if (sortOrder == SortOrder.Asc)
        {
            query = query.OrderBy(DeviceConstants.SORT_KEY_SELECTORS[sortKey]);
        }
        else
        {
            query = query.OrderByDescending(DeviceConstants.SORT_KEY_SELECTORS[sortKey]);
        }

        return query;
    }

    // ページング (Skip/Take の適用)
    public IQueryable<Device> PagingDevices(IQueryable<Device> query, int pageNumber, int pageSize)
    {
        // ページングを適用
        query = query.Skip((pageNumber - 1) * pageSize);
        query = query.Take(pageSize);

        return query;
    }

    // ViewModel 変換(結果を DeviceListViewModel に詰める)
    public DeviceListViewModel ToViewModel(DeviceListViewModel condition, List<Device> devices)
    {
        // プルダウン用のデータを定数から取得
        SelectListHelper.SetEnumSelectList<DeviceCategory>(condition, selectList => condition.Category = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(condition, selectList => condition.Purpose = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(condition, selectList => condition.Status = selectList);
        SelectListHelper.SetEnumSelectList<SortOrder>(condition, selectList => condition.SortOrderList = selectList);
        SelectListHelper.SetEnumSelectList<SortKeyColums>(condition, selectList => condition.SortKeyList = selectList);

        // 検索結果の一覧表示のデータをDTO型で詰める
        condition.Devices = devices
            .Select(x => new DeviceDto
            {
                ManagementId = x.ManagementId,
                Category = x.Category,
                Purpose = x.Purpose,
                ModelNumber = x.ModelNumber,
                SerialNumber = x.SerialNumber,
                HostName = x.HostName,
                Location = x.Location,
                UserName = x.UserName,
                Status = x.Status,
                PurchaseDate = x.PurchaseDate.ToString("yyyy/MM/dd"),
            })
            .ToList();

        return condition;
    }

    // Create,Edit,Details画面のinitialize
    public DeviceManageViewModel InitializeDeviceManage(DeviceManageViewModel model, ViewMode viewMode)
    {
        if (viewMode == ViewMode.Create)
        {
            // ビューモデルに、ドロップダウン用のSelectListをセット
            SetSelectList(model);
            model.PurchaseDate = DateTime.Now;
        }
        return model;
    }

    // Create,Edit,Details のビューモデルに、ドロップダウン用のSelectListをセット
    public void SetSelectList(DeviceManageViewModel vm)
    {
        // プルダウン用のデータを定数から取得
        SelectListHelper.SetEnumSelectList<DeviceCategory>(vm, selectList => vm.Category = selectList);
        SelectListHelper.SetEnumSelectList<DevicePurpose>(vm, selectList => vm.Purpose = selectList);
        SelectListHelper.SetEnumSelectList<DeviceStatus>(vm, selectList => vm.Status = selectList);
    }
}
