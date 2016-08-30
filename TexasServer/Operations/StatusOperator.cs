

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Interfaces.Messaging;

namespace VuiLen.TexasServer.Operations
{
    public class StatusOperator : Operation
    {
        public StatusOperator(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public StatusOperator()
        {

        }

        [DataMember(Code = (byte)ServerParameterCode.PeerId, IsOptional = false)]
        public Byte[] PeerId { get; set; }


        [DataMember(Code = (byte)RoomParameterCode.RoomID, IsOptional = false)]
        public string RoomID { get; set; }
    }
}
