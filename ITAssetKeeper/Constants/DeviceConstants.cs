using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using System.Linq.Expressions;

namespace ITAssetKeeper.Constants;

public class DeviceConstants
{
    // 各カラムに対応するラムダ式のdintionaryを定義
    // Expression で メソッドを IQueryable にバインドする
    public static readonly Dictionary<SortKeyColums, Expression<Func<Device, object>>> SORT_KEY_SELECTORS =
    new()
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
}
