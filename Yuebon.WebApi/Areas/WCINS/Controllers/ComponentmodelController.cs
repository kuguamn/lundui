using Yuebon.Commons.Attributes;
using Yuebon.Commons.Net.TencentIp;
using Yuebon.Commons.Pages;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;

namespace Yuebon.WebApi.Areas.WCINS.Controllers
{
    /// <summary>
    /// 部件模型接口
    /// </summary>
    [ApiController]
    [Route("api/WCINS/[controller]")]
    public partial class ComponentmodelController : AreaApiController<Componentmodel, ComponentmodelOutputDto, ComponentmodelInputDto, IComponentmodelService>
    {
        private readonly IPonenttypeService ponenttypeService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="ponenttypeService"></param>
        public ComponentmodelController(IComponentmodelService _iService, IPonenttypeService ponenttypeService) : base(_iService)
        {
            iService = _iService;
            this.ponenttypeService = ponenttypeService;

        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Componentmodel info)
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
        protected override void OnBeforeUpdate(Componentmodel info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Componentmodel info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }


        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="winfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(ComponentmodelInputDto winfo)

        {
            CommonResult result = new CommonResult();

            Componentmodel info = iService.GetById((long)winfo.Id);
            OnBeforeUpdate(info);
            info.PonentTypeid = long.Parse(winfo.PonentTypeid);
            info.Model_name = winfo.Model_name;
            info.Model_type = winfo.Model_type;
            info.Version = winfo.Version;
           // info.FilePath = winfo.FilePath;
            info.Remarks = winfo.Remarks;
            info.EnabledMark = winfo.EnabledMark;
            bool bl = await iService.UpdateAsync(info);
            if (bl)
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            else
            {
                result.ErrMsg = ErrCode.err43002;
                result.ErrCode = "43002";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 新增部件模型
        /// </summary>
        /// <param name="inputDto"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("InsertComponentmodel")]
        [YuebonAuthorize("Add")]
        public async Task<IActionResult> InsertComponentmodelAsync([FromForm] ComponentmodelInputDto inputDto, IFormFile[] file)
        {
            CommonResult result = new CommonResult();

            // 检查文件是否上传
            if (file == null || file.Length == 0)
            {
                result.ErrCode = "400";
                result.ErrMsg = "文件上传失败，文件不能为空";
                return ToJsonContent(result);
            }
            //查询部件类型

            // 查询部件类型
            var ponenttype = ponenttypeService.GetById(long.Parse(inputDto.PonentTypeid));
            if (ponenttype==null)
            {
                result.ErrCode = ErrCode.err40007;
                result.ErrMsg = "无效的部件类型";
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

                // 将 DTO 映射到实体并执行插入操作
                Componentmodel info = inputDto.MapTo<Componentmodel>();
                OnBeforeInsert(info);
                var where = string.Format("PonentTypeid='{0}'", inputDto.PonentTypeid);
                // 设置版本号
                var existingModels = await iService.GetListWhereAsync(where);
                if (existingModels.Count() == 0)
                {
                    info.Version = "1.0";
                }
                else
                {
                    var latestVersion = existingModels.Max(m => double.Parse(m.Version));
                    info.Version = (latestVersion + 0.1).ToString("F1");
                }

                // 静态文件保存路径（使用部件类型的TypeCode和版本号作为文件夹）
                string uploadsFolder = Path.Combine(wwwrootPath, ponenttype.Typecode, info.Version);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var formFile in file)
                {
                    // 文件名处理
                    string fileName = formFile.FileName; // 获取上传的文件名
                    string filePath = Path.Combine(uploadsFolder, fileName); // 绝对路径保存文件

                    // 保存文件
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // 将相对路径保存到 DTO 的 Filepath 属性（相对于 wwwroot 的路径）
                    string relativePath = Path.Combine(ponenttype.Typecode, info.Version, fileName).Replace("\\", "/"); // 替换为相对路径

                    // 创建 Modelfile 实体
                    Modelfile modelfile = new Modelfile
                    {
                        Id = IdGeneratorHelper.IdSnowflake(),
                        Modelid= info.Id,
                        FilePath = relativePath,
                        FileName = fileName,
                        FileSize = formFile.Length.ToString()
                    };
                    var mosum = await iService.InsertModelfileAsync(modelfile);
                    if (mosum > 0)
                    {
                        result.ErrCode = ErrCode.successCode;
                        result.ErrMsg = ErrCode.err0;
                    }
                    else
                    {
                        result.ErrMsg = "新增模型文件失败";
                        result.ErrCode = "43001";
                    }
                }

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
        /// 根据条件查询数据库,并返回对象集合(用于PC分页数据显示)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindPCWithPagerAsync")]
        [YuebonAuthorize("List")]
        public virtual async Task<CommonResult<PageResult<ComponentmodelOutputDto>>> FindPCWithPagerAsync(SearchInputDto<ComponentmodelSearch> search)
        {
            CommonResult<PageResult<ComponentmodelOutputDto>> result = new CommonResult<PageResult<ComponentmodelOutputDto>>();
            result.ResData = await iService.FindWithPagerAsync(search);
            result.ErrCode = ErrCode.successCode;
            return result;
        }
    }
}