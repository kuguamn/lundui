namespace Yuebon.Security.IServices
{
    /// <summary>
    /// 定义APP版本管理表服务接口
    /// </summary>
    public  interface IAppversionService:IService<Appversion,AppversionOutputDto>
    {
        /// <summary>
        /// 获取最新版本
        /// </summary>
        /// <param name="version">版本</param>
        /// <returns></returns>
        Task<Appversion> GetNewVersion();
    }
}
