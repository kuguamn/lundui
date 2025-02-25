

namespace Yuebon.Security.IRepositories;

/// <summary>
/// 
/// </summary>
public interface IAPPRepository:IRepository<APP>
{
    /// <summary>
    /// 获取app对象
    /// </summary>
    /// <param name="appid">应用ID</param>
    /// <param name="secret">应用密钥AppSecret</param>
    /// <returns></returns>
    APP GetAPP(string appid, string secret);

    /// <summary>
    /// 获取app对象
    /// </summary>
    /// <param name="appid">应用ID</param>
    /// <returns></returns>
    APP GetAPP(string appid);

}