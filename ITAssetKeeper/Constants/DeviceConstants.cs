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

    // Editor, Viewerには表示しない種別の配列
    public static readonly DeviceCategory[] HIDE_CATEGORIES =
    [
        DeviceCategory.Switch_L2,
        DeviceCategory.Switch_L3,
        DeviceCategory.Router_Core,
        DeviceCategory.Server,
        DeviceCategory.Firewall,
        DeviceCategory.Ups
    ];

    // Deviceの各カラムに対応するdintionary
    public static readonly Dictionary<DeviceColumns, Expression<Func<Device, object>>> SORT_KEY_SELECTORS =
    new()
    {
        { DeviceColumns.ManagementId, d => d.ManagementId },
        { DeviceColumns.Category, d => d.Category },
        { DeviceColumns.Purpose, d => d.Purpose },
        { DeviceColumns.ModelNumber, d => d.ModelNumber },
        { DeviceColumns.SerialNumber, d => d.SerialNumber },
        { DeviceColumns.HostName, d => d.HostName },
        { DeviceColumns.Location, d => d.Location },
        { DeviceColumns.UserName, d => d.UserName },
        { DeviceColumns.Status, d => d.Status },
        { DeviceColumns.PurchaseDate, d => d.PurchaseDate },
        { DeviceColumns.UpdatedAt, d => d.UpdatedAt }
    };
}
