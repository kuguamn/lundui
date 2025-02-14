using AutoMapper;
using Yuebon.WCINS.Models;

namespace Yuebon.WCINS.Dtos
{
    public class WCINSProfile : Profile
    {
        public WCINSProfile()
        {
           CreateMap<Componentmodel, ComponentmodelOutputDto>();
           CreateMap<ComponentmodelInputDto, Componentmodel>();
           CreateMap<Orderresults, OrderresultsOutputDto>();
           CreateMap<OrderresultsInputDto, Orderresults>();
           CreateMap<Ponentfiled, PonentfiledOutputDto>();
           CreateMap<PonentfiledInputDto, Ponentfiled>();
           CreateMap<Ponenttype, PonenttypeOutputDto>();
           CreateMap<PonenttypeInputDto, Ponenttype>();
           CreateMap<Workorder, WorkorderOutputDto>();
           CreateMap<WorkorderInputDto, Workorder>();
           CreateMap<Workorderdetail, WorkorderdetailOutputDto>();
           CreateMap<WorkorderdetailInputDto, Workorderdetail>();

        }
    }
}
