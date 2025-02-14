using NPOI.SS.Formula.Functions;
using NuGet.Protocol.Core.Types;
using Yuebon.Security.Dtos;
using Yuebon.Security.IServices;
using Yuebon.Security.Models;
namespace Yuebon.SecurityApi.Areas.Security.Controllers
{
    /// <summary>
    /// APP版本管理表接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class AppversionController : AreaApiController<Appversion, AppversionOutputDto,AppversionInputDto,IAppversionService>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public AppversionController(IAppversionService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Appversion info)
        {
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Appversion info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Appversion info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDto"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("InsertAppversion")]
        [YuebonAuthorize("Add")]
        public virtual async Task<IActionResult> InsertAsync([FromForm] AppversionInputDto inputDto, IFormFile file)
        {
            CommonResult result = new CommonResult();

            // 检查文件是否上传
            if (file == null || file.Length == 0)
            {
                result.ErrCode = "400";
                result.ErrMsg = "文件上传失败，文件不能为空";
                return ToJsonContent(result);
            }

            try
            {
                // wwwroot 目录
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }

                // 静态文件保存路径（APP 文件夹）
                string uploadsFolder = Path.Combine(wwwrootPath, "APP");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 文件名处理
                string fileName = file.FileName; // 获取上传的文件名
                string filePath = Path.Combine(uploadsFolder, fileName); // 绝对路径保存文件

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 将相对路径保存到 DTO 的 Filepath 属性（相对于 wwwroot 的路径）
                inputDto.Filepath = Path.Combine("APP", fileName).Replace("\\", "/"); // 替换为相对路径

                // 将 DTO 映射到实体并执行插入操作
                Appversion info = inputDto.MapTo<Appversion>();
                OnBeforeInsert(info);

                long ln = await iService.InsertAsync(info);
                if (ln > 0)
                {
                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = ErrCode.err0;
                }
                else
                {
                    result.ErrMsg = ErrCode.err43001;
                    result.ErrCode = "43001";
                }
            }
            catch (Exception ex)
            {
                // 异常处理
                result.ErrCode = "500";
                result.ErrMsg = "插入操作失败: " + ex.Message;
            }

            return ToJsonContent(result);
        }



        /// <summary>
        /// 获取最新版本信息，并判断当前版本是否为最新版本
        /// </summary>
        /// <param name="version">当前版本号</param>
        /// <returns>最新版本信息和是否为最新版本</returns>
        [HttpGet("GetNewVersion")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetNewVersion(string version)
        {
            CommonResult result = new CommonResult();

            try
            {
                // 获取最新版本信息
                var latestVersion = await iService.GetNewVersion();

                if (latestVersion != null)
                {
                    // 判断当前版本是否为最新版本
                    bool isLatest = false;
                    if (!string.IsNullOrEmpty(version))
                    {
                        isLatest = CompareVersions(version, latestVersion.Version) >= 0;
                    }

                    // 返回最新版本信息和是否为最新的标志
                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = ErrCode.err0;
                    result.ResData = new
                    {
                        LatestVersion = latestVersion,
                        IsNew = isLatest
                    };
                }
                else
                {
                    result.ErrCode = "404";
                    result.ErrMsg = "未找到最新版本信息";
                }
            }
            catch (Exception ex)
            {
                result.ErrCode = "500";
                result.ErrMsg = "获取最新版本失败: " + ex.Message;
            }

            return ToJsonContent(result);
        }

        #region 方法
        /// <summary>
        /// 比较两个版本号
        /// </summary>
        /// <param name="version1">版本号1</param>
        /// <param name="version2">版本号2</param>
        /// <returns>1 如果版本1大于版本2，-1 如果版本1小于版本2，0 如果两者相等</returns>
        private int CompareVersions(string version1, string version2)
        {
            var v1Parts = version1.Split('.').Select(int.Parse).ToArray();
            var v2Parts = version2.Split('.').Select(int.Parse).ToArray();
            int length = Math.Max(v1Parts.Length, v2Parts.Length);

            for (int i = 0; i < length; i++)
            {
                int v1 = i < v1Parts.Length ? v1Parts[i] : 0;
                int v2 = i < v2Parts.Length ? v2Parts[i] : 0;

                if (v1 > v2) return 1;
                if (v1 < v2) return -1;
            }

            return 0;
        }
        #endregion



    }
}