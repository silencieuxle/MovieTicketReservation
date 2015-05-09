using Owin;

namespace MovieTicketReservation.App_Start {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}
