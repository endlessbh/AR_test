using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;

namespace XCSJ.PluginTools.Gif
{
    public static class UniGif
    {
        #region UniGif
        /// <summary>
        /// Get GIF texture list (This is a possibility of lock up)
        /// </summary>
        /// <param name="bytes">GIF file byte data</param>
        /// <param name="loopCount">out Animation loop count</param>
        /// <param name="width">out GIF image width (px)</param>
        /// <param name="height">out GIF image height (px)</param>
        /// <param name="filterMode">Textures filter mode</param>
        /// <param name="wrapMode">Textures wrap mode</param>
        /// <param name="debugLog">Debug Log Flag</param>
        /// <returns>GIF texture list</returns>
        public static List<FrameTexture> GetTextureList(byte[] bytes, out int loopCount, out int width, out int height,
            FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool debugLog = false)
        {
            loopCount = -1;
            width = 0;
            height = 0;

            // Set GIF data
            var gifData = new GifData();
            if (SetGifData(bytes, ref gifData, debugLog) == false)
            {
                Debug.LogError("GIF file data set error.");
                return null;
            }

            // Decode to textures from GIF data
            var gifTexList = new List<FrameTexture>();
            if (DecodeTexture(gifData, gifTexList, filterMode, wrapMode) == false)
            {
                Debug.LogError("GIF texture decode error.");
                return null;
            }

            loopCount = gifData.appEx.loopCount;
            width = gifData.logicalScreenWidth;
            height = gifData.logicalScreenHeight;
            return gifTexList;
        }

        /// <summary>
        /// Get GIF texture list Coroutine (Avoid lock up but more slow)
        /// </summary>
        /// <param name="mb">MonoBehaviour to start the coroutine</param>
        /// <param name="bytes">GIF file byte data</param>
        /// <param name="cb">Callback method(param is GIF texture list, Animation loop count, GIF image width (px), GIF image height (px))</param>
        /// <param name="filterMode">Textures filter mode</param>
        /// <param name="wrapMode">Textures wrap mode</param>
        /// <param name="debugLog">Debug Log Flag</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator GetTextureListCoroutine(MonoBehaviour mb, byte[] bytes, Action<List<FrameTexture>, int, int, int> cb,
            FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool debugLog = false)
        {
            int loopCount = -1;
            int width = 0;
            int height = 0;

            // Set GIF data
            var gifData = new GifData();
            if (SetGifData(bytes, ref gifData, debugLog) == false)
            {
                Debug.LogError("GIF file data set error.");
                if (cb != null)
                {
                    cb(null, loopCount, width, height);
                }
                yield break;
            }

            // avoid lock up
            yield return 0;

            // Decode to textures from GIF data
            List<FrameTexture> gifTexList = null;
            yield return mb.StartCoroutine(UniGif.DecodeTextureCoroutine(gifData, gtList =>
            {
                gifTexList = gtList;
            }, filterMode, wrapMode));

            if (gifTexList == null)
            {
                Debug.LogError("GIF texture decode error.");
                if (cb != null)
                {
                    cb(null, loopCount, width, height);
                }
                yield break;
            }

            loopCount = gifData.appEx.loopCount;
            width = gifData.logicalScreenWidth;
            height = gifData.logicalScreenHeight;

            if (cb != null)
            {
                cb(gifTexList, loopCount, width, height);
            }
        }
        #endregion

        #region UniGifConst

        /// <summary>
        /// GIF Data Format
        /// </summary>
        struct GifData
        {
            // Signature
            public byte sig0, sig1, sig2;
            // Version
            public byte ver0, ver1, ver2;
            // Logical Screen Width
            public ushort logicalScreenWidth;
            // Logical Screen Height
            public ushort logicalScreenHeight;
            // Global Color Table Flag
            public bool globalColorTableFlag;
            // Color Resolution
            public int colorResolution;
            // Sort Flag
            public bool sortFlag;
            // Size of Global Color Table
            public int sizeOfGlobalColorTable;
            // Background Color Index
            public byte bgColorIndex;
            // Pixel Aspect Ratio
            public byte pixelAspectRatio;
            // Global Color Table
            public List<byte[]> globalColorTable;
            // ImageBlock
            public List<ImageBlock> imageBlockList;
            // GraphicControlExtension
            public List<GraphicControlExtension> graphicCtrlExList;
            // Comment Extension
            public List<CommentExtension> commentExList;
            // Plain Text Extension
            public List<PlainTextExtension> plainTextExList;
            // Application Extension
            public ApplicationExtension appEx;
            // Trailer
            public byte trailer;

            public string signature
            {
                get
                {
                    char[] c = { (char)sig0, (char)sig1, (char)sig2 };
                    return new string(c);
                }
            }
            public string version
            {
                get
                {
                    char[] c = { (char)ver0, (char)ver1, (char)ver2 };
                    return new string(c);
                }
            }
            public void Dump()
            {
                Debug.Log("GIF Type: " + signature + "-" + version);
                Debug.Log("Image Size: " + logicalScreenWidth + "x" + logicalScreenHeight);
                Debug.Log("Animation Image Count: " + imageBlockList.Count);
                Debug.Log("Animation Loop Count (0 is infinite): " + appEx.loopCount);
                Debug.Log("Application Identifier: " + appEx.applicationIdentifier);
                Debug.Log("Application Authentication Code: " + appEx.applicationAuthenticationCode);
            }
        }

