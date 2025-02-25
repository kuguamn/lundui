namespace Yuebon.Security.IServices;

public interface IUserLogOnService:IService<UserLogOn, UserLogOnOutputDto>
{

    /// <summary>
    /// 根据会员ID获取用户登录信息实体
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    UserLogOn GetByUserId(long userId);

    /// <summary>
    /// 根据会员ID获取用户登录信息实体
    /// </summary>
    /// <param name="info">主题配置信息</param>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    Task<bool> SaveUserTheme(UserThemeInputDto info, long userId);
    Task<bool> UpdateAsync(UserLogOn entity,long id);
    
}
