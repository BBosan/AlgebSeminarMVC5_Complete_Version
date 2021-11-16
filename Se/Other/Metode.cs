using Se.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity; //https://stackoverflow.com/questions/23562368/cannot-convert-lambda-expression-to-type-string-because-it-is-not-a-delegate-t
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.Helpers;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Web.Hosting;
using System.Web.Routing;

namespace Se.Other
{
    public static class Metode
    {
        public static string IsActive(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "activeLink")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (String.IsNullOrEmpty(actions))
                actions = currentAction;

            if (String.IsNullOrEmpty(controllers))
                controllers = currentController;

            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static bool IsSortQueryValid(string sort)
        {
            string[] sort_keywords = new string[] {
             "naziv_silaz",
             "naziv_uzlaz",
             "datum_uzlaz",
             "datum_silaz",
             "ime_uzlaz",
             "ime_silaz",
             "prezime_silaz",
             "prezime_uzlaz",
             "adresa_silaz",
             "adresa_uzlaz",
             "broj_uzlaz",
             "broj_silaz",
             "",
             null
            };

            return sort_keywords.Any(x => x == sort);
        }

        #region StaroSaWebImageKojiNeradiUCore
        //public static FileContentResult imgQueryTroll(string sort)
        //{
        //    NameValueCollection col = HttpContext.Current.Request.QueryString;
        //    string allQueries = "";
        //    for (int i = 0; i < col.Count; i++)
        //    {
        //        allQueries += string.Format("{0}. {1}={2}", (i + 1), (col.AllKeys[i]), ((col[i].Length > 10) ? col[i].Substring(0, 10) + "..." : col[i]));
        //        if (i < col.Count - 1)
        //        {
        //            allQueries += "\n";
        //        }
        //    }

        //    var watermarkText = sort.Length > 10 ? sort.Substring(0, 7) + "..." : sort;
        //    var image = new WebImage(HttpContext.Current.Server.MapPath(Path.Combine("~.", VirtualPathUtility.ToAbsolute("~/Content/watermark/cage.png") )));
        //    image.Resize(500, 500, true, true);

        //    if (col.Keys.Count > 1)
        //    {
        //        image.AddTextWatermark(allQueries, fontSize: 10, fontColor: "white", horizontalAlign: "Center", verticalAlign: "Middle", opacity: 30);
        //    }

        //    image.AddTextWatermark(HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString(), fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Bottom", opacity: 80);
        //    image.AddTextWatermark(watermarkText.ToUpper(), fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Top", opacity: 70);
        //    image.AddTextWatermark(HttpContext.Current.Request.Browser.Browser + "/" + HttpContext.Current.Request.Browser.Version, fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Top", opacity: 80);
        //    image.AddTextWatermark("WHAT?", fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Bottom", opacity: 70);

        //    byte[] content = image.GetBytes("image/png");

        //    FileContentResult result = new FileContentResult(content, "image/png")
        //    {
        //        //FileDownloadName = "cage_rage",
        //    };

        //    return result;
        //} 
        #endregion

        #region practice2
        //public static Bitmap ConvertTextToImage(string txt, Font font, Color fcolor, int width, int Height)
        //{
        //    Bitmap bmp = new Bitmap(width, Height);
        //    using (Graphics graphics = Graphics.FromImage(bmp))
        //    {
        //        graphics.DrawString(txt, font, new SolidBrush(Color.FromArgb(31, fcolor)), 0, 0);
        //        graphics.Flush();
        //        font.Dispose();
        //        graphics.Dispose();
        //    }
        //    return bmp;
        //}
        #endregion

