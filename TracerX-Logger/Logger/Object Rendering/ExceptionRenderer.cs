using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TracerX {
    /// <summary>
    /// Renders an Exception object and its inner exceptions.  
    /// This includes the key:value pairs in Exception.Data, which
    /// Exception.ToString() does not.  Exception.Message is typically
    /// overridden in derived classes as necessary to render additional information
    /// that is not present in the base Exception class (e.g. ArgumentException.ParamName).
    /// </summary>
    public class ExceptionRenderer : IObjectRenderer {
        public ExceptionRenderer() {}
        /// <summary>
        /// Render the object <paramref name="obj"/> to a string
        /// </summary>
        public void RenderObject(object obj, TextWriter writer) {
            Exception ex = (Exception)obj;

            writer.WriteLine("Exception type: {0}", ex.GetType());
            if (ex.Message != null) writer.WriteLine("Message: {0}", ex.Message);
            if (ex.Source != null) writer.WriteLine("Source: {0}", ex.Source);
            if (ex.StackTrace != null) writer.WriteLine("StackTrace:\n{0}", ex.StackTrace);

            if (ex.Data != null && ex.Data.Count > 0) {
                foreach (DictionaryEntry entry in ex.Data) {
                    writer.WriteLine("{0}: {1}", entry.Key, entry.Value);
                }
            }

            if (ex.InnerException != null) {
                writer.Write("\nInner ");
                // The inner exception type may have a custom renderer.
                RendererMap.FindAndRender(ex.InnerException, writer);                
            }
        }
    }
}
