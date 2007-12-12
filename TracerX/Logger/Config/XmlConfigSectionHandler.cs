#region Copyright & License
// This is a modified copy of a file in the Apache Log4Net project.
// The modifications include changes to the namespace and comments.
// The remainder of this comment is taken verbatim from the original Apache file.
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//       
#endregion

using System.Configuration;
using System.Xml;

namespace BBS.TracerX {
	/// <summary>
	/// Class to register for the TracerX section of the app.config file
	/// </summary>
	/// <remarks>
	/// The TracerX section of the configuration file needs to have a section
	/// handler registered. This is the section handler used. It simply returns
	/// the XML element that is the root of the section.
	/// </remarks>
	/// <example>
	/// Example of registering the TracerX section handler :
	/// <code lang="XML" escaped="true">
	/// <configuration>
	///		<configSections>
	///			<section name="TracerX" type="BBS.TracerX.XmlConfigSectionHandler, TracerX" />
	///		</configSections>
	///		<TracerX>
	///			TracerX configuration XML goes here
    ///		</TracerX>
	/// </configuration>
	/// </code>
	/// </example>
	public class XmlConfigSectionHandler : IConfigurationSectionHandler
	{
		#region Public Instance Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigSectionHandler"/> class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default constructor.
		/// </para>
		/// </remarks>
		public XmlConfigSectionHandler()
		{
		}

		#endregion Public Instance Constructors

		#region Implementation of IConfigurationSectionHandler

		/// <summary>
		/// Parses the configuration section.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">The configuration context when called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The <see cref="XmlNode" /> for the TracerX section.</param>
		/// <returns>The <see cref="XmlNode" /> for the TracerX section.</returns>
		/// <remarks>
		/// <para>
		/// Returns the <see cref="XmlNode"/> containing the configuration data,
		/// </para>
		/// </remarks>
		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}

		#endregion Implementation of IConfigurationSectionHandler
	}
}
