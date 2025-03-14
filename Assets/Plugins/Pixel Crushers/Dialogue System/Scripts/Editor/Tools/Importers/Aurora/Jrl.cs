#if USE_AURORA
// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Aurora
{

    /// <summary>
    /// XML schema for journal files.
    /// </summary>
    [XmlRoot("gff")]
    [System.Serializable]
    public class Jrl
    {
        [XmlAttribute("name")]
        public string name;

        [XmlAttribute("type")]
        public string type;

        [XmlAttribute("version")]
        public string version;

        [XmlElement("struct")]
        public Struct topLevelStruct;

        [XmlIgnore]
        public List<Struct> Categories { get { return topLevelStruct.GetElementStructs("Categories"); } }

        public static Jrl Load(TextAsset xmlFile)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Jrl));
            return xmlSerializer.Deserialize(new StringReader(xmlFile.text)) as Jrl;
        }

        public static Jrl Load(string filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Jrl));
            return xmlSerializer.Deserialize(new StreamReader(filename)) as Jrl;
        }

        public static Jrl Load(string filename, Encoding encoding)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Jrl));
            return xmlSerializer.Deserialize(new StreamReader(filename, encoding)) as Jrl;
        }
    }

}
#endif
