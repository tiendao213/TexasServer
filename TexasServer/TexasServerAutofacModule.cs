using Autofac;
using VuiLen.Texas.Game.Controllers.Room;
using VuiLen.TexasServer.Handlers;
using VuiLenGameFramework.BackgroundThreads;
using VuiLenGameFramework.Handlers;

namespace VuiLen.TexasServer
{
    public class TexasServerAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<TexasBuyinRoomHandler>().AsImplementedInterfaces();

            builder.RegisterType<TexasStartGameHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasStatusHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasSitHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasBuyinSuccessHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasDepositSuccessHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasBetHandler>().AsImplementedInterfaces();
            

            builder.RegisterType<InfoRoomHandler>().AsImplementedInterfaces();
            builder.RegisterType<UserListRoomHandler>().AsImplementedInterfaces();
            builder.RegisterType<JoinRoomHandler>().AsImplementedInterfaces();
            builder.RegisterType<JoinRoomRequestHandler>().AsImplementedInterfaces();
            builder.RegisterType<LeaveRoomHandler>().AsImplementedInterfaces();
            builder.RegisterType<ListRoomHandler>().AsImplementedInterfaces();

            builder.RegisterType<RemoveAllRoomHandler>().AsImplementedInterfaces();
            builder.RegisterType<RemoveRoomHandler>().AsImplementedInterfaces();


            builder.RegisterType<ClientDisconnectHandler>().AsImplementedInterfaces();
            builder.RegisterType<ClientConnectHandler>().AsImplementedInterfaces();
            builder.RegisterType<ClientLoginHandler>().AsImplementedInterfaces();
            builder.RegisterType<ClientLogoutHandler>().AsImplementedInterfaces();
            builder.RegisterType<ClientUpdateHandler>().AsImplementedInterfaces();




            builder.RegisterType<WithdrawRequestRoomHandler>().AsImplementedInterfaces();

            //builder.RegisterType<WithdrawSuccessRoomHandler>().AsImplementedInterfaces();


            // builder.RegisterType<ClientDataList>().AsImplementedInterfaces().SingleInstance();


            builder.RegisterType<ChatRoomHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RoomBackgroundThread>().AsImplementedInterfaces();

            /*
            builder.RegisterType<CreateRoomTexasHandler>().AsImplementedInterfaces();



            builder.RegisterType<TexasUserStatusHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasUserListHandler>().AsImplementedInterfaces();

            builder.RegisterType<TexasStatusHandler>().AsImplementedInterfaces();

            builder.RegisterType<TexasBetHandler>().AsImplementedInterfaces();

            builder.RegisterType<TexasMoveHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasDealHandler>().AsImplementedInterfaces();


            builder.RegisterType<TexasBackgroundThread>().AsImplementedInterfaces();


            builder.RegisterType<TexasBuyinSuccessHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasDepositSuccessHandler>().AsImplementedInterfaces();

            builder.RegisterType<TexasSetHandler>().AsImplementedInterfaces();
            builder.RegisterType<TexasSetServerDealerCreditHandler>().AsImplementedInterfaces();
            */



        }
    }
}
