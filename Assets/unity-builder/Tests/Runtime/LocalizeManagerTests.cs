using NUnit.Framework;
using System;
using UnityEngine;
using UNKO.Localize;

public class LocalizeManagerTests
{
    public class TestData : ILocalizeData
    {

        public string LocalizeID;
        public string ko;
        public string en;

        public TestData(string LocalizeID, string ko, string en)
        {
            this.LocalizeID = LocalizeID;
            this.ko = ko;
            this.en = en;
        }

        public string GetLocalizeID() => LocalizeID;

        public string GetLocalizeText(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                case SystemLanguage.Korean: return ko;
                case SystemLanguage.English: return en;

                default:
                    throw new Exception();
            }
        }
    }

    [Test]
    public void Test()
    {
        TestData[] data = new TestData[]
        {
            new TestData("1", "일", "one"),
            new TestData("2", "이", "two"),
        };

        ILocalizeManager manager = new LocalizeManagerComponent();
        manager.AddData(data);

        manager.ChangeLanguage(SystemLanguage.English);
        Assert.AreEqual(manager.GetLocalizeText("1"), "one");
        Assert.AreEqual(manager.GetLocalizeText("2"), "two");

        manager.ChangeLanguage(SystemLanguage.Korean);
        Assert.AreEqual(manager.GetLocalizeText("1"), "일");
        Assert.AreEqual(manager.GetLocalizeText("2"), "이");
    }
}
