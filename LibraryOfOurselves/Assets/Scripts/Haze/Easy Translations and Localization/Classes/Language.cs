/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Haze{
	[Serializable]
	public enum Language {
		
		// Haze Translations supports these 144 languages.
		// Let me know if yours isn't included! :)
		
		Default,
		// These will be detected automatically from the user's device
		// unless they select another display language
		English,
		Afrikaans,
		Arabic,
		Basque,
		Belarusian,
		Bulgarian,
		Catalan,
		ChineseSimplified,
		ChineseTraditional,
		Czech,
		Danish,
		Dutch,
		Estonian,
		Faroese,
		Finnish,
		French,
		German,
		Greek,
		Hebrew,
		Hungarian,
		Icelandic,
		Indonesian,
		Italian,
		Japanese,
		Korean,
		Latvian,
		Lithuanian,
		Norwegian,
		Polish,
		Portuguese,
		Romanian,
		Russian,
		SerboCroatian,
		Slovak,
		Slovenian,
		Spanish,
		Swedish,
		Thai,
		Turkish,
		Ukrainian,
		Vietnamese,
		// The following will not be detected by default from a user's device
		// but they could be selected by the user through a settings menu
		Akan,
		Albanian,
		Amharic,
		Armenian,
		Assamese,
		Awadhi,
		Azerbaijani,
		Bengali,
		Balochi,
		Bhojpuri,
		Bosnian,
		Burmese,
		Cebuano,
		Chewa,
		Chhattisgarhi,
		Chittagonian,
		Corsican,
		Croatian,
		Deccan,
		Dhundhari,
		EasternMin,
		Esperanto,
		Frisian,
		Fula,
		Galician,
		GanChinese,
		Georgian,
		Gujarati,
		HaitianCreole,
		Hakka,
		Haryanvi,
		Hausa,
		Hawaiian,
		Hindi,
		Hmong,
		Igbo,
		Ilocano,
		Irish,
		Javanese,
		Jin,
		Kannada,
		Kazakh,
		Khmer,
		Kinyarwanda,
		Kirundi,
		Konkani,
		Kurdish,
		Kyrgyz,
		Lao,
		Latin,
		Luxembourgish,
		Macedonian,
		Madurese,
		Magahi,
		Maithili,
		Malagasy,
		Malay,
		Malayalam,
		Maltese,
		Maori,
		Marathi,
		Marwari,
		Mongolian,
		Mossi,
		Nepali,
		NorthernMin,
		Odia,
		Oromo,
		Pashto,
		Persian,
		Punjabi,
		Quechua,
		Samoan,
		Saraiki,
		ScottishGaelic,
		Serbian,
		Sesotho,
		Shona,
		Sindhi,
		Sinhalese,
		Somali,
		SouthernMin,
		Sundanese,
		Swahili,
		Sylheti,
		Tagalog,
		Tajik,
		Tamil,
		Telugu,
		Turkmen,
		Urdu,
		Uyghur,
		Uzbek,
		Visayan,
		Welsh,
		Wu,
		Xhosa,
		Xiang,
		Yiddish,
		Yoruba,
		Yue,
		Zhuang,
		Zulu
		
	}

	/** Use LanguageEvent as a way to pass a Language variable around dynamically. */
	[Serializable] public class LanguageEvent : UnityEvent<Language>{}

	public static class Languages {
		
		static Language current = Language.Default;
		
		/** Helper function to get language from system locale */
		static Language fromSystem(){
			Language userLanguage = Language.English;
			switch(Application.systemLanguage){
				case SystemLanguage.English:
					userLanguage = Language.English; break;
				case SystemLanguage.Afrikaans:
					userLanguage = Language.Afrikaans; break;
				case SystemLanguage.Arabic:
					userLanguage = Language.Arabic; break;
				case SystemLanguage.Basque:
					userLanguage = Language.Basque; break;
				case SystemLanguage.Belarusian:
					userLanguage = Language.Belarusian; break;
				case SystemLanguage.Bulgarian:
					userLanguage = Language.Bulgarian; break;
				case SystemLanguage.Catalan:
					userLanguage = Language.Catalan; break;
				case SystemLanguage.Chinese:
				case SystemLanguage.ChineseSimplified:
					userLanguage = Language.ChineseSimplified; break;
				case SystemLanguage.ChineseTraditional:
					userLanguage = Language.ChineseTraditional; break;
				case SystemLanguage.Czech:
					userLanguage = Language.Czech; break;
				case SystemLanguage.Danish:
					userLanguage = Language.Danish; break;
				case SystemLanguage.Dutch:
					userLanguage = Language.Dutch; break;
				case SystemLanguage.Estonian:
					userLanguage = Language.Estonian; break;
				case SystemLanguage.Faroese:
					userLanguage = Language.Faroese; break;
				case SystemLanguage.Finnish:
					userLanguage = Language.Finnish; break;
				case SystemLanguage.French:
					userLanguage = Language.French; break;
				case SystemLanguage.German:
					userLanguage = Language.German; break;
				case SystemLanguage.Greek:
					userLanguage = Language.Greek; break;
				case SystemLanguage.Hebrew:
					userLanguage = Language.Hebrew; break;
				case SystemLanguage.Hungarian:
					userLanguage = Language.Hungarian; break;
				case SystemLanguage.Icelandic:
					userLanguage = Language.Icelandic; break;
				case SystemLanguage.Indonesian:
					userLanguage = Language.Indonesian; break;
				case SystemLanguage.Italian:
					userLanguage = Language.Italian; break;
				case SystemLanguage.Japanese:
					userLanguage = Language.Japanese; break;
				case SystemLanguage.Korean:
					userLanguage = Language.Korean; break;
				case SystemLanguage.Latvian:
					userLanguage = Language.Latvian; break;
				case SystemLanguage.Lithuanian:
					userLanguage = Language.Lithuanian; break;
				case SystemLanguage.Norwegian:
					userLanguage = Language.Norwegian; break;
				case SystemLanguage.Polish:
					userLanguage = Language.Polish; break;
				case SystemLanguage.Portuguese:
					userLanguage = Language.Portuguese; break;
				case SystemLanguage.Romanian:
					userLanguage = Language.Romanian; break;
				case SystemLanguage.Russian:
					userLanguage = Language.Russian; break;
				case SystemLanguage.SerboCroatian:
					userLanguage = Language.SerboCroatian; break;
				case SystemLanguage.Slovak:
					userLanguage = Language.Slovak; break;
				case SystemLanguage.Slovenian:
					userLanguage = Language.Slovenian; break;
				case SystemLanguage.Spanish:
					userLanguage = Language.Spanish; break;
				case SystemLanguage.Swedish:
					userLanguage = Language.Swedish; break;
				case SystemLanguage.Thai:
					userLanguage = Language.Thai; break;
				case SystemLanguage.Turkish:
					userLanguage = Language.Turkish; break;
				case SystemLanguage.Ukrainian:
					userLanguage = Language.Ukrainian; break;
				case SystemLanguage.Vietnamese:
					userLanguage = Language.Vietnamese; break;
				default:
					userLanguage = Language.English; break;
			}
			
			//check whether userLanguage is supported by the app
			if(!SupportedLanguages.IsLanguageSupported(userLanguage)){
				//a few different cases here.
				switch(userLanguage){
					//from either form of Chinese, attempt to fall back onto the other form, or English
					case Language.ChineseSimplified:
						if(SupportedLanguages.IsLanguageSupported(Language.ChineseTraditional))
							userLanguage = Language.ChineseTraditional;
						else
							userLanguage = Language.English;
						break;
					case Language.ChineseTraditional:
						if(SupportedLanguages.IsLanguageSupported(Language.ChineseSimplified))
							userLanguage = Language.ChineseSimplified;
						else
							userLanguage = Language.English;
						break;
					//from Catalan, attempt to fall back onto Spanish, or English
					case Language.Catalan:
						if(SupportedLanguages.IsLanguageSupported(Language.Spanish))
							userLanguage = Language.Spanish;
						else
							userLanguage = Language.English;
						break;
					//from everything else, attempt to fall back onto English
					default:
						userLanguage = Language.English;
						break;
				}
			}
			
			//if the language has fallen back onto English but English isn't supported, fall back to the default language
			if(!SupportedLanguages.IsLanguageSupported(userLanguage))
				userLanguage = SupportedLanguages.DefaultLanguage;
			
			return userLanguage;
		}
		
		/** Use this to get/set current language as defined either by system locale or user. */
		public static Language Current{
			get{
				if(current == Language.Default){
					
					//let's try to fetch it from HazePrefs
					if(HazePrefs.HasKey("haze-language")){
						current = (Language)HazePrefs.GetInt("haze-language");
					}else{
						//determine from system language
						current = fromSystem();
					}
					
				}
				return current;
			}
			set{
				if(current == value) return;
				current = value;
				if(current == Language.Default){
					//erase HazePrefs key and get it back from settings
					HazePrefs.DeleteKey("haze-language");
					current = fromSystem();
				}else{
					//write it down in HazePrefs
					HazePrefs.SetInt("haze-language", (int)current);
				}
				//notify all TranslatedElements and LocalizedSprites
				foreach(TranslatedElement elem in GameObject.FindObjectsOfType<TranslatedElement>()){
					elem.OnLanguageChange();
				}
				foreach(LocalizedSprite elem in GameObject.FindObjectsOfType<LocalizedSprite>()){
					elem.OnLanguageChange();
				}
			}
		}
		
		/** Use this to display a Language value (language.ToString()) */
		public static string ToString(this Language language){
			switch(language){
				case Language.ChineseSimplified: return "Chinese (Simplified)";
				case Language.ChineseTraditional: return "Chinese (Traditional)";
				case Language.SerboCroatian: return "Serbo-Croatian";
				case Language.EasternMin: return "Eastern Min";
				case Language.GanChinese: return "Gan Chinese";
				case Language.HaitianCreole: return "Haitian Creole";
				case Language.NorthernMin: return "Northern Min";
				case Language.ScottishGaelic: return "Scottish Gaelic";
				case Language.SouthernMin: return "Southern Min";
				default: return ""+language;
			}
		}
		
	}	
}

