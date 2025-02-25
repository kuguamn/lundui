﻿using Yuebon.Commons.Enums;
using Yuebon.Commons.Extensions;
using Yuebon.Security.IServices;
using Yuebon.Security.Services;
using Yuebon.Security.Services.CommandHandlers;

namespace Yuebon.WebApi.Controllers;

/// <summary>
/// 用户登录接口控制器
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class LoginController : ApiController
{
    private IUserService _userService;
    private IUserLogOnService _userLogOnService;
    private ISystemTypeService _systemTypeService;
    private IAPPService _appService;
    private IRoleService _roleService;
    private IRoleDataService _roleDataService;
    private ILoginLogService _loginLogService;
    private IFilterIPService _filterIPService;
    private IMenuService _menuService;
    private ITenantService _tenantService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;
    private ItemsDetailService _itemsDetailService;
    private IOrganizeService _organizeService;
    /// <summary>
    /// 构造函数注入服务
    /// </summary>
    /// <param name="iService"></param>
    /// <param name="userLogOnService"></param>
    /// <param name="systemTypeService"></param>
    /// <param name="logService"></param>
    /// <param name="appService"></param>
    /// <param name="roleService"></param>
    /// <param name="filterIPService"></param>
    /// <param name="roleDataService"></param>
    /// <param name="menuService"></param>
    /// <param name="tenantService"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="mediator"></param>
    /// <param name="organizeService"></param>
    public LoginController(IUserService iService, 
        IUserLogOnService userLogOnService,
        ISystemTypeService systemTypeService,
        ILoginLogService logService,
        IAPPService appService,
        IRoleService roleService,
        IFilterIPService filterIPService,
        IRoleDataService roleDataService,
        IMenuService menuService,
        ITenantService tenantService,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediator, IOrganizeService organizeService)
    {
        _userService = iService;
        _userLogOnService = userLogOnService;
        _systemTypeService = systemTypeService;
        _loginLogService = logService;
        _appService = appService;
        _roleService = roleService;
        _filterIPService = filterIPService;
        _roleDataService = roleDataService;
        _menuService = menuService;
        _tenantService = tenantService;
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
        _organizeService = organizeService;

    }
    /// <summary>
    /// 用户登录，必须要有验证码
    /// </summary>
    /// <returns>返回用户User对象</returns>
    [HttpPost("GetCheckUser")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCheckUser(LoginInput input)
    {
        CommonResult result = new CommonResult();
        RemoteIpParser remoteIpParser = new RemoteIpParser();
        string strIp = _httpContextAccessor.HttpContext.GetClientUserIp();
        YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
        string code = yuebonCacheHelper.Get(CacheConst.KeyVerCode + input.Vkey).ToString();
        if (input.Vcode.ToLower() != code.ToLower())
        {
            result.ErrMsg = "验证码错误";
            return ToJsonContent(result);
        }
        if (string.IsNullOrEmpty(input.Username))
        {
            result.ErrMsg = "用户名不能为空！";
        }
        else if (string.IsNullOrEmpty(input.Password))
        {
            result.ErrMsg = "密码不能为空！";
        }
        UserInfo userInfo = new UserInfo();
        
        bool blIp = _filterIPService.ValidateIP(strIp);
        if (blIp)
        {
            result.ErrMsg = strIp + "该IP已被管理员禁止登录！";
        }
        else
        {             
            
            if (string.IsNullOrEmpty(input.SystemCode))
            {
                result.ErrMsg = ErrCode.err403;
            }
            else
            {
                List<AllowCacheApp> list = yuebonCacheHelper.Get<object>(CacheConst.KeyAppList).ToJson().ToList<AllowCacheApp>();
                if (list == null)
                {
                    IEnumerable<APP> appList = _appService.GetAllByIsNotDeleteAndEnabledMark();
                    yuebonCacheHelper.Add(CacheConst.KeyAppList, appList);
                }
                string strHost = Request.Headers["Origin"].ToString();
                APP app = await _appService.GetAPP(input.AppId);
                if (app == null)
                {
                    result.ErrCode = "40001";
                    result.ErrMsg = ErrCode.err40001;
                }
                else
                {
                    if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal)&&!strHost.Contains("localhost"))
                    {
                        result.ErrCode = "40002";
                        result.ErrMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                    }
                    else
                    {
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
                                string tenantName =input.Host.IsNullOrEmpty()?"": input.Host?.Split(".")[0];
                                Tenant tenant = tenants.FindLast(o => o.TenantName == tenantName|| tenantName.Contains(o.HostDomain));//通过租户名称或者通过客户绑定的独立域名
                                if (tenant == null && tenantName != "default")
                                {
                                    if (tenant == null)
                                    {
                                        User tempUser = await _userService.GetByUserName(input.Username);
                                        if (tempUser != null)
                                        {
                                            tenant = await _tenantService.GetAsync(tempUser.TenantId);
                                            userInfo.CreateOrgId =(long)tempUser.CreateOrgId;
                                            userInfo.UserType= tempUser.UserType;
                                        }
                                        if (tenant == null)
                                        {
                                            result.ErrMsg = "非法访问";
                                            return ToJsonContent(result);
                                        }
                                    }
                                } else if(tenant.TenantName== "default")
                                {
                                    User tempUser = await _userService.GetByUserName(input.Username);
                                    if (tempUser != null)
                                    {
                                        tenant = await _tenantService.GetAsync(tempUser.TenantId);
                                        userInfo.CreateOrgId = (long)tempUser.CreateOrgId;
                                    }

                                }
                                if (tenant != null)
                                {
                                    userInfo.TenantId = tenant.Id;
                                    userInfo.TenantSchema = tenant.Schema;
                                    userInfo.TenantDataSource = tenant.DataSource;
                                    userInfo.TenantName = tenant.TenantName;
                                }
                            }
                        }
                        else
                        {
                            userInfo.TenantId = 9242772129579077;
                            userInfo.TenantName = "default";
                        }
                        Appsettings.User = userInfo;
                        SystemType systemType = _systemTypeService.GetByCode(input.SystemCode,userInfo.TenantId);
                        if (systemType == null)
                        {
                            result.ErrMsg = ErrCode.err403;
                        }
                        else
                        {
                            Tuple<User, string> userLogin = await this._userService.Validate(input.Username, input.Password);
                            if (userLogin != null)
                            {

                                var client = Parser.GetDefault().Parse(_httpContextAccessor.HttpContext.Request.Headers["User-Agent"]);
                                string requestPath = _httpContextAccessor.HttpContext.Request.Path.ToString();
                                string queryString = _httpContextAccessor.HttpContext.Request.QueryString.ToString();
                                string requestUrl = requestPath + queryString;
                                if (userLogin.Item1 != null)
                                {
                                    result.Success = true;
                                    User user = userLogin.Item1;
                                    userInfo.UserId = user.Id;
                                    userInfo.UserName = user.Account;
                                    userInfo.Role = await _roleService.GetRoleIdsByUserId(user.Id);
                                    userInfo.UserType = user.UserType;
                                    userInfo.CreateOrgId =(long)user.CreateOrgId;

                                    JwtOption jwtModel = Appsettings.GetService<JwtOption>();
                                    TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                    TokenResult tokenResult = tokenProvider.LoginToken(userInfo, input.AppId);
                                    YuebonCurrentUser currentSession = new YuebonCurrentUser
                                    {
                                        UserId = user.Id,
                                        RealName = user.RealName,
                                        AccessToken = tokenResult.AccessToken,
                                        TokenExpiresIn=tokenResult.ExpiresIn,
                                        AppKey = input.AppId,
                                        CreateTime = DateTime.Now,
                                        Role = userInfo.Role,
                                        ActiveSystemId = systemType.Id,
                                        CurrentLoginIP = strIp,
                                        OrganizeId=user.CreateOrgId,
                                        UserType = user.UserType
                                    };
                                    if (isTenant)
                                    {
                                        currentSession.TenantId = userInfo.TenantId;
                                    }
                                    SysSetting sysSetting = yuebonCacheHelper.Get(CacheConst.KeySysSetting).ToJson().ToObject<SysSetting>();
                                    if (sysSetting != null)
                                    {
                                        if (sysSetting.Webstatus == "1" && Appsettings.User.UserType!=UserTypeEnum.SuperAdmin)
                                        {
                                            result.ErrCode = "40900";
                                            result.ErrMsg = sysSetting.Webclosereason;
                                            return ToJsonContent(result);
                                        }
                                    }
                                    TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                    yuebonCacheHelper.Add(CacheConst.KeyLoginUser + user.Id.ToString(), currentSession, expiresSliding, true);
                                    yuebonCacheHelper.Add(CacheConst.KeyLoginUserInfo + user.Id.ToString(), userInfo, expiresSliding, true);
                                   
                                    CurrentUser = currentSession;
                                    result.ResData = currentSession;
                                    result.ErrCode = ErrCode.successCode;
                                    result.Success = true;
                                    
                                    LoginLog logEntity = new LoginLog();
                                    logEntity.Id = IdGeneratorHelper.IdSnowflake();
                                    logEntity.Account = user.Account;
                                    logEntity.NickName = user.NickName;
                                    logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                    logEntity.IPAddress = strIp;
                                    logEntity.Browser = client.UA.Family + client.UA.Major;
                                    logEntity.OS = client.OS.Family + client.OS.Major;
                                    logEntity.Result = true;
                                    logEntity.Description = "登录成功";
                                    LogInLogCommand logInLogCommand = new LogInLogCommand();
                                    logInLogCommand.LoginLogInputDto = logEntity;
                                    await _mediator.Send(logInLogCommand);
                                }
                                else
                                {
                                    result.ErrCode = ErrCode.failCode;
                                    result.ErrMsg = userLogin.Item2;
                                    LoginLog logEntity = new LoginLog();
                                    logEntity.Id = IdGeneratorHelper.IdSnowflake();
                                    logEntity.Account = input.Username;
                                    logEntity.NickName = input.Username;
                                    logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                    logEntity.IPAddress = strIp;
                                    logEntity.Browser = client.UA.Family + client.UA.Major;
                                    logEntity.OS = client.OS.Family + client.OS.Major;
                                    logEntity.Result = true;
                                    logEntity.Description = "登录失败，" + userLogin.Item2;
                                    LogInLogCommand logInLogCommand = new LogInLogCommand();
                                    logInLogCommand.LoginLogInputDto = logEntity;
                                    await _mediator.Send(logInLogCommand);
                                }
                            }
                        }

                    }
                }
            }
        }
        yuebonCacheHelper.Remove(CacheConst.KeyVerCode+ input.Vkey);
        return ToJsonContent(result, true);
    }

    /// <summary>
    /// 获取登录用户权限信息
    /// </summary>
    /// <returns>返回用户User对象</returns>
    [HttpGet("GetUserInfo")]
    [YuebonAuthorize("")]
    public async Task<IActionResult> GetUserInfo()
    {
        CommonResult result = new CommonResult();
        if (CurrentUser == null)
        {
            return Logout();
        }
        User user = await _userService.GetAsync(CurrentUser.UserId);
        SystemType systemType =await _systemTypeService.GetAsync(CurrentUser.ActiveSystemId);
        Log4NetHelper.Info(CurrentUser.ToJson());
        YuebonCurrentUser currentSession = new YuebonCurrentUser
        {
            UserId = user.Id,
            Account = user.Account,
            RealName = user.RealName,
            NickName = user.NickName,
            AccessToken = CurrentUser.AccessToken,
            AppKey = CurrentUser.AppKey,
            CreateTime = DateTime.Now,
            HeadIcon = user.HeadIcon,
            Gender = user.Gender,
            UserType =user.UserType,
            ReferralUserId = user.ReferralUserId,
            MemberGradeId = user.MemberGradeId,
            Role =await _roleService.GetRoleIdsByUserId(user.Id),
            MobilePhone = user.MobilePhone,
            OrganizeId = user.CreateOrgId,
            CurrentLoginIP = CurrentUser.CurrentLoginIP,
            IPAddressName = CurrentUser.IPAddressName,
            TenantId = null
        };
		CurrentUser = currentSession;
        CurrentUser.HeadIcon = user.HeadIcon;

        CurrentUser.ActiveSystemId = systemType.Id;
        CurrentUser.ActiveSystem = systemType.FullName;
        CurrentUser.ActiveSystemUrl = systemType.Url;

        await GetUserPermision();
        result.ResData = CurrentUser;
        result.ErrCode = ErrCode.successCode;
        result.Success = true;
        return ToJsonContent(result, true);
    }


    private async Task  GetUserPermision()
    {
        YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
        SystemType systemType = await _systemTypeService.GetAsync(CurrentUser.ActiveSystemId);
        List<UserVisitMenus> listFunction = new List<UserVisitMenus>();
        if (Permission.IsAdmin(CurrentUser))
        {
            CurrentUser.SubSystemList = _systemTypeService.GetAllByIsNotDeleteAndEnabledMark().MapTo<UserVisitSystemnTypes>();
            //取得用户可使用的授权功能信息，并存储在缓存中
            listFunction = _menuService.GetFunctionsBySystem(CurrentUser.ActiveSystemId);
            CurrentUser.MenusRouter = _menuService.GetVueRouter(null, systemType.EnCode);
        }
        else
        {
            CurrentUser.SubSystemList = await _systemTypeService.GetSubSystemList(CurrentUser.Role);
            //取得用户可使用的授权功能信息，并存储在缓存中
            listFunction = await _menuService.GetFunctionsByUser(CurrentUser.UserId, CurrentUser.ActiveSystemId);
            CurrentUser.MenusRouter = _menuService.GetVueRouter(CurrentUser.Role, systemType.EnCode);
        }
        UserLogOn userLogOn = _userLogOnService.GetByUserId(CurrentUser.UserId);
        CurrentUser.UserTheme = userLogOn.Theme == null ? "default" : userLogOn.Theme;
        TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
        yuebonCacheHelper.Add(CacheConst.KeyUserFunction + CurrentUser.UserId, listFunction, expiresSliding, true);
        List<string> listModules = new List<string>();
        foreach (UserVisitMenus item in listFunction)
        {
            listModules.Add(item.EnCode);
        }
        CurrentUser.Modules = listModules;
        yuebonCacheHelper.Add(CacheConst.KeyLoginUser + CurrentUser.UserId, CurrentUser, expiresSliding, true);
        //该用户的数据权限
        List<long> roleDataList = await _roleDataService.GetListDeptByRole(CurrentUser.Role);
        yuebonCacheHelper.Add(CacheConst.KeyUserOrg + CurrentUser.UserId, roleDataList, expiresSliding, true);
    }
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetProfile")]
    public IActionResult GetProfile()
    {
        CommonResult result = new CommonResult();
        if (CurrentUser == null)
        {
            return Logout();
        }
        User user = _userService.GetById(CurrentUser.UserId);
        result.ResData = user.MapTo<UserOutputDto>();
        result.ErrCode = ErrCode.successCode;
        result.ErrMsg = ErrCode.err0;
        return ToJsonContent(result, true);
    }
    /// <summary>
    /// 用户登录，无验证码，主要用于app登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="appId">AppId</param>
    /// <param name="systemCode">系统编码</param>
    /// <param name="mac">设备mac</param>
    /// <returns>返回用户User对象</returns>
    [HttpGet("UserLogin")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLogin(string username, string password, string appId, string systemCode,string mac)
    //public async Task<IActionResult> UserLogin(string username= "admin", string password = "admin888",  string appId = "system", string systemCode = "openauth")
    {

        CommonResult result = new CommonResult();
        RemoteIpParser remoteIpParser = new RemoteIpParser();
        string strIp = _httpContextAccessor.HttpContext.GetClientUserIp();
        YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();

        //判断mac地址是否存在
        //var isMac = await _userService.CheckMacExists(mac);

        if (string.IsNullOrEmpty(username))
        {
            result.ErrMsg = "用户名不能为空！";
        }
        else if (string.IsNullOrEmpty(password))
        {
            result.ErrMsg = "密码不能为空！";
        }

        //if (!isMac)
        //{
        //    result.ErrMsg = "移动设备识别码不匹配!";
        //    result.ErrCode = ErrCode.failCode;
        //    return ToJsonContent(result, true);
        //}

        UserInfo userInfo = new UserInfo();

        bool blIp = _filterIPService.ValidateIP(strIp);
        if (blIp)
        {
            result.ErrMsg = strIp + "该IP已被管理员禁止登录！";
        }
        else
        {

            if (string.IsNullOrEmpty(systemCode))
            {
                result.ErrMsg = ErrCode.err403;
            }
            else
            {
                List<AllowCacheApp> list = yuebonCacheHelper.Get<object>(CacheConst.KeyAppList).ToJson().ToList<AllowCacheApp>();
                if (list == null)
                {
                    IEnumerable<APP> appList = _appService.GetAllByIsNotDeleteAndEnabledMark();
                    yuebonCacheHelper.Add(CacheConst.KeyAppList, appList);
                }
                string strHost = Request.Headers["Origin"].ToString();
                APP app = await _appService.GetAPP(appId);
                if (app == null)
                {
                    result.ErrCode = "40001";
                    result.ErrMsg = ErrCode.err40001;
                }
                else
                {
                    if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal))
                    {
                        result.ErrCode = "40002";
                        result.ErrMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                    }
                    else
                    {
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
                               string tenantName = strHost.Split(".")[0];;
                                Tenant tenant = tenants.FindLast(o => o.TenantName == tenantName || tenantName.Contains(o.HostDomain));//通过租户名称或者通过客户绑定的独立域名
                                if (tenant == null && tenantName != "default")
                                {
                                    if (tenant == null)
                                    {
                                        User tempUser = await _userService.GetByUserName(username);
                                        if (tempUser != null)
                                        {
                                            tenant = await _tenantService.GetAsync(tempUser.TenantId);
                                        }
                                        if (tenant == null)
                                        {
                                            result.ErrMsg = "非法访问";
                                            return ToJsonContent(result);
                                        }
                                    }
                                }
                                else if (tenant.TenantName == "default")
                                {
                                    User tempUser = await _userService.GetByUserName(username);
                                    if (tempUser != null)
                                    {
                                        tenant = await _tenantService.GetAsync(tempUser.TenantId);
                                    }

                                }
                                if (tenant != null)
                                {
                                    userInfo.TenantId = tenant.Id;
                                    userInfo.TenantSchema = tenant.Schema;
                                    userInfo.TenantDataSource = tenant.DataSource;
                                    userInfo.TenantName = tenant.TenantName;
                                }
                            }
                        }
                        else
                        {
                            userInfo.TenantId = 9242772129579077;
                            userInfo.TenantName = "default";
                        }
                        Appsettings.User = userInfo;
                        SystemType systemType = _systemTypeService.GetByCode(systemCode, userInfo.TenantId);
                        if (systemType == null)
                        {
                            result.ErrMsg = ErrCode.err403;
                        }
                        else
                        {
                            Tuple<User, string> userLogin = await this._userService.Validate(username, password);
                            if (userLogin != null)
                            {

                                var client = Parser.GetDefault().Parse(_httpContextAccessor.HttpContext.Request.Headers["User-Agent"]);
                                string requestPath = _httpContextAccessor.HttpContext.Request.Path.ToString();
                                string queryString = _httpContextAccessor.HttpContext.Request.QueryString.ToString();
                                string requestUrl = requestPath + queryString;
                                if (userLogin.Item1 != null)
                                {
                                    result.Success = true;
                                    User user = userLogin.Item1;
                                    userInfo.UserId = user.Id;
                                    userInfo.UserName = user.Account;
                                    userInfo.Role = await _roleService.GetRoleIdsByUserId(user.Id);
                                    userInfo.UserType = user.UserType;
                                    userInfo.CreateOrgId = user.CreateOrgId;

                                    JwtOption jwtModel = Appsettings.GetService<JwtOption>();
                                    TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                    TokenResult tokenResult = tokenProvider.LoginToken(userInfo, appId);
                                    YuebonCurrentUser currentSession = new YuebonCurrentUser
                                    {
                                        UserId = user.Id,
                                        Account= user.Account,
                                        RealName = user.RealName,
                                        Station= user.Station,
                                        AccessToken = tokenResult.AccessToken,
                                        TokenExpiresIn = tokenResult.ExpiresIn,
                                        AppKey = appId,
                                        CreateTime = DateTime.Now,
                                        Role = userInfo.Role,
                                        OrganizeId = user.CreateOrgId,
                                        OrganizeName = _organizeService.GetById(user.Organizeid.Value)?.FullName ,
                                        UserType = userInfo.UserType,
                                        ActiveSystemId = systemType.Id,
                                        CurrentLoginIP = strIp
                                    };
                                    CurrentUser = currentSession;
                                    CurrentUser.HeadIcon = user.HeadIcon;

                                    CurrentUser.ActiveSystemId = systemType.Id;
                                    CurrentUser.ActiveSystem = systemType.FullName;
                                    CurrentUser.ActiveSystemUrl = systemType.Url;
                                    if (isTenant)
                                    {
                                        currentSession.TenantId = userInfo.TenantId;
                                    }
                                    SysSetting sysSetting = yuebonCacheHelper.Get(CacheConst.KeySysSetting).ToJson().ToObject<SysSetting>();
                                    if (sysSetting != null)
                                    {
                                        if (sysSetting.Webstatus == "1" && userInfo.UserType!=UserTypeEnum.SuperAdmin)
                                        {
                                            result.ErrCode = "40900";
                                            result.ErrMsg = sysSetting.Webclosereason;
                                            return ToJsonContent(result);
                                        }
                                    }
                                    TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                    yuebonCacheHelper.Add(CacheConst.KeyLoginUser + user.Id.ToString(), currentSession, expiresSliding, true);
                                    yuebonCacheHelper.Add(CacheConst.KeyLoginUserInfo + user.Id.ToString(), userInfo, expiresSliding, true);
                                    await GetUserPermision();
                                    CurrentUser = currentSession;
                                    result.ResData = currentSession;
                                    result.ErrCode = ErrCode.successCode;
                                    result.Success = true;

                                    LoginLog logEntity = new LoginLog();
                                    logEntity.Id = IdGeneratorHelper.IdSnowflake();
                                    logEntity.Account = user.Account;
                                    logEntity.NickName = user.NickName;
                                    logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                    logEntity.IPAddress = strIp;
                                    logEntity.Browser = client.UA.Family + client.UA.Major;
                                    logEntity.OS = client.OS.Family + client.OS.Major;
                                    logEntity.Result = true;
                                    logEntity.Description = "登录成功";
                                    logEntity.CreateOrgId = userInfo.CreateOrgId;
                                    LogInLogCommand logInLogCommand = new LogInLogCommand();
                                    logInLogCommand.LoginLogInputDto = logEntity;
                                    await _mediator.Send(logInLogCommand);
                                }
                                else
                                {
                                    result.ErrCode = ErrCode.failCode;
                                    result.ErrMsg = userLogin.Item2;
                                    LoginLog logEntity = new LoginLog();
                                    logEntity.Id = IdGeneratorHelper.IdSnowflake();
                                    logEntity.Account = username;
                                    logEntity.NickName = username;
                                    logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                    logEntity.IPAddress = strIp;
                                    logEntity.Browser = client.UA.Family + client.UA.Major;
                                    logEntity.OS = client.OS.Family + client.OS.Major;
                                    logEntity.Result = true;
                                    logEntity.Description = "登录失败，" + userLogin.Item2;
                                    logEntity.CreateOrgId=userInfo.CreateOrgId;
                                    LogInLogCommand logInLogCommand = new LogInLogCommand();
                                    logInLogCommand.LoginLogInputDto = logEntity;
                                    await _mediator.Send(logInLogCommand);
                                }
                            }
                        }

                    }
                }
            }
        }
        return ToJsonContent(result, true);
    }


    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpGet("Logout")]
    [YuebonAuthorize("")]
    public IActionResult Logout()
    {
        CommonResult result = new CommonResult();
        if (CurrentUser != null)
        {
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            yuebonCacheHelper.Remove(CacheConst.KeyLoginUser + CurrentUser.UserId);
            yuebonCacheHelper.Remove(CacheConst.KeyUserFunction + CurrentUser.UserId);
            UserLogOn userLogOn = _userLogOnService.GetWhere("UserId='" + CurrentUser.UserId + "'");
            userLogOn.UserOnLine = false;
            _userLogOnService.Update(userLogOn);
        }
        CurrentUser = null;
        result.Success = true;
        result.ErrCode = ErrCode.successCode;
        result.ErrMsg = "成功退出";
        return ToJsonContent(result);
    }

    /// <summary>
    /// 子系统切换登录
    /// </summary>
    /// <param name="openmf">凭据</param>
    /// <param name="appId">应用Id</param>
    /// <param name="systemCode">子系统编码</param>
    /// <returns>返回用户User对象</returns>
    [HttpGet("SysConnect")]
    [AllowAnonymous]
    [NoPermissionRequired]
    public async Task<IActionResult> SysConnect(string openmf, string appId, string systemCode)
    {
        CommonResult result = new CommonResult();
        RemoteIpParser remoteIpParser = new RemoteIpParser();
        YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
        string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();
        if (string.IsNullOrEmpty(openmf))
        {
            result.ErrMsg = "切换参数错误！";
        }
        bool isTenant = Appsettings.app(new string[] { "AppSetting", "IsTenant" }).ObjToBool();
        UserInfo userInfo = null;
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
                string strHost = Request.Host.ToString();
                Tenant tenant = tenants.FindLast(o => o.HostDomain == strHost);
                if (tenant == null)
                {
                    result.ErrMsg = "非法访问";
                    return ToJsonContent(result);
                }
                else
                {
                    userInfo.TenantId = tenant.Id;
                    userInfo.TenantSchema = tenant.Schema;
                    userInfo.TenantDataSource = tenant.DataSource;
                }
            }
        }
        bool blIp = _filterIPService.ValidateIP(strIp);
        if (blIp)
        {
            result.ErrMsg = strIp + "该IP已被管理员禁止登录！";
        }
        else
        {
            string ipAddressName =await IpAddressUtil.GetCityByIp(strIp);
            if (string.IsNullOrEmpty(systemCode))
            {
                result.ErrMsg = ErrCode.err403;
            }
            else
            {
                string strHost = Request.Host.ToString();
                APP app = await _appService.GetAPP(appId);
                if (app == null)
                {
                    result.ErrCode = "40001";
                    result.ErrMsg = ErrCode.err40001;
                }
                else
                {
                    if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                    {
                        result.ErrCode = "40002";
                        result.ErrMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                    }
                    else
                    {
                        SystemType systemType = _systemTypeService.GetByCode(systemCode);
                        if (systemType == null)
                        {
                            result.ErrMsg = ErrCode.err403;
                        }
                        else
                        {
                            object cacheOpenmf = yuebonCacheHelper.Get("openmf" + openmf);
                            yuebonCacheHelper.Remove("openmf" + openmf);
                            if (cacheOpenmf == null)
                            {
                                result.ErrCode = "40007";
                                result.ErrMsg = ErrCode.err40007;
                            }
                            else
                            {
                                User user = _userService.GetById(cacheOpenmf.ToInt());
                                if (user != null)
                                {
                                    userInfo.UserId = user.Id;
                                    userInfo.UserName = user.Account;
                                    userInfo.Role = await _roleService.GetRoleIdsByUserId(user.Id);
                                    userInfo.UserType = user.UserType;
                                    result.Success = true;
                                    JwtOption jwtModel = Appsettings.GetService<JwtOption>();
                                    TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                    TokenResult tokenResult = tokenProvider.LoginToken(userInfo, appId);
                                    YuebonCurrentUser currentSession = new YuebonCurrentUser
                                    {
                                        UserId = user.Id,
                                        RealName = user.RealName,
                                        AccessToken = tokenResult.AccessToken,
                                        AppKey = appId,
                                        CreateTime = DateTime.Now,
                                        Role = userInfo.Role,
                                        UserType = userInfo.UserType,
                                        ActiveSystemId = systemType.Id,
                                        CurrentLoginIP = strIp,
                                        IPAddressName = ipAddressName,
                                        ActiveSystemUrl= systemType.Url

                                    };
                                    TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                    yuebonCacheHelper.Add(CacheConst.KeyLoginUser + user.Id, currentSession, expiresSliding, true);
                                    CurrentUser = currentSession;
                                    result.ResData = currentSession;
                                    result.ErrCode = ErrCode.successCode;
                                    result.Success = true;
                                }
                                else
                                {
                                    result.ErrCode = ErrCode.failCode;
                                }
                            }
                        }
                    }
                }
            }
        }
        return ToJsonContent(result);
    }

    /// <summary>
    /// 弃用接口演示
    /// </summary>
    /// <returns></returns>
    [HttpGet("TestLogin")]
    [Obsolete]
    public IActionResult TestLogin()
    {
        CommonResult result = new CommonResult();
        result.Success = true;
        result.ErrCode = ErrCode.successCode;
        result.ResData = "弃用接口演示";
        result.ErrMsg = "成功退出";
        return ToJsonContent(result);
    }
}