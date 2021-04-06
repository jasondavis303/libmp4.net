using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace libmp4.net.Internal
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Proper name of atom")]
    class iTunMOVI
    {
        public List<string> Cast { get; set; } = new List<string>();
        public List<string> Directors { get; set; } = new List<string>();
        public List<string> Producers { get; set; } = new List<string>();
        public List<string> ScreenWriters { get; set; } = new List<string>();
        public string Studio { get; set; }

        private bool ListHasData(List<string> lst) =>  lst
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct()
            .Any();

        public bool HasData =>
            ListHasData(Cast) ||
            ListHasData(Directors) ||
            ListHasData(Producers) ||
            ListHasData(ScreenWriters) ||
            !string.IsNullOrEmpty(Studio);


        public override string ToString()
        {
            try { return Encoding.UTF8.GetString(ToData()); }
            catch { return string.Empty; }
        }

        public byte[] ToData()
        {
            /*﻿
                    ﻿<?xml version="1.0" encoding="UTF-8"?>
                    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                    <plist version="1.0">
                        <dict>
                            <key>cast</key>
                            <array>
                                <dict>
                                    <key>name</key>
                           <string>Matt Smithn</string>
                                </dict>
                            </array>

                            <key>studio</key>
                         <string>BBC</string>
                        </dict>
                    </plist>
                */

            if (!HasData)
                return null;

            XmlDocument doc = new XmlDocument();

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            doc.AppendChild(doc.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null));

            XmlNode plist = doc.CreateNode(XmlNodeType.Element, "plist", null);
            plist.Attributes.Append(doc.CreateAttribute("version"));
            plist.Attributes[0].Value = "1.0";
            doc.AppendChild(plist);

            XmlNode dict = doc.CreateNode(XmlNodeType.Element, "dict", null);
            plist.AppendChild(dict);

            BuildXmlArray(doc, dict, "cast", Cast);
            BuildXmlArray(doc, dict, "directors", Directors);
            BuildXmlArray(doc, dict, "producers", Producers);
            BuildXmlArray(doc, dict, "screenwriters", ScreenWriters);

            if (!string.IsNullOrWhiteSpace(Studio))
            {
                XmlNode key = doc.CreateNode(XmlNodeType.Element, "key", null);
                key.InnerText = "studio";
                dict.AppendChild(key);

                XmlNode xstring = doc.CreateNode(XmlNodeType.Element, "string", null);
                xstring.InnerText = Studio.Trim();
                dict.AppendChild(xstring);
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] xmlData = Encoding.UTF8.GetBytes(doc.InnerXml);

            byte[] data = new byte[preamble.Length + xmlData.Length];
            Buffer.BlockCopy(preamble, 0, data, 0, preamble.Length);
            Buffer.BlockCopy(xmlData, 0, data, preamble.Length, xmlData.Length);

            return data;
        }

        void BuildXmlArray(XmlDocument doc, XmlNode parentDict, string name, List<string> lst)
        {
            if (lst == null)
                return;

            lst = lst
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .ToList();

            if (lst.Count == 0)
                return;

            XmlNode key = doc.CreateNode(XmlNodeType.Element, "key", null);
            key.InnerText = name;
            parentDict.AppendChild(key);

            XmlNode array = doc.CreateNode(XmlNodeType.Element, "array", null);
            parentDict.AppendChild(array);
            foreach (string item in lst)
            {
                XmlNode dict = doc.CreateNode(XmlNodeType.Element, "dict", null);
                array.AppendChild(dict);

                key = doc.CreateNode(XmlNodeType.Element, "key", null);
                key.InnerText = "name";
                dict.AppendChild(key);

                XmlNode xstring = doc.CreateNode(XmlNodeType.Element, "string", null);
                xstring.InnerText = item;
                dict.AppendChild(xstring);
            }
        }

        public static iTunMOVI Read(string text)
        {
            try { return Read(Encoding.UTF8.GetBytes(text + string.Empty)); }
            catch { return new iTunMOVI(); }
        }

        public static iTunMOVI Read(byte[] data)
        {
            /*
                iTunes XML:

                    ﻿<?xml version="1.0" encoding="UTF-8"?>
                    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">

                    <plist version="1.0">
                        <dict>
                            <key>cast</key>
                            <array>
                                <dict>
                                    <key>name</key>
                           <string>Matt Smithn</string>
                                </dict>
                            </array>

                            <key>studio</key>
                         <string>BBC</string>
                        </dict>
                    </plist>

                    cast, directors, producers, screenwriters
                */

            var ret = new iTunMOVI();
            if (data == null || data.Length == 0)
                return ret;

            //Check for BOM
            Encoding xmlEnc = null;
            foreach (EncodingInfo encInfo in Encoding.GetEncodings())
            {
                Encoding test = encInfo.GetEncoding();
                byte[] preamble = test.GetPreamble();
                if (preamble.Length > 0)
                {
                    byte[] prefix = new byte[preamble.Length];
                    Array.Copy(data, 0, prefix, 0, preamble.Length);
                    if (preamble.SequenceEqual(prefix))
                    {
                        xmlEnc = test;
                        break;
                    }
                }
            }
            if (xmlEnc == null)
                xmlEnc = Encoding.UTF8;

            string xmlText = xmlEnc.GetString(data);
            xmlText = xmlText.Substring(xmlText.IndexOf('<'));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);
            XmlNode rootDict = doc.SelectSingleNode("plist").SelectSingleNode("dict");

            foreach (XmlNode keyNode in rootDict.SelectNodes("key"))
            {
                string cleanKey = (keyNode.InnerText + string.Empty).Trim().ToLower();
                if (new string[] { "cast", "directors", "producers", "screenwriters", "studio" }.Contains(cleanKey))
                {
                    List<string> lst = null;
                    if (cleanKey == "cast") lst = ret.Cast;
                    if (cleanKey == "directors") lst = ret.Directors;
                    if (cleanKey == "producers") lst = ret.Producers;
                    if (cleanKey == "screenwriters") lst = ret.ScreenWriters;
                    if (cleanKey == "studio") lst = new List<string>();

                    XmlNode dataNode = keyNode.NextSibling;
                    if (dataNode != null)
                    {
                        if (dataNode.Name == "string")
                        {
                            string childValue = (dataNode.InnerText + string.Empty).Trim();
                            if (!string.IsNullOrWhiteSpace(childValue))
                                lst.Add(childValue);
                        }
                        else if (dataNode.Name == "array")
                        {
                            foreach (XmlNode dictChild in dataNode.ChildNodes)
                            {
                                XmlNode childKey = dictChild.SelectSingleNode("key");
                                if (childKey != null)
                                {
                                    XmlNode childString = dictChild.SelectSingleNode("string");
                                    if (childString != null)
                                    {
                                        string childValue = (childString.InnerText + string.Empty).Trim();
                                        if (!string.IsNullOrWhiteSpace(childValue))
                                            lst.Add(childValue);
                                    }
                                }

                            }
                        }
                    }

                    if (cleanKey == "studio")
                        if (lst.Count > 0)
                            ret.Studio = string.Join(",", lst);
                }
            }

            return ret;
        }

    }
}