        /// <summary>
        /// Image Block
        /// </summary>
        struct ImageBlock
        {
            // Image Separator
            public byte imageSeparator;
            // Image Left Position
            public ushort imageLeftPosition;
            // Image Top Position
            public ushort imageTopPosition;
            // Image Width
            public ushort imageWidth;
            // Image Height
            public ushort imageHeight;
            // Local Color Table Flag
            public bool localColorTableFlag;
            // Interlace Flag
            public bool interlaceFlag;
            // Sort Flag
            public bool sortFlag;
            // Size of Local Color Table
            public int sizeOfLocalColorTable;
            // Local Color Table
            public List<byte[]> localColorTable;
            // LZW Minimum Code Size
            public byte LzwMinimumCodeSize;
            // Block Size & Image Data List
            public List<ImageDataBlock> imageDataList;

            public struct ImageDataBlock
            {
                // Block Size
                public byte blockSize;
                // Image Data
                public byte[] imageData;
            }
        }

        /// <summary>
        /// Graphic Control Extension
        /// </summary>
        struct GraphicControlExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;
            // Graphic Control Label
            public byte graphicControlLabel;
            // Block Size
            public byte blockSize;
            // Disposal Mothod
            public ushort disposalMethod;
            // Transparent Color Flag
            public bool transparentColorFlag;
            // Delay Time
            public ushort delayTime;
            // Transparent Color Index
            public byte transparentColorIndex;
            // Block Terminator
            public byte blockTerminator;
        }

        /// <summary>
        /// Comment Extension
        /// </summary>
        struct CommentExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;
            // Comment Label
            public byte commentLabel;
            // Block Size & Comment Data List
            public List<CommentDataBlock> commentDataList;

            public struct CommentDataBlock
            {
                // Block Size
                public byte blockSize;
                // Image Data
                public byte[] commentData;
            }
        }

        /// <summary>
        /// Plain Text Extension
        /// </summary>
        struct PlainTextExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;
            // Plain Text Label
            public byte plainTextLabel;
            // Block Size
            public byte blockSize;
            // Block Size & Plain Text Data List
            public List<PlainTextDataBlock> plainTextDataList;

            public struct PlainTextDataBlock
            {
                // Block Size
                public byte blockSize;
                // Plain Text Data
                public byte[] plainTextData;
            }
        }

        /// <summary>
        /// Application Extension
        /// </summary>
        struct ApplicationExtension
        {
            // Extension Introducer
            public byte extensionIntroducer;
            // Extension Label
            public byte extensionLabel;
            // Block Size
            public byte blockSize;
            // Application Identifier
            public byte appId1, appId2, appId3, appId4, appId5, appId6, appId7, appId8;
            // Application Authentication Code
            public byte appAuthCode1, appAuthCode2, appAuthCode3;
            // Block Size & Application Data List
            public List<ApplicationDataBlock> appDataList;

            public struct ApplicationDataBlock
            {
                // Block Size
                public byte blockSize;
                // Application Data
                public byte[] applicationData;
            }

            public string applicationIdentifier
            {
                get
                {
                    char[] c = { (char)appId1, (char)appId2, (char)appId3, (char)appId4, (char)appId5, (char)appId6, (char)appId7, (char)appId8 };
                    return new string(c);
                }
            }

            public string applicationAuthenticationCode
            {
                get
                {
                    char[] c = { (char)appAuthCode1, (char)appAuthCode2, (char)appAuthCode3 };
                    return new string(c);
                }
            }

            public int loopCount
            {
                get
                {
                    if (appDataList == null || appDataList.Count < 1 ||
                        appDataList[0].applicationData.Length < 3 ||
                        appDataList[0].applicationData[0] != 0x01)
                    {
                        return 0;
                    }
                    return BitConverter.ToUInt16(appDataList[0].applicationData, 1);
                }
            }
        }
        #endregion

        #region UniGifDecoder
        /// <summary>
        /// Decode to textures from GIF data
        /// </summary>
        /// <param name="gifData">GIF data</param>
        /// <param name="gifTexList">GIF texture list</param>
        /// <param name="filterMode">Textures filter mode</param>
        /// <param name="wrapMode">Textures wrap mode</param>
        /// <returns>IEnumerator</returns>
        static IEnumerator DecodeTextureCoroutine(GifData gifData, Action<List<FrameTexture>> cb, FilterMode filterMode, TextureWrapMode wrapMode)
        {
            if (gifData.imageBlockList == null || gifData.imageBlockList.Count < 1)
            {
                yield break;
            }

            List<FrameTexture> gifTexList = new List<FrameTexture>();

            Color32? bgColor = GetGlobalBgColor(gifData);

            // Disposal Method
            // 0 (No disposal specified)
            // 1 (Do not dispose)
            // 2 (Restore to background color)
            // 3 (Restore to previous)
            ushort disposalMethod = 0;

            int imgBlockIndex = 0;
            foreach (var imgBlock in gifData.imageBlockList)
            {
                var decodedData = GetDecodedData(imgBlock);

                var colorTable = GetColorTable(gifData, imgBlock, ref bgColor);

                var graphicCtrlEx = GetGraphicCtrlExt(gifData, imgBlockIndex);

                int transparentIndex = GetTransparentIndex(graphicCtrlEx);

                // avoid lock up
                yield return 0;

                bool useBeforeTex = false;
                var tex = CreateTexture2D(gifData, gifTexList, imgBlockIndex, disposalMethod, filterMode, wrapMode, ref useBeforeTex);

                // Set pixel data
                int dataIndex = 0;
                // Reverse set pixels. because GIF data starts from the top left.
                for (int y = tex.height - 1; y >= 0; y--)
                {
                    SetTexturePixelRow(tex, y, imgBlock, decodedData, ref dataIndex, colorTable, bgColor, transparentIndex, useBeforeTex);

                    // avoid lock up
                    if (y % 10 == 0)
                    {
                        yield return 0;
                    }
                }
                tex.Apply();

                float delaySec = GetDelaySec(graphicCtrlEx);

                // Add to GIF texture list
                gifTexList.Add(new FrameTexture(tex, delaySec));

                disposalMethod = GetDisposalMethod(graphicCtrlEx);

                imgBlockIndex++;

                // avoid lock up
                yield return 0;
            }

            if (cb != null)
            {
                cb(gifTexList);
            }
        }

        /// <summary>
        /// Decode to textures from GIF data
        /// </summary>
        /// <param name="gifData">GIF data</param>
        /// <param name="gifTexList">GIF texture list</param>
        /// <param name="filterMode">Textures filter mode</param>
        /// <param name="wrapMode">Textures wrap mode</param>
        /// <returns>Result</returns>
        static bool DecodeTexture(GifData gifData, List<FrameTexture> gifTexList, FilterMode filterMode, TextureWrapMode wrapMode)
        {
            if (gifData.imageBlockList == null || gifData.imageBlockList.Count < 1)
            {
                return false;
            }

            Color32? bgColor = GetGlobalBgColor(gifData);

            // Disposal Method
            // 0 (No disposal specified)
            // 1 (Do not dispose)
            // 2 (Restore to background color)
            // 3 (Restore to previous)
            ushort disposalMethod = 0;

            int imgBlockIndex = 0;
            foreach (var imgBlock in gifData.imageBlockList)
            {
                var decodedData = GetDecodedData(imgBlock);

                var colorTable = GetColorTable(gifData, imgBlock, ref bgColor);

                var graphicCtrlEx = GetGraphicCtrlExt(gifData, imgBlockIndex);

                int transparentIndex = GetTransparentIndex(graphicCtrlEx);

                bool useBeforeTex = false;
                var tex = CreateTexture2D(gifData, gifTexList, imgBlockIndex, disposalMethod, filterMode, wrapMode, ref useBeforeTex);

                // Set pixel data
                int dataIndex = 0;
                // Reverse set pixels. because GIF data starts from the top left.
                for (int y = tex.height - 1; y >= 0; y--)
                {
                    SetTexturePixelRow(tex, y, imgBlock, decodedData, ref dataIndex, colorTable, bgColor, transparentIndex, useBeforeTex);
                }
                tex.Apply();

                float delaySec = GetDelaySec(graphicCtrlEx);

                // Add to GIF texture list
                if (gifTexList == null)
                {
                    gifTexList = new List<FrameTexture>();
                }
                gifTexList.Add(new FrameTexture(tex, delaySec));

                disposalMethod = GetDisposalMethod(graphicCtrlEx);

                imgBlockIndex++;
            }

            return true;
        }

        #region Call from DecodeTexture methods

        /// <summary>
        /// Get background color from global color table
        /// </summary>
        static Color32? GetGlobalBgColor(GifData gifData)
        {
            Color32? bgColor = null;
            if (gifData.globalColorTableFlag)
            {
                // Set background color from global color table
                byte[] bgRgb = gifData.globalColorTable[gifData.bgColorIndex];
                bgColor = new Color32(bgRgb[0], bgRgb[1], bgRgb[2], 255);
            }
            return bgColor;
        }

        /// <summary>
        /// Get decoded image data from ImageBlock
        /// </summary>
        static byte[] GetDecodedData(ImageBlock imgBlock)
        {
            // Combine LZW compressed data
            List<byte> lzwData = new List<byte>();
            foreach (var imageData in imgBlock.imageDataList)
            {
                foreach (var data in imageData.imageData)
                {
                    lzwData.Add(data);
                }
            }

            // LZW decode
            int needDataSize = imgBlock.imageHeight * imgBlock.imageWidth;
            byte[] decodedData = DecodeGifLZW(lzwData, imgBlock.LzwMinimumCodeSize, needDataSize);

            // Sort interlace GIF
            if (imgBlock.interlaceFlag)
            {
                decodedData = SortInterlaceGifData(decodedData, imgBlock.imageWidth);
            }
            return decodedData;
        }

        /// <summary>
        /// Get color table (local or global)
        /// </summary>
        static List<byte[]> GetColorTable(GifData gifData, ImageBlock imgBlock, ref Color32? bgColor)
        {
            var colorTable = imgBlock.localColorTableFlag ? imgBlock.localColorTable : gifData.globalColorTable;

            if (imgBlock.localColorTableFlag)
            {
                // Set background color from local color table
                byte[] bgRgb = imgBlock.localColorTable[gifData.bgColorIndex];
                bgColor = new Color32(bgRgb[0], bgRgb[1], bgRgb[2], 255);
            }

            return colorTable;
        }

        /// <summary>
        /// Get GraphicControlExtension from GifData
        /// </summary>
        static GraphicControlExtension? GetGraphicCtrlExt(GifData gifData, int imgBlockIndex)
        {
            if (gifData.graphicCtrlExList != null && gifData.graphicCtrlExList.Count > imgBlockIndex)
            {
                return gifData.graphicCtrlExList[imgBlockIndex];
            }
            return null;
        }

        /// <summary>
        /// Get transparent color index from GraphicControlExtension
        /// </summary>
        static int GetTransparentIndex(GraphicControlExtension? graphicCtrlEx)
        {
            int transparentIndex = -1;
            if (graphicCtrlEx != null && graphicCtrlEx.Value.transparentColorFlag)
            {
                transparentIndex = graphicCtrlEx.Value.transparentColorIndex;
            }
            return transparentIndex;
        }

        /// <summary>
        /// Get delay seconds from GraphicControlExtension
        /// </summary>
        static float GetDelaySec(GraphicControlExtension? graphicCtrlEx)
        {
            // Get delay sec from GraphicControlExtension
            float delaySec = graphicCtrlEx != null ? (float)graphicCtrlEx.Value.delayTime / 100f : 0.1f;
            // Minimum 0.1 seconds delay (because major browsers have become so...)
            if (delaySec < 0.1f)
            {
                delaySec = 0.1f;
            }
            return delaySec;
        }

        /// <summary>
        /// Get disposal method from GraphicControlExtension
        /// </summary>
        static ushort GetDisposalMethod(GraphicControlExtension? graphicCtrlEx)
        {
            return graphicCtrlEx != null ? graphicCtrlEx.Value.disposalMethod : (ushort)2;
        }

        /// <summary>
        /// Create Texture2D object and initial settings
        /// </summary>
        static Texture2D CreateTexture2D(GifData gifData, List<FrameTexture> gifTexList, int imgBlockIndex, ushort disposalMethod, FilterMode filterMode, TextureWrapMode wrapMode, ref bool useBeforeTex)
        {
            // Create texture
            Texture2D tex = new Texture2D(gifData.logicalScreenWidth, gifData.logicalScreenHeight, TextureFormat.ARGB32, false);
            tex.filterMode = filterMode;
            tex.wrapMode = wrapMode;

            // Check dispose
            useBeforeTex = false;
            int beforeIndex = -1;
            if (imgBlockIndex > 0 && disposalMethod == 0 || disposalMethod == 1)
            {
                // before 1
                beforeIndex = imgBlockIndex - 1;
            }
            else if (imgBlockIndex > 1 && disposalMethod == 3)
            {
                // before 2
                beforeIndex = imgBlockIndex - 2;
            }
            if (beforeIndex >= 0)
            {
                // Do not dispose
                useBeforeTex = true;
                Color32[] pix = gifTexList[beforeIndex]._texture2D.GetPixels32();
                tex.SetPixels32(pix);
                tex.Apply();
            }

            return tex;
        }

        /// <summary>
        /// Set texture pixel row
        /// </summary>
        static void SetTexturePixelRow(Texture2D tex, int y, ImageBlock imgBlock, byte[] decodedData, ref int dataIndex, List<byte[]> colorTable, Color32? bgColor, int transparentIndex, bool useBeforeTex)
        {
            // Row no (0~)
            int row = tex.height - 1 - y;

            for (int x = 0; x < tex.width; x++)
            {
                // Line no (0~)
                int line = x;

                // Out of image blocks
                if (row < imgBlock.imageTopPosition ||
                    row >= imgBlock.imageTopPosition + imgBlock.imageHeight ||
                    line < imgBlock.imageLeftPosition ||
                    line >= imgBlock.imageLeftPosition + imgBlock.imageWidth)
                {
                    if (useBeforeTex == false && bgColor != null)
                    {
                        tex.SetPixel(x, y, bgColor.Value);
                    }
                    continue;
                }

                // Out of decoded data
                if (dataIndex >= decodedData.Length)
                {
                    tex.SetPixel(x, y, Color.black);
                    // Debug.LogError ("dataIndex exceeded the size of decodedData. dataIndex:" + dataIndex + " decodedData.Length:" + decodedData.Length + " y:" + y + " x:" + x);
                    dataIndex++;
                    continue;
                }

                // Get pixel color from color table
                byte colorIndex = decodedData[dataIndex];
                if (colorTable == null || colorTable.Count <= colorIndex)
                {
                    Debug.LogError("colorIndex exceeded the size of colorTable. colorTable.Count:" + colorTable.Count + " colorIndex:" + colorIndex);
                    dataIndex++;
                    continue;
                }
                byte[] rgb = colorTable[colorIndex];

                // Set alpha
                byte alpha = transparentIndex >= 0 && transparentIndex == colorIndex ? (byte)0 : (byte)255;

                if (alpha != 0 || useBeforeTex == false)
                {
                    Color32 col = new Color32(rgb[0], rgb[1], rgb[2], alpha);
                    tex.SetPixel(x, y, col);
                }

                dataIndex++;
            }
        }

        #endregion

        #region Decode LZW & Sort interrace methods

        /// <summary>
        /// GIF LZW decode
        /// </summary>
        /// <param name="compData">LZW compressed data</param>
        /// <param name="lzwMinimumCodeSize">LZW minimum code size</param>
        /// <param name="needDataSize">Need decoded data size</param>
        /// <returns>Decoded data array</returns>
        static byte[] DecodeGifLZW(List<byte> compData, int lzwMinimumCodeSize, int needDataSize)
        {
            int clearCode = 0;
            int finishCode = 0;

            // Initialize dictionary
            Dictionary<int, string> dic = new Dictionary<int, string>();
            int lzwCodeSize = 0;
            InitDictionary(dic, lzwMinimumCodeSize, out lzwCodeSize, out clearCode, out finishCode);

            // Convert to bit array
            byte[] compDataArr = compData.ToArray();
            var bitData = new BitArray(compDataArr);

            byte[] output = new byte[needDataSize];
            int outputAddIndex = 0;

            string prevEntry = null;

            bool dicInitFlag = false;

            int bitDataIndex = 0;

            // LZW decode loop
            while (bitDataIndex < bitData.Length)
            {
                if (dicInitFlag)
                {
                    InitDictionary(dic, lzwMinimumCodeSize, out lzwCodeSize, out clearCode, out finishCode);
                    dicInitFlag = false;
                }

                int key = bitData.GetNumeral(bitDataIndex, lzwCodeSize);

                string entry = null;

                if (key == clearCode)
                {
                    // Clear (Initialize dictionary)
                    dicInitFlag = true;
                    bitDataIndex += lzwCodeSize;
                    prevEntry = null;
                    continue;

                }
                else if (key == finishCode)
                {
                    // Exit
                    Debug.LogWarning("early stop code. bitDataIndex:" + bitDataIndex + " lzwCodeSize:" + lzwCodeSize + " key:" + key + " dic.Count:" + dic.Count);
                    break;

                }
                else if (dic.ContainsKey(key))
                {
                    // Output from dictionary
                    entry = dic[key];

                }
                else if (key >= dic.Count)
                {
                    if (prevEntry != null)
                    {
                        // Output from estimation
                        entry = prevEntry + prevEntry[0];

                    }
                    else
                    {
                        Debug.LogWarning("It is strange that come here. bitDataIndex:" + bitDataIndex + " lzwCodeSize:" + lzwCodeSize + " key:" + key + " dic.Count:" + dic.Count);
                        bitDataIndex += lzwCodeSize;
                        continue;
                    }

                }
                else
                {
                    Debug.LogWarning("It is strange that come here. bitDataIndex:" + bitDataIndex + " lzwCodeSize:" + lzwCodeSize + " key:" + key + " dic.Count:" + dic.Count);
                    bitDataIndex += lzwCodeSize;
                    continue;
                }

                // Output
                // Take out 8 bits from the string.
                var temp = Encoding.Unicode.GetBytes(entry);
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        output[outputAddIndex] = temp[i];
                        outputAddIndex++;
                    }
                }

                if (outputAddIndex >= needDataSize)
                {
                    // Exit
                    break;
                }

                if (prevEntry != null)
                {
                    // Add to dictionary
                    dic.Add(dic.Count, prevEntry + entry[0]);
                }

                prevEntry = entry;

                bitDataIndex += lzwCodeSize;

                if (lzwCodeSize == 3 && dic.Count >= 8)
                {
                    lzwCodeSize = 4;
                }
                else if (lzwCodeSize == 4 && dic.Count >= 16)
                {
                    lzwCodeSize = 5;
                }
                else if (lzwCodeSize == 5 && dic.Count >= 32)
                {
                    lzwCodeSize = 6;
                }
                else if (lzwCodeSize == 6 && dic.Count >= 64)
                {
                    lzwCodeSize = 7;
                }
                else if (lzwCodeSize == 7 && dic.Count >= 128)
                {
                    lzwCodeSize = 8;
                }
                else if (lzwCodeSize == 8 && dic.Count >= 256)
                {
                    lzwCodeSize = 9;
                }
                else if (lzwCodeSize == 9 && dic.Count >= 512)
                {
                    lzwCodeSize = 10;
                }
                else if (lzwCodeSize == 10 && dic.Count >= 1024)
                {
                    lzwCodeSize = 11;
                }
                else if (lzwCodeSize == 11 && dic.Count >= 2048)
                {
                    lzwCodeSize = 12;
                }
                else if (lzwCodeSize == 12 && dic.Count >= 4096)
                {
                    int nextKey = bitData.GetNumeral(bitDataIndex, lzwCodeSize);
                    if (nextKey != clearCode)
                    {
                        dicInitFlag = true;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Initialize dictionary
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="lzwMinimumCodeSize">LZW minimum code size</param>
        /// <param name="lzwCodeSize">out LZW code size</param>
        /// <param name="clearCode">out Clear code</param>
        /// <param name="finishCode">out Finish code</param>
        static void InitDictionary(Dictionary<int, string> dic, int lzwMinimumCodeSize, out int lzwCodeSize, out int clearCode, out int finishCode)
        {
            int dicLength = (int)Math.Pow(2, lzwMinimumCodeSize);

            clearCode = dicLength;
            finishCode = clearCode + 1;

            dic.Clear();

            for (int i = 0; i < dicLength + 2; i++)
            {
                dic.Add(i, ((char)i).ToString());
            }

            lzwCodeSize = lzwMinimumCodeSize + 1;
        }

        /// <summary>
        /// Sort interlace GIF data
        /// </summary>
        /// <param name="decodedData">Decoded GIF data</param>
        /// <param name="xNum">Pixel number of horizontal row</param>
        /// <returns>Sorted data</returns>
        static byte[] SortInterlaceGifData(byte[] decodedData, int xNum)
        {
            int rowNo = 0;
            int dataIndex = 0;
            var newArr = new byte[decodedData.Length];
            // Every 8th. row, starting with row 0.
            for (int i = 0; i < newArr.Length; i++)
            {
                if (rowNo % 8 == 0)
                {
                    newArr[i] = decodedData[dataIndex];
                    dataIndex++;
                }
                if (i != 0 && i % xNum == 0)
                {
                    rowNo++;
                }
            }
            rowNo = 0;
            // Every 8th. row, starting with row 4.
            for (int i = 0; i < newArr.Length; i++)
            {
                if (rowNo % 8 == 4)
                {
                    newArr[i] = decodedData[dataIndex];
                    dataIndex++;
                }
                if (i != 0 && i % xNum == 0)
                {
                    rowNo++;
                }
            }
            rowNo = 0;
            // Every 4th. row, starting with row 2.
            for (int i = 0; i < newArr.Length; i++)
            {
                if (rowNo % 4 == 2)
                {
                    newArr[i] = decodedData[dataIndex];
                    dataIndex++;
                }
                if (i != 0 && i % xNum == 0)
                {
                    rowNo++;
                }
            }
            rowNo = 0;
            // Every 2nd. row, starting with row 1.
            for (int i = 0; i < newArr.Length; i++)
            {
                if (rowNo % 8 != 0 && rowNo % 8 != 4 && rowNo % 4 != 2)
                {
                    newArr[i] = decodedData[dataIndex];
                    dataIndex++;
                }
                if (i != 0 && i % xNum == 0)
                {
                    rowNo++;
                }
            }

            return newArr;
        }

        #endregion
        #endregion

        #region UniGifFormatter

        /// <summary>
        /// Set GIF data
        /// </summary>
        /// <param name="gifBytes">GIF byte data</param>
        /// <param name="gifData">ref GIF data</param>
        /// <param name="debugLog">Debug log flag</param>
        /// <returns>Result</returns>
        static bool SetGifData(byte[] gifBytes, ref GifData gifData, bool debugLog)
        {
            if (debugLog)
            {
                Debug.Log("SetGifData Start.");
            }

            if (gifBytes == null || gifBytes.Length <= 0)
            {
                Debug.LogError("bytes is nothing.");
                return false;
            }

            int byteIndex = 0;

            if (SetGifHeader(gifBytes, ref byteIndex, ref gifData) == false)
            {
                Debug.LogError("GIF header set error.");
                return false;
            }

            if (SetGifBlock(gifBytes, ref byteIndex, ref gifData) == false)
            {
                Debug.LogError("GIF block set error.");
                return false;
            }

            if (debugLog)
            {
                gifData.Dump();
                Debug.Log("SetGifData Finish.");
            }
            return true;
        }

        static bool SetGifHeader(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            // Signature(3 Bytes)
            // 0x47 0x49 0x46 (GIF)
            if (gifBytes[0] != 'G' || gifBytes[1] != 'I' || gifBytes[2] != 'F')
            {
                Debug.LogError("This is not GIF image.");
                return false;
            }
            gifData.sig0 = gifBytes[0];
            gifData.sig1 = gifBytes[1];
            gifData.sig2 = gifBytes[2];

            // Version(3 Bytes)
            // 0x38 0x37 0x61 (87a) or 0x38 0x39 0x61 (89a)
            if ((gifBytes[3] != '8' || gifBytes[4] != '7' || gifBytes[5] != 'a') &&
                (gifBytes[3] != '8' || gifBytes[4] != '9' || gifBytes[5] != 'a'))
            {
                Debug.LogError("GIF version error.\nSupported only GIF87a or GIF89a.");
                return false;
            }
            gifData.ver0 = gifBytes[3];
            gifData.ver1 = gifBytes[4];
            gifData.ver2 = gifBytes[5];

            // Logical Screen Width(2 Bytes)
            gifData.logicalScreenWidth = BitConverter.ToUInt16(gifBytes, 6);

            // Logical Screen Height(2 Bytes)
            gifData.logicalScreenHeight = BitConverter.ToUInt16(gifBytes, 8);

            // 1 Byte
            {
                // Global Color Table Flag(1 Bit)
                gifData.globalColorTableFlag = (gifBytes[10] & 128) == 128; // 0b10000000

                // Color Resolution(3 Bits)
                switch (gifBytes[10] & 112)
                {
                    case 112: // 0b01110000
                        gifData.colorResolution = 8;
                        break;
                    case 96: // 0b01100000
                        gifData.colorResolution = 7;
                        break;
                    case 80: // 0b01010000
                        gifData.colorResolution = 6;
                        break;
                    case 64: // 0b01000000
                        gifData.colorResolution = 5;
                        break;
                    case 48: // 0b00110000
                        gifData.colorResolution = 4;
                        break;
                    case 32: // 0b00100000
                        gifData.colorResolution = 3;
                        break;
                    case 16: // 0b00010000
                        gifData.colorResolution = 2;
                        break;
                    default:
                        gifData.colorResolution = 1;
                        break;
                }

                // Sort Flag(1 Bit)
                gifData.sortFlag = (gifBytes[10] & 8) == 8; // 0b00001000

                // Size of Global Color Table(3 Bits)
                int val = (gifBytes[10] & 7) + 1;
                gifData.sizeOfGlobalColorTable = (int)Math.Pow(2, val);
            }

            // Background Color Index(1 Byte)
            gifData.bgColorIndex = gifBytes[11];

            // Pixel Aspect Ratio(1 Byte)
            gifData.pixelAspectRatio = gifBytes[12];

            byteIndex = 13;
            if (gifData.globalColorTableFlag)
            {
                // Global Color Table(0～255×3 Bytes)
                gifData.globalColorTable = new List<byte[]>();
                for (int i = byteIndex; i < byteIndex + (gifData.sizeOfGlobalColorTable * 3); i += 3)
                {
                    gifData.globalColorTable.Add(new byte[] { gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] });
                }
                byteIndex = byteIndex + (gifData.sizeOfGlobalColorTable * 3);
            }

            return true;
        }

        static bool SetGifBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            try
            {
                int lastIndex = 0;
                while (true)
                {
                    int nowIndex = byteIndex;

                    if (gifBytes[nowIndex] == 0x2c)
                    {
                        // Image Block(0x2c)
                        SetImageBlock(gifBytes, ref byteIndex, ref gifData);

                    }
                    else if (gifBytes[nowIndex] == 0x21)
                    {
                        // Extension
                        switch (gifBytes[nowIndex + 1])
                        {
                            case 0xf9:
                                // Graphic Control Extension(0x21 0xf9)
                                SetGraphicControlExtension(gifBytes, ref byteIndex, ref gifData);
                                break;
                            case 0xfe:
                                // Comment Extension(0x21 0xfe)
                                SetCommentExtension(gifBytes, ref byteIndex, ref gifData);
                                break;
                            case 0x01:
                                // Plain Text Extension(0x21 0x01)
                                SetPlainTextExtension(gifBytes, ref byteIndex, ref gifData);
                                break;
                            case 0xff:
                                // Application Extension(0x21 0xff)
                                SetApplicationExtension(gifBytes, ref byteIndex, ref gifData);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (gifBytes[nowIndex] == 0x3b)
                    {
                        // Trailer(1 Byte)
                        gifData.trailer = gifBytes[byteIndex];
                        byteIndex++;
                        break;
                    }

                    if (lastIndex == nowIndex)
                    {
                        Debug.LogError("Infinite loop error.");
                        return false;
                    }

                    lastIndex = nowIndex;
                }
            }
            catch (Exception ex)
            {
                Log.Exception("SetGifBlock时异常:" + ex);
                return false;
            }

            return true;
        }

        static void SetImageBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            ImageBlock ib = new ImageBlock();

            // Image Separator(1 Byte)
            // 0x2c
            ib.imageSeparator = gifBytes[byteIndex];
            byteIndex++;

            // Image Left Position(2 Bytes)
            ib.imageLeftPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
            byteIndex += 2;

            // Image Top Position(2 Bytes)
            ib.imageTopPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
            byteIndex += 2;

            // Image Width(2 Bytes)
            ib.imageWidth = BitConverter.ToUInt16(gifBytes, byteIndex);
            byteIndex += 2;

            // Image Height(2 Bytes)
            ib.imageHeight = BitConverter.ToUInt16(gifBytes, byteIndex);
            byteIndex += 2;

            // 1 Byte
            {
                // Local Color Table Flag(1 Bit)
                ib.localColorTableFlag = (gifBytes[byteIndex] & 128) == 128; // 0b10000000

                // Interlace Flag(1 Bit)
                ib.interlaceFlag = (gifBytes[byteIndex] & 64) == 64; // 0b01000000

                // Sort Flag(1 Bit)
                ib.sortFlag = (gifBytes[byteIndex] & 32) == 32; // 0b00100000

                // Reserved(2 Bits)
                // Unused

                // Size of Local Color Table(3 Bits)
                int val = (gifBytes[byteIndex] & 7) + 1;
                ib.sizeOfLocalColorTable = (int)Math.Pow(2, val);

                byteIndex++;
            }

            if (ib.localColorTableFlag)
            {
                // Local Color Table(0～255×3 Bytes)
                ib.localColorTable = new List<byte[]>();
                for (int i = byteIndex; i < byteIndex + (ib.sizeOfLocalColorTable * 3); i += 3)
                {
                    ib.localColorTable.Add(new byte[] { gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] });
                }
                byteIndex = byteIndex + (ib.sizeOfLocalColorTable * 3);
            }

            // LZW Minimum Code Size(1 Byte)
            ib.LzwMinimumCodeSize = gifBytes[byteIndex];
            byteIndex++;

            // Block Size & Image Data List
            while (true)
            {
                // Block Size(1 Byte)
                byte blockSize = gifBytes[byteIndex];
                byteIndex++;

                if (blockSize == 0x00)
                {
                    // Block Terminator(1 Byte)
                    break;
                }

                var imageDataBlock = new ImageBlock.ImageDataBlock();
                imageDataBlock.blockSize = blockSize;

                // Image Data(? Bytes)
                imageDataBlock.imageData = new byte[imageDataBlock.blockSize];
                for (int i = 0; i < imageDataBlock.imageData.Length; i++)
                {
                    imageDataBlock.imageData[i] = gifBytes[byteIndex];
                    byteIndex++;
                }

                if (ib.imageDataList == null)
                {
                    ib.imageDataList = new List<ImageBlock.ImageDataBlock>();
                }
                ib.imageDataList.Add(imageDataBlock);
            }

            if (gifData.imageBlockList == null)
            {
                gifData.imageBlockList = new List<ImageBlock>();
            }
            gifData.imageBlockList.Add(ib);
        }

        static void SetGraphicControlExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            GraphicControlExtension gcEx = new GraphicControlExtension();

            // Extension Introducer(1 Byte)
            // 0x21
            gcEx.extensionIntroducer = gifBytes[byteIndex];
            byteIndex++;

            // Graphic Control Label(1 Byte)
            // 0xf9
            gcEx.graphicControlLabel = gifBytes[byteIndex];
            byteIndex++;

            // Block Size(1 Byte)
            // 0x04
            gcEx.blockSize = gifBytes[byteIndex];
            byteIndex++;

            // 1 Byte
            {
                // Reserved(3 Bits)
                // Unused

                // Disposal Mothod(3 Bits)
                // 0 (No disposal specified)
                // 1 (Do not dispose)
                // 2 (Restore to background color)
                // 3 (Restore to previous)
                switch (gifBytes[byteIndex] & 28)
                { // 0b00011100
                    case 4:     // 0b00000100
                        gcEx.disposalMethod = 1;
                        break;
                    case 8:     // 0b00001000
                        gcEx.disposalMethod = 2;
                        break;
                    case 12:    // 0b00001100
                        gcEx.disposalMethod = 3;
                        break;
                    default:
                        gcEx.disposalMethod = 0;
                        break;
                }

                // User Input Flag(1 Bit)
                // Unknown

                // Transparent Color Flag(1 Bit)
                gcEx.transparentColorFlag = (gifBytes[byteIndex] & 1) == 1; // 0b00000001

                byteIndex++;
            }

            // Delay Time(2 Bytes)
            gcEx.delayTime = BitConverter.ToUInt16(gifBytes, byteIndex);
            byteIndex += 2;

            // Transparent Color Index(1 Byte)
            gcEx.transparentColorIndex = gifBytes[byteIndex];
            byteIndex++;

            // Block Terminator(1 Byte)
            gcEx.blockTerminator = gifBytes[byteIndex];
            byteIndex++;

            if (gifData.graphicCtrlExList == null)
            {
                gifData.graphicCtrlExList = new List<GraphicControlExtension>();
            }
            gifData.graphicCtrlExList.Add(gcEx);
        }

        static void SetCommentExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            CommentExtension commentEx = new CommentExtension();

            // Extension Introducer(1 Byte)
            // 0x21
            commentEx.extensionIntroducer = gifBytes[byteIndex];
            byteIndex++;

            // Comment Label(1 Byte)
            // 0xfe
            commentEx.commentLabel = gifBytes[byteIndex];
            byteIndex++;

            // Block Size & Comment Data List
            while (true)
            {
                // Block Size(1 Byte)
                byte blockSize = gifBytes[byteIndex];
                byteIndex++;

                if (blockSize == 0x00)
                {
                    // Block Terminator(1 Byte)
                    break;
                }

                var commentDataBlock = new CommentExtension.CommentDataBlock();
                commentDataBlock.blockSize = blockSize;

                // Comment Data(n Byte)
                commentDataBlock.commentData = new byte[commentDataBlock.blockSize];
                for (int i = 0; i < commentDataBlock.commentData.Length; i++)
                {
                    commentDataBlock.commentData[i] = gifBytes[byteIndex];
                    byteIndex++;
                }

                if (commentEx.commentDataList == null)
                {
                    commentEx.commentDataList = new List<CommentExtension.CommentDataBlock>();
                }
                commentEx.commentDataList.Add(commentDataBlock);
            }

            if (gifData.commentExList == null)
            {
                gifData.commentExList = new List<CommentExtension>();
            }
            gifData.commentExList.Add(commentEx);
        }

        static void SetPlainTextExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            PlainTextExtension plainTxtEx = new PlainTextExtension();

            // Extension Introducer(1 Byte)
            // 0x21
            plainTxtEx.extensionIntroducer = gifBytes[byteIndex];
            byteIndex++;

            // Plain Text Label(1 Byte)
            // 0x01
            plainTxtEx.plainTextLabel = gifBytes[byteIndex];
            byteIndex++;

            // Block Size(1 Byte)
            // 0x0c
            plainTxtEx.blockSize = gifBytes[byteIndex];
            byteIndex++;

            // Text Grid Left Position(2 Bytes)
            // Not supported
            byteIndex += 2;

            // Text Grid Top Position(2 Bytes)
            // Not supported
            byteIndex += 2;

            // Text Grid Width(2 Bytes)
            // Not supported
            byteIndex += 2;

            // Text Grid Height(2 Bytes)
            // Not supported
            byteIndex += 2;

            // Character Cell Width(1 Bytes)
            // Not supported
            byteIndex++;

            // Character Cell Height(1 Bytes)
            // Not supported
            byteIndex++;

            // Text Foreground Color Index(1 Bytes)
            // Not supported
            byteIndex++;

            // Text Background Color Index(1 Bytes)
            // Not supported
            byteIndex++;

            // Block Size & Plain Text Data List
            while (true)
            {
                // Block Size(1 Byte)
                byte blockSize = gifBytes[byteIndex];
                byteIndex++;

                if (blockSize == 0x00)
                {
                    // Block Terminator(1 Byte)
                    break;
                }

                var plainTextDataBlock = new PlainTextExtension.PlainTextDataBlock();
                plainTextDataBlock.blockSize = blockSize;

                // Plain Text Data(n Byte)
                plainTextDataBlock.plainTextData = new byte[plainTextDataBlock.blockSize];
                for (int i = 0; i < plainTextDataBlock.plainTextData.Length; i++)
                {
                    plainTextDataBlock.plainTextData[i] = gifBytes[byteIndex];
                    byteIndex++;
                }

                if (plainTxtEx.plainTextDataList == null)
                {
                    plainTxtEx.plainTextDataList = new List<PlainTextExtension.PlainTextDataBlock>();
                }
                plainTxtEx.plainTextDataList.Add(plainTextDataBlock);
            }

            if (gifData.plainTextExList == null)
            {
                gifData.plainTextExList = new List<PlainTextExtension>();
            }
            gifData.plainTextExList.Add(plainTxtEx);
        }

        static void SetApplicationExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
        {
            // Extension Introducer(1 Byte)
            // 0x21
            gifData.appEx.extensionIntroducer = gifBytes[byteIndex];
            byteIndex++;

            // Extension Label(1 Byte)
            // 0xff
            gifData.appEx.extensionLabel = gifBytes[byteIndex];
            byteIndex++;

            // Block Size(1 Byte)
            // 0x0b
            gifData.appEx.blockSize = gifBytes[byteIndex];
            byteIndex++;

            // Application Identifier(8 Bytes)
            gifData.appEx.appId1 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId2 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId3 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId4 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId5 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId6 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId7 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appId8 = gifBytes[byteIndex];
            byteIndex++;

            // Application Authentication Code(3 Bytes)
            gifData.appEx.appAuthCode1 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appAuthCode2 = gifBytes[byteIndex];
            byteIndex++;
            gifData.appEx.appAuthCode3 = gifBytes[byteIndex];
            byteIndex++;

            // Block Size & Application Data List
            while (true)
            {
                // Block Size (1 Byte)
                byte blockSize = gifBytes[byteIndex];
                byteIndex++;

                if (blockSize == 0x00)
                {
                    // Block Terminator(1 Byte)
                    break;
                }

                var appDataBlock = new ApplicationExtension.ApplicationDataBlock();
                appDataBlock.blockSize = blockSize;

                // Application Data(n Byte)
                appDataBlock.applicationData = new byte[appDataBlock.blockSize];
                for (int i = 0; i < appDataBlock.applicationData.Length; i++)
                {
                    appDataBlock.applicationData[i] = gifBytes[byteIndex];
                    byteIndex++;
                }

                if (gifData.appEx.appDataList == null)
                {
                    gifData.appEx.appDataList = new List<ApplicationExtension.ApplicationDataBlock>();
                }
                gifData.appEx.appDataList.Add(appDataBlock);
            }
        }
        #endregion
    }
}
