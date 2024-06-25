using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SettlementBookingSystem.Application;
using SettlementBookingSystem.Application.Exceptions;
using SettlementBookingSystem.ProblemDetails;
using System;

namespace SettlementBookingSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();

            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (ctx, ex) => { return false; };
                // Configure problem details per exception type here.
                options.Map<ConflictException>(ex => new ConflictProblemDetails(ex));
                options.Map<ValidationException>(ex => new BadRequestProblemDetails(ex));
                options.Map<AggregateException>(ex =>
                {
                    return ex.InnerException switch
                    {
                        null => new UnhandledExceptionProblemDetails(ex),
                        ValidationException validation => new BadRequestProblemDetails(validation),
                        ConflictException conflict => new ConflictProblemDetails(conflict),
                        _ => new UnhandledExceptionProblemDetails(ex.InnerException),
                    };
                });

                // This must always be last as this will serve to handle unhandled exceptions.
                options.Map<Exception>(ex => new UnhandledExceptionProblemDetails(ex));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SettlementBookingSystem", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SettlementBookingSystem v1"));
            }

            app.UseProblemDetails();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
