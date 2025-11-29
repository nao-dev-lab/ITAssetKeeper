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
        // 検索メソッド実行、結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);

        // 詳細検索で何か値が入れられたかをチェックし、プロパティにセット
        vm.IsSearchExecuted = vm.HasAnyFilter;

        // ビューに渡す
        return View(vm);
    }

    // GET: DeviceHistory/GetSortedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(DeviceHistoryColumns sortKey, SortOrders sortOrder, DeviceHistoryListViewModel model)
    {
        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }

    // GET: DeviceHistory/GetPagedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceHistoryListViewModel model)
    {
        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model, User);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }

    // GET: DeviceHistory/Details
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        //Id が不正なら中断
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "不正なパラメータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // 差分ロジックを含む詳細データを取得
        var historyDetail = await _deviceHistoryService.BuildHistoryDetailAsync(id);

        // 取得できなければ一覧に戻す
        if (historyDetail == null)
        {
            TempData["ErrorMessage"] = "対象の履歴情報が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        return View(historyDetail);
    }
}
