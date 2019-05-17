using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze{
	[RequireComponent(typeof(AudioSource))]
	public class MusicNote : MonoBehaviour{
		
		[SerializeField] Note inputNote = Note.C;
		[SerializeField] int inputOctave = 4;
		[SerializeField] Note outputNote = Note.C;
		[SerializeField] int outputOctave = 4;
		
		public enum Note{
			C,
			CSharp,
			DBemol,
			D,
			DSharp,
			EBemol,
			E,
			ESharp,
			FBemol,
			F,
			FSharp,
			GBemol,
			G,
			GSharp,
			ABemol,
			A,
			ASharp,
			BBemol,
			B,
			BSharp,
			CBemol,
			//Romance variants
			Do,
			DoSharp,
			ReBemol,
			Re,
			ReSharp,
			MiBemol,
			Mi,
			MiSharp,
			FaBemol,
			Fa,
			FaSharp,
			SolBemol,
			Sol,
			SolSharp,
			LaBemol,
			La,
			LaSharp,
			SiBemol,
			Si,
			SiSharp,
			DoBemol
		}
		
		public Note OutputNote { get { return outputNote; } set { outputNote = value; } }
		public int OutputOctave { get { return outputOctave; } set { outputOctave = value; } }

		static int GetPitchClass(Note note){
			switch(note){
				case Note.C:
				case Note.BSharp:
				case Note.Do:
				case Note.SiSharp:
					return 0;
				case Note.CSharp:
				case Note.DBemol:
				case Note.DoSharp:
				case Note.ReBemol:
					return 1;
				case Note.D:
				case Note.Re:
					return 2;
				case Note.DSharp:
				case Note.EBemol:
				case Note.ReSharp:
				case Note.MiBemol:
					return 3;
				case Note.E:
				case Note.FBemol:
				case Note.Mi:
				case Note.FaBemol:
					return 4;
				case Note.ESharp:
				case Note.F:
				case Note.MiSharp:
				case Note.Fa:
					return 5;
				case Note.FSharp:
				case Note.GBemol:
				case Note.FaSharp:
				case Note.SolBemol:
					return 6;
				case Note.G:
				case Note.Sol:
					return 7;
				case Note.GSharp:
				case Note.ABemol:
				case Note.SolSharp:
				case Note.LaBemol:
					return 8;
				case Note.A:
				case Note.La:
					return 9;
				case Note.ASharp:
				case Note.BBemol:
				case Note.LaSharp:
				case Note.SiBemol:
					return 10;
				case Note.B:
				case Note.CBemol:
				case Note.Si:
				case Note.DoBemol:
					return 11;
			}
			Debug.LogError("Note " + note + " does not have a pitch class...");
			return -1;
		}
		
		public static float GetFrequency(Note note, int octave){
			int pitchClass = GetPitchClass(note);
			float[] frequencies = {16.35f, 17.32f, 18.35f, 19.45f, 20.6f, 21.83f, 23.12f, 24.5f, 25.96f, 27.5f, 29.14f, 30.87f, 32.7f, 34.65f, 36.71f, 38.89f, 41.2f, 43.65f, 46.25f, 49.0f, 51.91f, 55.0f, 58.27f, 61.74f, 65.41f, 69.30f, 73.42f, 77.78f, 82.41f, 87.31f, 92.5f, 	98.00f, 103.83f, 110.00f, 116.54f, 123.47f, 130.81f, 138.59f, 146.83f, 155.56f, 164.81f, 174.61f, 185.00f, 196.00f, 207.65f, 220.00f, 233.08f, 246.94f, 261.63f, 277.18f, 293.66f, 311.13f, 329.63f, 349.23f, 369.99f, 392.00f, 415.30f, 440.00f, 466.16f, 493.88f, 523.25f, 554.37f, 587.33f, 622.25f, 659.25f, 698.46f, 739.99f, 783.99f, 830.61f, 880.00f, 932.33f, 987.77f, 1046.50f, 1108.73f, 1174.66f, 1244.51f, 1318.51f, 1396.91f, 1479.98f, 1567.98f, 1661.22f, 1760.00f, 1864.66f, 1975.53f, 2093.00f, 2217.46f, 2349.32f, 2489.02f, 2637.02f, 2793.83f, 2959.96f, 3135.96f, 3322.44f, 3520.00f, 3729.31f, 3951.07f, 4186.01f, 4434.92f, 4698.63f, 4978.03f, 5274.04f, 5587.65f, 5919.91f, 6271.93f, 6644.88f, 7040.00f, 7458.62f, 7902.13f};
			return frequencies[pitchClass + octave * 12];
		}
		
		///Using pitch = 1 corresponding to inputNote, converts to the specified note.
		public float ConvertNote(Note note, int octave){
			
			int noteTransposed = GetPitchClass(note) + octave * 12;
			noteTransposed -= GetPitchClass(inputNote) + inputOctave * 12;
			float pitch = Mathf.Pow(2, noteTransposed / 12.0f);
			
			return pitch;
		}
		
		void Start(){
			float pitch = ConvertNote(outputNote, outputOctave);
			GetComponent<AudioSource>().pitch = pitch;
		}
		
	}
}