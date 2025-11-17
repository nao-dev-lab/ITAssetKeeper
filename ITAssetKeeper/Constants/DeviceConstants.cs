using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using System.Linq.Expressions;

namespace ITAssetKeeper.Constants;

public class DeviceConstants
{
    // 機器管理IDのプレフィックス
    public static readonly string DEVICE_ID_PREFIX = "DE";
    // 機器管理IDの数字の桁数
    public static readonly int DEVICE_ID_NUM_DIGIT_COUNT = 6;

    // 各カラムに対応するdintionary
    // Expression で メソッドを IQueryable にバインドする
    public static readonly Dictionary<SortKeys, Expression<Func<Device, object>>> SORT_KEY_SELECTORS =
    new()
    {
        { SortKeys.ManagementId, d => d.ManagementId },
        { SortKeys.Category, d => d.Category },
        { SortKeys.Purpose, d => d.Purpose },
        { SortKeys.ModelNumber, d => d.ModelNumber },
        { SortKeys.SerialNumber, d => d.SerialNumber },
        { SortKeys.HostName, d => d.HostName },
        { SortKeys.Location, d => d.Location },
        { SortKeys.UserName, d => d.UserName },
        { SortKeys.Status, d => d.Status },
        { SortKeys.PurchaseDate, d => d.PurchaseDate },
        { SortKeys.UpdatedAt, d => d.UpdatedAt }
    };
}
