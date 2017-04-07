using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ApplicationModel : MonoBehaviour {
	public static string difficulty = "Easy";
	public static float volume = 0.5f;
	[Range(0, 3)]
	public static int savefile = 0;
	public static string savePath;

	public static int getEmptySaveFile() {
		if (!System.IO.File.Exists (ApplicationModel.savePath + "savefile1.txt")) {
			return 1;
		} else if (!System.IO.File.Exists(ApplicationModel.savePath + "savefile2.txt")) {
			return 2;
		} else if (!System.IO.File.Exists(ApplicationModel.savePath + "savefile3.txt")) {
			return 3;
		}
		return -1;
	}
}
