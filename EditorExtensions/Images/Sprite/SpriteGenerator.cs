using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MadsKristensen.EditorExtensions.Images
{
  internal static class SpriteGenerator
  {
    public async static Task<IEnumerable<SpriteFragment>> MakeImage(SpriteDocument document, string imageFile, Func<string, bool, Task> updateSprite)
    {
      ProjectHelpers.CheckOutFileFromSourceControl(imageFile);

      Dictionary<string, Image> images = await WatchFiles(document, updateSprite);

      var width = 0;
      var height = 0;
      var cols = 0;
      
      if (document.Direction == SpriteDirection.Vertical)
      {
        width = images.Values.Max(i => i.Width) + (document.Margin * 2);
        height = images.Values.Sum(img => img.Height) + (document.Margin * images.Count);
      }
      else if (document.Direction == SpriteDirection.Horizontal)
      {
        width = images.Values.Sum(i => i.Width) + (document.Margin * images.Count) + document.Margin;
        height = images.Values.Max(img => img.Height) + (document.Margin * 2);
      }
      else if (document.Direction == SpriteDirection.Both)
      {
        var sqrt = Math.Sqrt(images.Count);
        cols = Convert.ToInt32(Math.Ceiling(sqrt));
        var rows = (sqrt < (Math.Floor(sqrt) + 0.5)) ? cols - 1 : cols;
        
        // In this case, the remainder will create another row
        width = (images.Values.Max(img => img.Width) * cols) + (document.Margin * cols) + document.Margin;
        height = (images.Values.Max(img => img.Height) * rows) + (document.Margin * rows) + document.Margin;
      }

      List<SpriteFragment> fragments = new List<SpriteFragment>();

      using (var bitmap = new Bitmap(width, height))
      {
        using (Graphics canvas = Graphics.FromImage(bitmap))
        {
          switch (document.Direction)
          {
          case SpriteDirection.Both:
            Both(images, fragments, canvas, document.Margin);
            break;
          case SpriteDirection.Horizontal:
            Horizontal(images, fragments, canvas, document.Margin);
            break;
          case SpriteDirection.Vertical:
          default:
            Vertical(images, fragments, canvas, document.Margin);
            break;
          }

          bitmap.Save(imageFile, PasteImage.GetImageFormat("." + document.FileExtension));
        }
      }

      return fragments;
    }

    public async static Task<Dictionary<string, Image>> WatchFiles(SpriteDocument document, Func<string, bool, Task> updateSprite)
    {
      if (document == null)
        return null;

      Dictionary<string, Image> images = GetImages(document);

      await new BundleFileObserver().AttachFileObserver(document, document.FileName, updateSprite);

      foreach (string file in images.Keys)
      {
        await new BundleFileObserver().AttachFileObserver(document, file, updateSprite);
      }

      return images;
    }
    private static void Both(Dictionary<String, Image> images, List<SpriteFragment> fragments ,Graphics canvas, int margin)
    {
      int currentY = margin;
      int currentX = margin;
      var cols = Math.Ceiling(Math.Sqrt(images.Count));
      // Lazy way of making sure images don't overlap is to make ALL OF THEM to be the size of the largest one
      var rowHeight = images.Max(img => img.Value.Height);
      var colWidth = images.Max(img => img.Value.Width);

      Queue<String> imageQueue = new Queue<String>(images.Keys);

      while (imageQueue.Count > 0)
      {
        for (var c = 0; c < cols && imageQueue.Count > 0; c++)
        {
          var imgKey = imageQueue.Dequeue();
          Image img = images[imgKey];
          fragments.Add(new SpriteFragment(imgKey, img.Width, img.Height, currentX, currentY));
          canvas.DrawImage(img, currentX, currentY);
          currentX += colWidth + margin;
        }
        currentY += rowHeight + margin;
        currentX = margin;
      }

    }
    private static void Vertical(Dictionary<string, Image> images, List<SpriteFragment> fragments, Graphics canvas, int margin)
    {
      int currentY = margin;

      foreach (string file in images.Keys)
      {
        Image img = images[file];
        fragments.Add(new SpriteFragment(file, img.Width, img.Height, margin, currentY));

        canvas.DrawImage(img, margin, currentY);
        currentY += img.Height + margin;
      }
    }

    private static void Horizontal(Dictionary<string, Image> images, List<SpriteFragment> fragments, Graphics canvas, int margin)
    {
      int currentX = margin;

      foreach (string file in images.Keys)
      {
        Image img = images[file];
        fragments.Add(new SpriteFragment(file, img.Width, img.Height, currentX, margin));

        canvas.DrawImage(img, currentX, margin);
        currentX += img.Width + margin;
      }
    }

    private static Dictionary<string, Image> GetImages(SpriteDocument sprite)
    {
      Dictionary<string, Image> images = new Dictionary<string, Image>();

      foreach (string file in sprite.BundleAssets)
      {
        Image image = Image.FromFile(file);

        // Only touch the resolution of the image if it isn't 96. 
        // That way we keep the original image 'as is' in all other cases.
        if (Math.Round(image.VerticalResolution) != 96F || Math.Round(image.HorizontalResolution) != 96F)
          image = new Bitmap(image);

        images.Add(file, image);
      }

      return images;
    }
  }
}
