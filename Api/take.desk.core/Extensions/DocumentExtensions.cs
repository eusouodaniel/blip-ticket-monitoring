using Lime.Protocol;

namespace take.desk.core.Extensions
{
    public static class DocumentExtensions
    {
        /// <summary>
        /// Extension that puts the given Document inside a DocumentContainer.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static DocumentContainer ToDocumentContainer(this Document document)
        {
            return new DocumentContainer { Value = document };
        }
    }
}
