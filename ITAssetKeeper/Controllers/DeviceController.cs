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
    public async Task<IActionResult> GetPagedList(DeviceListViewModel model)
    {
        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        return View();
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    //public async Task<IActionResult> Create()
    //{
    //    return View();
    //}


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
