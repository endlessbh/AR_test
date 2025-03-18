using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Net;
using XCSJ.Net.Http;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Kernel
{
    public static class AudioClipHelper
    {
        public static AudioClip LoadFromMemory(byte[] fileBytes, string name = "wav")
        {
            //string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
            //string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
            int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
            UInt16 audioFormat = BitConverter.ToUInt16(fileBytes, 20);

            // NB: Only uncompressed PCM wav files are supported.
            string formatCode = FormatCode(audioFormat);
            Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

            UInt16 channels = BitConverter.ToUInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            //int byteRate = BitConverter.ToInt32 (fileBytes, 28);
            //UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
            UInt16 bitDepth = BitConverter.ToUInt16(fileBytes, 34);

            int headerOffset = 16 + 4 + subchunk1 + 4;
            int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);
            //Debug.LogFormat ("riff={0} wave={1} subchunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subchunk2={10} filesize={11}", riff, wave, subchunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subchunk2, fileBytes.Length);

            float[] data;
            switch (bitDepth)
            {
                case 8:
                    data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 16:
                    data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 24:
                    data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 32:
                    data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                default:
                    throw new Exception(bitDepth + " bit depth is not supported.");
            }

            AudioClip audioClip = AudioClip.Create(name, data.Length, (int)channels, sampleRate, false);
            audioClip.SetData(data, 0);
            return audioClip;
        }

        private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            float[] data = new float[wavSize];

            sbyte maxValue = sbyte.MaxValue;

            int i = 0;
            while (i < wavSize)
            {
                data[i] = (float)source[i] / maxValue;
                ++i;
            }

            return data;
        }

        private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(Int16); // block size = 2
            int convertedSize = wavSize / x;

            float[] data = new float[convertedSize];

            Int16 maxValue = Int16.MaxValue;

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = 3; // block size = 3
            int convertedSize = wavSize / x;

            int maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                Buffer.BlockCopy(source, offset, block, 1, x);
                data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(float); //  block size = 4
            int convertedSize = wavSize / x;

            Int32 maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static string FormatCode(UInt16 code)
        {
            switch (code)
            {
                case 1:
                    return "PCM";
                case 2:
                    return "ADPCM";
                case 3:
                    return "IEEE";
                case 7:
                    return "μ-law";
                case 65534:
                    return "WaveFormatExtensable";
                default:
                    Debug.LogWarning("Unknown wav code format:" + code);
                    return "";
            }
        }

        #region AudioClip扩展

        public static byte[] GetData(this AudioClip clip)
        {
            var data = new float[clip.samples * clip.channels];

            clip.GetData(data, 0);

            byte[] bytes = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public static void SetData(this AudioClip clip, byte[] bytes)
        {
            float[] data = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);

            clip.SetData(data, 0);
        }

        public static byte[] GetData16(this AudioClip clip)
        {
            var data = new float[clip.samples * clip.channels];

            clip.GetData(data, 0);

            byte[] bytes = new byte[data.Length * 2];

            int rescaleFactor = 32767;

            for (int i = 0; i < data.Length; i++)
            {
                short value = (short)(data[i] * rescaleFactor);
                BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
            }

            return bytes;
        }

        public static byte[] EncodeToWAV(this AudioClip clip)
        {
            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                //[0,43]
                memoryStream.Write(new byte[44], 0, 44);//预留44字节头部信息

                byte[] bytesData = clip.GetData16();

                //[44,44 + bytesData.Length -1]
                memoryStream.Write(bytesData, 0, bytesData.Length);

                memoryStream.Seek(0, SeekOrigin.Begin);

                //[0,3]
                byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                memoryStream.Write(riff, 0, 4);

                //[4,7]
                byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
                memoryStream.Write(chunkSize, 0, 4);

                //[8,11]
                byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                memoryStream.Write(wave, 0, 4);

                //[12,15]
                byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                memoryStream.Write(fmt, 0, 4);

                //[16,19]
                byte[] subChunk1 = BitConverter.GetBytes(16);
                memoryStream.Write(subChunk1, 0, 4);

                //UInt16 two = 2;
                UInt16 one = 1;

                //[20,21]
                byte[] audioFormat = BitConverter.GetBytes(one);
                memoryStream.Write(audioFormat, 0, 2);

                //[22,23]
                byte[] numChannels = BitConverter.GetBytes(clip.channels);
                memoryStream.Write(numChannels, 0, 2);

                //[24,27]
                byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
                memoryStream.Write(sampleRate, 0, 4);

                //[28,31]
                byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample*number of channels
                memoryStream.Write(byteRate, 0, 4);

                //[32,33]
                UInt16 blockAlign = (ushort)(clip.channels * 2);
                memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                //[34,35]
                UInt16 bps = 16;
                byte[] bitsPerSample = BitConverter.GetBytes(bps);
                memoryStream.Write(bitsPerSample, 0, 2);

                //[36,39]
                byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                memoryStream.Write(datastring, 0, 4);

                //[40,43]
                byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
                memoryStream.Write(subChunk2, 0, 4);

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }

        #endregion
    }
}
