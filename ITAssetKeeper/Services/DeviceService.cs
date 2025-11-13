using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
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
            query = query.Where(x => x.HostName!.Contains(condition.HostName));
        }

        if (!string.IsNullOrWhiteSpace(condition.Location))
        {
            query = query.Where(x => x.Location.Contains(condition.Location));
        }

        if (!string.IsNullOrWhiteSpace(condition.UserName))
        {
            query = query.Where(x => x.UserName!.Contains(condition.UserName));
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
        if (condition.SelectedSortOrder != null && condition.SelectedSortKey == null)
        {
            if (condition.SelectedSortOrder == SortOrder.Asc)
            {
                query = query.OrderBy(d => d.ManagementId);
            }
            else
            {
                query = query.OrderByDescending(d => d.ManagementId);
            }
        }
        else if (condition.SelectedSortOrder == null && condition.SelectedSortKey != null)
        {
            switch (condition.SelectedSortKey)
            {
                case SortKeyColums.ManagementId:
                    query = query.OrderBy(d => d.ManagementId);
                    break;
                case SortKeyColums.Category:
                    query = query.OrderBy(d => d.Category);
                    break;
                case SortKeyColums.ModelNumber:
                    query = query.OrderBy(d => d.ModelNumber);
                    break;
                case SortKeyColums.SerialNumber:
                    query = query.OrderBy(d => d.SerialNumber);
                    break;
                case SortKeyColums.HostName:
                    query = query.OrderBy(d => d.HostName);
                    break;
                case SortKeyColums.Location:
                    query = query.OrderBy(d => d.Location);
                    break;
                case SortKeyColums.UserName:
                    query = query.OrderBy(d => d.UserName);
                    break;
                case SortKeyColums.Status:
                    query = query.OrderBy(d => d.Status);
                    break;
            }
        }
        else if (condition.SelectedSortOrder != null && condition.SelectedSortKey != null)
        {
            if (condition.SelectedSortOrder == SortOrder.Asc)
            {
                switch (condition.SelectedSortKey)
                {
                    case SortKeyColums.ManagementId:
                        query = query.OrderBy(d => d.ManagementId);
                        break;
                    case SortKeyColums.Category:
                        query = query.OrderBy(d => d.Category);
                        break;
                    case SortKeyColums.ModelNumber:
                        query = query.OrderBy(d => d.ModelNumber);
                        break;
                    case SortKeyColums.SerialNumber:
                        query = query.OrderBy(d => d.SerialNumber);
                        break;
                    case SortKeyColums.HostName:
                        query = query.OrderBy(d => d.HostName);
                        break;
                    case SortKeyColums.Location:
                        query = query.OrderBy(d => d.Location);
                        break;
                    case SortKeyColums.UserName:
                        query = query.OrderBy(d => d.UserName);
                        break;
                    case SortKeyColums.Status:
                        query = query.OrderBy(d => d.Status);
                        break;
                }
            }
            else
            {
                switch (condition.SelectedSortKey)
                {
                    case SortKeyColums.ManagementId:
                        query = query.OrderByDescending(d => d.ManagementId);
                        break;
                    case SortKeyColums.Category:
                        query = query.OrderByDescending(d => d.Category);
                        break;
                    case SortKeyColums.ModelNumber:
                        query = query.OrderByDescending(d => d.ModelNumber);
                        break;
                    case SortKeyColums.SerialNumber:
                        query = query.OrderByDescending(d => d.SerialNumber);
                        break;
                    case SortKeyColums.HostName:
                        query = query.OrderByDescending(d => d.HostName);
                        break;
                    case SortKeyColums.Location:
                        query = query.OrderByDescending(d => d.Location);
                        break;
                    case SortKeyColums.UserName:
                        query = query.OrderByDescending(d => d.UserName);
                        break;
                    case SortKeyColums.Status:
                        query = query.OrderByDescending(d => d.Status);
                        break;
                }
            }
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
        listVm.Category = new SelectList(Enum.GetNames<DeviceCategory>());
        listVm.Purpose = new SelectList(Enum.GetNames<DevicePurpose>());
        listVm.Status = new SelectList(Enum.GetNames<DeviceStatus>());
        listVm.SortOrder = new SelectList(Enum.GetNames<SortOrder>());
        listVm.SortKey = new SelectList(Enum.GetNames<SortKeyColums>());

        // 検索結果の一覧表示のデータをDTO型で詰める
        listVm.Devices = result
            .Select(x => new DeviceDto
            {
                ManagementId = x.ManagementId,
                Category = x.Category,
                ModelNumber = x.ModelNumber,
                SerialNumber = x.SerialNumber,
                HostName = x.HostName,
                Location = x.Location,
                UserName = x.UserName,
                Status = x.Status
            })
            .ToList();

        return listVm;
    }
}
