/*
   This file implements fake dithering, meaning NO dithering is done.

   This is free and unencumbered software released into the public domain.
*/
using System;

/// <summary>
/// Fake dithering doesn't do any dithering. It only does color reduction
/// </summary>
public class FakeDitheringRGBByte : DitheringBase
{
	/// <summary>
	/// Constructor for fake dithering (no dither, just color reduction)
	/// </summary>
	/// <param name="colorfunc"></param>
	/// <returns></returns>
	public FakeDitheringRGBByte(Func<object[],object[]> colorfunc) : base(colorfunc)
	{
		this.methodLongName = "No dithering";
		this.fileNameAddition = "_NONE";
	}

	/// <summary>
	/// Push error method for Fake dithering
	/// </summary>
	/// <param name="x">X coordinate</param>
	/// <param name="y">Y coordinate</param>
	/// <param name="quantError">Quantization error</param>
	override protected void PushError(int x, int y, double[] quantError)
	{
		// Don't do anything
	}
}

