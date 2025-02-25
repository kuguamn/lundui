using Yuebon.Commons.Encrypt;
using Yuebon.Commons.Pages;

namespace Yuebon.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 用户接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class UserController : AreaApiController<User, UserOutputDto, UserInputDto, IUserService>
    {
        private IOrganizeService organizeService;
        private IRoleService roleService;
        private IUserLogOnService userLogOnService;
        private readonly ITenantService _tenantService;
        private readonly IUserRoleService _userRoleService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="_organizeService"></param>
        /// <param name="_roleService"></param>
        /// <param name="_userLogOnService"></param>
        /// <param name="tenantService"></param>
        /// <param name="userRoleService"></param>
        public UserController(IUserService _iService, IOrganizeService _organizeService, IRoleService _roleService, IUserLogOnService _userLogOnService, ITenantService tenantService, IUserRoleService userRoleService) : base(_iService)
        {
            iService = _iService;
            organizeService = _organizeService;
            roleService = _roleService;
            userLogOnService = _userLogOnService;
            _tenantService = tenantService;
            _userRoleService = userRoleService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(User info)
        {
            info.Id=IdGeneratorHelper.IdSnowflake();
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(User info)
        {
            
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(User info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }
        /// <summary>
        /// 根据用户Id获取角色集合
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOwnRoleList")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetOwnRoleList(long userId)
        {
            CommonResult result = new CommonResult();
            result.ResData= await _userRoleService.GetUserRoleIdList(userId);

            result.ErrCode = ErrCode.successCode;
            result.ErrMsg = ErrCode.err0;
            return ToJsonContent(result);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public  async Task<IActionResult> RegisterAsync(RegisterViewModel tinfo)
        {
            CommonResult result = new CommonResult();
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            var vCode = yuebonCacheHelper.Get(CacheConst.KeyVerCode + tinfo.VerifyCodeKey);
            string code = vCode != null ? vCode.ToString() : "";
            if (code!= tinfo.VerificationCode.ToUpper())
            {
                result.ErrMsg = "验证码错误";
                return ToJsonContent(result);
            }
            if (!string.IsNullOrEmpty(tinfo.Account))
            {
                if (string.IsNullOrEmpty(tinfo.Password) || tinfo.Password.Length < 6)
                {
                    result.ErrMsg = "密码不能为空或小于6位";
                    return ToJsonContent(result);
                }
                User user =await iService.GetByUserName(tinfo.Account);
                if (user != null)
                {
                    result.ErrMsg = "登录账号不能重复";
                    return ToJsonContent(result);
                }
            }
            else
            {
                result.ErrMsg = "登录账号不能为空";
                return ToJsonContent(result);
            }
            User info = new User();
            info.Id = IdGeneratorHelper.IdSnowflake();
            info.Account = tinfo.Account;
            info.RealName = info.Account;
            info.Email = tinfo.Email;
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = info.Id;
            info.CreateOrgId = 0;
            info.EnabledMark = true;
            info.UserType = Commons.Enums.UserTypeEnum.Member;
            info.RoleId =roleService.GetRole("usermember").Id.ToString();
            info.DeleteMark = false;
            info.SortCode = 99;

            bool isTenant = Appsettings.app(new string[] { "AppSetting", "IsTenant" }).ObjToBool();
            if (isTenant)
            {
                List<Tenant> tenants = null;
                if (!yuebonCacheHelper.Exists(CacheConst.KeyTenants))
                {
                    IEnumerable<Tenant> templist = _tenantService.GetAllByIsEnabledMark();
                    yuebonCacheHelper.Add(CacheConst.KeyTenants, templist);
                }
                tenants = JsonHelper.ToObject<List<Tenant>>(yuebonCacheHelper.Get(CacheConst.KeyTenants).ToJson());
                if (tenants != null)
                {
                    string tenantName = tinfo.Host.Split(".")[0];
                    Tenant tenant = tenants.FindLast(o => o.TenantName == tenantName);//通过租户名称
                    if (tenant == null && tenantName != "default")
                    {
                        tenant = tenants.FindLast(o => o.HostDomain.Contains(tinfo.Host));//通过客户绑定的独立域名
                        if (tenant == null)
                        {
                            result.ErrMsg = "非法访问";
                            return ToJsonContent(result);
                        }
                    }
                    if (tenant != null)
                    {
                        info.TenantId = tenant.Id;
                    }
                }

            }
            UserLogOn userLogOn = new UserLogOn();
            userLogOn.UserPassword = tinfo.Password;
            userLogOn.AllowStartTime = userLogOn.LockStartDate = userLogOn.LockEndDate = userLogOn.ChangePasswordDate = DateTime.Now;
            userLogOn.AllowEndTime =  DateTime.Now.AddMonths(1);
            userLogOn.MultiUserLogin = userLogOn.CheckIPAddress = false;
            userLogOn.LogOnCount = 0;
            result.Success = await iService.InsertAsync(info, userLogOn);
            if (result.Success)
            {
                yuebonCacheHelper.Remove(CacheConst.KeyVerCode);
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            else
            {
                result.ErrMsg = ErrCode.err43001;
                result.ErrCode = "43001";
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
        public override async Task<IActionResult> InsertAsync(UserInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            if (!string.IsNullOrEmpty(tinfo.Account))
            {
                string where = string.Format("Account='{0}' or Email='{0}' or MobilePhone='{0}'", tinfo.Account);
                User user = iService.GetWhere(where);
                if (user != null)
                {
                    result.ErrMsg = "登录账号不能重复";
                    return ToJsonContent(result);
                }
            }
            else
            {
                result.ErrMsg = "登录账号不能为空";
                return ToJsonContent(result);
            }
            User info = tinfo.MapTo<User>();
            info.CreateOrgId=tinfo.CreateOrgId;
            OnBeforeInsert(info);
            UserLogOn userLogOn = new UserLogOn();
            userLogOn.UserPassword = "123456@ld";
            userLogOn.AllowStartTime =userLogOn.LockEndDate=userLogOn.LockStartDate=userLogOn.ChangePasswordDate= DateTime.Now;
            userLogOn.AllowEndTime = DateTime.Now.AddYears(100);
            userLogOn.MultiUserLogin = userLogOn.CheckIPAddress= false;
            userLogOn.LogOnCount = 0;
            result.Success = await iService.InsertAsync(info, userLogOn);
            if (result.Success)
            {
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            else
            {
                result.ErrMsg = ErrCode.err43001;
                result.ErrCode = "43001";
            }
            return ToJsonContent(result);
        }
        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("")]
        public override async Task<IActionResult> UpdateAsync(UserInputDto tinfo)
        {
            CommonResult result = new CommonResult();
            if (!string.IsNullOrEmpty(tinfo.Account))
            {
                string where = string.Format(" Account='{0}'  and id!={1} ", tinfo.Account, tinfo.Id);
                User user = iService.GetWhere(where);
                if (user != null)
                {
                    result.ErrMsg = "登录账号不能重复";
                    return ToJsonContent(result);
                }
            }
            else
            {
                result.ErrMsg = "登录账号不能为空";
                return ToJsonContent(result);
            }
            User info = iService.GetById(tinfo.Id);
            info.Account = tinfo.Account;
            info.HeadIcon = tinfo.HeadIcon;
            info.RealName = tinfo.RealName;
            info.NickName = tinfo.NickName;
            info.Gender = tinfo.Gender;
            info.Birthday = tinfo.Birthday;
            info.MobilePhone = tinfo.MobilePhone;
            info.WeChat = tinfo.WeChat;
            info.CreateOrgId = tinfo.CreateOrgId;
            info.RoleId = tinfo.RoleId;
            info.Organizeid = tinfo.Organizeid;
            info.UserType = tinfo.UserType; 
            info.Email = tinfo.Email;
            info.EnabledMark = tinfo.EnabledMark;
            info.Description = tinfo.Description;
            info.Station = tinfo.Station;

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
        /// 根据用户登录账号获取详细信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        [HttpGet("GetByUserName")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            CommonResult result = new CommonResult();
            try
            {
                User user = await iService.GetByUserName(userName);
                result.ResData = user.MapTo<UserOutputDto>();
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取用户异常", ex);//错误记录
                result.ErrMsg = ex.Message;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步分页查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("List")]
        public  async Task<IActionResult> FindWithPagerSearchAsync(SearchUserModel search)
        {
            CommonResult<PageResult<UserOutputDto>> result = new CommonResult<PageResult<UserOutputDto>>();
            result.ResData = await iService.FindWithPagerSearchAsync(search);
            result.ErrCode = ErrCode.successCode;
            return ToJsonContent(result);
        }


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [YuebonAuthorize("ResetPassword")]
        public async Task<IActionResult> ResetPassword(long userId)
        {
            CommonResult result = new CommonResult();
            try
            {
                string where = string.Format("UserId='{0}'", userId);
                UserLogOn userLogOn = userLogOnService.GetWhere(where);
                string strRandom = GenerateRandomPassword(8); //生成8位随机密码
                userLogOn.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
                userLogOn.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(strRandom).ToLower(), userLogOn.UserSecretkey).ToLower()).ToLower();
                userLogOn.ChangePasswordDate = DateTime.Now;
                bool bl = await userLogOnService.UpdateAsync(userLogOn, userLogOn.Id);
                if (bl)
                {
                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = strRandom;
                    result.Success = true;
                }
                else
                {
                    result.ErrMsg = ErrCode.err43002;
                    result.ErrCode = "43002";
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("重置密码异常", ex);//错误记录
                result.ErrMsg = ex.Message;
            }
            return ToJsonContent(result);
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldpassword">原密码</param>
        /// <param name="password">新密码</param>
        /// <param name="password2">重复新密码</param>
        /// <returns></returns>

        [HttpPost("ModifyPassword")]
        [YuebonAuthorize("ModifyPassword")]
        public async Task<IActionResult> ModifyPassword(ModifyPasswordDto modifyPasswordDto)
        {
            CommonResult result = new CommonResult();
            try
            {
                if (modifyPasswordDto == null)
                {
                    return BadRequest("请求参数不能为空");
                }
                else if (modifyPasswordDto.password == modifyPasswordDto.password2)
                {
                    var userSinginEntity = userLogOnService.GetByUserId(CurrentUser.UserId);
                    string inputPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(modifyPasswordDto.oldpassword).ToLower(), userSinginEntity.UserSecretkey).ToLower()).ToLower();
                    if (inputPassword != userSinginEntity.UserPassword)
                    {
                        result.ErrMsg = "原密码错误！";
                    }
                    else
                    {
                        string where = string.Format("UserId='{0}'", CurrentUser.UserId);
                        UserLogOn userLogOn = userLogOnService.GetWhere(where);

                        userLogOn.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
                        userLogOn.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(modifyPasswordDto.password).ToLower(), userLogOn.UserSecretkey).ToLower()).ToLower();
                        userLogOn.ChangePasswordDate = DateTime.Now;
                        bool bl = await userLogOnService.UpdateAsync(userLogOn, userLogOn.Id);
                        if (bl)
                        {
                            result.ErrCode = ErrCode.successCode;
                        }
                        else
                        {
                            result.ErrMsg = ErrCode.err43002;
                            result.ErrCode = "43002";
                        }
                    }
                }
                else
                {
                    result.ErrMsg = "两次输入的密码不一样";
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("重置密码异常", ex);//错误记录
                result.ErrMsg = ex.Message;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 保存用户自定义的软件主题
        /// </summary>
        /// <param name="info">主题配置信息</param>
        /// <returns></returns>
        [HttpPost("SaveUserTheme")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> SaveUserTheme(UserThemeInputDto info)
        {
            CommonResult result = new CommonResult();
            try
            {
                result.Success= await userLogOnService.SaveUserTheme(info, CurrentUser.UserId);
                result.ErrCode = ErrCode.successCode;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("保存用户自定义的软件主题异常", ex);//错误记录
                result.ErrMsg = ex.Message;
            }
            return ToJsonContent(result);
        }



        public class ModifyPasswordDto
        {
            public string oldpassword { get; set; }
            public string password { get; set; }
            public string password2 { get; set; }
        }
    }
}