using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mywebsite.Startup))]
namespace Mywebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
