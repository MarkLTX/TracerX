#region Copyright & License
// This is a modified copy of a file in the Apache Log4Net project.
// The primary change was to the namespace.
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

using System;
using System.IO;

namespace TracerX {
    /// <summary>
    /// Maps object types to <see cref="IObjectRenderer"/>s that render objects of each registered
    /// type as strings for logging.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Maintains a mapping between types that require special
    /// rendering and the <see cref="IObjectRenderer"/> that
    /// is used to render them.  Users should implement IObjectRenderer as required
    /// for types where ToString() is insufficient and add the IObjectRenderer objects
    /// to the RendererMap.  
    /// </para>
    /// <para>
    /// The <see cref="FindAndRender(object)"/> method is called by TracerX to render an
    /// <c>object</c> using the appropriate renderers defined in this map.
    /// </para>
    /// </remarks>
    public static class RendererMap {
        #region Member Variables

        private static System.Collections.Hashtable m_map;
        private static System.Collections.Hashtable m_cache = new System.Collections.Hashtable();

        private static IObjectRenderer s_defaultRenderer = new DefaultRenderer();

        #endregion

        #region Constructors

        static RendererMap() {
            m_map = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
            Put(typeof(System.Exception), new ExceptionRenderer());
        }

        #endregion

        /// <summary>
        /// Render <paramref name="obj"/> using the appropriate renderer.
        /// </summary>
        /// <param name="obj">the object to render to a string</param>
        /// <returns>the object rendered as a string</returns>
        /// <remarks>
        /// <para>
        /// This is a convenience method used to render an object to a string.
        /// The alternative method <see cref="FindAndRender(object,TextWriter)"/>
        /// should be used when streaming output to a <see cref="TextWriter"/>.
        /// </para>
        /// </remarks>
        public static string FindAndRender(object obj) {
            // Optimization for strings
            string strData = obj as String;
            if (strData != null) {
                return strData;
            }

            StringWriter stringWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            FindAndRender(obj, stringWriter);
            return stringWriter.ToString();
        }

        /// <summary>
        /// Render <paramref name="obj"/> using the appropriate renderer.
        /// </summary>
        /// <param name="obj">the object to render to a string</param>
        /// <param name="writer">The writer to render to</param>
        /// <remarks>
        /// <para>
        /// Find the appropriate renderer for the type of the
        /// <paramref name="obj"/> parameter. This is accomplished by calling the
        /// <see cref="Get(Type)"/> method. Once a renderer is found, it is
        /// applied on the object <paramref name="obj"/> and the result is returned
        /// as a <see cref="string"/>.
        /// </para>
        /// </remarks>
        public static void FindAndRender(object obj, TextWriter writer) {
            if (obj == null) {
                writer.Write("<null>");
            } else {
                // Optimization for strings
                string str = obj as string;
                if (str != null) {
                    writer.Write(str);
                } else {
                    // Lookup the renderer for the specific type
                    try {
                        Get(obj.GetType()).RenderObject(obj, writer);
                    } catch (Exception ex) {
                        // return default message
                        string objectTypeName = "";
                        if (obj != null && obj.GetType() != null) {
                            objectTypeName = obj.GetType().FullName;
                        }

                        writer.Write("<TracerX.Error>Exception rendering object type [" + objectTypeName + "]");
                        if (ex != null) {
                            string exceptionText = null;

                            try {
                                exceptionText = ex.ToString();
                            } catch {
                                // Ignore exception
                            }

                            writer.Write("<stackTrace>" + exceptionText + "</stackTrace>");
                        }
                        writer.Write("</TracerX.Error>");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the renderer for the specified object type
        /// </summary>
        /// <param name="obj">the object to lookup the renderer for</param>
        /// <returns>the renderer for <paramref name="obj"/></returns>
        /// <remarks>
        /// <param>
        /// Gets the renderer for the specified object type.
        /// </param>
        /// <param>
        /// Syntactic sugar method that calls <see cref="Get(Type)"/> 
        /// with the type of the object parameter.
        /// </param>
        /// </remarks>
        public static IObjectRenderer Get(Object obj) {
            if (obj == null) {
                return null;
            } else {
                return Get(obj.GetType());
            }
        }

        /// <summary>
        /// Gets the renderer for the specified type
        /// </summary>
        /// <param name="type">the type to lookup the renderer for</param>
        /// <returns>the renderer for the specified type</returns>
        /// <remarks>
        /// <para>
        /// Returns the renderer for the specified type.
        /// If no specific renderer has been defined the
        /// <see cref="DefaultRenderer"/> will be returned.
        /// </para>
        /// </remarks>
        public static IObjectRenderer Get(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            IObjectRenderer result = null;

            // Check cache
            result = (IObjectRenderer)m_cache[type];

            if (result == null) {
                for (Type cur = type; cur != null; cur = cur.BaseType) {
                    // Search the type's interfaces
                    result = SearchTypeAndInterfaces(cur);
                    if (result != null) {
                        break;
                    }
                }

                // if not set then use the default renderer
                if (result == null) {
                    result = s_defaultRenderer;
                }

                // Add to cache
                m_cache[type] = result;
            }

            return result;
        }

        /// <summary>
        /// Internal function to recursively search interfaces
        /// </summary>
        /// <param name="type">the type to lookup the renderer for</param>
        /// <returns>the renderer for the specified type</returns>
        private static IObjectRenderer SearchTypeAndInterfaces(Type type) {
            IObjectRenderer r = (IObjectRenderer)m_map[type];
            if (r != null) {
                return r;
            } else {
                foreach (Type t in type.GetInterfaces()) {
                    r = SearchTypeAndInterfaces(t);
                    if (r != null) {
                        return r;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the default renderer instance
        /// </summary>
        /// <value>the default renderer</value>
        /// <remarks>
        /// <para>
        /// Get the default renderer
        /// </para>
        /// </remarks>
        public static IObjectRenderer DefaultRenderer {
            get { return s_defaultRenderer; }
        }

        /// <summary>
        /// Clear the map of renderers
        /// </summary>
        /// <remarks>
        /// <para>
        /// Clear the custom renderers defined by using
        /// <see cref="Put"/>. The <see cref="DefaultRenderer"/>
        /// cannot be removed.
        /// </para>
        /// </remarks>
        public static void Clear() {
            m_map.Clear();
            m_cache.Clear();
        }

        /// <summary>
        /// Register an <see cref="IObjectRenderer"/> for <paramref name="typeToRender"/>. 
        /// </summary>
        /// <param name="typeToRender">the type that will be rendered by <paramref name="renderer"/></param>
        /// <param name="renderer">the renderer for <paramref name="typeToRender"/></param>
        /// <remarks>
        /// <para>
        /// Register an object renderer for a specific source type.
        /// This renderer will be returned from a call to <see cref="Get(Type)"/>
        /// specifying the same <paramref name="typeToRender"/> as an argument.
        /// </para>
        /// </remarks>
        public static void Put(Type typeToRender, IObjectRenderer renderer) {
            m_cache.Clear();

            if (typeToRender == null) {
                throw new ArgumentNullException("typeToRender");
            }
            if (renderer == null) {
                throw new ArgumentNullException("renderer");
            }

            m_map[typeToRender] = renderer;
        }
    }
}
