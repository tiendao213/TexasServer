using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Interfaces.Messaging;

namespace VuiLen.TexasServer.Operations
{
    public class WithdrawRoomOperator : Operation
    {
        public WithdrawRoomOperator(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }
        public WithdrawRoomOperator() { }

        [DataMember(Code = (byte)ServerParameterCode.PeerId, IsOptional = false)]
        public Byte[] PeerId { get; set; }

        [DataMember(Code = (byte)RoomParameterCode.RoomID, IsOptional = false)]
        public string RoomID { get; set; }

        [DataMember(Code = (byte)RoomParameterCode.WithdrawAmount, IsOptional = false)]
        public long Amount { get; set; }

    }
}
