using NUnit.Framework;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using System.IO;
using System.Collections.Generic;

namespace tests
{
	public class TempDoubleImageFormatTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test, Description("Test that Data is correctly loaded as 3d")]
		public void LoadTest3d()
		{
			// Arrange
			FileStream pngStream = new FileStream("half.png", FileMode.Open, FileAccess.Read);

			// Act
			var image = new Bitmap(pngStream);
			Color firstColor = image.GetPixel(0, 0);
			
			TempDoubleImageFormat test = new TempDoubleImageFormat(ReadTo3DDoubles(image));
			double[] firstPixel = test.GetPixelChannels(0, 0);

			// Assert
			Assert.AreEqual(firstColor.R / byteMax, firstPixel[0]);
			Assert.AreEqual(firstColor.G / byteMax, firstPixel[1]);
			Assert.AreEqual(firstColor.B / byteMax, firstPixel[2]);
		}

		[Test, Description("Test that Data is correctly loaded as 1d")]
		public void LoadTest1d()
		{
			// Arrange
			FileStream pngStream = new FileStream("half.png", FileMode.Open, FileAccess.Read);

			// Act
			var image = new Bitmap(pngStream);
			Color firstColor = image.GetPixel(0, 0);
			
			TempDoubleImageFormat test = new TempDoubleImageFormat(ReadTo1DDoubles(image), image.Width, image.Height, 3);
			double[] firstPixel = test.GetPixelChannels(0, 0);

			// Assert
			Assert.AreEqual(firstColor.R / byteMax, firstPixel[0]);
			Assert.AreEqual(firstColor.G / byteMax, firstPixel[1]);
			Assert.AreEqual(firstColor.B / byteMax, firstPixel[2]);
		}

		[Test, Description("Test that indexing works equally")]
		public void IndexingTest()
		{
			// Arrange
			FileStream pngStream = new FileStream("half.png", FileMode.Open, FileAccess.Read);

			int[] indexes = new int[] { 0, 1, 12, 37, 56, 132, 200 };

			// Act
			var image = new Bitmap(pngStream);

			TempDoubleImageFormat test3d = new TempDoubleImageFormat(ReadTo3DDoubles(image));
			TempDoubleImageFormat test1d = new TempDoubleImageFormat(ReadTo1DDoubles(image), image.Width, image.Height, 3);

			// Assert
			for (int x = 0; x < indexes.Length; x++)
			{
				for (int y = 0; y < indexes.Length; y++)
				{
					double[] pixel3d = test3d.GetPixelChannels(x, y);
					double[] pixel1d = test1d.GetPixelChannels(x, y);

					CollectionAssert.AreEqual(pixel3d, pixel1d, $"Pixels at {x} x {y} should be equal");
				}
			}
		}

		[Test, Description("Test that 1d copy works")]
		public void CheckThat1dCopyWorks()
		{
			// Arrange
			FileStream pngStream = new FileStream("half.png", FileMode.Open, FileAccess.Read);

			// Act
			var image = new Bitmap(pngStream);
			double[] doubles1d = ReadTo1DDoubles(image);
			TempDoubleImageFormat test1d_1 = new TempDoubleImageFormat(doubles1d, image.Width, image.Height, 3, createCopy: false);
			TempDoubleImageFormat test1d_2 = new TempDoubleImageFormat(doubles1d, image.Width, image.Height, 3, createCopy: true);
			doubles1d[0] = 0.0;

			double[] firstPixel1 = test1d_1.GetPixelChannels(0, 0);
			double[] firstPixel2 = test1d_2.GetPixelChannels(0, 0);

			// Assert
			Assert.AreEqual(0.0, firstPixel1[0]);
			Assert.AreNotEqual(0.0, firstPixel2[0]);
		}

		[Test, Description("Test that raw content works")]
		public void CheckThatRawContentWorks()
		{
			// Arrange
			FileStream pngStream = new FileStream("half.png", FileMode.Open, FileAccess.Read);

			// Act
			var image = new Bitmap(pngStream);
			double[] doubles1d = ReadTo1DDoubles(image);
			TempDoubleImageFormat test1d_1 = new TempDoubleImageFormat(doubles1d, image.Width, image.Height, 3, createCopy: true);

			// Assert
			Assert.Greater(doubles1d.Length, 1000, "There should be some bytes in image data");
			CollectionAssert.AreEqual(doubles1d, test1d_1.GetRawContent());
		}

		private static readonly double byteMax = byte.MaxValue / 1.0;
		private static double[,,] ReadTo3DDoubles(Bitmap bitmap)
		{
			double[,,] returnValue = new double[bitmap.Width, bitmap.Height, 3];
			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					Color color = bitmap.GetPixel(x, y);
					returnValue[x, y, 0] = color.R / byteMax;
					returnValue[x, y, 1] = color.G / byteMax;
					returnValue[x, y, 2] = color.B / byteMax;
				}
			}
			return returnValue;
		}

		private static double[] ReadTo1DDoubles(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			int channelsPerPixel = 3;
			double[] returnValue = new double[width * height * 3];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Color color = bitmap.GetPixel(x, y);
					int arrayIndex = y * width * channelsPerPixel + x * channelsPerPixel;
					returnValue[arrayIndex + 0] = color.R / byteMax;
					returnValue[arrayIndex + 1] = color.G / byteMax;
					returnValue[arrayIndex + 2] = color.B / byteMax;
				}
			}
			return returnValue;
		}
	}
}