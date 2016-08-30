
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using TexasServerCommon.Codes;
using VuiLenServerCommon.Codes;
using VuiLenServerFramework.Interfaces.Messaging;

namespace VuiLen.SicBoServer.Operations
{

    public class BetOperator : Operation
    {
        public BetOperator(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }

        public BetOperator()
        {

        }

        [DataMember(Code = (byte)ServerParameterCode.PeerId, IsOptional = false)]
        public Byte[] PeerId { get; set; }


        [DataMember(Code = (byte)RoomParameterCode.RoomID, IsOptional = false)]
        public string RoomID { get; set; }

        [DataMember(Code = (byte)ParameterCode.TexasBet, IsOptional = false)]
        public string TexasBetRequest { get; set; }


        //[DataMember(Code = (byte)TexasParameterCode.BetItem, IsOptional = false)]
        // public string BetItem { get; set; }

    }
}
