using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeetingCatalogue.Startup))]
namespace MeetingCatalogue
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
