using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DeviceHistoryController : Controller
{
    private readonly IDeviceHistoryService _deviceHistoryService;
    private readonly ILogger<DeviceHistoryController> _logger;
    public DeviceHistoryController(
        IDeviceHistoryService deviceHistoryService,
        ILogger<DeviceHistoryController> logger)
    {
        _deviceHistoryService = deviceHistoryService;
        _logger = logger;
    }

    // GET: DeviceHistory/Index
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(DeviceHistoryListViewModel model)
    {
        _logger.LogInformation("Index(GET) 開始");

        // 検索メソッド実行、結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);
        _logger.LogInformation("Index(GET) 検索完了 件数={Count}", vm.Histories?.Count);

        // 詳細検索で何か値が入れられたかをチェックし、プロパティにセット
        vm.IsSearchExecuted = vm.HasAnyFilter;
        _logger.LogInformation("Index(GET) 詳細検索実行フラグ={IsSearchExecuted}", vm.IsSearchExecuted);

        // ビューに渡す
        _logger.LogInformation("Index(GET) 終了");
        return View(vm);
    }

    // GET: DeviceHistory/GetSortedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(DeviceHistoryColumns sortKey, SortOrders sortOrder, DeviceHistoryListViewModel model)
    {
        _logger.LogInformation("GetSortedList 開始 SortKey={SortKey}, SortOrder={SortOrder}", sortKey, sortOrder);

        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);
        _logger.LogInformation("GetSortedList 検索完了 件数={Count}", vm.Histories?.Count);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }

    // GET: DeviceHistory/GetPagedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceHistoryListViewModel model)
    {
        _logger.LogInformation("GetPagedList 開始 Page={Page}, Size={Size}", model.PageNumber, model.PageSize);

        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);
        _logger.LogInformation("GetPagedList 検索完了 件数={Count}", vm.Histories?.Count);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }

    // GET: DeviceHistory/Details
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Details 開始 Id={Id}", id);

        //Id が不正なら中断
        if (id <= 0)
        {
            _logger.LogWarning("Details 不正なパラメータ Id={Id}", id);
            TempData["ErrorMessage"] = "不正なパラメータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // 差分ロジックを含む詳細データを取得
        var historyDetail = await _deviceHistoryService.BuildHistoryDetailAsync(id);
        _logger.LogInformation("Details 取得結果 DeviceHistory={DeviceHistory}", historyDetail != null ? "Found" : "NotFound");

        // 取得できなければ一覧に戻す
        if (historyDetail == null)
        {
            _logger.LogWarning("Details 対象の履歴情報が見つかりません Id={Id}", id);
            TempData["ErrorMessage"] = "対象の履歴情報が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Details 終了 Id={Id}", id);
        return View(historyDetail);
    }
}