        private static Bitmap CreateImageFromText(string Text, Color? TextColor = null)
        {
            Text = Text.Trim();
            TextColor = TextColor ?? Color.ForestGreen; //Default

            // Create the Font object for the image text drawing.
            Font textFont = new Font("Impact", 30, FontStyle.Bold, GraphicsUnit.Pixel);

            //create a dummy
            Bitmap ImageObject = new Bitmap(1, 1);

            Graphics GraphicsObject = Graphics.FromImage(ImageObject);

            //measure the string to see how big the image needs to be
            SizeF stringSize = GraphicsObject.MeasureString(Text, textFont);

            //free up the dummy image and old graphics object
            ImageObject.Dispose();
            GraphicsObject.Dispose();

            //create a new image of the right size
            ImageObject = new Bitmap((int)stringSize.Width + 20, (int)stringSize.Height + 20);
            GraphicsObject = Graphics.FromImage(ImageObject);

            // Set Background color
            GraphicsObject.Clear(Color.Transparent);

            GraphicsObject.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
            GraphicsObject.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            GraphicsObject.FillRectangle(new SolidBrush(Color.FromArgb(40, Color.GhostWhite)), 0, 0, (int)stringSize.Width + 16, (int)stringSize.Height + 16);
            //GraphicsObject.FillEllipse(new SolidBrush(Color.FromArgb(80, Color.Black)), 2, 2, (int)stringSize.Width + 12, (int)stringSize.Height + 12);
            GraphicsObject.DrawString(Text, textFont, new SolidBrush(Color.FromArgb(90, (Color)TextColor)), 8, 9);

            GraphicsObject.Dispose();

            return ImageObject;
        }

        public static FileContentResult imgQueryWatermarkTroll(string sort, string tiled_text="What?")
        {
            NameValueCollection col = HttpContext.Current.Request.QueryString;
            string allQueries = "\n";
            for (int i = 0; i < col.Count; i++)
            {
                allQueries += string.Format("{0}. {1}={2}", (i + 1), (col.AllKeys[i]), ((col[i].Length > 15) ? col[i].Substring(0, 15) + "..." : col[i]));
                if (i < col.Count - 1)
                {
                    allQueries += "\n";
                }
            }

            string watermarkText = sort.Length > 20 ? sort.Substring(0, 20) + "..." : sort;
            string image = HostingEnvironment.MapPath("~/Content/watermark/cage.png");
            #region otherWays
            //string image = HttpContext.Current.Server.MapPath("~/Content/watermark/cage.png");
            //string image = HttpContext.Current.Server.MapPath(Path.Combine("~.", VirtualPathUtility.ToAbsolute("~/Content/watermark/cage.png"))); 
            #endregion

            string tiled_watermark = HostingEnvironment.MapPath("~/Content/watermark/tiled_watermark.png");

            Font small_text_font = new Font("Impact", 22, FontStyle.Regular, GraphicsUnit.Pixel);
            Font large_text_font = new Font("Impact", 52, FontStyle.Regular, GraphicsUnit.Pixel);

            StringFormat bottom_center = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };
            StringFormat top_center = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            StringFormat top_left = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            StringFormat top_right = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near, FormatFlags = StringFormatFlags.LineLimit };

