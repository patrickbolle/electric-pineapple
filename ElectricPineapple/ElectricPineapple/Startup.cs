using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElectricPineapple.Startup))]
namespace ElectricPineapple
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
