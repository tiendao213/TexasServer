
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using TexasServerCommon.Codes;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Interfaces.Messaging;

namespace VuiLen.TexasServer.Operations
{
    class SitOperator : Operation
    {
        public SitOperator(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public SitOperator()
        {

        }

        [DataMember(Code = (byte)ServerParameterCode.PeerId, IsOptional = false)]
        public Byte[] PeerId { get; set; }


        [DataMember(Code = (byte)RoomParameterCode.RoomID, IsOptional = false)]
        public string RoomID { get; set; }

        [DataMember(Code = (byte)ParameterCode.TexasSitDown, IsOptional = false)]
        public string TexasSitDown { get; set; }
    }
}
