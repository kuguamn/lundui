using Microsoft.AspNetCore.Identity;
using Yuebon.Commons.Enums;

namespace Yuebon.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 组织机构接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class OrganizeController : AreaApiController<Organize, OrganizeOutputDto, OrganizeInputDto, IOrganizeService>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public OrganizeController(IOrganizeService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Organize info)
        {
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
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

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Organize info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
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

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Organize info)
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
        public override async Task<IActionResult> UpdateAsync(OrganizeInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Organize info = iService.GetById(tinfo.Id);
            info.ParentId = tinfo.ParentId;
            info.FullName = tinfo.FullName;
            info.EnCode = tinfo.EnCode;
            info.ShortName = tinfo.ShortName;
            info.OrgType = tinfo.OrgType;
            info.CategoryId = tinfo.CategoryId;
            info.ManagerId = tinfo.ManagerId;
            info.TelePhone = tinfo.TelePhone;
            info.MobilePhone = tinfo.MobilePhone;
            info.WeChat = tinfo.WeChat;
            info.Fax = tinfo.Fax;
            info.Email = tinfo.Email;
            info.Address = tinfo.Address;
            info.AllowEdit = tinfo.AllowEdit;
            info.AllowDelete = tinfo.AllowDelete;
            info.ManagerId = tinfo.ManagerId;
            info.EnabledMark = tinfo.EnabledMark;
            info.DeleteMark = tinfo.DeleteMark;
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
        ///// <summary>
        ///// 根据用户Id获取机构Id集合
        ///// </summary>
        ///// <returns></returns>
        //public async Task<List<long>> GetUserOrgIdList()
        //{
        //    if (CurrentUser.UserType==UserTypeEnum.SuperAdmin)
        //        return new List<long>();
        //    YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
        //    var userId = CurrentUser.UserId;
        //    var orgIdList = yuebonCacheHelper.Get<List<long>>($"{CacheConst.KeyUserOrg}{userId}"); // 取缓存
        //    //if (orgIdList == null || orgIdList.Count < 1)
        //    //{
        //    //    // 扩展机构集合
        //    //    //var orgList1 = await _sysUserExtOrgService.GetUserExtOrgList(userId);
        //    //    // 角色机构集合
        //    //    var orgList2 = await GetUserRoleOrgIdList(userId);
        //    //    // 机构并集
        //    //    orgIdList = orgList1.Select(u => u.OrgId).Union(orgList2).ToList();
        //    //    // 当前所属机构
        //    //    if (!orgIdList.Contains(CurrentUser.OrganizeId))
        //    //        orgIdList.Add(CurrentUser.OrganizeId);
        //    //    _sysCacheService.Set($"{CacheConst.KeyUserOrg}{userId}", orgIdList); // 存缓存
        //    //}
        //    return orgIdList;
        //}
        /// <summary>
        /// 获取组织机构适用于Vue 树形列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllOrganizeTreeTable")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetAllOrganizeTreeTable()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Organize> list = await iService.GetAllOrganizeTreeTable();
                result.Success = true;
                result.ErrCode = ErrCode.successCode;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取组织结构异常", ex);
                result.ErrMsg = ErrCode.err40110;
                result.ErrCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取组织机构适用于Vue Tree树形
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllOrganizeTree")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetAllOrganizeTree()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Organize> list = await iService.GetAllOrganizeTreeTable();
                result.Success = true;
                result.ErrCode = ErrCode.successCode;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取组织结构异常", ex);
                result.ErrMsg = ErrCode.err40110;
                result.ErrCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步批量物理删除
        /// </summary>
        /// <param name="info"></param>
        [HttpDelete("DeleteBatchAsync")]
        [YuebonAuthorize("Delete")]
        public override async Task<IActionResult> DeleteBatchAsync(DeletesInputDto info)
        {
            CommonResult result = new CommonResult();

            if (info.Ids.Length > 0)
            {
                result = await iService.DeleteBatchWhereAsync(info).ConfigureAwait(false);
                if (result.Success)
                {
                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = ErrCode.err0;
                }
                else
                {
                    result.ErrCode = "43003";
                }
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 根据组织类型获取公司级组织机构
        /// </summary>
        /// <param name="orgType">组织类型</param>
        /// <returns></returns>
        [HttpGet("GetOrganizesByOrgType")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> GetOrganizesByOrgTypeAsync(string orgType)
        {
            CommonResult result = new CommonResult();
            if (!string.IsNullOrEmpty(orgType))
            {
                result.ResData = await iService.GetOrganizesByOrgTypeAsync(orgType);
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;

            }
            return ToJsonContent(result);
        }
    }
}