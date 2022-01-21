using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APPCV_CSE.Startup))]
namespace APPCV_CSE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
