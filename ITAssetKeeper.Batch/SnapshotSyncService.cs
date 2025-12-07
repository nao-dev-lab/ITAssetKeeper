using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ITAssetKeeper.Batch;

public class SnapshotSyncService : ISnapshotSyncService
{
    private readonly string _connectionString;

    public SnapshotSyncService(IConfiguration config)
    {
        // 接続文字列を取得
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    // スナップショットから本番テーブルへ同期
    public async Task ExecuteAsync()
    {
        // DB接続
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // トランザクション開始
        using var transaction = connection.BeginTransaction();

        try
        {
            // Devices_Snapshot 件数チェック
            var countCmdDevice = new SqlCommand(
                "SELECT COUNT(*) FROM Devices_Snapshot",
                connection, transaction);

            // 件数取得
            var snapshotCountDevice = (int)await countCmdDevice.ExecuteScalarAsync();

            // 件数0件の場合は例外
            if (snapshotCountDevice == 0)
                throw new InvalidOperationException("Devices_Snapshot table is empty.");

            // DevicesHistories_Snapshot 件数チェック
            var countCmdHistories = new SqlCommand(
                "SELECT COUNT(*) FROM DeviceHistories_Snapshot",
                connection, transaction);

            // 件数取得
            var snapshotCountHistories = (int)await countCmdHistories.ExecuteScalarAsync();

            // 件数0件の場合は例外
            if (snapshotCountHistories == 0)
                throw new InvalidOperationException("DeviceHistories_Snapshot table is empty.");

            // Devices 削除
            await new SqlCommand(
                "DELETE FROM Devices",
                connection, transaction).ExecuteNonQueryAsync();

            // DeviceHistories 削除
            await new SqlCommand(
                "DELETE FROM DeviceHistories",
                connection, transaction).ExecuteNonQueryAsync();

            // Devices 再構築
            await new SqlCommand(
                @"
                INSERT INTO Devices (
                ManagementId,
                Category,
                Purpose,
                ModelNumber,
                SerialNumber,
                HostName,
                Location,
                UserName,
                Status,
                PurchaseDate,
                CreatedAt,
                UpdatedAt,
                IsDeleted,
                DeletedAt,
                DeletedBy
                )
                SELECT
                ManagementId,
                Category,
                Purpose,
                ModelNumber,
                SerialNumber,
                HostName,
                Location,
                UserName,
                Status,
                PurchaseDate,
                CreatedAt,
                UpdatedAt,
                IsDeleted,
                DeletedAt,
                DeletedBy
                FROM Devices_Snapshot
                ",
                connection, transaction).ExecuteNonQueryAsync();

            // DeviceHistories 再構築
            // Devices 再構築
            await new SqlCommand(
                @"
                INSERT INTO DeviceHistories (
                HistoryId,
                ChangeType,
                UpdatedBy,
                UpdatedAt,
                ManagementIdAtHistory,
                CategoryAtHistory,
                PurposeAtHistory,
                ModelNumberAtHistory,
                SerialNumberAtHistory,
                HostNameAtHistory,
                LocationAtHistory,
                UserNameAtHistory,
                StatusAtHistory,
                MemoAtHistory,
                PurchaseDateAtHistory,
                CreatedAtHistory,
                IsDeletedAtHistory,
                DeletedAtHistory,
                DeletedByAtHistory
                )
                SELECT
                HistoryId,
                ChangeType,
                UpdatedBy,
                UpdatedAt,
                ManagementIdAtHistory,
                CategoryAtHistory,
                PurposeAtHistory,
                ModelNumberAtHistory,
                SerialNumberAtHistory,
                HostNameAtHistory,
                LocationAtHistory,
                UserNameAtHistory,
                StatusAtHistory,
                MemoAtHistory,
                PurchaseDateAtHistory,
                CreatedAtHistory,
                IsDeletedAtHistory,
                DeletedAtHistory,
                DeletedByAtHistory
                FROM DeviceHistories_Snapshot
                ",
                connection, transaction).ExecuteNonQueryAsync();

            // --- 採番同期（Devices） ---
            await new SqlCommand(
                @"
                UPDATE DeviceSequences
                SET LastUsedNumber = (
                SELECT ISNULL(
                    MAX(CAST(SUBSTRING(ManagementId, 3, LEN(ManagementId)) AS INT)),
                    0
                    )
                FROM Devices
                )
                WHERE Id = 1;
                ",
                connection, transaction
            ).ExecuteNonQueryAsync();

            // --- 採番同期（DeviceHistories） ---
            await new SqlCommand(
                @"
                UPDATE DeviceHistorySequences
                SET LastUsedNumber = (
                SELECT ISNULL(
                    MAX(CAST(SUBSTRING(HistoryId, 3, LEN(HistoryId)) AS INT)),
                    0
                )
                 FROM DeviceHistories
                )
                WHERE Id = 1;
                ",
                connection, transaction
            ).ExecuteNonQueryAsync();


            // コミット
            transaction.Commit();
        }
        catch (Exception)
        {
            // エラー発生時はロールバック
            await transaction.RollbackAsync();
            throw;
        }
    }
}
