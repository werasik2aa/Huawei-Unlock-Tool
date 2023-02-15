using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using static HuaweiUnlocker.LangProc;
using System.Xml;
using HuaweiUnlocker.TOOLS;
using System.IO.Ports;

namespace HuaweiUnlocker.FlashTool
{
    public class FlashToolHisi
    {
        public static void FlashBootloader(Bootloader bootloader, string port)
        {
            var flasher = new ImageFlasher();

            LOG("Verifying images...");

            int asize = 0, dsize = 0;

            foreach (var image in bootloader.Images)
            {
                if (!image.IsValid)
                {
                    LOG("Image " + image.Role + "is not valid!");
                    return;
                }

                asize += image.Size;
            }

            if (debug) LOG($"Opening: " + port);

            SerialManager.Open(port, 115200, false);

            LOG("Uploading bootloader: " + bootloader.Name);

            foreach (var image in bootloader.Images)
            {
                var size = image.Size;

                LOG("-" + image.Role);

                flasher.Write(image.Path, (int)image.Address, x => {
                    Action action = () => progr.Value = int.Parse((dsize + (int)(size / 100f * x), asize).ToString());
                    if (LOGGBOX.InvokeRequired)
                        LOGGBOX.Invoke(action);
                    else
                        action();
                });

                dsize += size;
            }

            SerialManager.CloseSerial();
        }
    }
    public class Bootloader
    {
        public class Image
        {
            private bool? valid = null;
            private int? size = null;
            public string Path { get; }
            public string Role { get; }
            public uint Address { get; }
            private string Hash { get; }
            public bool IsValid { get => valid ?? Validate(); }
            public int Size { get => size ?? GetSize(); }
            private bool Validate()
            {
                if (Hash == null)
                    return true;

                using (var stream = File.OpenRead(Path))
                {
                    using (var sha1 = SHA1.Create())
                    {
                        stream.Position = 0;
                        byte[] hash = sha1.ComputeHash(stream);
                        stream.Close();
                        valid = BitConverter.ToString(hash).Replace("-", "").ToLower() == Hash;
                    }
                }

                return valid.Value;
            }

            private int GetSize()
            {
                size = (int)new FileInfo(Path).Length;
                return size.Value;
            }

            public Image(string path, string role, uint address, string hash = null)
            {
                Path = path;
                Role = role;
                Address = address;
                Hash = hash;
            }
        }
        public Image[] Images;
        public string Title;
        public string Name;
        public Bootloader(string name, string title, Image[] images)
        {
            Title = title;
            Name = name;
            Images = images;
        }
        private static uint ParseAddress(string str) => Convert.ToUInt32(str, str.StartsWith("0x") ? 16 : 10);
        public static Bootloader ParseBootloader(string filename)
        {
            var xml = new XmlDocument();
            xml.Load(filename);

            var root = xml.DocumentElement;

            if (root.Name != "bootloader")
                throw new Exception("XML root name is invalid.");

            var dir = Path.GetDirectoryName(filename);

            var title = root.GetAttribute("name");

            if (string.IsNullOrEmpty(title))
            {
                LOG("Name attribute is invalid!");
                title = "Unknown bootloader";
            }

            var images = new List<Image>();

            foreach (XmlNode node in root)
            {
                bool isBad = node.Name != "image";

                foreach (var key in new[] { "path", "role", "hash", "address" })
                {
                    var item = node.Attributes.GetNamedItem(key);
                    isBad |= item == null || string.IsNullOrWhiteSpace(item.Value);
                }

                if (isBad)
                    throw new Exception("Failed to parse image");

                images.Add(new Image(
                        Path.Combine(dir, node.Attributes.GetNamedItem("path").Value),
                        node.Attributes.GetNamedItem("role").Value,
                        ParseAddress(node.Attributes.GetNamedItem("address").Value),
                        node.Attributes.GetNamedItem("hash").Value));
            }

            return new Bootloader(Path.GetFileName(dir), title, images.ToArray());
        }
    }
}
