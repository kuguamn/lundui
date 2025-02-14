using Yuebon.Commons.Attributes;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WebApi.Areas.WCINS.Controllers
{
    /// <summary>
    /// 部件类型识别字段接口
    /// </summary>
    [ApiController]
    [Route("api/WCINS/[controller]")]
    public partial class PonentfiledController : AreaApiController<Ponentfiled, PonentfiledOutputDto, PonentfiledInputDto, IPonentfiledService>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public PonentfiledController(IPonentfiledService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Ponentfiled info)
        {
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            info.IsMain = 0;
        }

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Ponentfiled info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Ponentfiled info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }


        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(PonentfiledInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Ponentfiled info = iService.GetById(tinfo.Id);
            info.PonentTypeid = tinfo.PonentTypeid;
            info.FiledName = tinfo.FiledName;
            info.FiledValue = tinfo.FiledValue;
            info.EnabledMark = tinfo.EnabledMark;
            info.IsMain = tinfo.IsMain;

            OnBeforeUpdate(info);
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
        /// 根据部件类型id查询部件类型识别字段
        /// </summary>
        /// <param name="ponentTypeId"></param>
        /// <returns></returns>
        [HttpGet("GetByPonentTypeId")]
        public async Task<IActionResult> GetByPonentTypeIdAsync(long ponentTypeId)
        {
            CommonResult<List<PonentfiledOutputDto>> result = new CommonResult<List<PonentfiledOutputDto>>();
            var list = await iService.GetAllByIsNotDeleteAndEnabledMarkAsync($"PonentTypeid = {ponentTypeId}");
            if (list != null && list.Any())
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
                result.ResData = list.Select(item => new PonentfiledOutputDto
                {
                    Id=item.Id,
                    PonentTypeid=item.PonentTypeid,
                    FiledName = item.FiledName,
                    FiledValue= item.FiledValue,
                    IsMain = item.IsMain,
                }).ToList();
            }
            else
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
                result.ResData = Array.Empty<PonentfiledOutputDto>();
                //result.ErrMsg = ErrCode.err60001;
                //result.ErrCode = "60001";
            }
            return ToJsonContent(result);
        }
    }
}