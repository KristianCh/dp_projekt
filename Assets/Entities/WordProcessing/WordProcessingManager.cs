using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Entities.GameManagement;
using Entities.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Entities.WordProcessing
{
    public class WordProcessingManager : MonoBehaviour, IService
    {
        private struct WordDataEntry
        {
            public string MainWord { get; set; }
            public float RecordedAoA { get; set; }
            public List<string> RelatedWords { get; set; }

            [UsedImplicitly]
            public bool Equals(WordDataEntry x, WordDataEntry y) => x.MainWord == y.MainWord;
            
            public WordDataEntry(string mainWord, float recordedAoA, List<string> relatedWords)
            {
                MainWord = mainWord;
                RecordedAoA = recordedAoA;
                RelatedWords = relatedWords;
            }
        }
        
        [SerializeField]
        private string _WordDataFilePath = "Assets/Entities/DataManagement/WordData.csv";
        
        [SerializeField]
        private char _MainSeparator = ';';
        
        [SerializeField]
        private char _RelatedWordsSeparator = ',';
        
        private List<WordDataEntry> _relatedWordData = new List<WordDataEntry>();
        
        private List<string> _unparsedRelatedWords = new List<string>()
        {
            "prístrešie;6.245283018867925;stavba,zemľanka,prístav,chatrč,chata,útočisko,stan",
            "pole;6.62;farma,pestovanie,lúka,trakt,obilné pole, trávnik, dvor",
            "jablko;3.66;plod,ovocie",
            "orol;7.839285714285714;vták,predátor,lovec",
            "doktor;7.18;chirurg,veterinár,internista,gastroenterológ",
            "azyl;10.0;útočisko,prístrešie",
            "atóm;12.38888888888889;látka,izotop,prvok",
            "auto;4.1;vozidlo,limuzína,kabriolet,kupé",
            "bábätko;3.792452830188679;dieťa,novorodenec,batoľa,dojča",
            "hora;5.758064516129032;kopec,Tatry,Alpy",
            "kniha;4.838709677419355;publikácia,brožúra,katalóg,rozprávka,román",
            "farmár;5.196428571428571;poľnohospodár,včelár,pestovateľ",
            "ikry;13.928571428571429;kaviár,ryba,vajcia",
            "oheň;5.796296296296297;požiar,uhlie,vatra,táborák",
            "brat;3.792452830188679;súrodenec,rodina",
            "chlieb;3.8518518518518516;jedlo,pečivo,múka,sendvič",
            "démon;11.851851851851851;diaboľ,satan,peklo,zlo",
            "zviera;3.8;organizmus,cicavec,plaz,tvor",
            "obeť;10.62962962962963;zavraždený,mučeník,korisť",
            "rozdiel;12.882352941176471;nedorozumenie,rozmanitosť,inakosť,odlišnosť,rozpor",
            "elita;12.543859649122806;vyššia_trieda,vyvolený,smotánka,šľachta",
            "chyba;13.392156862745098;nezvládnutie,zmätok,nedopatrenie,hlúpsť",
            "povesť;11.481481481481481;česť,renomé,sláva,meno",
            "zločinec;12.133333333333333;zloduch,zlodej,mafián,vrah,gangster",
            "flóra;10.481481481481481;stromy,rastliny,záhrada,porast",
            "peniaze;4.842105263157895;hotovosť,platidlo",
            "cech;11.338709677419354;spolok,asociácia,klub",
            "sklo;4.666666666666667;okno,fľaša,zrkadlo",
            "sociálne_zabezpečenie;11.0;pomoc,poistenie,podpora",
            "dutina;7.935483870967742;diera,jaskyňa,jama",
            "pes;4.048387096774194;cicavec,korgi,dunčo,šteňa,domáce_zviera",
            "kosť;10.040816326530612;lebka,dreň,časť_tela,rebro",
            "náboj;7.092592592592593;projektil,strela,pištol,zbraň",
            "pieseň;12.363636363636363;skladba,balada,hymna,serenáda",
            "plač;7.824561403508772;rev,krik,húkanie",
            "smiech;5.0701754385964914;chichot,úsmev,vtip,zábava",
            "lampa;4.821428571428571;zdroj_svetla,sviečka,lanterna,osvetlenie,svetlo",
            "slogan;10.975609756097562;príslovie",
            "sila;8.641509433962264;elektrická,moc,vplyv,potenciál",
            "žena;5.859649122807017;človek,manželka,dievča,matriarcha",
            "mlieko;4.140350877192983;nápoj,krava,laktóza,cmar,acidko",
            "človek;4.767857142857143;muž,žena,hominid,neandrtálec,homo_sapiens",
            "maloletý;13.372093023255815;mladistvý,dieťa,školák,batoľa",
            "oxid;13.5;zlúčenina,chemikália,hrdza",
            "papier;13.36;materiál,papyrus,dokument,zošit",
            "otec;7.403225806451613;rodič,muž,rodina",
            "pizza;7.321428571428571;jedlo,prílohy,syr,margarita",
            "mor;8.157894736842104;epidémia,choroba,čierny_kašeľ,zhuba",
            "cena;6.264150943396227;hodnota,nákup,platba",
            "princ;4.56;aristokrat,kráľ,dedič",
            "lopta;8.407407407407407;hra,fotbal,ping_pong,guľa",
            "próza;12.84;novla,román,kniha,literatúra",
            "rozsah;12.547619047619047;spektrum,miera",
            "rozum;13.767857142857142;motív,rozhodovanie,zmýšľanie,uvažovanie",
            "rečník;14.2;hlásateľ,debatér,hovorca",
            "obrad;12.552631578947368;ceremónia,liturgia,omša,pohreb",
            "novela;10.38888888888889;fikcia,kniha,príbeh,román",
            "dobytok;7.351851851851852;krava,býk,vôl,hovädzie",
            "húsenica;5.357142857142857;larva,hmyz,motýľ",
            "loď;5.87037037037037;plavidlo,bárka,čln,kajak,ponorka",
            "sekta;11.981481481481481;náboženstvo,kult,rád",
            "tabak;14.051282051282051;cigareta,droga,fajčenie,cigára",
            "šáľ;5.796296296296297;oblečenie,šatka,krk",
            "otrok;7.87037037037037;sluha,služobník,vlastníctvo,nevoľník",
            "slimák;5.357142857142857;gastropod,skrupina,sliz",
            "had;6.4;užovka,pytón,plaz,zmija",
            "ohováranie;12.285714285714286;znevažovanie,pošpinenie,osočovanie,ocierňovanie",
            "hmla;10.435483870967742;ráno,oblak",
            "občerstvenie;10.12962962962963;jedlo,pamlsok,olovrant,desiata",
            "polievka;4.160714285714286;vývar,bujón,jedlo,boršč",
            "slanina;7.314814814814815;prasa,bravčové,mäso",
            "hra;4.214285714285714;zábava,stolová,spoločenská,kartová",
            "sval;6.885245901639344;teľo,mäso,cvičenie,tensor",
            "pavúk;5.314814814814815;sieť,tarantula,tkáč",
            "šport;5.892857142857143;atletika,gymnastika,cvičenie,preteky",
            "mesto;5.615384615384615;magistrát,centrum,miesto",
            "štart;5.131147540983607;začiatok,nástup,otvorenie,úsvit",
            "kameň;4.6415094339622645;balvan,skala,žula",
            "hviezda;5.555555555555555;nebeské_teleso,slnko,žiara",
            "palica;11.11320754716981;papek,tyč,konár",
            "tím;9.407407407407407;jednotka,spolok,družina,partia",
            "čaj;6.8;nápoj,pohár,teplý,bylinky"
        };
        
        public void Start()
        {
            GameManager.AddService(this);
            //CsvReader.Read(_WordDataFilePath, _MainSeparator, out var result);
            

            foreach (var line in _unparsedRelatedWords)
            {
                var result = CsvReader.ParseLine(line, _MainSeparator);
                var aoa = float.Parse(result[1], CultureInfo.InvariantCulture.NumberFormat);
                var relatedWords = result[2].Split(_RelatedWordsSeparator).ToList();
                
                _relatedWordData.Add(new WordDataEntry(result[0], aoa, relatedWords));
            }      
        }

        public WordTriple GetWordTriple()
        {
            var mainWordData = RandomUtils.RandomFromList(_relatedWordData);
            var otherWordData = mainWordData;
            while (otherWordData.Equals(mainWordData)) 
                otherWordData = RandomUtils.RandomFromList(_relatedWordData);
            return new WordTriple
            (
                mainWordData.MainWord, 
                RandomUtils.RandomFromList(mainWordData.RelatedWords), 
                RandomUtils.RandomFromList(otherWordData.RelatedWords),
                mainWordData.RecordedAoA
            );
        }

        public string NicifyWord(string word)
        {
            var newWord = word.Replace("_", " ");
            return char.ToUpper(newWord[0]) + newWord[1..];
        }
    }
}