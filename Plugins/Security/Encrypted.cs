using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using ImageResizer.Util;
using ImageResizer.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Web.Security;
using ImageResizer.Configuration.Issues;
using ImageResizer.Plugins.Security;
using System.Diagnostics;
using ImageResizer.Plugins.Security.Cryptography;

namespace ImageResizer.Plugins.Encrypted {
    public class EncryptedPlugin:IssueSink, IPlugin, IMultiInstancePlugin {


        public static EncryptedPlugin First {
            get {
                Config.Current.Plugins.LoadPlugins();
                return Config.Current.Plugins.Get<EncryptedPlugin>();
            }
        }

        /// <summary>
        /// By default, the Encrypted plugin uses the prefix "~/images/enc/", and stores a permanent key in ~/App_Data/encryption-keys.config
        /// </summary>
        public EncryptedPlugin()
            : base("Encrypted plugin") {
                VirtualPrefix = VirtualPrefix;

                this.KeyProvider = new AppDataKeyProvider();
        }

        /// <summary>
        /// By default, the Encrypted plugin stores a separate permanent key in ~/App_Data/encryption-keys.config for each unique prefix.
        /// </summary>
        /// <param name="prefix"></param>
        public EncryptedPlugin(string prefix)
            : base("Encrypted plugin")
        {
            VirtualPrefix = prefix;
            this.KeyProvider = new AppDataKeyProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="keyProvider">An implementation responsible for creating and storing secret keys</param>
        public EncryptedPlugin(string prefix, ISecretKeyProvider keyProvider)
            : base("Encrypted plugin")
        {
            VirtualPrefix = prefix;
            this.KeyProvider = keyProvider;
        }



        /// <summary>
        /// Provide the encryption service directly to the plugin
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="encryptor"></param>
        public EncryptedPlugin(string prefix, ISimpleBlockEncryptionService encryptor)
            : base("Encrypted plugin")
        {
            VirtualPrefix = prefix;
            Encryptor = encryptor;
        }

        /// <summary>
        /// Create an EncryptedPlugin instance, using the provided prefix and encryption key byte aray. 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="keyBytes"></param>
        public EncryptedPlugin(string prefix, byte[] keyBytes)
            : this(prefix, new SimpleSecureEncryption(keyBytes))
        { }
        public EncryptedPlugin(string prefix, string keyString)
            : this(prefix, new SimpleSecureEncryption(keyString))
        { }

        /// <summary>
        /// Used by the web.config parser. use key="external" to specify use of ~/App_Data/encryption-keys.config.
        /// </summary>
        /// <param name="args"></param>
        public EncryptedPlugin(NameValueCollection args)
            : base("Encrypted plugin")
        {
            if (!string.IsNullOrEmpty(args["prefix"])) VirtualPrefix = args["prefix"];
            else VirtualPrefix = VirtualPrefix;

            byte[] _encryptionKey = null;
            if (!string.IsNullOrEmpty(args["key"]))
            {
                _encryptionKey = UTF8Encoding.UTF8.GetBytes(args["key"]);
            }
            if (!"external".Equals(args["key"], StringComparison.OrdinalIgnoreCase) && (_encryptionKey == null || _encryptionKey.Length < 16))
                this.AcceptIssue(new Issue("Please specify key='external' or an encryption key that is at least 16 characters and optimally 32-> <add name='Encrypted' key='[32chars please]' />", IssueSeverity.Critical));

            if (_encryptionKey != null) 
                Encryptor = new SimpleSecureEncryption(_encryptionKey);

            AddedByWebConfig = true;
        }

        

        public ISimpleBlockEncryptionService Encryptor { get; private set; }

        public ISecretKeyProvider KeyProvider { get; private set; }

        private bool AddedByWebConfig = false;


        private string _virtualPrefix = "~/images/enc/";
        /// <summary>
        /// Requests starting with this path will be decrypted. Should be in app-relative form: "~/images/enc/". Will be converted to root-relative form upon assigment. Trailing slash required, auto-added.
        /// </summary>
        public string VirtualPrefix {
            get { return _virtualPrefix; }
            set { if (!value.EndsWith("/")) value += "/"; _virtualPrefix = PathUtils.ResolveAppRelativeAssumeAppRelative(value); }
        }

        Config c;
        public IPlugin Install(Configuration.Config c) {
            this.c = c;
            this.c.Plugins.add_plugin(this);
            c.Pipeline.PostAuthorizeRequestStart += Pipeline_PostAuthorizeRequestStart;
            return this;
        }

        private void EnsureEncryptionAvailable()
        {
            if (Encryptor != null) return;
            if (KeyProvider == null && AddedByWebConfig)
            {
                if (c == null){
                    throw new InvalidOperationException("The Encrypted plugin must be installed before it can be used.");
                }
                KeyProvider = this.c.Plugins.Get<ISecretKeyProvider>();
                
            }
            if (KeyProvider == null)
            {
                KeyProvider = new AppDataKeyProvider(); //If no IKeyProvider is registered, create a new instance.
            }
            Encryptor = new SimpleSecureEncryption(KeyProvider.GetKey("EncryptedPlugin:" + VirtualPrefix, 32));           
        }


        public string EncryptPathAndQuery(string virtualPath, NameValueCollection query) {
            return EncryptPathAndQuery(virtualPath + PathUtils.BuildQueryString(query));
        }
        public string EncryptPathAndQuery(string virtualPathAndQuery) {

            if (!virtualPathAndQuery.StartsWith("~") && !virtualPathAndQuery.StartsWith("/"))
            {
                throw new ArgumentOutOfRangeException("virtualPathAndQuery",virtualPathAndQuery,"Only domain-relative or application-relative paths can be encrypted. Relative paths are not permitted. Call ResolveUrl on your path before passing it to EncryptPathAndQuery.");
            }
            var path = PathUtils.ResolveAppRelative(virtualPathAndQuery);

            return VirtualPrefix.TrimEnd('/') + '/' + Encrypt(path) + ".ashx";

        }

        private string Encrypt(string text) {
            EnsureEncryptionAvailable(); //May cause I/O write
            byte[] iv;
            byte[] data = Encryptor.Encrypt(UTF8Encoding.UTF8.GetBytes(text), out iv);
            return PathUtils.ToBase64U(iv) + '/' + PathUtils.ToBase64U(data);
        }

        void Pipeline_PostAuthorizeRequestStart(System.Web.IHttpModule sender, System.Web.HttpContext context) {
            if (!c.Pipeline.PreRewritePath.StartsWith(VirtualPrefix, StringComparison.OrdinalIgnoreCase)) return;


            EnsureEncryptionAvailable(); //May cause I/O write
            //Okay, decrypt
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string both = c.Pipeline.PreRewritePath.Substring(VirtualPrefix.Length); //Strip prefix
            string[] parts = both.Split('/'); //Split

            if (parts.Length != 2) return; //There must be exactly two parts

            parts[1] = PathUtils.RemoveFullExtension(parts[1]); //Remove the .ashx or .jpg.ashx or whatever it is.

            byte[] iv = PathUtils.FromBase64UToBytes(parts[0]);
            if (iv.Length != Encryptor.BlockSizeInBytes) return; //16-byte IV required
            byte[] data = PathUtils.FromBase64UToBytes(parts[1]);

            string result = UTF8Encoding.UTF8.GetString(Encryptor.Decrypt(data, iv));

            string path;
            string fragment;
            //We do not merge the old and new query strings. We do not accept plaintext additions to an encrypted URL
            c.Pipeline.ModifiedQueryString = PathUtils.ParseQueryString(result, true, out path, out fragment);
            if (!path.StartsWith(PathUtils.AppVirtualPath)) return; //Don't permit rewrites outside of our application path.
            c.Pipeline.PreRewritePath = path;
            sw.Stop();
        }

        public bool Uninstall(Configuration.Config c) {
            c.Plugins.remove_plugin(this);
            c.Pipeline.PostAuthorizeRequestStart -= Pipeline_PostAuthorizeRequestStart;
            return true;
        }



    }


}
