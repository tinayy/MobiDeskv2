using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobideskv2
{
    public class bind: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChangeProperty(String propertyname)
        {
            if(this.PropertyChanged!=null){
                PropertyChanged(this,new PropertyChangedEventArgs(propertyname));
            }
        }

        public String status
        {
            get { return _stat; }
            set
            {
                if (value != _stat)
                {
                    _stat = value;
                NotifyChangeProperty("status");
             }
           }
        }
        //objects obj = new objects();
        private String _stat = "status";

        public String file
        {
            get { return _file; }
            set
            {
                if (value != _file)
                {
                    _file = value;
                    NotifyChangeProperty("file");
                }
            }
        }

        private String _file = "File";

        public String changes
        {
            get { return _changes; }
            set
            {
                if (value != _changes)
                {
                    _changes = value;
                    NotifyChangeProperty("changes");
                }
            }
        }

        private String _changes = "";

   
    }
}
