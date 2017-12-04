﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.JWTAuthorizePolicy;
using System.Data;
using System.Data.SqlClient;
using HisAPI.Model.Repository;
using HisAPI.Model.DataModel;

namespace HisAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {  
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
            services.AddOcelotPolicyJwtBearer(audienceConfig["Issuer"], audienceConfig["Issuer"], audienceConfig["Secret"], "GSWBearer", "Permission", "/hisapi/denied");

            //这个集合模拟用户权限表,可从数据库中查询出来
            var permission = new List<Permission> {                            
                              new Permission {  Url="/hisapi/hisuser", Name="system"},
                              new Permission {  Url="/", Name="system"}
                          };
            services.AddSingleton(permission);
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(connectionString);
            //sqlieconnection注放
            services.AddScoped<IDbConnection, SqlConnection>();
            services.AddScoped<IHisDB, HisDB>();
            services.AddScoped<IFeeItemRepository, FeeItemRepository>();
            services.AddMvc();
        }

     
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}