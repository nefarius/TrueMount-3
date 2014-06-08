using System;
using System.IO;
using TrueLib.Remote;
using TrueLib.Exceptions;
using TrueLib.Events;

namespace TrueLib
{
    [Serializable()]
    public class Password : KeyItem
    {
        private PasswordType type;
        private string _staticPassword;
        public string StaticPassword
        {
            get
            {
                if (type.Equals(PasswordType.Static))
                {
                    return _staticPassword;
                }
                else
                {
                    throw new StaticPasswordException("No static password defined.");
                }
            }
            set
            {
                type = PasswordType.Static;
                _staticPassword = value;
            }
        }
        public int Index { get; set; }
        public int Count { get; set; }
        public delegate string PasswordPromptEventHandler(object sender,
            PasswordPromptEventArgs e);
        public event PasswordPromptEventHandler OnPasswordPromptHandler;

        protected string OnPasswordPrompt()
        {
            string returnValue = string.Empty;
            if (OnPasswordPromptHandler != null)
            {
                returnValue = OnPasswordPromptHandler(this, new PasswordPromptEventArgs());
            }
            return returnValue;
        }

        public Password(PasswordType type)
        {
            this.type = type;
        }

        public string PlainPassword
        {
            get
            {
                switch (type)
                {
                    case PasswordType.File:
                        // return content of password file
                        using (StreamReader pws =
                            new StreamReader(this.CachedFileName, System.Text.Encoding.UTF8))
                        {
                            if (Index == 0 && Count == 0)
                            {
                                return pws.ReadLine();
                            }
                            else
                            {
                                char[] buffer = new char[Count];
                                pws.Read(buffer, Index, Count);
                                return buffer.ToString();
                            }
                        }
                    case PasswordType.Prompt:
                        return OnPasswordPrompt();
                    case PasswordType.Static:
                        return StaticPassword;
                    default:
                        return string.Empty;
                }
            }
        }
    }

    public enum PasswordType
    {
        File,
        Prompt,
        Static
    }
}
