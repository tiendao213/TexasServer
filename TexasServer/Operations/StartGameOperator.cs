using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using TexasServerCommon.Codes;
using TexasServerCommon.MessageObjects;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Interfaces.Messaging;

namespace VuiLen.TexasServer.Operations
{
    class StartGameOperator : Operation
    {
        public StartGameOperator(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public StartGameOperator()
        {

        }

        [DataMember(Code = (byte)ServerParameterCode.PeerId, IsOptional = false)]
        public Byte[] PeerId { get; set; }


        [DataMember(Code = (byte)RoomParameterCode.RoomID, IsOptional = false)]
        public string RoomID { get; set; }

        [DataMember(Code = (byte)ParameterCode.TexasStart, IsOptional = false)]
        public string TexasStartGame { get; set; }
    }
}
