using Swashbuckle.AspNetCore.Annotations;
using Yuebon.Commons.Encrypt;

namespace Yuebon.WebApi.Areas.Security.Controllers;

/// <summary>
/// 应用管理接口
/// </summary>
[SwaggerTag("APP")]
[ApiController]
[Route("api/Security/[controller]")]
public class APPController : AreaApiController<APP, AppOutputDto, APPInputDto, IAPPService>
{
    IMediator _mediator;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_iService"></param>
    /// <param name="mediator"></param>
    public APPController(IAPPService _iService, IMediator mediator) : base(_iService)
    {
        iService = _iService;
        _mediator = mediator;
    }
    /// <summary>
    /// 新增前处理数据
    /// </summary>
    /// <param name="info"></param>
    protected override void OnBeforeInsert(APP info)
    {
        info.Id = IdGeneratorHelper.IdSnowflake();
        info.AppSecret = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
        if (info.IsOpenAEKey)
        {
            info.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
        }
        info.CreatorTime = DateTime.Now;
        info.CreatorUserId = CurrentUser.UserId;
        info.CreateOrgId = CurrentUser.OrganizeId;
        info.DeleteMark = false;
    }
    
    /// <summary>
    /// 在更新数据前对数据的修改操作
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    protected override void OnBeforeUpdate(APP info)
    {
        if (info.IsOpenAEKey && string.IsNullOrEmpty(info.EncodingAESKey)){
            info.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
        }
        info.LastModifyUserId = CurrentUser.UserId;
        info.LastModifyTime = DateTime.Now;
    }

    /// <summary>
    /// 在软删除数据前对数据的修改操作
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    protected override void OnBeforeSoftDelete(APP info)
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
    public override async Task<IActionResult> UpdateAsync(APPInputDto tinfo)
    {
        CommonResult result = new CommonResult();

        APP info = iService.GetById(tinfo.Id);
        info.AppId = tinfo.AppId;
        info.RequestUrl = tinfo.RequestUrl;
        info.Token = tinfo.Token;
        info.EnabledMark = tinfo.EnabledMark;
        info.IsOpenAEKey=tinfo.IsOpenAEKey;
        info.Description = tinfo.Description;

        OnBeforeUpdate(info);
        bool bl = await iService.UpdateAsync(info);
        if (bl)
        {
            result.ErrCode = ErrCode.successCode;
            result.ErrMsg = ErrCode.err0;
            await _mediator.Send(info);
        }
        else
        {
            result.ErrMsg = ErrCode.err43002;
            result.ErrCode = "43002";
        }
        return ToJsonContent(result);
    }



    /// <summary>
    /// 重置AppSecret
    /// </summary>
    /// <returns></returns>
    [HttpGet("ResetAppSecret")]
    [YuebonAuthorize("ResetAppSecret")]
    public async Task<IActionResult> ResetAppSecret(long id)
    {
        CommonResult result = new CommonResult();
        APP aPP = iService.GetById(id);
        aPP.AppSecret = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
        bool bl = await iService.UpdateAsync(aPP);
        if (bl)
        {
            result.ErrCode = ErrCode.successCode;
            result.ErrMsg = aPP.AppSecret;
            result.Success = true;
        }
        else
        {
            result.ErrMsg = ErrCode.err43002;
            result.ErrCode = "43002";
        }
        return ToJsonContent(result);
    }

    /// <summary>
    /// 重置消息加密密钥EncodingAESKey
    /// </summary>
    /// <returns></returns>
    [HttpGet("ResetEncodingAESKey")]
    [YuebonAuthorize("ResetEncodingAESKey")]
    public async Task<IActionResult> ResetEncodingAESKey(long id)
    {
        CommonResult result = new CommonResult();
        APP aPP = iService.GetById(id);
        aPP.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
        bool bl = await iService.UpdateAsync(aPP);
        if (bl)
        {
            result.ErrCode = ErrCode.successCode;
            result.ErrMsg = aPP.EncodingAESKey;
            result.Success = true;
        }
        else
        {
            result.ErrMsg = ErrCode.err43002;
            result.ErrCode = "43002";
        }
        return ToJsonContent(result);
    }
}