#region Copyright & License
// This is a modified copy of a file in the Apache Log4Net project.
// The primary change was to the namespace and comments.
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
using System.IO;

namespace TracerX {
    /// <summary>
    /// Implement this interface and add the new class to the
    /// RendererMap in order to render objects as strings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Certain types require special-case conversion to
    /// string form. This conversion is done by an object renderer.
    /// Object renderers implement the <see cref="IObjectRenderer"/>
    /// interface, which has only one method.
    /// </para>
    /// </remarks>
    public interface IObjectRenderer {
        /// <summary>
        /// Render the object <paramref name="obj"/> to a string.
        /// </summary>
        /// <param name="obj">The object to render</param>
        /// <param name="writer">The writer to render to</param>
        void RenderObject(object obj, TextWriter writer);
    }
}
