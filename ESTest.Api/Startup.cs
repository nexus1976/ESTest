using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ESTest.Api.Startup))]

namespace ESTest.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
