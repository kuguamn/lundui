using NPOI.SS.Formula.Functions;
using Yuebon.CMS.Dtos;
using Yuebon.CMS.Models;
using Yuebon.Commons.Attributes;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WebApi.Areas.WCINS.Controllers
{
    /// <summary>
    /// 部件类型接口
    /// </summary>
    [ApiController]
    [Route("api/WCINS/[controller]")]
    public partial class PonenttypeController : AreaApiController<Ponenttype, PonenttypeOutputDto, PonenttypeInputDto, IPonenttypeService>
    {

        private IMenuService _menuService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public PonenttypeController(IPonenttypeService _iService, IMenuService menuService) : base(_iService)
        {
            iService = _iService;
            this._menuService = menuService;

        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Ponenttype info)
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
        protected override void OnBeforeUpdate(Ponenttype info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Ponenttype info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }

        #region 权限系统操作

        /// <summary>
        /// 在更新数据前对数据的修改操作(数据权限)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected void OnBeforeUpdate(Menu info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;

            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
            if (info.ParentId == 0)
            {
                info.Layers = 1;
                info.ParentId = 0;
            }
            else
            {
                info.Layers = _menuService.GetById(info.ParentId).Layers + 1;
            }
        }

        /// <summary>
        /// 新增前处理数据（数据权限）
        /// </summary>
        /// <param name="info"></param>
        protected void OnBeforeInsert(Menu info)
        {
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
            if (info.ParentId == 0)
            {
                info.Layers = 1;
                info.ParentId = 0;
            }
            else
            {
                info.Layers = _menuService.GetById(info.ParentId).Layers + 1;
            }

            if (info.MenuType == "F")
            {
                info.IsFrame = false;
                info.Component = "";
                info.UrlAddress = "";
            }

        }

        #endregion
        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(PonenttypeInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Ponenttype info = iService.GetById(tinfo.Id);
            info.Typename = tinfo.Typename;
            info.Typecode = tinfo.Typecode;
            info.EnabledMark = tinfo.EnabledMark;

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info);
            if (bl)
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;

                Menu menu = _menuService.GetListWhere("EnCode='" + info.Id + "'").FirstOrDefault();
                menu.FullName = tinfo.Typename;
                OnBeforeUpdate(menu);
                await _menuService.UpdateAsync(menu);

            }
            else
            {
                result.ErrMsg = ErrCode.err43002;
                result.ErrCode = "43002";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        [YuebonAuthorize("Add")]
        public override async Task<IActionResult> InsertAsync(PonenttypeInputDto tinfo)
        {
            CommonResult result = new CommonResult();
            Ponenttype info = tinfo.MapTo<Ponenttype>();
            OnBeforeInsert(info);

            long ln = await iService.InsertAsync(info);
            if (ln > 0)
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;

                Menu menu = new Menu();              
                menu.FullName = info.Typename;
                menu.EnCode = info.Id.ToString();
                menu.ParentId = 20362383857418309;
                menu.SystemTypeId = 9166287964471363;
                menu.Icon = "app";
                menu.UrlAddress = "部件类型权限";
                menu.Component = "";
                menu.ActiveMenu = "";
                menu.MenuType = "F";
                menu.IsPublic = false;
                menu.IsShow = true;
                menu.IsFrame = false;
                menu.SortCode = 99;
                menu.IsCache = false;

                OnBeforeInsert(menu);
                menu.Id = info.Id;

                await _menuService.InsertAsync(menu);
            }
            else
            {
                result.ErrMsg = ErrCode.err43001;
                result.ErrCode = "43001";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步批量物理删除
        /// </summary>
        /// <param name="info"></param>
        [HttpPost("DeleteBatchAsync")]
        [YuebonAuthorize("Delete")]
        public override async Task<IActionResult> DeleteBatchAsync(DeletesInputDto info)
        {
            CommonResult result = new CommonResult();
            string where = string.Empty;
            where = "id in (" + String.Join(",", info.Ids) + ")";
            if (!string.IsNullOrEmpty(where))
            {
                bool bl = await iService.DeleteBatchWhereAsync(where).ConfigureAwait(false);
                if (bl)
                {
                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = ErrCode.err0;

                    var menuData = await _menuService.GetListWhereAsync("ParentId = '20362383857418309'");
                    var ids = menuData.Where(x => info.Ids.Contains(x.EnCode.ToLong())).Select(x => x.Id).ToArray();
                    var infoMenu = new DeletesInputDto() { Ids = ids };
                    await _menuService.DeleteBatchWhereAsync(infoMenu);
                }
                else
                {
                    result.ErrMsg = ErrCode.err43003;
                    result.ErrCode = "43003";
                }
            }
            return ToJsonContent(result);
        }

    }
}