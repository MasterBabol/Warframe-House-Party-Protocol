//////////////////////////////////////////////////////////////////////////////////
// DictionarySaver for Warframe House Party Protocol
// Copyright (c) 2017 Babol.
// Contributed by Babol <babol@live.co.kr>
//
// This file is part of Warframe House Party Protocol.
//
// Warframe House Party Protocol is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3, or (at your option)
// any later version.
//
// Warframe House Party Protocol is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Warframe House Party Protocol; see the file COPYING.
// If not, see <http://www.gnu.org/licenses/>.
//
//////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Xml
{
    class DictionarySaver : IDisposable
    {
        private string filePath;
        private Dictionary<string, string> items;

        public DictionarySaver(string filePath)
        {
            items = new Dictionary<string, string>();
            this.filePath = filePath;
            ReloadFile();
        }

        public void SetItem(string name, string objectToSet)
        {
            items[name] = objectToSet;
        }

        public string GetItem(string name)
        {
            object item;
            try
            {
                item = items[name];
                if (item != null)
                    return item.ToString();
                else throw new Exception();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void EmptyItems()
        {
            items.Clear();
        }

        public bool ReloadFile()
        {
            XmlDocument configXML = new XmlDocument();
            try
            {
                configXML.Load(filePath);
                XmlNode rootNode = configXML.SelectSingleNode("root");

                foreach (XmlNode child in rootNode.ChildNodes)
                {
                    items[child.Name] = child.InnerText;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        public bool UpdateFile()
        {
            XmlDocument configXML = new XmlDocument();
            configXML.AppendChild(configXML.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlNode rootNode = configXML.CreateElement("root");
            configXML.AppendChild(rootNode);
            try
            {
                XmlNode node;
                foreach (string itemName in items.Keys)
                {
                    node = configXML.CreateElement(itemName);
                    node.InnerText = items[itemName];
                    rootNode.AppendChild(node);
                }
                configXML.Save(filePath);
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        private bool disposed = false;
        ~DictionarySaver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            UpdateFile();

            disposed = true;
        }
    }
}