            FileContentResult output;
            Bitmap original = (Bitmap)Image.FromFile(image);
            using (Bitmap bitMapImage = new Bitmap(original, new Size(original.Width-140, original.Height-140)))
            {
                RectangleF rectf = new RectangleF(0, 0, bitMapImage.Width, bitMapImage.Height);
                //Rectangle rect = new Rectangle(0, 0, bitMapImage.Width, bitMapImage.Height);

                using (Graphics graphicImage = Graphics.FromImage(bitMapImage))
                {
                    graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
                    graphicImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphicImage.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    //Okviri
                    graphicImage.DrawRectangle(new Pen(Color.Bisque, 25), Rectangle.Round(rectf) /*convert rectf to rect*/);
                    graphicImage.DrawRectangle(new Pen(Color.Black, 10), Rectangle.Round(rectf) /*convert rectf to rect*/);

                    #region SimpleRoundedRectangle
                    //Pen myPen = new Pen(Brushes.Black);
                    //myPen.Width = 10.0f;
                    //// Set the LineJoin property
                    //myPen.LineJoin = LineJoin.Round;

                    ////myPen.DashStyle = DashStyle.Dot;
                    ////myPen.DashOffset = 20f;
                    //// Draw the rectangle
                    //graphicImage.DrawRectangle(myPen, 222, 222, 165, 55); //add manual params 

                    #endregion

                    if (col.Keys.Count > 1)
                    {
                        graphicImage.DrawString(allQueries.ToLower(), small_text_font, Brushes.White, rectf, top_center);
                    }
                    else
                    {
                        #region ImageTiledWatermark
                        using (TextureBrush brush = new TextureBrush(Image.FromFile(tiled_watermark), WrapMode.Tile))
                        {
                            brush.RotateTransform(-45);
                            //brush.TranslateTransform(-35, 0);
                            brush.ScaleTransform((float)1.6, (float)1.6, MatrixOrder.Prepend);
                            graphicImage.FillRectangle(brush, 0, 0, bitMapImage.Width, bitMapImage.Height);
                        }
                        #endregion

                        #region TextTiledWatermark
                        using (TextureBrush brush = new TextureBrush(CreateImageFromText(tiled_text, Color.Violet), WrapMode.Tile))
                        {
                            brush.RotateTransform(-45);
                            brush.ScaleTransform((float)1.3, (float)1.3, MatrixOrder.Prepend);
                            brush.TranslateTransform(-35, 0);
                            graphicImage.FillRectangle(brush, 0, 0, bitMapImage.Width, bitMapImage.Height);
                        }
                        #endregion

                        graphicImage.TranslateTransform(0, -10);

                        graphicImage.DrawString(watermarkText.ToUpper()+"?", large_text_font, Brushes.GhostWhite, rectf, bottom_center);
                        graphicImage.ResetTransform();

                        graphicImage.TranslateTransform(15, 15);

                        //Browser and Version
                        graphicImage.DrawString(
                            HttpContext.Current.Request.Browser.Browser.ToUpper() + "/" + HttpContext.Current.Request.Browser.Version, 
                            small_text_font,
                            new SolidBrush(Color.FromArgb(80, Color.Red)), 
                            rectf, top_left);

                        graphicImage.ResetTransform();

                        //Current Controller
                        graphicImage.TranslateTransform(-15, 15);
                        graphicImage.DrawString(
                            HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString(), 
                            small_text_font, 
                            new SolidBrush(Color.FromArgb(80, Color.Aquamarine)), 
                            rectf, top_right);
                    }
                    graphicImage.Flush();
                }

                using (var memoryStream = new MemoryStream())
                {
                    bitMapImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageByteArray = memoryStream.ToArray();
                    output = new FileContentResult(imageByteArray, "image/png") { };
                }
            }

            #region ShorterWithImageConverter
            //Image imageVerzija2 = Image.FromFile(image);
            //Graphics graph = Graphics.FromImage(imageVerzija2);
            //StringFormat sf2 = new StringFormat();
            //sf2.LineAlignment = StringAlignment.Center;
            //sf2.Alignment = StringAlignment.Center;
            //graph.DrawString(watermarkText.ToUpper(), new Font("Impact", 50, FontStyle.Bold), Brushes.Red, new RectangleF(0, 0, imageVerzija2.Width, imageVerzija2.Height), sf2);

            //ImageConverter converter = new ImageConverter();
            //byte[] imageByteArray2 = (byte[])converter.ConvertTo(imageVerzija2, typeof(byte[])); 
            //FileContentResult result = new FileContentResult(imageByteArray2, "image/png"){ };
            #endregion


            return /*result*/output;
        }


        public static int Starost(this DateTime datumRodjenja) 
        {
            #region other
            //DateTime n = DateTime.Now; // To avoid a race condition around midnight
            //int age = n.Year - datumRodjenja.Year;

            //if (n.Month < datumRodjenja.Month || (n.Month == datumRodjenja.Month && n.Day < datumRodjenja.Day))
            //    age--;

            //return age; 
            #endregion
            return (int)Math.Floor((DateTime.Now - datumRodjenja).TotalDays / 365.25D);
        }

        #region OtherTest
        //public static void ForEach2<T>(this IEnumerable<T> enumeration, Action<T> action)
        //{
        //    foreach (T item in enumeration)
        //    {
        //        action(item);
        //    }
        //}

        //public static void ForEachPredbiljezba(this IEnumerable<Predbiljezba> enumeration, int id = 2)
        //{
        //    foreach (Predbiljezba item in enumeration)
        //    {
        //        item.idSeminar = id;
        //    }
        //}

        //public static DateTime? TryParse(this string text)
        //{
        //    DateTime date;
        //    return DateTime.TryParse(text, out date) ? date : (DateTime?)null;
        //}

        //https://stackoverflow.com/a/3063267
        //public static string GenerateHASH(string pass, string salt)
        //{
        //    byte[] data = System.Text.Encoding.ASCII.GetBytes(salt + pass);
        //    data = System.Security.Cryptography.MD5.Create().ComputeHash(data);
        //    return Convert.ToBase64String(data);
        //} 
        #endregion

    }
}