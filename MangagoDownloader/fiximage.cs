﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ImageProcessor.Imaging;
using ImageProcessor.Processors;
using ImageProcessor.Configuration;
using ImageProcessor.Common;
using ImageProcessor;
using System.IO;
using System.Net;


namespace MangagoDownloader
{
    public class fiximage
    {
        private Dictionary<string, string> imagekeys = new Dictionary<string, string>()
        {
            //Keys provided by the tachiyomi extension for Mangago
            {"60a2b0ed56cd458c4633d04b1b76b7e9","18a72a69a64a13a1a43a3aa42a23a66a26a19a51a54a78a34a17a31a35a15a58a29a61a48a73a74a44a52a60a24a63a20a32a7a45a53a75a55a62a59a41a76a68a2a36a21a10a38a33a71a40a67a22a4a50a80a65a27a37a47a70a14a28a16a6a56a30a57a5a11a79a9a77a46a39a25a49a8a12"},
            {"400df5e8817565e28b2e141c533ed7db","61a74a10a45a3a37a72a22a57a39a25a56a52a29a70a60a67a41a63a55a27a28a43a18a5a9a8a40a17a48a44a79a38a47a32a73a4a6a13a34a33a49a2a42a50a76a54a36a35a14a58a7a69a46a16a30a21a11aa51a53a77a26a31a1a19a20a80a24a62a68a59a66a75a12a64a78a71a15a65a23"},
            {"84ba0d8098f405b14f4dbbcc04c93bac","61a26a35a16a55a10a72a37a2a60a66a65a33a44a7a28a70a62a32a56a30a40a58a15a74a47aa36a78a75a11a6a77a67a39a23a9a31a64a59a13a24a80a14a38a45a21a63a19a51a17a34a50a46a5a29a73a8a57a69a48a68a49a71a41a12a52a18a79a76a54a42a22a4a1a3a53a20a25a43a27"},
            {"56665708741979f716e5bd64bf733c33","23a7a41a48a57a27a69a36a76a62a40a75a26a2a51a6a10a65a43a24a1aa20a71a28a30a13a38a79a78a72a14a49a55a56a58a25a70a12a80a3a66a11a39a42a17a15a54a45a34a74a31a8a61a46a73a63a22a64a19a77a50a9a59a37a68a52a18a32a16a33a60a67a21a44a53a5a35a4a29a47"},
            {"a67e15ed870fe4aab0a502478a5c720f","8a12a59a52a24a13a37a21a55a56a41a71a65a43a40a66a11a79a67a44a33a20a72a2a31a42a29a34a58a60a27a48a28a15a35a51a76a80a0a63a69a53a39a46a64a50a75a1a57a9a62a74a18a16a73a14a17a6a19a61a23a38a10a3a32a26a36a54a4a30a45a47a70a22a7a68a49a77a5a25a78"},
            {"b6a2f75185754b691e4dfe50f84db57c","47a63a76a58a37a4a56a21a1a48a62a2a36a44a34a42a23a9a60a72a11a74a70a20a77a16a15a35a69a0a55a46a24a6a32a75a68a43a41a78a31a71a52a33a67a25a80a30a5a28a40a65a39a14a29a64a3a53a49a59a12a66a38a27a79a45a18a22a8a61a50a17a51a10a26a13a57a19a7a54a73"},
            {"db99689c5a26a09d126c7089aedc0d86","57a31a46a61a55a41a26a2a39a24a75a4a45a13a23a51a15a8a64a37a72a34a12a3a79a42a80a17a62a49a19a77a48a68a78a65a14a10a29a16a20a76a38a36a54a30a53a40a33a21a44a22a32a5a1a7a70a67a58a0a71a74a43a66a6a63a35a56a73a9a27a25a59a47a52a11a50a18a28a60a69"},
            {"37abcb7424ce8df47ccb1d2dd9144b49","67a45a39a72a35a38a61a11a51a60a13a22a31a25a75a30a74a43a69a50a6a26a16a49a77a68a59a64a17a56a18a1a10a54a44a62a53a80a5a23a48a32a29a79a24a70a28a58a71a3a52a42a55a9a14a36a73a34a2a27a57a0a21a41a33a37a76a8a40a65a7a20a12a19a47a4a78a15a63a66a46"},
            {"874b83ba76a7e783d13abc2dabc08d76","26a59a42a43a4a20a61a28a12a64a37a52a2a77a34a13a46a74a70a0a44a29a73a66a55a38a69a67a62a9a63a6a54a79a21a33a8a58a40a47a71a49a22a50a57a78a56a25a17a15a36a16a48a32a5a10a14a80a24a72a76a45a3a53a23a41a60a11a65a19a27a51a68a35a31a1a75a39a30a7a18"},
            {"d320d2647d70c068b89853e1a269c609","77a38a53a40a16a3a20a18a63a9a24a64a50a61a45a59a27a37a8a34a11a55a79a13a47a68a12a22a46a33a1a69a52a54a31a23a62a43a0a2a35a28a57a36a51a78a70a5a32a75a41a30a4a80a19a21a42a71a49a10a56a74a17a7a25a6a14a73a29a44a48a39a60a58a15a66a67a72a65a76a26"},
            {"930b87ad89c2e2501f90d0f0e92a6b97","9a29a49a67a62a40a28a50a64a77a46a31a16a73a14a45a51a44a7a76a22a78a68a37a74a69a25a65a41a11a52aa18a36a10a38a12a15a2a58a48a8a27a75a20a4a80a61a55a42a13a43a47a39a35a60a26a30a63a66a57a33a72a24a71a34a23a3a70a54a56a32a79a5a21a6a59a53a17a1a19"},
            {"c587e77362502aaedad5b7cddfbe3a0d","50aa59a70a68a30a56a10a49a43a45a29a23a28a61a15a40a71a14a44a32a34a17a26a63a76a75a33a74a12a11a21a67a31a19a80a7a64a8a3a51a53a38a18a6a42a27a9a52a20a41a60a1a22a77a16a54a47a79a24a78a2a46a37a73a65a36a35a39a5a4a25a72a13a62a55a57a58a69a66a48"},
            {"1269606c6c3d8bb6508426468216d6b1","49a15a0a60a14a26a34a69a61a24a35a4a77a80a70a40a39a6a68a17a41a56a28a46a79a16a21a1a37a42a44a58a78a18a52a73a32a9a12a50a8a13a20a19a67a36a45a75a48a10a65a7a38a66a3a2a43a27a29a31a72a74a55a23a54a22a59a57a11a62a47a53a30a5a64a25a76a71a51a33a63"},
            {"33a3b21bb2d14a09d15f995224ae4284","30a59a35a34a42a8a10a56a70a64a48a69a26a18a6a16a54a24a73a79a68a33a32a2a63a53a31a14a17a57a41a80a76a40a60a12a43a29a39a4a77a58a66a36a38a52a13a19a0a75a28a55a25a61a71a11a67a49a23a45a5a15a1a50a51a9a44a47a65a74a72a27a7a37a46a20a22a62a78a21a3"},
            {"9ae6640761b947e61624671ef841ee78","62a25a21a75a42a61a73a59a23a19a66a38a71a70a6a55a3a16a43a32a53a37a41a28a49a63a47a17a7a30a78a46a20a67a56a79a65a14a69a60a8a52a22a9a24a2a4a13a36a27a0a18a33a12a44a5a76a26a29a40a1a11a64a48a39a51a80a72a68a10a58a35a77a54a34a74a57a31a50a45a15"},
            {"f4ab0903149b5d94baba796a5cf05938","40a37a55a73a18a42a15a59a50a13a22a63a52a58a6a80a47a17a38a71a74a70a30a11a10a19a0a31a36a21a51a68a1a3a14a66a45a2a79a7a76a75a8a67a20a78a25a69a43a28a35a60a4a23a65a54a34a9a5a39a27a57a26a33a12a24a46a72a56a44a49a61a64a29a53a48a32a62a41a16a77"},
            {"f5baf770212313f5e9532ec5e6103b61","55a69a78a75a38a25a20a60a6a80a46a5a48a18a23a24a17a67a64a70a63a57a22a10a49a19a8a16a11a12a61a76a34a27a54a73a44a0a56a3a15a29a28a13a4a2a7a77a74a35a37a26a30a58a9a71a50a1a43a79a47a32a14a53a52a66a72a59a68a31a42a45a62a51a40a39a33a65a41a36a21"},
            {"e2169a4bfd805e9aa21d3112d498d68c","54a34a68a69a26a20a66a1a67a74a22a39a63a70a5a37a75a15a6a14a62a50a46a35a44a45a28a8a40a25a29a76a51a77a17a47a0a42a2a9a48a27a13a64a58a57a18a30a80a23a61a36a60a59a71a32a7a38a41a78a12a49a43a79a24a31a52a19a3a53a72a10a73a11a33a16a4a55a65a21a56"},
            {"1796550d20f64decb317f9b770ba0e78","37a55a39a79a2a53a75a1a30a32a3a13a25a49a45a5a60a62a71a78a63a24a27a33a19a64a67a57a0a8a54a9a41a61a50a73a7a65a58a51a15a14a43a4a35a77a68a72a34a80a22a17a48a10a70a46a40a28a20a74a52a23a38a76a42a18a66a11a59a6a69a31a56a16a47a21a12a44a36a29a26"},
            {"bf53be6753a0037c6d80ca670f5d12d5","55a41a18a19a4a13a36a12a56a69a64a80a30a39a57a50a48a26a46a73a17a52a49a66a11a25a61a51a68a24a70a7a67a53a43a8a29a75a65a42a38a58a9a28a0a78a54a31a22a5a15a3a79a77a59a23a45a40a47a44a6a2a1a35a14a62a63a76a20a16a32a21a71a10a74a60a34a37a33a72a27"},
            {"6c41ff7fbed622aa76e19f3564e5d52a","40a3a13a59a68a34a66a43a67a14a26a46a8a24a33a73a69a31a2a57a10a51a62a77a74a41a47a35a64a52a15a53a6a80a76a50a28a75a56a79a17a45a25a49a48a65a78a27a9a63a12a55a32a21a58a38a0a71a44a30a61a36a16a23a20a70a22a37a4a19a7a60a11a5a18a39a1a54a72a29a42"}
        };
        //default image key from the same tachiyomi extension
        private const string defimgkey = "18a72a69a64a13a1a43a3aa42a23a66a26a19a51a54a78a34a17a31a35a15a58a29a61a48a73a74a44a52a60a24a63a20a32a7a45a53a75a55a62a59a41a76a68a2a36a21a10a38a33a71a40a67a22a4a50a80a65a27a37a47a70a14a28a16a6a56a30a57a5a11a79a9a77a46a39a25a49a8a12";
        
