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
    internal sealed class MapEntriesWithPredicateCodec
    {

        public static readonly MapMessageType RequestType = MapMessageType.MapEntriesWithPredicate;
        public const int ResponseType = 117;
        public const bool Retryable = false;

        //************************ REQUEST *************************//

        public class RequestParameters
        {
            public static readonly MapMessageType TYPE = RequestType;
            public string name;
            public IData predicate;

            public static int CalculateDataSize(string name, IData predicate)
            {
                int dataSize = ClientMessage.HeaderSize;
                dataSize += ParameterUtil.CalculateDataSize(name);
                dataSize += ParameterUtil.CalculateDataSize(predicate);
                return dataSize;
            }
        }

        public static ClientMessage EncodeRequest(string name, IData predicate)
        {
            int requiredDataSize = RequestParameters.CalculateDataSize(name, predicate);
            ClientMessage clientMessage = ClientMessage.CreateForEncode(requiredDataSize);
            clientMessage.SetMessageType((int)RequestType);
            clientMessage.SetRetryable(Retryable);
            clientMessage.Set(name);
            clientMessage.Set(predicate);
            clientMessage.UpdateFrameLength();
            return clientMessage;
        }

        //************************ RESPONSE *************************//


        public class ResponseParameters
        {
            public IList<KeyValuePair<IData, IData>> entrySet;
        }

        public static ResponseParameters DecodeResponse(IClientMessage clientMessage)
        {
            ResponseParameters parameters = new ResponseParameters();
            IList<KeyValuePair<IData, IData>> entrySet = null;
            int entrySet_size = clientMessage.GetInt();
            entrySet = new List<KeyValuePair<IData, IData>>();
            for (int entrySet_index = 0; entrySet_index < entrySet_size; entrySet_index++)
            {
                KeyValuePair<IData, IData> entrySet_item;
                entrySet_item = clientMessage.GetMapEntry();
                entrySet.Add(entrySet_item);
            }
            parameters.entrySet = entrySet;
            return parameters;
        }

    }
}
