/***

	This script was made by Jonathan Kings for use within the Unity Asset "Haze Easy Translations and Localizations".
	You are free to modify this file for your own use, but do not redistribute this file or its contents.
	Please do not remove this header.
	Thanks for using Haze assets in your projects :)

***/

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Haze{
	
	/** Provides sample strings to show how each letter will render in your preferred font */
	public static class LanguageSamples {
		
		static string LATIN = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
		static string ENE = "Ññ";
		static string ARABIC = "غظضذخثتشرقصفعسنملكيطحزوهدجبأ";//not sure..
		static string CYRILLIC = "АаБбВвГгҐґДдЂђЃѓЕеЁёЄєЖжЗзЗ́з́ЅѕИиІіЇїЙйЈјКкЛлЉљМмНнЊњОоПпРрСсС́с́ТтЋћЌќУуЎўФфХхЦцЧчЏџШшЩщЪъЫыЬьЭэЮюЯя";
		static string FAROESE_BASE = "AaÁáBbDdÐðEeFfGgHhIiÍíJjKkLlMmNnOoÓóPpRrSsTtUuÚúVvYyÝýÆæ";
		static string GREEK = "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσςΤτΥυΦφΧχΨψΩω";
		static string HEBREW = "אבּבגדהוזחטיכּכךּךלמםנןסעפּפףצץקרשׁשׂתּ‬ת";
		static string HANGUL = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎㄳㄵㄶㄺㄻㄼㄽㄾㄿㅀㅄㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";//not sure..
		static string DEVANAGARI = "अपआपाइपिईपीउपुऊपूऋपृॠपॄऌपॢॡपॣएपेऐपैओपोऔपौअंपंअःपःॲऍपॅऑ7पॉकखगघङहचछजझञयशटठडढणरषतथदधनलसपफबभमव";//not sure..
		static string EASTERN_NAGARI = "অআইঈউঊঋৠঌৡএঐওঔকখগঘঙহচছজঝঞযশটঠডঢণরষত্যতথদধনলসঠ্যপফবভমবড়ঢ়য়";
		static string GAJ_LATIN = LATIN + "ČčĆćĐđŠšŽž";
		static string BOKO = LATIN + "ƁɓƊɗƘƙƳƴ";
		static string HIRAGANA = "ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゙゚ゝゞゟ";
		static string KATAKANA = "゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿ";
		static string MIAO = "漀漁漂漃漄漅漆漇漈漉漊漋漌漍漎漏漐漑漒漓演漕漖漗漘漙漚漛漜漝漞漟漠漡漢漣漤漥漦漧漨漩漪漫漬漭漮漯漰漱漲漳漴漵漶漷漸漹漺漻漼漽漾漿潀潁潂潃潄潐";
		static string CHINESE = "一一一一一一丁丁丁丁丁丁丂丂丂七七七七七七丄丄丄丄丅丅丅丆丆万万万万万万丈丈丈丈丈丈三三三三三三上上上上上上下下下下下下丌丌丌丌丌不不不不不不与与与与与与丏丏丏丏丏丐丐丐丐丐丐丑丑丑丑丑丑丒丒丒专";//this is just a small sample, to check that fonts support Chinese characters. I'm not going to include all kanji :)
		
		/** Returns a string containing all the letters the source language is likely to contain */
		static string Letters(Language source){
			switch(source){
				case Language.English: return LATIN;
				case Language.Afrikaans: return LATIN;
				case Language.Arabic: return ARABIC;
				case Language.Basque: return LATIN + ENE;
				case Language.Belarusian: return CYRILLIC;
				case Language.Bulgarian: return CYRILLIC;
				case Language.Catalan: return LATIN;
				case Language.ChineseSimplified: return CHINESE;
				case Language.ChineseTraditional: return CHINESE;
				case Language.Czech: return LATIN + "ÁáČčĎďÉéĚěÍíŇňÓóŘřŠšŤťÚúŮůÝýŽž";
				case Language.Danish: return LATIN + "ÆæØøÅå";
				case Language.Dutch: return LATIN;
				case Language.Estonian: return LATIN + "ŠšŽžÕõÄäÖöÜü";
				case Language.Faroese: return FAROESE_BASE + "Øø";
				case Language.Finnish: return LATIN + "ÅåÄäÖö";
				case Language.French: return LATIN + "ÀàÂâÆæÇçÉéÈèÊêËëÎîÏïÔôŒœÙùÛûÜüŸÿ";
				case Language.German: return LATIN + "ÄäÖöÜüẞß";
				case Language.Greek: return GREEK;
				case Language.Hebrew: return HEBREW;
				case Language.Hungarian: return LATIN + "ÁáÉéÍíÓóÖöŐőÚúÜüŰű";
				case Language.Icelandic: return FAROESE_BASE + "ÉéXxÞþÖö";
				case Language.Indonesian: return LATIN;
				case Language.Italian: return LATIN;
				case Language.Japanese: return HIRAGANA + KATAKANA + CHINESE;
				case Language.Korean: return HANGUL;
				case Language.Latvian: return LATIN + "ĀāČčĒēĢģĪīĶķĻļŅņŠšŪūŽž";
				case Language.Lithuanian: return "";
				case Language.Norwegian: return LATIN + "ÆæØøÅå";
				case Language.Polish: return LATIN + "ĄąĆćĘęŁłŃńÓóŚśŹźŻż";
				case Language.Portuguese: return LATIN;
				case Language.Romanian: return LATIN + "ĂăÂâÎîȘșȚț";
				case Language.Russian: return CYRILLIC;
				case Language.SerboCroatian: return Letters(Language.Serbian) + Letters(Language.Croatian) + Letters(Language.Bosnian);
				case Language.Slovak: return LATIN + "ÁáÄäČčĎďÉéÍíĹĺĽľŇňÓóÔôŔŕŠšŤťÚúÝýŽž";
				case Language.Slovenian: return LATIN + "ČčŠšŽž";
				case Language.Spanish: return LATIN + ENE + "Áá";
				case Language.Swedish: return LATIN + "ÅåÄäÖö";
				case Language.Thai: return "กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮ";
				case Language.Turkish: return LATIN + "ÇçĞğİıÖöŞşÜü";
				case Language.Ukrainian: return CYRILLIC;
				case Language.Vietnamese: return LATIN + "ĂăÂâĐđÊêÔôƠơƯư";
				case Language.Akan: return LATIN + "ƐɛƆɔ";
				case Language.Albanian: return LATIN + "ÇçËë";
				case Language.Amharic: return "ሀሁሂሃሄህሆለሉሊላሌልሎሏሐሑሒሓሔሕሖሗመሙሚማሜምሞሟሠሡሢሣሤሥሦሧረሩሪራሬርሮሯሰሱሲሳሴስሶሷሸሹሺሻሼሽሾሿቀቁቂቃቄቅቆቈቊቋቌቍበቡቢባቤብቦቧቨቩቪቫቬቭቮቯተቱቲታቴትቶቷቸቹቺቻቼችቾቿኀኁኂኃኄኅኆኈኊኋኌኍነኑኒናኔንኖኗኘኙኚኛኜኝኞኟአኡኢኣኤእኦኧከኩኪካኬክኮኰኲኳኴኵኸኹኺኻኼኽኾዀዂዃዄዅወዉዊዋዌውዎዐዑዒዓዔዕዖዘዙዚዛዜዝዞዟዠዡዢዣዤዥዦዧየዩዪያዬይዮደዱዲዳዴድዶዷጀጁጂጃጄጅጆጇገጉጊጋጌግጎጐጒጓጔጕጠጡጢጣጤጥጦጧጨጩጪጫጬጭጮጯጰጱጲጳጴጵጶጷጸጹጺጻጼጽጾጿፀፁፂፃፄፅፆፈፉፊፋፌፍፎፏፐፑፒፓፔፕፖፗ";
				case Language.Armenian: return "ԱաԲբԳգԴդԵեԶզԷէԸըԹթԺժԻիԼլԽխԾծԿկՀհՁձՂղՃճՄմՅյՆնՇշՈոՉչՊպՋջՌռՍսՎվՏտՐրՑցՒւՓփՔքևՕօՖֆ";
				case Language.Assamese: return "অঅআকাই কিঈকীউকুঊকূঋকৃএকেঐকৈওকোঔকৌকখগঘঙচছজঝঞটঠডঢণতথদধনপফবভমযৰলৱশষসহক্ষড়ঢ়য়";
				case Language.Awadhi: return DEVANAGARI;//not sure..
				case Language.Azerbaijani: return LATIN + "ÇçƏəĞğİiÖöŞşÜü";
				case Language.Bengali: return EASTERN_NAGARI;
				case Language.Balochi: return LATIN + "ÀàČčĎďÈèÒòŠšŤťŽž";
				case Language.Bhojpuri: return DEVANAGARI;
				case Language.Bosnian: return GAJ_LATIN;
				case Language.Burmese: return "ကခဂဃငစဆဇဈဉညဋဌဍဎဏတထဒဓနပဖဗဘမယရလဝသဟဠအဣဤဥဦဩဪ";
				case Language.Cebuano: return LATIN;
				case Language.Chewa: return LATIN;
				case Language.Chhattisgarhi: return DEVANAGARI;
				case Language.Chittagonian: return EASTERN_NAGARI;
				case Language.Corsican: return LATIN + "ÀàÈèÌìÒòÙù";
				case Language.Croatian: return GAJ_LATIN;
				case Language.Deccan: return Letters(Language.Urdu);
				case Language.Dhundhari: return LATIN;//oral language so latin i guess? (i mean, it doesn't make much sense to include it in the first place ;))
				case Language.EasternMin: return CHINESE;
				case Language.Esperanto: return LATIN + "ĈĉĜĝĤĥĴĵŜŝŬŭ";
				case Language.Frisian: return LATIN;
				case Language.Fula: return LATIN;
				case Language.Galician: return LATIN + ENE;
				case Language.GanChinese: return CHINESE;
				case Language.Georgian: return "ႠႡႢႣႤႥႦႧႨႩႪႫႬႭႮႯႰႱႲႳႴႵႶႷႸႹႺႻႼႽႾႿჀჁჂჃჄჅჇჍაბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰჱჲჳჴჵჶჷჸჹჺ჻ჼჽჾჿ";
				case Language.Gujarati: return "અઆઇઈઉઊએઐઓઔઅંઅ:ઋઍઑકખગઘઙચછજઝઞયશટઠડઢણરષતથદધનલસપફબભમવહળક્ષજ્ઞ";
				case Language.HaitianCreole: return LATIN + "ÈèÒò";
				case Language.Hakka: return CHINESE;
				case Language.Haryanvi: return DEVANAGARI;
				case Language.Hausa: return BOKO;
				case Language.Hawaiian: return LATIN;
				case Language.Hindi: return DEVANAGARI;
				case Language.Hmong: return MIAO;
				case Language.Igbo: return LATIN + "ƁɓƊɗƎǝẸẹỊịƘƙỌọṢṣỤụ";
				case Language.Ilocano: return LATIN;
				case Language.Irish: return LATIN + "ÁáÉéÍíÓóÚú";
				case Language.Javanese: return LATIN + "ꦀꦁꦂꦃꦄꦅꦆꦇꦈꦉꦊꦋꦌꦍꦎꦏꦐꦑꦒꦓꦔꦕꦖꦗꦘꦙꦚꦛꦜꦝꦞꦟꦠꦡꦢꦣꦤꦥꦦꦧꦨꦩꦪꦫꦬꦭꦮꦯꦰꦱꦲ꦳ꦴꦵꦶꦷꦸꦹꦺꦻꦼꦽꦾꦿ꧀꧁꧂꧃꧄꧅꧆꧇꧈꧉꧊꧋꧌꧍ꧏ꧐꧑꧒꧓꧔꧕꧖꧗꧘꧙꧞꧟";
				case Language.Jin: return CHINESE;
				case Language.Kannada: return "ಅಆಇಈಉಊಋಎಏಐಒಓಔಕಖಗಘಙಚಛಜಝಞಟಠಡಢಣತಥದಧನಪಫಬಭಮಯರಱಲವಶಷಸಹಳೞ";
				case Language.Kazakh: return LATIN + CYRILLIC + ARABIC;
				case Language.Khmer: return "កខគឃងចឆជឈញដឋឌឍណតថទធនបផពភមយរលវសហឡអឥឦឧឩឪឫឬឭឮឯឰឱឲឳ";
				case Language.Kinyarwanda: return LATIN;
				case Language.Kirundi: return LATIN;
				case Language.Konkani: return DEVANAGARI;
				case Language.Kurdish: return LATIN + "ÇçÊêÎîŞşÛû" + "ئـ‎ش‎س‎ژ‎ز‎ڕ‎ر‎د‎خ‎ح‎چ‎ج‎ت‎پ‎ب‎ی‎وو‎ۆ‎و‎ە‎ھ‎ن‎م‎ڵ‎ل‎گ‎ک‎ق‎ڤ‎ف‎غ‎ێ";
				case Language.Kyrgyz: return CYRILLIC + ARABIC;
				case Language.Lao: return "ກຂຄງຈສຊຍດຕຖທນບປຜຝພຟມຢຣລວຫອຮ" + Letters(Language.Thai);
				case Language.Latin: return LATIN;
				case Language.Luxembourgish: return LATIN + "ÄäÉéËë";
				case Language.Macedonian: return CYRILLIC;
				case Language.Madurese: return LATIN;
				case Language.Magahi: return DEVANAGARI;
				case Language.Maithili: return DEVANAGARI;
				case Language.Malagasy: return LATIN;
				case Language.Malay: return LATIN;
				case Language.Malayalam: return "അആഇഈഉഊഋഌഎഏഐഒഓഔകഖഗഘങചഛജഝഞടഠഡഢണതഥദധനഩപഫബഭമയരറലളഴവശഷസഹഺഽാിീുൂൃൄെേൈൊോൌ്ൎൗൠൡൢൣ൧൨൩൪൫൬൭൮൯൰൱൲൳൴൵൹ൺൻർൽൾൿ";
				case Language.Maltese: return LATIN + "ĊċĠġĦħŻż";
				case Language.Maori: return LATIN + "ĀāĒēĪīŌōŪū";
				case Language.Marathi: return DEVANAGARI;
				case Language.Marwari: return DEVANAGARI;
				case Language.Mongolian: return "᠀᠁᠂᠃᠄᠅᠆᠇᠈᠉᠊᠐᠑᠒᠓᠔᠕᠖᠗᠘᠙ᠠᠡᠢᠣᠤᠥᠦᠧᠨᠩᠪᠫᠬᠭᠮᠯᠰᠱᠲᠳᠴᠵᠶᠷᠸᠹᠺᠻᠼᠽᠾᠿᡀᡁᡂᡃᡄᡅᡆᡇᡈᡉᡊᡋᡌᡍᡎᡏᡐᡑᡒᡓᡔᡕᡖᡗᡘᡙᡚᡛᡜᡝᡞᡟᡠᡡᡢᡣᡤᡥᡦᡧᡨᡩᡪᡫᡬᡭᡮᡯᡰᡱᡲᡳᡵᡶᡷᢀᢁᢂᢃᢄᢅᢆᢇᢈᢉᢊᢋᢌᢍᢎᢏᢐᢑᢒᢓᢔᢕᢖᢗᢘᢙᢚᢛᢜᢝᢞᢟᢠᢡᢢᢣᢤᢥᢦᢧᢨᢩ";
				case Language.Mossi: return LATIN;
				case Language.Nepali: return DEVANAGARI;
				case Language.NorthernMin: return CHINESE;
				case Language.Odia: return "ଁଂଃଅଆଇଈଉଊଋଌଏଐଓଔକଖଗଘଙଚଛଜଝଞଟଠଡଢଣତଥଦଧନପଫବଭମଯରଲଳଵଶଷସହ଼ଽାିୀୁୂୃୄେୈୋୌ୍ୖୗଡ଼ଢ଼ୟୠୡୢୣ୦୧୨୩୪୫୬୭୮୯୰ୱ୲୳୴୵୶୷";
				case Language.Oromo: return LATIN;
				case Language.Pashto: return ARABIC;
				case Language.Persian: return "ابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهی‬";
				case Language.Punjabi: return "ਁਂਃਅਆਇਈਉਊਏਐਓਔਕਖਗਘਙਚਛਜਝਞਟਠਡਢਣਤਥਦਧਨਪਫਬਭਮਯਰਲਲ਼ਵਸ਼ਸਹ਼ਾਿੀੁੂੇੈੋੌ੍ੑਖ਼ਗ਼ਜ਼ੜਫ਼੦੧੨੩੪੫੬੭੮੯ੰੱੲੳੴੵᇇ";
				case Language.Quechua: return Letters(Language.Spanish);
				case Language.Samoan: return LATIN + "ĀāĒēĪīŌōŪū";
				case Language.Saraiki: return "";
				case Language.ScottishGaelic: return LATIN + "ÀàÈèÌìÒòÙù";
				case Language.Serbian: return Letters(Language.Persian);
				case Language.Sesotho: return LATIN;
				case Language.Shona: return LATIN;
				case Language.Sindhi: return Letters(Language.Persian) + DEVANAGARI;
				case Language.Sinhalese: return "ංඃඅආඇඈඉඊඋඌඍඎඏඐඑඒඓඔඕඖකඛගඝඞඟචඡජඣඤඥඦටඨඩඪණඬතථදධනඳපඵබභමඹයරලවශෂසහළෆ්ාැෑිීුූෘෙේෛොෝෞෟ෦෧෨෩෪෫෬෭෮෯ෳෲ෴";
				case Language.Somali: return LATIN;
				case Language.SouthernMin: return CHINESE;
				case Language.Sundanese: return Letters(Language.Javanese);
				case Language.Swahili: return ARABIC;
				case Language.Sylheti: return "ꠀꠁꠂꠃꠄꠅ꠆ꠇꠈꠉꠊꠋꠌꠍꠎꠏꠐꠑꠒꠓꠔꠕꠖꠗꠘꠙꠚꠛꠜꠝꠞꠟꠠꠡꠢꠣꠤꠥꠦꠧ꠨꠩꠪꠫";
				case Language.Tagalog: return LATIN;
				case Language.Tajik: return CYRILLIC + LATIN + Letters(Language.Persian);
				case Language.Tamil: return "ஂஃஅஆஇஈஉஊஎஏஐஒஓஔகஙசஜஞடணதநனபமயரறலளழவஶஷஸஹாிீுூெேைொோௌ்ௐௗ௦௧௨௩௪௫௬௭௮௯௰௱௲௳௴௵௶௷௸௹௺";
				case Language.Telugu: return "ఀఁంః፽అఆఇఈఉఊఋఌఎఏఐఒఓఔకఖగఘఙచఛజఝఞటఠడఢణతథదధనపఫబభమయరఱలళవశషసహఽాిీుూృౄెేైొోౌ్ౕౖౘౙౚౠౡౢౣ౦౧౨౩౪౫౬౭౮౯౸౹౺౻౼౽౾౿";
				case Language.Turkmen: return LATIN + CYRILLIC;
				case Language.Urdu: return ARABIC;
				case Language.Uyghur: return ARABIC;
				case Language.Uzbek: return LATIN + CYRILLIC + ARABIC;
				case Language.Visayan: return LATIN;
				case Language.Welsh: return LATIN;
				case Language.Wu: return CHINESE;
				case Language.Xhosa: return LATIN;
				case Language.Xiang: return CHINESE;
				case Language.Yiddish: return HEBREW;
				case Language.Yoruba: return LATIN + "ƁɓƊɗƎǝẸẹỊịƘƙỌọṢṣỤụ";
				case Language.Yue: return CHINESE;
				case Language.Zhuang: return CHINESE;
				case Language.Zulu: return LATIN;
				default:
					return "";
			}
		}
		
		public class Sample{
			
			string defaultFontResults = "";
			Dictionary<Font, string> results = new Dictionary<Font, string>();
			
			/** Adds a language with a defined font to this sample */
			public void AddLanguage(Language language, Font font){
				
				string sample = LanguageSamples.Letters(language);
				
				if(font == null){
					defaultFontResults += sample;
				}else if(results.ContainsKey(font)){
					results[font] += sample;
				}else{
					results.Add(font, sample);
				}
				
			}
			
			string removeDuplicates(string input){
				string output = "";
				foreach(char c in input){
					if(output.IndexOf(c) == -1)
						output += c;
				}
				return output;
			}
			
			/** Prepare the results once before displaying */
			public void Prepare(){
				defaultFontResults = removeDuplicates(defaultFontResults);
				foreach(KeyValuePair<Font, string> pair in results){
					results[pair.Key] = removeDuplicates(pair.Value);
				}
			}
			
			/** Call from an OnGUI() method */
			public void DisplayResults(Font defaultFont, int fontSize){
				//start with default font:
				GUIStyle style = new GUIStyle();
				style.font = defaultFont;
				style.wordWrap = true;
				style.fontSize = fontSize;
				GUILayout.Label(defaultFontResults, style);
				//then move on to languages that have custom fonts:
				foreach(KeyValuePair<Font, string> pair in results){
					style.font = pair.Key;
					GUILayout.Label(pair.Value, style);
				}
			}
			
		}
		
	}
}
#endif
