using ITAssetKeeper.Models.Entities;
using System.Runtime.InteropServices;

namespace ITAssetKeeper.Services;

public class DeviceDiffService : IDeviceDiffService
{
    // 変更項目を取得する
    public List<DeviceChange> GetChanges(Device before, Device after)
    {
        // 戻り値用のListを定義
        var changes = new List<DeviceChange>();

        // format用の変数を用意
        var datefm = "yyyy/MM/dd";
        var dateTimefm = "yyyy/MM/dd HH:mm:ss";

        // Deviceの各項目に変更があるかチェック
        // 変更されたものは List に追加していく
        // 更新日の変更は対象外の為、以下には含めない
        Compare(nameof(Device.Category), before.Category, after.Category, changes);
        Compare(nameof(Device.Purpose), before.Purpose, after.Purpose, changes);
        Compare(nameof(Device.ModelNumber), before.ModelNumber, after.ModelNumber, changes);
        Compare(nameof(Device.SerialNumber), before.SerialNumber, after.SerialNumber, changes);
        Compare(nameof(Device.HostName), before.HostName, after.HostName, changes);
        Compare(nameof(Device.Location), before.Location, after.Location, changes);
        Compare(nameof(Device.UserName), before.UserName, after.UserName, changes);
        Compare(nameof(Device.Status), before.Status, after.Status, changes);
        Compare(nameof(Device.Memo), before.Memo, after.Memo, changes);
        Compare(nameof(Device.PurchaseDate), before.PurchaseDate.ToString(datefm), after.PurchaseDate.ToString(datefm), changes);
        Compare(nameof(Device.CreatedAt), before.CreatedAt.ToString(dateTimefm), after.CreatedAt.ToString(dateTimefm), changes);
        Compare(nameof(Device.IsDeleted), before.IsDeleted.ToString(), after.IsDeleted.ToString(), changes);
        Compare(nameof(Device.DeletedAt), before.DeletedAt.HasValue == false ? null : before.DeletedAt.Value.ToString(dateTimefm), after.DeletedAt.HasValue == false ? null : after.DeletedAt.Value.ToString(dateTimefm), changes);
        Compare(nameof(Device.DeletedBy), before.DeletedBy, after.DeletedBy, changes);

        // 変更項目と値が格納されたListを返す
        return changes;
    }

    // 変更された項目がどれかを特定するメソッド
    private void Compare(string field, string? before, string? after, List<DeviceChange> collector)
    {
        // 同一だったら何もしない
        if (before == after)
        {
            return;
        }

        // 同一ではない場合、Dictionaryに詰める
        collector.Add(new DeviceChange
        {
            FieldName = field,
            BeforeValue = before,
            AfterValue = after
        });
    }
}