        public byte[] DescrambleImage(string url)
        {
            Uri u = new Uri(url);

            string key;

            if (imagekeys.ContainsKey(Path.GetFileNameWithoutExtension(u.LocalPath)))
            {
                key = imagekeys.Where(x => x.Key == Path.GetFileNameWithoutExtension(u.LocalPath)).First().Value;
            }
            else
            {
                key = defimgkey;
            }

            List<string> skey = key.Split('a').ToList();
            List<int> fkey = new List<int>();
            for(int i = 0; i < skey.Count; i++)
            {
                string k = skey[i];
                if (!String.IsNullOrEmpty(k))
                {
                    fkey.Add(int.Parse(k));
                }
                else
                {
                    fkey.Add(0);
                }
            }
            int dimmult = 9;
            Image img = GetImageFromURL(url);
            img.Save(@".\tmp\tmpscrambled.jpeg");
            img = Image.FromFile(@".\tmp\tmpscrambled.jpeg");
            int w = img.Width;
            int h = img.Height;
            int tile_h = h/dimmult;
            int tile_w = w / dimmult;
            Bitmap final = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(final);


            MemoryStream ms = new MemoryStream();
            for (int i = 0; i < 81; i++)
            {
 
                int r = (int)Math.Floor(fkey[i] / 9d);
                int dy = tile_h * r;
                int dx = (fkey[i] - r*9) * tile_w;
                int sr = (int)Math.Floor(i / 9d);
                int sy = sr * tile_h;
                int sx = (i - sr*9) * tile_w;

                g.DrawImage(img, new Rectangle(dx, dy, tile_w, tile_h), sx, sy, tile_w, tile_h, GraphicsUnit.Pixel);
                g.Flush();
            }
            
            img.Dispose();
            g.Dispose();
            final.Save(ms, ImageFormat.Jpeg);
            byte[] bytes = ms.ToArray();
            return bytes;
        }

       

        public Image GetImageFromURL(string url)
        {
            WebClient w = new WebClient();
            byte[] bytes = w.DownloadData(url);
            MemoryStream m = new MemoryStream(bytes);
            Image img = Image.FromStream(m);

            return img;

        }



    }
}
