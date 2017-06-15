using System;
using System.Runtime.Serialization;

namespace Webpack.NET
{
    /// <summary>
    /// The exception that is thrown when an attempt to get a webpack asset that does not exist fails.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class AssetNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotFoundException"/> class.
        /// </summary>
        public AssetNotFoundException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotFoundException"/> class.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="innerException">The inner exception.</param>
        public AssetNotFoundException(string assetName, string assetType, Exception innerException = null)
            : base($"Asset '{assetName}' with type '{assetType}' could not be found.", innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected AssetNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
