using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA2ModManager48
{
    public class Helper
    {
        public static string FindSingleThumbImage(string folderPath)
        {
            string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

            foreach (var ext in extensions)
            {
                var files = Directory.GetFiles(folderPath, $"Thumb{ext}", SearchOption.AllDirectories);
                if (files.Length > 0)
                    return files[0]; // Return first match
            }

            return null; // No match found
        }


        public static void FindOriginalFiles(string startDirectory)
        {
            try
            {
                string[] files = Directory.GetFiles(startDirectory, "*.original", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    // Do something with the file (e.g., print it)
                    Debug.WriteLine(file);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static void CopyWithStructureAndBackup(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

            foreach (string sourceFilePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                // Compute the relative path from source root
                string relativePath = dotnet48shim.GetRelativePath(sourceDir, sourceFilePath);

                // Compute the corresponding target file path
                string targetFilePath = Path.Combine(targetDir, relativePath);

                // Ensure the target directory exists
                string targetFolder = Path.GetDirectoryName(targetFilePath);
                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                // If file exists in target, rename it by adding .original extension
                if (File.Exists(targetFilePath))
                {
                    string backupFilePath = targetFilePath + ".original";

                    // If backup file exists, delete or handle it (here we delete)
                    if (File.Exists(backupFilePath))
                        // TODO: DO BETTER!
                        throw new DirectoryNotFoundException($"Cannot backup a backup");

                    File.Move(targetFilePath, backupFilePath);
                }

                // Copy the file from source to target
                File.Copy(sourceFilePath, targetFilePath);
            }
        }


        public static void ReverseOriginalRestore(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            if (!Directory.Exists(targetDir))
                throw new DirectoryNotFoundException($"Target directory not found: {targetDir}");

            foreach (string sourceFilePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
            {
                string relativePath = dotnet48shim.GetRelativePath(sourceDir, sourceFilePath);
                string targetFilePath = Path.Combine(targetDir, relativePath);
                string originalBackupPath = targetFilePath + ".original";

                // If target has the file and source does too, delete the target copy
                if (File.Exists(targetFilePath))
                {
                    File.SetAttributes(targetFilePath, FileAttributes.Normal);
                    File.Delete(targetFilePath);
                    // Cannot delete folders here 

                    string parentDir = Path.GetDirectoryName(targetFilePath);

                    if(Directory.GetFileSystemEntries(parentDir).Length == 0)
                    {
                        Directory.Delete(parentDir);
                    }
                }

                // If .original backup exists, restore it by renaming
                if (File.Exists(originalBackupPath))
                {
                    // Ensure target folder exists just in case
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                    File.Move(originalBackupPath, targetFilePath);
                }
            }
        }

        public static Image ResizeToFit(Image source, Size boxSize)
        {
            double ratioX = (double)boxSize.Width / source.Width;
            double ratioY = (double)boxSize.Height / source.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(source.Width * ratio);
            int newHeight = (int)(source.Height * ratio);

            Bitmap result = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Black); // Or your background color
                g.DrawImage(source, 0, 0, newWidth, newHeight);
            }

            return result;
        }

        public static Image ResizeAndCenter(Image source, Size boxSize)
        {
            double ratioX = (double)boxSize.Width / source.Width;
            double ratioY = (double)boxSize.Height / source.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(source.Width * ratio);
            int newHeight = (int)(source.Height * ratio);

            // Centering offsets
            int offsetX = (boxSize.Width - newWidth) / 2;
            int offsetY = (boxSize.Height - newHeight) / 2;

            Bitmap result = new Bitmap(boxSize.Width, boxSize.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent); // Background fill, change if needed

                //g.InterpolationMode = InterpolationMode.HighQualityBilinear; // ~2x faster than Bicubic


                g.CompositingMode = CompositingMode.SourceCopy;          // Important for clean transparency
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.Low;             // Blazingly fast
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.None;


                g.DrawImage(source, offsetX, offsetY, newWidth, newHeight);
            }

            return result;
        }

        public static Bitmap ResizeAndCenter_LockBits(Bitmap source, Size boxSize)
        {
            double ratio = Math.Min(
                (double)boxSize.Width / source.Width,
                (double)boxSize.Height / source.Height);

            int newWidth = (int)(source.Width * ratio);
            int newHeight = (int)(source.Height * ratio);
            int offsetX = (boxSize.Width - newWidth) / 2;
            int offsetY = (boxSize.Height - newHeight) / 2;

            Bitmap result = new Bitmap(boxSize.Width, boxSize.Height, PixelFormat.Format32bppArgb);

            // Lock bits for source (read-only)
            var srcData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            // Lock bits for result (write-only)
            var dstData = result.LockBits(
                new Rectangle(0, 0, boxSize.Width, boxSize.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* srcScan0 = (byte*)srcData.Scan0;
                byte* dstScan0 = (byte*)dstData.Scan0;

                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;

                for (int y = 0; y < newHeight; y++)
                {
                    int srcY = (int)(y / ratio);
                    if (srcY >= source.Height) continue;

                    byte* dstRow = dstScan0 + (y + offsetY) * dstStride + offsetX * 4;
                    byte* srcRow = srcScan0 + srcY * srcStride;

                    for (int x = 0; x < newWidth; x++)
                    {
                        int srcX = (int)(x / ratio);
                        if (srcX >= source.Width) continue;

                        byte* srcPixel = srcRow + srcX * 4;
                        byte* dstPixel = dstRow + x * 4;

                        // Copy ARGB
                        dstPixel[0] = srcPixel[0]; // B
                        dstPixel[1] = srcPixel[1]; // G
                        dstPixel[2] = srcPixel[2]; // R
                        dstPixel[3] = srcPixel[3]; // A
                    }
                }
            }

            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }


        public static unsafe Bitmap ResizeAndCenter_Bilinear(Bitmap source, Size boxSize)
        {
            double ratio = Math.Min(
                (double)boxSize.Width / source.Width,
                (double)boxSize.Height / source.Height);

            int newWidth = (int)(source.Width * ratio);
            int newHeight = (int)(source.Height * ratio);
            int offsetX = (boxSize.Width - newWidth) / 2;
            int offsetY = (boxSize.Height - newHeight) / 2;

            Bitmap result = new Bitmap(boxSize.Width, boxSize.Height, PixelFormat.Format32bppArgb);

            // Lock source and destination bitmaps
            var srcData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            var dstData = result.LockBits(
                new Rectangle(0, 0, boxSize.Width, boxSize.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int srcW = source.Width;
            int srcH = source.Height;
            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;

            for (int y = 0; y < newHeight; y++)
            {
                double srcY = y / ratio;
                int y0 = (int)srcY;
                int y1 = Math.Min(y0 + 1, srcH - 1);
                double yLerp = srcY - y0;

                for (int x = 0; x < newWidth; x++)
                {
                    double srcX = x / ratio;
                    int x0 = (int)srcX;
                    int x1 = Math.Min(x0 + 1, srcW - 1);
                    double xLerp = srcX - x0;

                    byte* p00 = srcPtr + y0 * srcStride + x0 * 4;
                    byte* p01 = srcPtr + y0 * srcStride + x1 * 4;
                    byte* p10 = srcPtr + y1 * srcStride + x0 * 4;
                    byte* p11 = srcPtr + y1 * srcStride + x1 * 4;

                    // Bilinear interpolation per channel
                    for (int c = 0; c < 4; c++)
                    {
                        double top = p00[c] * (1 - xLerp) + p01[c] * xLerp;
                        double bottom = p10[c] * (1 - xLerp) + p11[c] * xLerp;
                        byte value = (byte)(top * (1 - yLerp) + bottom * yLerp);

                        int dstY = y + offsetY;
                        int dstX = x + offsetX;
                        dstPtr[dstY * dstStride + dstX * 4 + c] = value;
                    }
                }
            }

            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }




    }


}
