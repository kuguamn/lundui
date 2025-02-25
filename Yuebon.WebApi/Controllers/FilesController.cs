﻿namespace Yuebon.WebApi.Controllers;

/// <summary>
/// 文件上传
/// </summary>
[Route("api/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FilesController : ApiController
{

    private string _filePath;
    private string _dbFilePath;   //数据库中的文件路径
    private string _dbThumbnail;   //数据库中的缩略图路径
    private string _belongApp;//所属应用
    private string _belongAppId;//所属应用ID 
    private string _fileName;//文件名称
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IUploadFileService _iService;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostingEnvironment"></param>
    /// <param name="iService"></param>
    public FilesController(IWebHostEnvironment hostingEnvironment, IUploadFileService iService)
    {
        _hostingEnvironment = hostingEnvironment;
        _iService= iService;
    }

    /// <summary>
    ///  单文件上传接口
    /// </summary>
    /// <param name="formCollection"></param>
    /// <returns>服务器存储的文件信息</returns>
    [HttpPost("Upload")]
    [NoSignRequired]
    public IActionResult Upload([FromForm] IFormCollection formCollection)
    {
        CommonResult result = new CommonResult();

        FormFileCollection filelist = (FormFileCollection)formCollection.Files;
        string belongApp = formCollection["belongApp"].ToString();
        string belongAppId = formCollection["belongAppId"].ToString();
        _fileName = filelist[0].FileName;
        try
        {
            result.ResData = Add(filelist[0], belongApp, belongAppId);
            result.ErrCode = ErrCode.successCode;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.ErrCode = "500";
            result.ErrMsg = ex.Message;
            Log4NetHelper.Error("", ex);
            throw;
        }
        return ToJsonContent(result);
    }
    /// <summary>
    ///  批量上传文件接口
    /// </summary>
    /// <param name="formCollection"></param>
    /// <returns>服务器存储的文件信息</returns>
    [HttpPost("Uploads")]
    public IActionResult  Uploads([FromForm] IFormCollection formCollection)
    {
        CommonResult result = new CommonResult();
        FormFileCollection filelist = (FormFileCollection)formCollection.Files;
        string belongApp = formCollection["belongApp"].ToString();
        string belongAppId = formCollection["belongAppId"].ToString();
        try
        {
           result.ResData = Adds(filelist, belongApp, belongAppId);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error("", ex);
            result.ErrCode = "500";
            result.ErrMsg = ex.Message;
        }

        return ToJsonContent(result);
    }
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("DeleteFile")]
    public IActionResult DeleteFile(long id)
    {
        CommonResult result = new CommonResult();
        try
        {
            UploadFile uploadFile = _iService.GetById(id);

            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            SysSetting sysSetting = yuebonCacheHelper.Get(CacheConst.KeySysSetting).ToJson().ToObject<SysSetting>();
            string localpath = _hostingEnvironment.WebRootPath;
            if (uploadFile != null)
            {
                string filepath = (localpath + "/" + uploadFile.FilePath).ToFilePath();
                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);
                string filepathThu = (localpath + "/" + uploadFile.Thumbnail).ToFilePath();
                if (System.IO.File.Exists(filepathThu))
                    System.IO.File.Delete(filepathThu);

                result.ErrCode = ErrCode.successCode;
                result.Success = true;
            }
            else
            {
                result.ErrCode = ErrCode.failCode;
                result.Success = false;
            }

        }
        catch (Exception ex)
        {
            Log4NetHelper.Error("", ex);
            result.ErrCode = "500";
            result.ErrMsg = ex.Message;
        }
        return ToJsonContent(result);
    }

    /// <summary>
    /// 批量上传文件
    /// </summary>
    /// <param name="files">文件</param>
    /// <param name="belongApp">所属应用，如文章article</param>
    /// <param name="belongAppId">所属应用ID，如文章id</param>
    /// <returns></returns>
    private List<UploadFileResultOuputDto> Adds(IFormFileCollection files, string belongApp, string belongAppId)
    {
        List<UploadFileResultOuputDto> result = new List<UploadFileResultOuputDto>();
        foreach (var file in files)
        {
            if (file != null)
            {
                result.Add(Add(file, belongApp, belongAppId));
            }
        }
        return result;
    }
    /// <summary>
    /// 单个上传文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="belongApp"></param>
    /// <param name="belongAppId"></param>
    /// <returns></returns>
    private UploadFileResultOuputDto Add(IFormFile file, string belongApp, string belongAppId)
    {
        _belongApp = belongApp;
        _belongAppId = belongAppId;
        if (file != null && file.Length > 0 && file.Length < 10485760)
        {
            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
            {
                var fileName = string.Empty;
                    fileName = _fileName;
                
                var data = binaryReader.ReadBytes((int)file.Length);
                UploadFile(fileName, data);
                UploadFile filedb = new UploadFile
                {
                    Id = IdGeneratorHelper.IdSnowflake(),
                    FilePath = _dbFilePath,
                    Thumbnail = _dbThumbnail,
                    FileName = fileName,
                    FileSize = file.Length.ToInt(),
                    FileType = Path.GetExtension(fileName),
                    Extension = Path.GetExtension(fileName),
                    BelongApp = _belongApp,
                    BelongAppId = _belongAppId
                };
                _iService.Insert(filedb);
                UploadFileResultOuputDto uploadFileResultOuputDto = filedb.MapTo<UploadFileResultOuputDto>();
                uploadFileResultOuputDto.PhysicsFilePath = (_hostingEnvironment.WebRootPath + "/"+ _dbThumbnail).ToFilePath();
                return uploadFileResultOuputDto;
            }
        }
        else
        {
            Log4NetHelper.Info("文件过大");
            throw new Exception("文件过大");
        }
    }
    /// <summary>
    /// 实现文件上传到服务器保存，并生成缩略图
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <param name="fileBuffers">文件字节流</param>
    private void UploadFile(string fileName, byte[] fileBuffers)
    {

        //判断文件是否为空
        if (string.IsNullOrEmpty(fileName))
        {
            Log4NetHelper.Info("文件名不能为空");
            throw new Exception("文件名不能为空");
        }

        //判断文件是否为空
        if (fileBuffers.Length < 1)
        {
            Log4NetHelper.Info("文件不能为空");
            throw new Exception("文件不能为空");
        }

        YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
        SysSetting sysSetting = yuebonCacheHelper.Get(CacheConst.KeySysSetting).ToJson().ToObject<SysSetting>();
        if (sysSetting==null) //缓存为空重新设置缓存
        {
            SysSetting sysSetting1 = XmlConverter.Deserialize<SysSetting>("xmlconfig/sys.config");
            sysSetting = sysSetting1;
            yuebonCacheHelper.Add(CacheConst.KeySysSetting, sysSetting1);
        }
        string folder = DateTime.Now.ToString("yyyyMMdd");
        _filePath = _hostingEnvironment.WebRootPath;
        var _tempfilepath = sysSetting.Filepath;

        if (!string.IsNullOrEmpty(_belongApp))
        {
            _tempfilepath += "/"+_belongApp;
        }
        if (!string.IsNullOrEmpty(_belongAppId))
        {
            _tempfilepath += "/" + _belongAppId;
        }
        if (sysSetting.Filesave == "1")
        {
            _tempfilepath = _tempfilepath + "/" + folder + "/";
        }
        if (sysSetting.Filesave == "2")
        {
            DateTime date = DateTime.Now;
            _tempfilepath = _tempfilepath + "/" + date.Year + "/" + date.Month + "/" + date.Day + "/";
        }

        var uploadPath = _filePath +"/"+ _tempfilepath;
        if (sysSetting.Fileserver == "localhost")
        {
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
        }
        string ext = Path.GetExtension(fileName).ToLower();
        long newName = IdGeneratorHelper.IdSnowflake();
        string newfileName= newName + ext;

        using (var fs = new FileStream(uploadPath + newfileName, FileMode.Create))
        {
            fs.Write(fileBuffers, 0, fileBuffers.Length);
            fs.Close();
            //生成缩略图
            //if (ext.Contains(".jpg") || ext.Contains(".jpeg") || ext.Contains(".png") || ext.Contains(".bmp") || ext.Contains(".gif"))
            //{
            //    string thumbnailName = newName + "_" + sysSetting.Thumbnailwidth + "x" + sysSetting.Thumbnailheight + ext;
            //    ImgHelper.MakeThumbnail(uploadPath + newfileName, uploadPath + thumbnailName, sysSetting.Thumbnailwidth.ToInt(), sysSetting.Thumbnailheight.ToInt());
            //    _dbThumbnail = _tempfilepath +  thumbnailName;
            //}
            _dbFilePath = _tempfilepath + newfileName;
        }
    }
}
