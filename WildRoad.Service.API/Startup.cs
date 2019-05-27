using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using System;
using System.Configuration;
using System.Web.Http;
using WildRoad.Service.API;

[assembly: OwinStartup(typeof(Startup))]
namespace WildRoad.Service.API
{
        public class Startup
        {

            public void Configuration(IAppBuilder app)
            {
                HttpConfiguration config = new HttpConfiguration();
                //ConfigureOAuth(app);

                WebApiConfig.Register(config);
                app.UseCors(CorsOptions.AllowAll);
                app.UseWebApi(config);
                /*if (Utilities.MemoryCache == null)
                    Utilities.InitCache();*/
            }


         
        }

    }