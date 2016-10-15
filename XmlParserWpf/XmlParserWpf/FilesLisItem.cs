﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using TracerLib;
using TracerLibXmlParser;

namespace XmlParserWpf
{
    public class FilesListItem: INotifyPropertyChanged
    {
        public string Path { get; private set; }
        public string Name => System.IO.Path.GetFileName(Path);
        public List<ThreadsListItem> ThreadsList { get; }
        private bool _isSaved;

        public bool IsSaved
        {
            get
            {
                return _isSaved;
            }
            set
            {
                _isSaved = value;
                OnPropertyChanged("IsSaved");
            }
        }

        public static FilesListItem LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            FilesListItem result = new FilesListItem()
            {
                Path = path,
                IsSaved = false
            };

            var doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch (XmlException ex)
            {
                throw new BadXmlException("Error loading XML-file", ex);
            }
            result.FromXmlDocument(doc);

            return result;
        }

        public void OnChanged()
        {
            IsSaved = false;
        }

        // Internal

        private void FromXmlDocument(XmlDocument doc)
        {
            XmlElement xe = doc.FirstChild as XmlElement;
            if (xe == null || xe.Name != XmlConstants.RootTag)
            {
                throw new BadXmlException();
            }

            foreach (XmlElement child in xe.ChildNodes)
            {
                ThreadsList.Add(ThreadsListItem.FromXmlElement(child));
            }

            IsSaved = true;
        }

        private FilesListItem()
        {
            ThreadsList = new List<ThreadsListItem>();
        }

        // INotifyPropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
