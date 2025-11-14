using ITAssetKeeper.Data;
using ITAssetKeeper.Helpers;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            query = query.Where(x => x.PurchaseDate < condition.PurchaseDateTo);
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
                query = query.Where(x => x.PurchaseDate > condition.PurchaseDateFrom && x.PurchaseDate < condition.PurchaseDateTo);
            }
        }

        // 並び替え
        // 各カラムに対応するラムダ式のdintionaryを定義
        // Expression で メソッドを IQueryable にバインドする
        var dictSortKeys = new Dictionary<SortKeyColums, Expression<Func<Device, object>>>
        {
            { SortKeyColums.ManagementId, d => d.ManagementId },
            { SortKeyColums.Category, d => d.Category },
            { SortKeyColums.Purpose, d => d.Purpose },
            { SortKeyColums.ModelNumber, d => d.ModelNumber },
            { SortKeyColums.SerialNumber, d => d.SerialNumber },
            { SortKeyColums.HostName, d => d.HostName },
            { SortKeyColums.Location, d => d.Location },
            { SortKeyColums.UserName, d => d.UserName },
            { SortKeyColums.Status, d => d.Status },
            { SortKeyColums.PurchaseDate, d => d.PurchaseDate }
        };

        // 指定されたSortKeyを基準に、
        // 指定されたSortOrderに応じて、昇順 or 降順でソートする
        if (condition.SortOrderValue == SortOrder.Asc)
        {
            query = query.OrderBy(dictSortKeys[condition.SortKeyValue]);
        }
        else
        {
            query = query.OrderByDescending(dictSortKeys[condition.SortKeyValue]);
        }

        // ページング
        query = query.Skip((condition.PageNumber - 1) * condition.PageSize);
        query = query.Take(condition.PageSize);

        // SQLを実行
        var result = await query.ToListAsync();

        // 機器一覧用のViewModelのインスタンス生成
        var listVm = new DeviceListViewModel();

        // プルダウン用のデータを定数から取得し、
        // DTO型でデータを詰める
        listVm.Category = EnumHelper.ToSelectList<DeviceCategory>();
        listVm.Purpose = EnumHelper.ToSelectList<DevicePurpose>();
        listVm.Status = EnumHelper.ToSelectList<DeviceStatus>();
        listVm.SortOrderList = EnumHelper.ToSelectList<SortOrder>();
        listVm.SortKeyList = EnumHelper.ToSelectList<SortKeyColums>();

        // 検索結果の一覧表示のデータをDTO型で詰める
        listVm.Devices = result
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
                PurchaseDate = x.PurchaseDate.ToString("yyyy/MM/dd")
            })
            .ToList();

        return listVm;
    }
}
