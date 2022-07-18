using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace SymPdf
{
    internal class Hyperlink
    {
        static private readonly string fileName = "hyperlink.csv";
        static private readonly char[] quot = new[] { '"' };

        public int Page { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool HasBorder { get; private set; }
        public string Uri { get; private set; }

        public Hyperlink(int page, float x, float y, float width, float height, bool hasBorder, string uri)
        {
            this.Page = page;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.HasBorder = hasBorder;
            this.Uri = uri;
        }

        public static IList<Hyperlink> ReadSettings()
        {
            var settings = new List<Hyperlink>();
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = Path.Combine(dir, fileName);

            if (!File.Exists(file))
            {
                throw new Exception("設定ファイルが存在しません。[" + file + "]");
            }
            else
            {
                var lines = File.ReadAllLines(file);
                for (int i = 1; i < lines.Length; i++)
                {
                    // FIXME: カンマを含む値に対応しないと括った意味ない
                    var items = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                    try
                    {
                        var idx = 0;
                        var setting = new Hyperlink(
                            int.Parse(items[idx++].Trim(quot)),
                            float.Parse(items[idx++].Trim(quot)),
                            float.Parse(items[idx++].Trim(quot)),
                            float.Parse(items[idx++].Trim(quot)),
                            float.Parse(items[idx++].Trim(quot)),
                            bool.Parse(items[idx++].Trim(quot)),
                            items[idx++].Trim(quot));
                        settings.Add(setting);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("設定ファイルが不正です。[" + (i + 1) + "行目：" + lines[i] + "]", ex);
                    }
                }
            }

            return settings;
        }
    }
}
