using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Common.CustomExceptions;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbDataController : ControllerBase
    {
        IExcelExportService _excelExportService;
        public DbDataController(IExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }
        public async Task<IActionResult> ExportToExcel()
        {
            FileDTO file = await _excelExportService.ExportAllTablesToExcelAsync();
            return File(file.FileContent,file.ContentType, file.FileName);
        }
    }
}
