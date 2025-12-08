using ITAssetKeeper.Batch;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITAssetKeeper.Functions;

public class SnapshotSyncFunction
{
    private readonly ISnapshotSyncService _snapshotSyncService;
    private readonly ILogger<SnapshotSyncFunction> _logger;

    // コンストラクタで DI されたサービスを受け取る
    public SnapshotSyncFunction(ISnapshotSyncService snapshotSyncService, ILogger<SnapshotSyncFunction> logger)
    {
        _snapshotSyncService = snapshotSyncService;
        _logger = logger;
    }

    // 毎日深夜2:10に実行されるタイマートリガー関数
    [Function("SnapshotSync")]
    public async Task Run([TimerTrigger("0 10 2 * * *")] TimerInfo timer)
    {
        _logger.LogInformation("SnapshotSync function started at: {time}", DateTime.Now);

        try
        {
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
}