/*

Here is a template switch statement with all of the 144 languages, provided to save you time typing in case you need to write your own such code ;)

switch(language){
	case Language.English:
	case Language.Afrikaans:
	case Language.Arabic:
	case Language.Basque:
	case Language.Belarusian:
	case Language.Bulgarian:
	case Language.Catalan:
	case Language.ChineseSimplified:
	case Language.ChineseTraditional:
	case Language.Czech:
	case Language.Danish:
	case Language.Dutch:
	case Language.Estonian:
	case Language.Faroese:
	case Language.Finnish:
	case Language.French:
	case Language.German:
	case Language.Greek:
	case Language.Hebrew:
	case Language.Hungarian:
	case Language.Icelandic:
	case Language.Indonesian:
	case Language.Italian:
	case Language.Japanese:
	case Language.Korean:
	case Language.Latvian:
	case Language.Lithuanian:
	case Language.Norwegian:
	case Language.Polish:
	case Language.Portuguese:
	case Language.Romanian:
	case Language.Russian:
	case Language.SerboCroatian:
	case Language.Slovak:
	case Language.Slovenian:
	case Language.Spanish:
	case Language.Swedish:
	case Language.Thai:
	case Language.Turkish:
	case Language.Ukrainian:
	case Language.Vietnamese:
	case Language.Akan:
	case Language.Albanian:
	case Language.Amharic:
	case Language.Armenian:
	case Language.Assamese:
	case Language.Awadhi:
	case Language.Azerbaijani:
	case Language.Bengali:
	case Language.Balochi:
	case Language.Bhojpuri:
	case Language.Bosnian:
	case Language.Burmese:
	case Language.Cebuano:
	case Language.Chewa:
	case Language.Chhattisgarhi:
	case Language.Chittagonian:
	case Language.Corsican:
	case Language.Croatian:
	case Language.Deccan:
	case Language.Dhundhari:
	case Language.EasternMin:
	case Language.Esperanto:
	case Language.Frisian:
	case Language.Fula:
	case Language.Galician:
	case Language.GanChinese:
	case Language.Georgian:
	case Language.Gujarati:
	case Language.HaitianCreole:
	case Language.Hakka:
	case Language.Haryanvi:
	case Language.Hausa:
	case Language.Hawaiian:
	case Language.Hindi:
	case Language.Hmong:
	case Language.Igbo:
	case Language.Ilocano:
	case Language.Irish:
	case Language.Javanese:
	case Language.Jin:
	case Language.Kannada:
	case Language.Kazakh:
	case Language.Khmer:
	case Language.Kinyarwanda:
	case Language.Kirundi:
	case Language.Konkani:
	case Language.Kurdish:
	case Language.Kyrgyz:
	case Language.Lao:
	case Language.Latin:
	case Language.Luxembourgish:
	case Language.Macedonian:
	case Language.Madurese:
	case Language.Magahi:
	case Language.Maithili:
	case Language.Malagasy:
	case Language.Malay:
	case Language.Malayalam:
	case Language.Maltese:
	case Language.Maori:
	case Language.Marathi:
	case Language.Marwari:
	case Language.Mongolian:
	case Language.Mossi:
	case Language.Nepali:
	case Language.NorthernMin:
	case Language.Odia:
	case Language.Oromo:
	case Language.Pashto:
	case Language.Persian:
	case Language.Punjabi:
	case Language.Quechua:
	case Language.Samoan:
	case Language.Saraiki:
	case Language.ScottishGaelic:
	case Language.Serbian:
	case Language.Sesotho:
	case Language.Shona:
	case Language.Sindhi:
	case Language.Sinhalese:
	case Language.Somali:
	case Language.SouthernMin:
	case Language.Sundanese:
	case Language.Swahili:
	case Language.Sylheti:
	case Language.Tagalog:
	case Language.Tajik:
	case Language.Tamil:
	case Language.Telugu:
	case Language.Turkmen:
	case Language.Urdu:
	case Language.Uyghur:
	case Language.Uzbek:
	case Language.Visayan:
	case Language.Welsh:
	case Language.Wu:
	case Language.Xhosa:
	case Language.Xiang:
	case Language.Yiddish:
	case Language.Yoruba:
	case Language.Yue:
	case Language.Zhuang:
	case Language.Zulu:
	default: // Language.Default
}

*/