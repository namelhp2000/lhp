using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 图片操作帮助类
    /// </summary>
    public static class ImgHelper
    {
        /// <summary>
        /// 剪切图片
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <param name="newImgPath">新图片路径</param>
        /// <param name="pointX">坐标X</param>
        /// <param name="pointY">坐标Y</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static bool CutAvatar(string imgUrl, string newImgPath, int pointX = 0, int pointY = 0, int width = 0, int height = 0)
        {
            Bitmap originalImage = null;
            Image image = null;
            Graphics graphics = null;
            Image image2 = null;
            bool flag;
            try
            {
                int thumMaxWidth = 180;
                int thumMaxHeight = 180;
                if (!string.IsNullOrEmpty(imgUrl))
                {
                    originalImage = new Bitmap(width, height);
                    image = Image.FromFile(imgUrl);
                    graphics = Graphics.FromImage(originalImage);

                    graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(pointX, pointY, width, height), GraphicsUnit.Pixel);
                    image2 = GetThumbNailImage(originalImage, thumMaxWidth, thumMaxHeight);
                    EncoderParameters parameters = new EncoderParameters();
                    long[] numArray = new long[] { 80L };
                    EncoderParameter parameter = new EncoderParameter(Encoder.Quality, numArray);
                    parameters.Param[0] = parameter;
                    ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo info = null;
                    for (int i = 0; i < imageEncoders.Length; i++)
                    {
                        if (imageEncoders[i].FormatDescription.Equals("JPEG"))
                        {
                            info = imageEncoders[i];
                            break;
                        }
                    }
                    string path = newImgPath;
                    string directoryName = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    if (info != null)
                    {
                        image2.Save(path, info, parameters);
                    }
                    else
                    {
                        image2.Save(path);
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            finally
            {
                originalImage.Dispose();
                image.Dispose();
                graphics.Dispose();
                image2.Dispose();
                GC.Collect();
            }
            return flag;
        }

        /// <summary>
        /// 设置新图片大小
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="imageOriginalWidth">图片原始宽度</param>
        /// <param name="imageOriginalHeight">图片原始高度</param>
        /// <returns></returns>
        public static Size GetNewSize(int maxWidth, int maxHeight, int imageOriginalWidth, int imageOriginalHeight)
        {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = Convert.ToDouble(imageOriginalWidth);
            double num4 = Convert.ToDouble(imageOriginalHeight);
            double num5 = Convert.ToDouble(maxWidth);
            double num6 = Convert.ToDouble(maxHeight);
            if ((num3 < num5) && (num4 < num6))
            {
                num = num3;
                num2 = num4;
            }
            else if ((num3 / num4) > (num5 / num6))
            {
                num = maxWidth;
                num2 = (num * num4) / num3;
            }
            else
            {
                num2 = maxHeight;
                num = (num2 * num3) / num4;
            }
            return new Size(Convert.ToInt32(num), Convert.ToInt32(num2));
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="originalImage">原始图片</param>
        /// <param name="thumMaxWidth">缩略图宽度</param>
        /// <param name="thumMaxHeight">缩略图高度</param>
        /// <returns></returns>
        public static Image GetThumbNailImage(Image originalImage, int thumMaxWidth, int thumMaxHeight)
        {
            Size empty = Size.Empty;
            Image image = originalImage;
            Graphics graphics = null;
            try
            {
                empty = GetNewSize(thumMaxWidth, thumMaxHeight, originalImage.Width, originalImage.Height);
                image = new Bitmap(empty.Width, empty.Height);
                graphics = Graphics.FromImage(image);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, empty.Width, empty.Height), new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
            }
            catch
            {
            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
            }
            return image;
        }

        /// <summary>
        /// 从文件获取图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static Image GetImgFromFile(string fileName)
        {
            return Image.FromFile(fileName);
        }

        /// <summary>
        /// 从base64字符串读入图片
        /// </summary>
        /// <param name="base64">base64字符串</param>
        /// <returns></returns>
        public static Image GetImgFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            Image img = Image.FromStream(memStream);

            return img;
        }

        /// <summary>
        /// 从URL格式的Base64图片获取真正的图片
        /// 即去掉data:image/jpg;base64,这样的格式
        /// </summary>
        /// <param name="base64Url">图片Base64的URL形式</param>
        /// <returns></returns>
        public static Image GetImgFromBase64Url(string base64Url)
        {
            string base64 = GetBase64String(base64Url);

            return GetImgFromBase64(base64);
        }

        /// <summary>
        /// 压缩图片
        /// 注:等比压缩
        /// </summary>
        /// <param name="img">原图片</param>
        /// <param name="width">压缩后宽度</param>
        /// <returns></returns>
        public static Image CompressImg(Image img, int width)
        {
            return CompressImg(img, width, (int)(((double)width) / img.Width * img.Height));
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="img">原图片</param>
        /// <param name="width">压缩后宽度</param>
        /// <param name="height">压缩后高度</param>
        /// <returns></returns>
        public static Image CompressImg(Image img, int width, int height)
        {
            Bitmap bitmap = new Bitmap(img, width, height);

            return bitmap;
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 默认使用jpg格式
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns></returns>
        public static string ToBase64String(Image img)
        {
            return ToBase64String(img, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 使用指定格式
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="imageFormat">指定格式</param>
        /// <returns></returns>
        public static string ToBase64String(Image img, ImageFormat imageFormat)
        {
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, imageFormat);
            byte[] bytes = memStream.ToArray();
            string base64 = Convert.ToBase64String(bytes);

            return base64;
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 默认使用jpg格式,并添加data:image/jpg;base64,前缀
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns></returns>
        public static string ToBase64StringUrl(Image img)
        {
            return "data:image/jpg;base64," + ToBase64String(img, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 使用指定格式,并添加data:image/jpg;base64,前缀
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="imageFormat">指定格式</param>
        /// <returns></returns>
        public static string ToBase64StringUrl(Image img, ImageFormat imageFormat)
        {
            string base64 = ToBase64String(img, imageFormat);

            return $"data:image/{imageFormat.ToString().ToLower()};base64,{base64}";
        }

        /// <summary>
        /// 获取真正的图片base64数据
        /// 即去掉data:image/jpg;base64,这样的格式
        /// </summary>
        /// <param name="base64UrlStr">带前缀的base64图片字符串</param>
        /// <returns></returns>
        public static string GetBase64String(string base64UrlStr)
        {
            string parttern = "^(data:image/.*?;base64,).*?$";

            var match = Regex.Match(base64UrlStr, parttern);
            if (match.Groups.Count > 1)
                base64UrlStr = base64UrlStr.Replace(match.Groups[1].ToString(), "");

            return base64UrlStr;
        }

        /// <summary>
        /// 将图片的URL或者Base64字符串转为图片并上传到服务器，返回上传后的完整图片URL
        /// </summary>
        /// <param name="imgBase64OrUrl">URL地址或者Base64字符串</param>
        /// <returns></returns>
        public static string GetImgUrl(string imgBase64OrUrl)
        {
            if (imgBase64OrUrl.Contains("data:image"))
            {
                Image img = ImgHelper.GetImgFromBase64Url(imgBase64OrUrl);
                string fileName = $"{Guid.NewGuid().ToSequentialGuid().ToUpper()}.jpg";

                string dir = Path.Combine(Config.FilePath, "Upload", "Img");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                img.Save(Path.Combine(dir, fileName));

                return $"{Config.FilePath}/Upload/Img/{fileName}";
            }
            else
                return imgBase64OrUrl;
        }




    }


}
