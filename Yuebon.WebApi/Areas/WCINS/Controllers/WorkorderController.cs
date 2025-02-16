using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Polly.Caching;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Transactions;
using Yuebon.CMS.IServices;
using Yuebon.Commons.Attributes;
using Yuebon.Commons.Extensions;
using Yuebon.Commons.Pages;
using Yuebon.Security.Models;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
using Yuebon.WCINS.Services;
using Yuebon.WebApi.Controllers;
namespace Yuebon.WebApi.Areas.WCINS.Controllers
{
    /// <summary>
    /// 工单接口
    /// </summary>
    [ApiController]
    [Route("api/WCINS/[controller]")]
    public partial class WorkorderController : AreaApiController<Workorder, WorkorderOutputDto, WorkorderInputDto, IWorkorderService>
    {
        private readonly ISequenceService sequenceService;
        private readonly IWorkorderdetailService workorderdetailService;
        private readonly IOrderresultsService orderresultsService;
        private readonly IConfiguration configuration;
        private readonly IPonentfiledService ponentfiledService;
        private readonly IPonenttypeService ponenttypeService;
        private readonly FilesController filesController;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="_sequenceService"></param>
        /// <param name="_workorderdetailService"></param>
        /// <param name="_orderresultsService"></param>
        /// <param name="_configuration"></param>
        /// <param name="_ponentfiledService"></param>
        /// <param name="_ponenttypeService"></param>
        /// <param name="_filesController"></param>
        public WorkorderController(IWorkorderService _iService,
            ISequenceService _sequenceService,
            IWorkorderdetailService _workorderdetailService, 
            IOrderresultsService _orderresultsService,
            IConfiguration _configuration,
            IPonentfiledService _ponentfiledService,
            IPonenttypeService _ponenttypeService,
            FilesController _filesController
            ) : base(_iService)
        {
            iService = _iService;
            sequenceService=_sequenceService;
            workorderdetailService=_workorderdetailService;
            orderresultsService = _orderresultsService;
            configuration = _configuration;
            ponentfiledService = _ponentfiledService;
            ponenttypeService = _ponenttypeService;
            filesController = _filesController;

        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Workorder info)
        {
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
        }

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Workorder info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Workorder info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于PC分页数据显示)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindPCWithPagerAsync")]
        [YuebonAuthorize("List")]
        public virtual async Task<CommonResult<PageResult<WorkorderOutputDto>>> FindPCWithPagerAsync(SearchInputDto<WorkorderSearch> search)
        {
            CommonResult<PageResult<WorkorderOutputDto>> result = new CommonResult<PageResult<WorkorderOutputDto>>();
            result.ResData = await iService.FindWithPagerAsync(search);
            result.ErrCode = ErrCode.successCode;
            return result;
        }

        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="winfo"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        [YuebonAuthorize("Add")]
        public override async Task<IActionResult> InsertAsync(WorkorderInputDto winfo)
        {
            CommonResult result = new CommonResult();
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 检查主表记录是否存在
                    var existingWorkorder = iService.GetById(winfo.Id);
                    // 如果主表数据不存在，新增主表记录
                    Workorder info = new Workorder
                    {
                        Id = winfo.Id,
                        OrderNo = winfo.OrderNo,
                        OrderType = winfo.OrderType,
                    };
                    if (existingWorkorder == null)
                    {
                        OnBeforeInsert(info); // 预处理
                        var mainInsert = await iService.InsertAsync(info);
                        if (mainInsert <= 0)
                        {
                            throw new Exception("新增主工单失败");
                        }
                    }
                    else //修改
                    {
                        OnBeforeUpdate(info); // 预处理
                        var mainInsert = await iService.UpdateAsync(info);
                        if (!mainInsert)
                        {
                            throw new Exception("新增主工单失败");
                        }
                    }

                    // 插入或更新工单明细及采集结果
                    if (winfo.workorderdetails != null && winfo.workorderdetails.Any())
                    {
                        var detailid = IdGeneratorHelper.IdSnowflake();
                        foreach (var detailDto in winfo.workorderdetails)
                        {
                            Workorderdetail detail = new Workorderdetail
                            {
                                Id = detailid,
                                Orderid = winfo.Id,
                                Imgurl = detailDto.Imgurl,
                                PonetTypeID = detailDto.PonetTypeID,
                                PonetTypeName = detailDto.PonetTypeName,
                                Status = detailDto.Status,
                                DeleteMark = false
                            };

                            var detailInsert = await workorderdetailService.InsertAsync(detail);
                            if (detailInsert <= 0)
                            {
                                throw new Exception($"新增工单明细失败，部件类型: {detailDto.PonetTypeName}");
                            }

                            // 处理采集结果
                            if (detailDto.orderresults != null && detailDto.orderresults.Any())
                            {
                                foreach (var resultDto in detailDto.orderresults)
                                {
                                    // 新增采集结果
                                    var resultId = IdGeneratorHelper.IdSnowflake();
                                    Orderresults orderresult = new Orderresults
                                    {
                                        Id = resultId,
                                        Orderdetailid = detailid,
                                        DisFiled = resultDto.DisFiled,
                                        DisResult = resultDto.DisResult,
                                        DeleteMark = false
                                    };

                                    var resultInsert = await orderresultsService.InsertAsync(orderresult);
                                    if (resultInsert <= 0)
                                    {
                                        throw new Exception($"新增采集结果失败");
                                    }
                                }
                            }
                        }
                    }

                    result.ErrCode = ErrCode.successCode;
                    result.ErrMsg = ErrCode.err0;
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    result.ErrMsg = $"操作失败: {ex.Message}";
                    result.ErrCode = "43002";
                }
            }

            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="winfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(WorkorderInputDto winfo)
        {
            CommonResult result = new CommonResult();

            Workorder info = iService.GetById(winfo.Id);
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
        /// 获取采集结果
        /// </summary>
        /// <param name="resultsDto"></param>
        /// <returns></returns>
        [HttpPost("GetCollectionResultsAsync")]
        public async Task<IActionResult> GetCollectionResultsAsync(CollectionResultsDto resultsDto)
        {
            CommonResult result = new CommonResult();

            try
            {
                // Step 1: 校验输入参数
                if (resultsDto.ponentTypeId <= 0 || string.IsNullOrEmpty(resultsDto.imgUrl))
                {
                    result.ErrMsg = "参数错误：部件类型ID或采集图片不能为空";
                    result.ErrCode = "400";
                    return ToJsonContent(result);
                }

                // Step 2: 获取部件类型信息
                var ponenttype = ponenttypeService.GetById(resultsDto.ponentTypeId);
                if (ponenttype == null)
                {
                    result.ErrMsg = "无效的部件类型";
                    result.ErrCode = "404";
                    return ToJsonContent(result);
                }

                // Step 3: 获取采集字段信息
                var ponentFields = await ponentfiledService.GetListWhereAsync($"PonentTypeid = {resultsDto.ponentTypeId} AND EnabledMark = 1");
                if (ponentFields == null || !ponentFields.Any())
                {
                    result.ErrMsg = "未找到对应的采集字段";
                    result.ErrCode = "404";
                    return ToJsonContent(result);
                }

                // Step 4: 请求采集接口获取识别结果
                var collectResults = await CallCollectPartyApiAsync(ponenttype.Typecode, resultsDto.imgUrl);
                if (collectResults == null || !collectResults.Any())
                {
                    result.ErrMsg = "采集接口未返回任何结果";
                    result.ErrCode = "500";
                    return ToJsonContent(result);
                }

                // Step 5: 处理图片上传
                string base64Image = collectResults["picture"];
                string uploadedFilePath = await SaveBase64ImageAsync(base64Image);
                if (string.IsNullOrEmpty(uploadedFilePath))
                {
                    result.ErrMsg = "图片保存失败";
                    result.ErrCode = "500";
                    return ToJsonContent(result);
                }

                // Step 6: 根据采集字段信息匹配并替换键值
                var matchedResults = ponentFields
                    .Select(field => new
                    {
                        DisFiled = field.FiledName,
                        DisResult = collectResults.ContainsKey(field.FiledValue) ? collectResults[field.FiledValue] : null
                    })
                    .Where(r => r.DisResult != null) // 过滤掉未匹配的字段
                    .ToList();

                // Step 7: 构建返回结果
                var collectionResults = new
                {
                    Imgurl = uploadedFilePath,
                    PonetTypeID = ponenttype.Id,
                    PonetTypeName = ponenttype.Typename,
                    OrderResults = matchedResults
                };

                // Step 8: 返回结果
                result.ResData = collectionResults;
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            catch (Exception ex)
            {
                result.ErrMsg = $"获取采集结果失败: {ex.Message}";
                result.ErrCode = "500";
            }

            return ToJsonContent(result);
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerAsync")]
        [YuebonAuthorize("List")]
        public override async Task<CommonResult<PageResult<WorkorderOutputDto>>> FindWithPagerAsync(SearchInputDto<Workorder> search)
        {
            CommonResult<PageResult<WorkorderOutputDto>> result = new CommonResult<PageResult<WorkorderOutputDto>>();

            //获取工单列表
            var workorder = await iService.FindWithPagerAsync(search);

            // 2. 批量预加载数据
            var workOrderIds = workorder.Items.Select(x => x.Id).ToList();
            var workOrderIdsString = string.Join(",", workOrderIds);
            // 2.1 批量获取所有工单明细
            var allDetails = await workorderdetailService.GetListWhereAsync($"Orderid in ({workOrderIdsString})");

            foreach (var item in workorder.Items) 
            {

                // 获取工单明细数据
                var workorderDetails = await workorderdetailService.GetWorkorderDetailsAsync(item.Id);
                if (workorderDetails.Any())
                {

                    item.workorderdetail = workorderDetails;

                    // 获取明细ID列表
                    var detailIds = workorderDetails.Select(d => d.Id).ToList();

                    // Step 3: 获取采集结果数据
                    var orderResults = await orderresultsService.GetOrderResultsAsync(detailIds);
                    foreach (var detail in workorderDetails)
                    {
                        var ponentfiled = await ponentfiledService.GetAllByIsNotDeleteAndEnabledMarkAsync($"PonentTypeid = {detail.PonetTypeID}");
                        var FiledName = ponentfiled.Where(p => p.IsMain == 1).Select(p => p.FiledName);

                        detail.orderresults = orderResults
                            .Where(r => r.Orderdetailid == detail.Id && FiledName.Contains(r.DisFiled))
                            .ToList();
                    }

                }
            }

            result.ResData = workorder;
            result.ErrCode = ErrCode.successCode;
            return result;
        }

        /// <summary>
        /// 获取工单详情
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <returns></returns>
        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetailsAsync(long id)
        {
            CommonResult result = new CommonResult();

            try
            {
                // Step 1: 获取主工单信息
                var workorderDto = await iService.GetWorkorderAsync(id);
                if (workorderDto == null)
                {
                    result.ErrMsg = "工单不存在";
                    result.ErrCode = "404";
                    return ToJsonContent(result);
                }

                // Step 2: 获取工单明细数据
                var workorderDetails = await workorderdetailService.GetWorkorderDetailsAsync(id);
                if (workorderDetails.Any())
                {
                    workorderDto.workorderdetail = workorderDetails;

                    // 部件类型名称拼接
                    workorderDto.PonetTypeNames = string.Join(", ", workorderDetails.Select(d => d.PonetTypeName));

                    // 获取明细ID列表
                    var detailIds = workorderDetails.Select(d => d.Id).ToList();

                    // Step 3: 获取采集结果数据
                    var orderResults = await orderresultsService.GetOrderResultsAsync(detailIds);
                    foreach (var detail in workorderDto.workorderdetail)
                    {
                        detail.orderresults = orderResults
                            .Where(r => r.Orderdetailid == detail.Id)
                            .ToList();
                    }
                }

                result.ResData = workorderDto;
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            catch (Exception ex)
            {
                result.ErrMsg = $"获取详情失败: {ex.Message}";
                result.ErrCode = "43002";
                // 可在此处添加日志记录
            }

            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工单id编号
        /// </summary>
        /// <returns>工单编号</returns>
        [HttpGet("GetWorkorderNumber")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWorkorderNumber()
        {
            CommonResult result = new CommonResult();
            string orderNo = await sequenceService.GetSequenceNextTask("WrokOrderNumber");
            var orderid = IdGeneratorHelper.IdSnowflake();
            if (!string.IsNullOrEmpty(orderNo))
            {
                result.ResData = new { orderid=orderid, orderNo = orderNo };
                result.ErrCode = ErrCode.successCode;
                result.ErrMsg = ErrCode.err0;
            }
            else
            {
                result.ErrCode = ErrCode.failCode;
                result.ErrMsg = "获取工单编号失败";
            }
            return ToJsonContent(result);
        }

        #region 方法

        #endregion

        private async Task<string> SaveBase64ImageAsync(string base64Image)
        {
            try
            {
                // Step 1: 转换 Base64 为字节数组
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                // Step 2: 创建临时文件
                string tempFileName = Guid.NewGuid() + ".jpg";
                string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                await System.IO.File.WriteAllBytesAsync(tempFilePath, imageBytes);

                // Step 3: 构造 IFormCollection
                var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", tempFileName);
                var formCollection = new FormCollection(new Dictionary<string, StringValues>
                {
                    { "belongApp", "" },
                    { "belongAppId", "" }
                }, new FormFileCollection { formFile });

                // Step 4: 调用 FilesController 的 Upload 方法
                var result = filesController.Upload(formCollection);

                // 检查返回值是否为 ContentResult
                if (result is ContentResult contentResult)
                {
                    // 提取 ContentResult 中的 JSON 内容
                    string content = contentResult.Content;

                    // 反序列化 JSON 内容为 CommonResult
                    var commonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonResult>(content);

                    if (commonResult != null && commonResult.ResData is Newtonsoft.Json.Linq.JObject resDataObject)
                    {
                        // 将 ResData 转换为 UploadFileResultOuputDto
                        var resData = resDataObject.ToObject<UploadFileResultOuputDto>();
                        if (resData != null)
                        {
                            // 返回 FilePath
                            return resData.FilePath;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"图片保存失败: {ex.Message}");
            }

            return null; // 失败返回 null
        }




        // 采集接口调用
        private async Task<Dictionary<string, string>> CallCollectPartyApiAsync(string option, string picture)
        {
            try
            {
                // 构建请求参数
                var requestPayload = new
                {
                    Option = option,
                    Picture = picture
                };

                var client = new HttpClient();

                // 从配置中读取 BaseUrl
                var baseUrl = configuration["CollectApiConfig:BaseUrl"];
                // 构建查询字符串
                var queryParams = $"?option={Uri.EscapeDataString(option)}&picture={Uri.EscapeDataString(picture)}";
                var requestUrl = $"{baseUrl}{queryParams}";

                // 发送 GET 请求
                var response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    // 使用键值对形式处理返回结果
                    var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                // 记录日志
                Console.WriteLine($"调用第三方接口失败: {ex.Message}");
            }

            // 返回空的键值对
            return new Dictionary<string, string>();
        }


    }
}