using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NLayer;

/// <summary>
/// From:
///  https://github.com/r2123b/Load-Mp3-into-Audioclip
/// </summary>

public class NLayerLoader{
	public static AudioClip LoadMp3(string filePath) {
		string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);

		MpegFile mpegFile = new MpegFile(filePath);

		// assign samples into AudioClip
		AudioClip ac = AudioClip.Create(filename,
										(int)(mpegFile.Length / sizeof(float) / mpegFile.Channels),
										mpegFile.Channels,
										mpegFile.SampleRate,
										true,
										data => { int actualReadCount = mpegFile.ReadSamples(data, 0, data.Length); },
										position => { mpegFile = new MpegFile(filePath); });

		return ac;
	}
}
