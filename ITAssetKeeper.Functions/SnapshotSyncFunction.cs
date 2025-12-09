using ITAssetKeeper.Batch;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ITAssetKeeper.Functions;

public class SnapshotSyncFunction
{
    private readonly ISnapshotSyncService _snapshotSyncService;
    private readonly ILogger<SnapshotSyncFunction> _logger;
    private readonly string _connectionString;

    // コンストラクタで DI されたサービスを受け取る
    public SnapshotSyncFunction(
        ISnapshotSyncService snapshotSyncService,
        ILogger<SnapshotSyncFunction> logger,
        IConfiguration config)
    {
        _snapshotSyncService = snapshotSyncService;
        _logger = logger;
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    // 毎日深夜2:10に実行されるタイマートリガー関数
    [Function("SnapshotSync")]
    public async Task Run([TimerTrigger("0 10 2 * * *")] TimerInfo timer)
    {
        _logger.LogInformation("SnapshotSync function started at: {time}", DateTime.Now);

        try
        {
            // Azure SQL Database が休止状態の場合に備え、起こす
            await WakeUpDatabaseAsync();

            // 少し待機（DB起動待ち）
            await Task.Delay(TimeSpan.FromMinutes(1));

            // スナップショット同期処理を実行
            await _snapshotSyncService.ExecuteAsync();
            _logger.LogInformation("SnapshotSync function completed successfully at: {time}", DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SnapshotSync function failed at: {time}", DateTime.Now);
            throw;
        }
    }

    // Azure SQL Database が休止状態 (paused) の場合に備え、
    // 起こす為に適当なクエリを投げる
    private async Task WakeUpDatabaseAsync()
    {
        try
        {
            // DB接続
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // 簡単なクエリを実行してデータベースを起こす
            using var command = new SqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();

        }
        catch (Exception)
        {
            // 失敗前提なので何もしない
        }
    }
}
