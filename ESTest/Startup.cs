using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ESTest.Startup))]
namespace ESTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
