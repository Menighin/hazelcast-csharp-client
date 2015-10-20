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

using System.Net;

namespace Hazelcast.Core
{
    /// <summary>
    ///     IClient interface allows to get information about
    ///     a connected clients socket address, type and uuid.
    /// </summary>
    /// <remarks>
    ///     IClient interface allows to get information about
    ///     a connected clients socket address, type and uuid.
    /// </remarks>
    /// <seealso cref="IClientService">IClientService</seealso>
    /// <seealso cref="IClientListener">IClientListener</seealso>
    public interface IClient : IEndpoint
    {
        /// <summary>Returns unique uuid for this client</summary>
        /// <returns>unique uuid for this client</returns>
        string GetUuid();

        /// <summary>Returns socket address of this client</summary>
        /// <returns>socket address of this client</returns>
        IPEndPoint GetSocketAddress();

        /// <summary>Returns type of this client</summary>
        /// <returns>type of this client</returns>
        ClientType GetClientType();
    }
}