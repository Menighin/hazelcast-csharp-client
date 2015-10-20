/*
* Copyright (c) 2008-2015, Hazelcast, Inc. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using Hazelcast.Client.Protocol;
using Hazelcast.Client.Protocol.Util;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using System.Collections.Generic;

namespace Hazelcast.Client.Protocol.Codec
{
    internal sealed class ClientGetPartitionsCodec
    {

        public static readonly ClientMessageType RequestType = ClientMessageType.ClientGetPartitions;
        public const int ResponseType = 108;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly ClientMessageType TYPE = RequestType;

            public static int CalculateDataSize()
            {
                int dataSize = ClientMessage.HeaderSize;
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest()
        {
            int requiredDataSize = RequestParameters.CalculateDataSize();
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        //************************ RESPONSE *************************//


        public class ResponseParameters
        {
            public IDictionary<Address,ISet<int>> partitions;
        }

        public static ResponseParameters DecodeResponse(IClientMessage clientMessage)
        {
            ResponseParameters parameters = new ResponseParameters();
            IDictionary<Address,ISet<int>> partitions = null;
        int partitions_size = clientMessage.GetInt();
        partitions = new Dictionary<Address,ISet<int>>(partitions_size);
        for (int partitions_index = 0;partitions_index<partitions_size;partitions_index++) {
            Address partitions_key;
            ISet<int> partitions_val;
            partitions_key = AddressCodec.Decode(clientMessage);
            int partitions_val_size = clientMessage.GetInt();
            partitions_val = new HashSet<int>();
            for (int partitions_val_index = 0; partitions_val_index<partitions_val_size; partitions_val_index++) {
                int partitions_val_item;
            partitions_val_item = clientMessage.GetInt();
                partitions_val.Add(partitions_val_item);
            }
            partitions.Add(partitions_key, partitions_val);
        }
            parameters.partitions = partitions;
            return parameters;
        }

    }
}
