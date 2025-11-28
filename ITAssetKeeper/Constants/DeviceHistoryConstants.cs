using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using System.Linq.Expressions;

namespace ITAssetKeeper.Constants;

public class DeviceHistoryConstants
{
    // 履歴IDのプレフィックス
    public static readonly string HISTORY_ID_PREFIX = "UH";
    // 機器管理IDの数字の桁数
    public static readonly int HISTORY_ID_NUM_DIGIT_COUNT = 6;

    // DeviceHistoryの各カラムに対応するdintionary
    public static readonly Dictionary<DeviceHistoryColumns, Expression<Func<DeviceHistory, object>>> SORT_KEY_SELECTORS =
    new()
    {
        { DeviceHistoryColumns.HistoryId, h => h.HistoryId },
        { DeviceHistoryColumns.ChangeType, h => h.ChangeType },
        { DeviceHistoryColumns.UpdatedBy, h => h.UpdatedBy },
        { DeviceHistoryColumns.UpdatedAt, h => h.UpdatedAt },
        { DeviceHistoryColumns.ManagementIdAtHistory, h => h.ManagementIdAtHistory },
        { DeviceHistoryColumns.CategoryAtHistory, h => h.CategoryAtHistory },
        { DeviceHistoryColumns.PurposeAtHistory, h => h.PurposeAtHistory },
        { DeviceHistoryColumns.ModelNumberAtHistory, h => h.ModelNumberAtHistory },
        { DeviceHistoryColumns.SerialNumberAtHistory, h => h.SerialNumberAtHistory },
        { DeviceHistoryColumns.HostNameAtHistory, h => h.HostNameAtHistory },
        { DeviceHistoryColumns.LocationAtHistory, h => h.LocationAtHistory },
        { DeviceHistoryColumns.UserNameAtHistory, h => h.UserNameAtHistory },
        { DeviceHistoryColumns.StatusAtHistory, h => h.StatusAtHistory },
        { DeviceHistoryColumns.PurchaseDateAtHistory, h => h.PurchaseDateAtHistory },
        { DeviceHistoryColumns.CreatedAtHistory, h => h.CreatedAtHistory }
    };

    // Snapshot取得対象
    public static readonly string[] SNAPSHOT_TARGET_COLUMNS =
    [
        nameof(DeviceSnapshotDto.ManagementId),
        nameof(DeviceSnapshotDto.Category),
        nameof(DeviceSnapshotDto.Purpose),
        nameof(DeviceSnapshotDto.ModelNumber),
        nameof(DeviceSnapshotDto.SerialNumber),
        nameof(DeviceSnapshotDto.HostName),
        nameof(DeviceSnapshotDto.Location),
        nameof(DeviceSnapshotDto.UserName),
        nameof(DeviceSnapshotDto.Status),
        nameof(DeviceSnapshotDto.Memo),
        nameof(DeviceSnapshotDto.PurchaseDate),
        nameof(DeviceSnapshotDto.CreatedAt),
        nameof(DeviceSnapshotDto.DeletedAt),
        nameof(DeviceSnapshotDto.DeletedBy)
    ];
}
