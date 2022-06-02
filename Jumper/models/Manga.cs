using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumper.models
{
    public class Manga
    {
        public string Name;

        public ObservableCollection<Chapter> Chapters ;

        public Manga(string n, List<Chapter> chapters)
        {
            Name = n;
            Chapters = new ObservableCollection<Chapter>();
            foreach (Chapter c in chapters)
            {
                Chapters.Add(c);
            }
        }
    }
}
