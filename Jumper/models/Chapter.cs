using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumper.models
{
    public class Chapter : INotifyPropertyChanged
    {
        
        private string name,mname,src;
        public string MName { get { return mname; } set { mname = value; OnPropertyChanged("MName"); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public string Source { get { return src; } set { src = value; OnPropertyChanged("src"); } }
        public Uri ChapterURL;
        public List<string> ImageLocations;

        public Chapter(string n, string url)
        {
            Name = n;
            ChapterURL = new Uri(url);
            ImageLocations = new List<string>();
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public void GetImageLocations(List<string> images)
        {
            foreach (string i in images)
            {
                ImageLocations.Add(i);
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
