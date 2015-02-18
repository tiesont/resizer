using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;
using System.IO;
using System.Xml;
using ImageResizer.Util;
using System.Threading;
using System.Security.AccessControl;

namespace ImageResizer.Plugins.Security.Cryptography
{
    
    public class AppDataKeyProvider : ISecretKeyProvider
    {

        private string _keyFilePath;
        /// <summary>
        /// The physical path to the keys file
        /// </summary>
        public string KeyFilePath
        {
            get
            {
                if (_keyFilePath == null) _keyFilePath = HostingEnvironment.MapPath("~/App_Data/encryption-keys.config");
                return _keyFilePath;
            }
        }


        //Basically a cache. We assume keys don't change values. On cache miss, we do an atomic add. We never modify the dictionary, we replace it.
        private Dictionary<string, byte[]> keys = null;


        private Dictionary<string, byte[]> Read(Stream input)
        {
            var k = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

            if (input.Length == 0) return k;

            var s = new XmlReaderSettings();
            s.ValidationType = ValidationType.None;
            s.CloseInput = false;
            using (XmlReader r = XmlReader.Create(input, s))
            {
                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        if (r.Name == "key")
                        {
                            r.MoveToAttribute("name");
                            string name = r.Value;
                            r.MoveToContent();
                            k.Add(name, PathUtils.FromBase64UToBytes(r.Value));
                        }
                    }
                }
            }
            return k;
        }

        private void Write(Dictionary<string, byte[]> data, Stream output)
        {
            using (XmlWriter w = XmlWriter.Create(output, new XmlWriterSettings()))
            {
                w.WriteStartDocument();
                w.WriteStartElement("keys");
                foreach (KeyValuePair<string, byte[]> p in data)
                {
                    w.WriteStartElement("key");
                    w.WriteAttributeString("name", p.Key);
                    w.WriteValue(Util.PathUtils.ToBase64U(p.Value));
                    w.WriteEndElement();
                }
                w.WriteEndDocument();
                w.Close();
                w.Flush();
            }
        }

        private void AtomicAddRetrying(string name, int sizeInBytes, int maxTries = 6)
        {
            int numTries = 0;
            while (true)
            {
                bool lastTry = numTries + 1 >= maxTries;
                if (lastTry)
                {
                    AtomicAdd(name, sizeInBytes);
                    return;
                }
                else
                {
                    try
                    {
                        AtomicAdd(name, sizeInBytes);
                        return;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(50);
                    }
                }
                numTries++;
            }
        }

        private static object domainWideLock = new object();
        private void AtomicAdd(string name, int sizeInBytes)
        {
            string lookup = name + "_" + sizeInBytes;

            //A process-wide lock is a much cheaper conflict resolution that a file lock with retrying. This should ensure retrying only happens with web gardens or external actors.
            //How many separate encryption key files do you expect to need, per application? If we're talking hundreds, then it makes sense to use a string-based locking system. In practice, I see 1 or 2, thus the static lock.
            lock (domainWideLock)
            {
                using (var s = File.Open(KeyFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    var current = Read(s);
                    if (current.ContainsKey(lookup) == false)
                    {
                        current[lookup] = GenerateKey(sizeInBytes);
                        s.Seek(0, SeekOrigin.Begin);
                        Write(current, s);
                        this.keys = current;
                    }
                    this.keys = current;
                }
            }
        }

        private byte[] GenerateKey(int sizeInBytes)
        {
            //Generate and insert if missing
            var key = new byte[sizeInBytes];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(key);
            return key;
        }

        public byte[] GetKey(string name, int sizeInBytes)
        {

            if (keys != null)
            {
                string lookup = name + "_" + sizeInBytes;
                byte[] key;
                if (keys.TryGetValue(lookup, out key))
                {
                    return key;
                }
            }

            //We didn't find the key. Add it, sync, and try again.
            AtomicAddRetrying(name, sizeInBytes);
            return GetKey(name, sizeInBytes);
        }



    }

}