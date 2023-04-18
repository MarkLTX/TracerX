using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace TracerX
{
    /// <summary>
    /// Renders an Exception object and its inner exceptions recursively.  
    /// Most public properties are rendered, including the exception's type,
    /// Message, Source, StackTrace, the key/value pairs of Exception.Data,
    /// and other properties discovered by reflection.
    /// </summary>
    internal class ExceptionRenderer : IObjectRenderer
    {
        public ExceptionRenderer() { }
        /// <summary>
        /// Renders the object <paramref name="obj"/>, which must be derived from System.Exception, to a string.
        /// </summary>
        /// <param name="obj">An Exception to be rendered as text.</param>
        /// <param name="writer">TextWriter to which the rendered text is written.</param>
        public void RenderObject(object obj, TextWriter writer)
        {
            Exception ex = (Exception)obj;

            writer.WriteLine("Exception type: {0}", ex.GetType());
            if (ex.Message != null) writer.WriteLine("Message: {0}", ex.Message);
            if (ex.Source != null) writer.WriteLine("Source: {0}", ex.Source);
            if (ex.StackTrace != null) writer.WriteLine("StackTrace:\n{0}", ex.StackTrace);
            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry entry in ex.Data)
                {
                    if (entry.Key != null && entry.Value != null)
                    {
                        writer.WriteLine("Data Key {0}: {1}", entry.Key, entry.Value);
                    }
                }
            }

            AddProperties(obj, writer);

            if (ex.InnerException != null)
            {
                writer.Write("\nInner ");
                // The inner exception type may have a custom renderer.
                RendererMap.FindAndRender(ex.InnerException, writer);
            }
        }

        private static void AddProperties(object obj, TextWriter writer)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo info in properties)
            {
                // Skip Exception properties that get explicitly processed.

                if (!(obj is Exception) ||
                    (info.Name != "InnerException" &&
                    info.Name != "Data" &&
                    info.Name != "Message" &&
                    info.Name != "Source" &&
                    info.Name != "StackTrace"))
                {
                    RenderProperty(obj, info, writer);
                }
            }
        }

        private static void RenderProperty(object obj, PropertyInfo info, TextWriter writer)
        {
            var value = info.GetValue(obj, null);

            if (value != null)
            {
                if (value is string)
                {
                    if ((value as string) != "") writer.WriteLine("{0}: {1}", info.Name, value);
                }
                else if (value is IDictionary)
                {
                    RenderDictionary(info.Name, value as IDictionary, writer);
                }
                else if (value is IEnumerable && !(value is string))
                {
                    RenderEnumerable(info.Name, value as IEnumerable, writer);
                }
                else
                {
                    writer.WriteLine("{0}: {1}", info.Name, value);
                }
            }
        }
        
        static void RenderEnumerable(string listName, IEnumerable list, TextWriter writer)
        {
            foreach (object elem in list)
            {
                if (elem != null) writer.WriteLine("{0} element: {1}", listName, elem);
            }
        }

        static void RenderDictionary(string dictName, IDictionary data, TextWriter writer)
        {
            foreach (DictionaryEntry entry in data)
            {
                if (entry.Key != null && entry.Value != null)
                {
                    writer.WriteLine("{0} Key {1}: {2}", dictName, entry.Key, entry.Value);
                }
            }
        }

    }
}
