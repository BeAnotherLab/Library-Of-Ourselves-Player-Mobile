/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

#pragma warning disable 0618

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze{
	
	/** Uses Google Translate to provide translations in Editor */
	public class AutoTranslation{
		
		bool finished = false;
		/** Returns true once source material has been translated */
		public bool Finished{
			get{
				return finished;
			}
		}
		
		string translated = "";
		/** Returns the translated string once it's been translated */
		public string Translated{
			get{
				return translated;
			}
		}
		
		Language targetLanguage;
		public Language TargetLanguage{ get{ return targetLanguage; }}
		Language sourceLanguage;
		public Language SourceLanguage{ get{ return sourceLanguage; }}
		string source;
		public string Source{ get{ return source; }}
		
		/** Constructs an AutoTranslation object, from which you will get results as soon as we get response from Google Translate */
		public AutoTranslation(string source, Language sourceLanguage, Language targetLanguage, MonoBehaviour behaviour){
			this.targetLanguage = targetLanguage;
			this.sourceLanguage = sourceLanguage;
			this.source = source;
			
			string srcCode = TranslationLanguageCode(sourceLanguage);
			string dstCode = TranslationLanguageCode(targetLanguage);
			if(srcCode != "" && dstCode != ""){
				behaviour.StartCoroutine(translate(source, srcCode, dstCode));
			}else{
				Debug.LogError("Cannot translate from " + sourceLanguage + " to " + targetLanguage + " using Google Translate.");
				finished = true;
			}
		}
		
		/** Ends once the source material has been translated */
		IEnumerator translate(string source, string sourceLanguage, string targetLanguage){
			string url = "https://translate.googleapis.com/translate_a/single?client=gtx&dt=t";
			url += "&sl=" + sourceLanguage;
			url += "&tl=" + targetLanguage;
			url += "&q=" + WWW.EscapeURL(source);
			
			WWW www = new WWW(url);
			yield return www;
			
			if(www.error != null && www.error != ""){
				Debug.LogWarning("Couldn't translate! Error: " + www.error);
			}else{
				//result will be R in string formatted as: [[["R"," (...)
				//get rid of beginning:
				string result = www.text.Substring(4);
				//get rid of end:
				for(int i = 0; i<result.Length; ++i){
					bool isEnd = false;
					if(result[i] == '\"'){
						//check whether previous char was "\"
						if(i != 0 && result[i-1] == '\\'){
							//then only proceed if char before that was "\" as well.
							if(i != 1 && result[i-2] == '\\'){
								isEnd = true;
							}else{
								isEnd = false;
							}
						}else{
							isEnd = true;
						}
					}
					if(isEnd){
						result = result.Substring(0, i);
						break;
					}
				}
				//remove any "\" in string:
				result = result.Replace("\\", "");
				Debug.Log("Translated " + source + " to " + result);
				translated = result;
			}
			finished = true;
		}
		
		public static string TranslationLanguageCode(Language lang){
			switch(lang){
				case Language.English: return "en";
				case Language.Afrikaans: return "af";
				case Language.Albanian: return "sq";
				case Language.Amharic: return "am";
				case Language.Arabic: return "ar";
				case Language.Armenian: return "hy";
				case Language.Azerbaijani: return "az";
				case Language.Basque: return "eu";
				case Language.Belarusian: return "be";
				case Language.Bengali: return "bn";
				case Language.Bosnian: return "bs";
				case Language.Bulgarian: return "bg";
				case Language.Burmese: return "my";
				case Language.Catalan: return "ca";
				case Language.Cebuano: return "ceb";
				case Language.Chewa: return "ny";
				case Language.ChineseSimplified: return "zh-CN";
				case Language.ChineseTraditional: return "zh-TW";
				case Language.Corsican: return "co";
				case Language.Croatian: return "hr";
				case Language.Czech: return "cs";
				case Language.Danish: return "da";
				case Language.Dutch: return "nl";
				case Language.Esperanto: return "eo";
				case Language.Estonian: return "et";
				case Language.Finnish: return "fi";
				case Language.French: return "fr";
				case Language.Galician: return "gl";
				case Language.Georgian: return "ka";
				case Language.German: return "de";
				case Language.Greek: return "el";
				case Language.Gujarati: return "gu";
				case Language.HaitianCreole: return "ht";
				case Language.Hausa: return "ha";
				case Language.Hawaiian: return "haw";
				case Language.Hebrew: return "iw";
				case Language.Hindi: return "hi";
				case Language.Hmong: return "hmn";
				case Language.Hungarian: return "hu";
				case Language.Icelandic: return "is";
				case Language.Igbo: return "ig";
				case Language.Indonesian: return "id";
				case Language.Irish: return "ga";
				case Language.Italian: return "it";
				case Language.Japanese: return "ja";
				case Language.Javanese: return "jw";
				case Language.Kannada: return "kn";
				case Language.Kazakh: return "kk";
				case Language.Khmer: return "km";
				case Language.Korean: return "ko";
				case Language.Kurdish: return "ku";
				case Language.Kyrgyz: return "ky";
				case Language.Lao: return "lo";
				case Language.Latin: return "la";
				case Language.Latvian: return "lv";
				case Language.Lithuanian: return "lt";
				case Language.Luxembourgish: return "lb";
				case Language.Macedonian: return "mk";
				case Language.Malagasy: return "mg";
				case Language.Malay: return "ms";
				case Language.Malayalam: return "ml";
				case Language.Maltese: return "mt";
				case Language.Maori: return "mi";
				case Language.Marathi: return "mr";
				case Language.Mongolian: return "mn";
				case Language.Nepali: return "ne";
				case Language.Norwegian: return "no";
				case Language.Pashto: return "ps";
				case Language.Persian: return "fa";
				case Language.Polish: return "pl";
				case Language.Portuguese: return "pt";
				case Language.Punjabi: return "pa";
				case Language.Romanian: return "ro";
				case Language.Russian: return "ru";
				case Language.Samoan: return "sm";
				case Language.ScottishGaelic: return "gd";
				case Language.Serbian: return "sr";
				case Language.Sesotho: return "st";
				case Language.Shona: return "sn";
				case Language.Sindhi: return "sd";
				case Language.Sinhalese: return "si";
				case Language.Slovak: return "sk";
				case Language.Slovenian: return "sl";
				case Language.Somali: return "so";
				case Language.Spanish: return "es";
				case Language.Sundanese: return "su";
				case Language.Swahili: return "sw";
				case Language.Swedish: return "sv";
				case Language.Tagalog: return "tl";
				case Language.Tajik: return "tg";
				case Language.Tamil: return "ta";
				case Language.Telugu: return "te";
				case Language.Thai: return "th";
				case Language.Turkish: return "tr";
				case Language.Ukrainian: return "uk";
				case Language.Urdu: return "ur";
				case Language.Uzbek: return "uz";
				case Language.Vietnamese: return "vi";
				case Language.Welsh: return "cy";
				case Language.Xhosa: return "xh";
				case Language.Yiddish: return "yi";
				case Language.Yoruba: return "yo";
				case Language.Zulu: return "zu";
				default: return "";
			}
		}
		
	}
}
#endif