using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NienLuan2.Startup))]
namespace NienLuan2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
