using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Queries
{
    public record GetPaymentsQuery(GetPaymentsRequest Request) : IRequest<PageResult<PaymentDto>>;
    
    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PageResult<PaymentDto>>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public GetPaymentsQueryHandler(IAppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<PageResult<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var queryable = _appDbContext.Payments.AsNoTracking();
            var parameters = request.Request;
            if (parameters.ShopperId != null)
            {
                queryable = queryable.Where(x => x.ShopperId == request.Request.ShopperId);
            }
            
            queryable = OrderBy(queryable, parameters.OrderBy, parameters.OrderByDescending);

            var count = await queryable.CountAsync(cancellationToken);

            queryable = queryable.Take(parameters.Top)
                .Skip(parameters.Skip);

            var payments = await queryable.ToListAsync(cancellationToken);

            return new PageResult<PaymentDto>
            {
                Records = _mapper.Map<List<PaymentDto>>(payments),
                Skip = parameters.Skip,
                Top = parameters.Top,
                Total = count
            };
        }
        
        private static IQueryable<T> OrderBy<T>(IQueryable<T> source, string orderByProperty, bool desc)
        {
            var type = typeof(T);
            var property = type.GetProperty(orderByProperty,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return source;
            }

            var command = desc ? "OrderByDescending" : "OrderBy";
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}