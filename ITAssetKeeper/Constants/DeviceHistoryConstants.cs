using ITAssetKeeper.Models.Entities;
using ITAssetKeeper.Models.Enums;
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
        { DeviceHistoryColumns.ManagementId, h => h.ManagementId },
        { DeviceHistoryColumns.ChangeField, h => h.ChangeField },
        { DeviceHistoryColumns.BeforeValue, h => h.BeforeValue },
        { DeviceHistoryColumns.AfterValue, h => h.AfterValue },
        { DeviceHistoryColumns.UpdatedBy, h => h.UpdatedBy },
        { DeviceHistoryColumns.UpdatedAt, h => h.UpdatedAt }
    };
}
