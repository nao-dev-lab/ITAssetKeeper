using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Dashboard;
using ITAssetKeeper.Models.ViewModels.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using ITAssetKeeper.Services;
using AspNetCoreGeneratedDocument;

namespace ITAssetKeeper.Controllers;

public class DeviceController : Controller
{
    private readonly IDeviceService _deviceService;

    public DeviceController(ITAssetKeeperDbContext context, IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    // GET: Device/Index
    [HttpGet]
    public async Task<IActionResult> Index(DeviceListViewModel model)
    {
        // 検索メソッド実行、結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // ビューに渡す
        return View(vm);
    }

    // GET: Device/GetSortedList
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(SortKeyColums sortKey, SortOrder sortOrder, DeviceListViewModel model)
    {
        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    // GET: Device/GetPagedList
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceListViewModel model)
    {
        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    [HttpGet]
    public IActionResult Cancel(string returnUrl)
    {
        // キャンセルボタンが押された時の処理
        // まだ作成していない
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // ドロップダウン用の項目を詰めて、ビューに渡す
        var model = new DeviceCreateViewModel();
        _deviceService.InitializeCreateView(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DeviceCreateViewModel model)
    {
        // 入力エラーがあった場合はビューに戻す
        if (!ModelState.IsValid)
        {
            model = _deviceService.InitializeCreateView(model);
            return View(model);
        }

        // クエリ実行結果の状態のエントリ数を取得
        var result = await _deviceService.RegisterNewDeviceAsync(model);

        if (result > 0)
        {
            TempData["SuccessMessage"] = "登録が完了しました。";
        }
        else
        {
            TempData["ErrorMessage"] = "登録に失敗しました。もう一度お試しください。";
        }

        // 機器登録画面に戻す
        return RedirectToAction(nameof(Create));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        // 対象の機器データを取得
        var dto = await _deviceService.GetDeviceByIdAsync(id);

        // 取得できなければ一覧に戻す
        if (dto == null)
        {
            TempData["ErrorMessage"] = "対象の機器が存在しません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューにDTOを渡す
        return View(dto);
    }


    [HttpGet]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    //public async Task<IActionResult> Edit()
    //{
    //    return View();
    //}


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed()
    {
        return View();
    }
}
