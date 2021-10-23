using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {

        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;
        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName).ConfigureAwait(false);

            if (coupon == null)
            {
                throw  new RpcException(new Status(StatusCode.NotFound,$"Discount with ProductName={request.ProductName} not found"));
            }


            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon)
                .ConfigureAwait(false);

            _logger.LogInformation("Discount is successfully created");

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }


        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {

           var deleted = await _repository.DeleteDiscount(request.ProductName)
                .ConfigureAwait(false);

            _logger.LogInformation("Discount is successfully updated");

            return new DeleteDiscountResponse() { Success = deleted };
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {

            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.UpdateDiscount(coupon)
                .ConfigureAwait(false);

            _logger.LogInformation("Discount is successfully updated");

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }
    }
}
