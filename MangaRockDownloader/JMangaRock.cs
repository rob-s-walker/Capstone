using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumper.models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;

namespace MangaRockDownloader
{
    public class JMangaRock : Downloader
    {
        public override string Stringvalue { get => "MANGAROCK"; set { } }

        public override List<string> GetImageURLs(Chapter chapter, ChromeDriver web, Action<double, string> a)
        {
            List<string> ulist = new List<string>();
            web.Navigate().GoToUrl(chapter.ChapterURL);
            if (ElementExists(web, By.CssSelector("div > div._3tgGR > div > h2")))
            {
                if (web.FindElementByCssSelector("div > div._3tgGR > div > h2").Text == "Mature content")
                {
                    List<IWebElement> e = web.FindElementsByCssSelector("button.mdl-button.mdl-js-button.mdl-js-ripple-effect._2hRSc").ToList();
                    e[1].Click();
                }
            }
            WaitForElement(web, By.CssSelector("div._3Oahl.vo2e_ > select"));
            SelectElement select = new SelectElement(web.FindElementByCssSelector("div._3Oahl.vo2e_ > select"));
            for (int i = 0;i < select.Options.Count; i++) 
            {
                select.SelectByIndex(i);
                WaitForElement(web, By.ClassName("_3ybIG"));
                double percent = ((double)i / select.Options.Count);
                a.Invoke(percent, "");
               
                string url = "";
                while (true)
                {
                    try
                    {
                        if (!ElementExists(web, By.CssSelector("div[data-index='" + i + "'] > figure > canvas")))
                        {
                            break;
                        }
                        WaitForElement(web, By.CssSelector("div[data-index='" + i + "'] > figure > canvas"),3);
                        IWebElement we = web.FindElementByCssSelector("div[data-index='" + i + "'] > figure > canvas");
                        url = (string)web.ExecuteScript("return document.querySelector(\"div[data-index='" + i + "'] > figure > canvas\").toDataURL();");
                        ulist.Add(url);
                        break;
                    }
                    catch (Exception)
                    {
                       // web.Navigate().GoToUrl(chapter.ChapterURL);
                      //  select = new SelectElement(web.FindElementByClassName("vo2e_").FindElement(By.TagName("select")));
                        continue;
                    }
                }
                
                
            }
            return ulist;
        }

        public override Manga GetManga(Uri manga, ChromeDriver web)
        {
            List<Chapter> clist = new List<Chapter>();
            WaitForElement(web, By.ClassName("ptmaY"));
            var b = web.FindElementsByCssSelector("button.mdl-button.mdl-js-button.mdl-js-ripple-effect._2hRSc._1tOBA")[1];
            b.Click();
            string name = web.FindElementByClassName("_3kDZW").Text;
            

            List<IWebElement> links = web.FindElementByClassName("ptmaY").FindElements(By.TagName("a")).ToList();

            for (int i = 0; i < links.Count; i++)
            {
                IWebElement l = links[i];
                string url = l.GetAttribute("href");

                string lt =l.Text;
                while(lt == "")
                {
                    lt = l.Text;
                }
                string n = GetChapterNumber(lt);
                Chapter c = new Chapter(n, url);
                c.MName = name;
                c.Source = Stringvalue;
                if (clist.Where(x => x.Name == n).Count() == 0)
                {
                    clist.Add(c);
                }

            }
            return new Manga(name, clist);
        }
    }
}
