using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.Localize.Sample
{
    public class CSVLocalizeInitializer : MonoBehaviour
    {
        public enum SupportLanguage
        {
            kor,
            eng,
        }

        public class CSVData : ILocalizeData
        {
            public string LocalizeID => _localizeID;

            string _localizeID;
            string _korea;
            string _english;

            public CSVData(string localizeID, string korea, string english)
            {
                this._localizeID = localizeID;
                this._korea = korea;
                this._english = english;
            }

            public string GetLocalizeID() => LocalizeID;

            public string GetLocalizeText(SystemLanguage systemLanguage)
            {
                switch (systemLanguage)
                {
                    case SystemLanguage.Korean: return _korea;
                    case SystemLanguage.English: return _english;

                    default:
                        Debug.LogError($"unsuport language:{systemLanguage}");
                        return "";
                }
            }
        }

        [SerializeField]
        private TextAsset _csvData;

        private void Awake()
        {
            CSVData[] data = ParsingCSVData(_csvData.text);
            ILocalizeManager manager = GetComponent<LocalizeManagerComponent>()
                .AddData(data)
                .ChangeLanguage(SystemLanguage.Korean);

            LocalizeComponentBase.Init(manager);

            // For FontInitializer.Awake()
            Invoke(nameof(InitLanguage), 0.1f);
        }

        void InitLanguage()
        {
            GetComponent<LocalizeManagerComponent>().ChangeLanguage(SystemLanguage.Korean);
        }

        private CSVData[] ParsingCSVData(string csv)
        {
            List<CSVData> datas = new List<CSVData>();
            string[] lines = csv.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string[] splitLine = lines[i].Split(',');
                if (splitLine.Length < 3)
                    break;

                CSVData data = new CSVData(splitLine[0], splitLine[1], splitLine[2]);
                datas.Add(data);
            }

            return datas.ToArray();
        }
    }
}
