﻿namespace Yuebon.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 数据字典明细
    /// </summary>
    [Route("api/Security/[controller]")]
    [ApiController]
    public class ItemsDetailController : AreaApiController<ItemsDetail, ItemsDetailOutputDto, ItemsDetailInputDto, IItemsDetailService>
    {
        private readonly IItemsService itemsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="itemsService"></param>
        public ItemsDetailController(IItemsDetailService _iService, IItemsService itemsService) : base(_iService)
        {
            iService = _iService;
            this.itemsService = itemsService;
        }

        /// <summary>
        /// 新增前操作
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(ItemsDetail info)
        {
            //留给子类对参数对象进行修改
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
            bool bltree = itemsService.GetById(info.ItemId).IsTree;
            if (bltree)
            {
                if (info.ParentId==0)
                {
                    info.Layers = 1;
                    info.ParentId = 0;
                }
                else
                {
                    info.Layers = iService.GetById(info.ParentId).Layers + 1;
                }
            }
            else
            {
                info.ParentId = 0;
            }

        }

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(ItemsDetail info)
        {
            //留给子类对参数对象进行修改 
            info.LastModifyTime = DateTime.Now;
            info.LastModifyUserId = CurrentUser.UserId;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
            bool bltree = itemsService.GetById(info.ItemId).IsTree;
            if (bltree)
            {
                if (info.ParentId == 0)
                {
                    info.Layers = 1;
                    info.ParentId = 0;
                }
                else
                {
                    info.Layers = iService.GetById(info.ParentId).Layers + 1;
                }
            }
            else
            {
                info.ParentId = 0;
            }
        }


        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(ItemsDetailInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            ItemsDetail info = iService.GetById(tinfo.Id);
            info.ItemName = tinfo.ItemName;
            info.ItemCode = tinfo.ItemCode;
            info.ItemId = tinfo.ItemId;
            info.IsDefault = tinfo.IsDefault;
            info.ParentId = tinfo.ParentId;
            info.EnabledMark = tinfo.EnabledMark;
            info.SortCode = tinfo.SortCode;
            info.Description = tinfo.Description;


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
        /// 获取某数据字典类别下的数据适用于Vue 树形列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllItemsDetailTreeTable")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> GetAllItemsDetailTreeTable(string itemId)
        {
            CommonResult result = new CommonResult();
            try
            {
                if (!string.IsNullOrEmpty(itemId))
                {
                    List<ItemsDetailOutputDto> list = await iService.GetAllItemsDetailTreeTable(itemId);
                    result.Success = true;
                    result.ErrCode = ErrCode.successCode;
                    result.ResData = list;
                }
                else
                {
                    result.ErrMsg = "数据字典类别ID不能为空";
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取数据字典异常", ex);
                result.ErrMsg = ErrCode.err40110;
                result.ErrCode = "40110";
            }
            return ToJsonContent(result);
        }
    }
}