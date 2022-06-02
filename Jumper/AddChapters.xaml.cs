using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jumper
{
    /// <summary>
    /// Interaction logic for AddChapters.xaml
    /// </summary>
    public partial class AddChapters : Window
    {
        public int StartIndx;
        public int EndIndx;
        public List<int> Excludes;
        private List<models.Chapter> clist;
        public AddChapters(models.Manga m)
        {
            InitializeComponent();
            Start.ItemsSource = m.Chapters;
            End.ItemsSource = m.Chapters;
            clist = m.Chapters.ToList();
            Excludes = new List<int>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartIndx = Start.SelectedIndex;
            EndIndx = End.SelectedIndex;
            List<string> exc = Exclude.Text.Split(',').ToList();
            foreach(string s in exc)
            {
                if(clist.Where(x => x.Name == s).Count() > 0)
                {
                    Excludes.Add(clist.IndexOf(clist.Where(x => x.Name == s).First()));
                }
            }
            Close();
        }

        private void Start_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clist.Count == 1)
            {
                End.ItemsSource = clist;
            }
            else
            {
                if (double.Parse(GetChapterNumber(clist[0].Name)) < double.Parse(GetChapterNumber(clist[1].Name)))
                {
                    End.ItemsSource = clist.Where(x => clist.IndexOf(x) >= Start.SelectedIndex);
                }
                else
                {
                    End.ItemsSource = clist.Where(x => clist.IndexOf(x) <= Start.SelectedIndex);
                }
            }
            
           
        }
        protected string GetChapterNumber(string linktext)
        {
            string s;
            string lt = linktext;

            if (lt.Contains("Ch."))
            {
                lt = lt.Replace("Ch.", "");
            }
            if (lt.Contains("ch."))
            {
                lt = lt.Replace("ch.", "");
            }
            if (lt.Contains("Vol."))
            {
                lt = lt.Replace("Vol.", "");
            }
            if (lt.Contains("vol."))
            {
                lt = lt.Replace("vol.", "");
            }
            if (lt.Contains("..."))
            {
                lt = lt.Replace("...", "");
            }
            if (lt.Contains("V"))
            {
                lt = lt.Replace("V", "");
            }
            List<string> sx = Regex.Split(lt, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "").ToList();

            if (sx.Count > 1)
            {
                List<double> ld = new List<double>();
                foreach (string x in sx)
                {
                    ld.Add(double.Parse(x));
                }
                return ld.Max().ToString();
            }

            s = sx[0];

            return s;
        }
    }
}
