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
    private readonly ITAssetKeeperDbContext _context;
    private readonly IDeviceService _deviceService;

    public DeviceController(ITAssetKeeperDbContext context, IDeviceService deviceService)
    {
        _context = context;
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DeviceListViewModel model)
    {
        // 検索メソッド実行、結果を取得
        var result = await _deviceService.SearchDevicesAsync(model);

        // ビューに渡す
        return View(result);
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
