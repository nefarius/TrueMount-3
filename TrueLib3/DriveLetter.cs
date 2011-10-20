using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrueLib
{
    /// <summary>
    /// The possible drive letter types.
    /// </summary>
    public enum DriveLetterType
    {
        Fixed,
        NextFree,
        RandomFree
    }

    [Serializable()]
    public class DriveLetter
    {
        public DriveLetterType Type { get; set; }

        private string _Letter = string.Empty;
        public string Letter
        {
            get
            {
                if (this.Type.Equals(DriveLetterType.Fixed))
                    return _Letter;
                else
                    return string.Empty;
            }

            set
            {
                if (!this.Type.Equals(DriveLetterType.Fixed))
                    throw new InvalidOperationException(
                        "You have to set the type to 'Fixed' if you want a static letter.");
                else
                    _Letter = value;
            }
        }

        /// <summary>
        /// Creates a new drive letter.
        /// </summary>
        /// <param name="type">The drive letter type.</param>
        public DriveLetter(DriveLetterType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Creates a new drive letter and assigned one.
        /// </summary>
        /// <param name="type">The drive letter type.</param>
        /// <param name="letter">The drive letter character.</param>
        public DriveLetter(DriveLetterType type, string letter)
            : this(type)
        {
            this._Letter = letter;
        }

        /// <summary>
        /// Contains all available drive letters (gets refreshed every call).
        /// </summary>
        public static List<string> FreeDriveLetters
        {
            get
            {
                List<string> alphabet = AllDriveLetters;

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                    alphabet.Remove(drive.Name.Substring(0, 1).ToUpper());

                if (alphabet.Count > 0)
                    return alphabet;
                else
                    return null;
            }
        }

        /// <summary>
        /// Contains all drive letters.
        /// </summary>
        public static List<string> AllDriveLetters
        {
            get
            {
                List<string> alphabet = new List<string>();
                int lowerBound = Convert.ToInt16('C');
                int upperBound = Convert.ToInt16('Z');
                int index = 0;

                for (index = lowerBound; index <= upperBound; index++)
                {
                    char driveLetter = (char)index;
                    alphabet.Add(driveLetter.ToString());
                }

                return alphabet;
            }
        }

        /// <summary>
        /// Contains a random free letter.
        /// </summary>
        public static string RandomFreeDriveLetter
        {
            get
            {
                Random r = new Random();
                List<string> dLetters = FreeDriveLetters;
                return dLetters[r.Next(dLetters.Count)];
            }
        }
    }
}
