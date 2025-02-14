namespace Yuebon.Security.Repositories
{
    /// <summary>
    /// APP版本管理表仓储接口的实现
    /// </summary>
    public  class AppversionRepository : BaseRepository<Appversion>, IAppversionRepository
    {

        public AppversionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}