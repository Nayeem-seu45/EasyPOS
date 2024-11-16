using System.Reflection;
using EasyPOS.Application.Common.Behaviours;
using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPOS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(RequestLoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            //cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(QueryCachingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });


        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ISaleReturnService, SaleReturnService>();


        return services;
    }
}
