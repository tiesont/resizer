using System;
namespace ImageResizer.Plugins.Security
{
    public interface ISimpleBlockEncryptionService
    {
        int BlockSizeInBytes { get; }
        byte[] Decrypt(byte[] data, byte[] iv);
        byte[] Encrypt(byte[] data, out byte[] iv);
    }
}
