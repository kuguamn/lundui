using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WebApi.Areas.WCINS.Controllers
{
    /// <summary>
    /// 工单详细接口
    /// </summary>
    [ApiController]
    [Route("api/WCINS/[controller]")]
    public partial class WorkorderdetailController : AreaApiController<Workorderdetail, WorkorderdetailOutputDto, WorkorderdetailInputDto, IWorkorderdetailService>
    {
        private readonly IOrderresultsService orderresultsService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        ///  <param name="_orderresultsService"></param>
        public WorkorderdetailController(IWorkorderdetailService _iService, IOrderresultsService _orderresultsService) : base(_iService)
        {
            iService = _iService;
            orderresultsService = _orderresultsService;

        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Workorderdetail info)
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
        protected override void OnBeforeUpdate(Workorderdetail info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Workorderdetail info)
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
        public override async Task<IActionResult> UpdateAsync(WorkorderdetailInputDto winfo)
        {
            CommonResult result = new CommonResult();

            Workorderdetail info =  iService.GetById(winfo.Id);
            if (info == null)
            {
                result.ErrMsg = "找不到采集结果明细";
                result.ErrCode = "404";
                return ToJsonContent(result);
            }

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info);

            foreach (var item in winfo.orderresults)
            {
                Orderresults resultinfo = orderresultsService.GetById(item.Id);
                if (resultinfo != null)
                {
                    resultinfo.DisResult = item.DisResult;
                    await orderresultsService.UpdateAsync(resultinfo);
                }
            }

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
        /// PC端批量异步更新数据
        /// </summary>
        /// <param name="winfo"></param>
        /// <returns></returns>
        [HttpPost("PCUpdate")]
        [YuebonAuthorize("Edit")]
        public async Task<IActionResult> PCUpdateAsync(List<WorkorderdetailInputDto> winfo)
        {
            CommonResult result = new CommonResult();

            foreach (var item in winfo)
            {
                await UpdateAsync(item);
            }

            result.ErrCode = ErrCode.successCode;
            result.ErrMsg = ErrCode.err0;

            return ToJsonContent(result);
        }

    }
}