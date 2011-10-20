using System;

namespace TrueLib
{
    [Serializable()]
    public class EncryptedContainerFile : EncryptedMedia
    {
        /// <summary>
        /// Local file name of the container file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Creates a new container file and assignes a name.
        /// </summary>
        /// <param name="fileName">The local file name.</param>
        public EncryptedContainerFile(string fileName)
            : base()
        {
            this.FileName = fileName;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(EncryptedContainerFile))
            {
                EncryptedContainerFile ecf = (EncryptedContainerFile)obj;
                if (ecf.FileName.Equals(this.FileName))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
