using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWords
{
	public static string Noun {
		get {
			string list = "fang	kettle eye activity crow side coal expert pigs shoe zebra war finger force head word feeling rest crime page hate sleet group attraction edge authority guide collar bubble marble weight " +
				"week egg hill slave knot action science crown limit riddle airport pipe receipt fowl vein spot trains duck mitten shoes board mom corn stream wave foot judge change ray language trail umbrella play dogs " +
				"wall letters impulse teeth railway cars bedroom rock bird whip throat mountain bells oatmeal education wrist wine believe room line dog clover relation idea texture dolls fire border button part bag bath " +
				"quartz nut month";
			string[] split = list.Split(' ');
			return split[Random.Range(0, split.Length)];
		}
	}
}
