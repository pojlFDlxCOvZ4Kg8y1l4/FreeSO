﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSO LoginServer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace TSO_LoginServer.Network
{
    /// <summary>
    /// Holds information about a connected CityServer.
    /// This information is relayed to a client when it creates a new character.
    /// </summary>
    public class CityServerInfo
    {
        private string m_Name;
        private string m_Description;
        private ulong m_Thumbnail;
        private string m_IP;
        //This is NOT the port that the CityServer uses to communicate with the LoginServer!
        private int m_Port;

        /// <summary>
        /// The name of the city that this server represents.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// The description of this city, as visible by the client in the city selection dialog.
        /// </summary>
        public string Description
        {
            get { return m_Description; }
        }

        /// <summary>
        /// The ID of the thumbnail image for this city, as visible by the client in the city selection dialog.
        /// </summary>
        public ulong Thumbnail
        {
            get { return m_Thumbnail; }
        }

        /// <summary>
        /// The IP of this server that clients are supposed to connect to.
        /// </summary>
        public string IP
        {
            get { return m_IP; }
        }

        /// <summary>
        /// The port that clients are supposed to use to communicate with this CityServer.
        /// This is NOT the port that the CityServer uses to communicate with the LoginServer!
        /// </summary>
        public int Port
        {
            get { return m_Port; }
        }

        public CityServerInfo(string Name, string Description, ulong Thumbnail, string IP, int Port)
        {
            m_Name = Name;
            m_Description = Description;
            m_Thumbnail = Thumbnail;
            m_IP = IP;
            m_Port = Port;
        }
    }
}
