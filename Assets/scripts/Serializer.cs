using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

public class Serializer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

//[XmlType(TypeName = "SceneAnalyzerOutput")]
//[XmlRoot("SceneAnalyzerOutput", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
//public class SceneAnalyzerOutput{
//	[XmlArray("AnalyzedFrames")]
//	[XmlArrayItem("AnalyzedFrame", typeof(AnalyzedFrame))]
//	public AnalyzedFrame[] AnalyzedFrame { get; set; }
//
//
//}
//
//[XmlType(TypeName = "AnalyzedFrame")]
//public class AnalyzedFrame{
//
//	[XmlAttribute]
//	public int id { get; set; }
//
//	[XmlAttribute]
//	public int timestamp { get; set; }
//
//	[XmlAttribute]
//	public string mode { get; set; }
//
//	[XmlAttribute]
//	public float cameraHeight { get; set; }
//
//	[XmlAttribute]
//	public bool muteState { get; set; }
//
//
//	[XmlElement("Orientation", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
//	public Orientation Orientation { get; set; }
//}
//
//[XmlType(TypeName = "Orientation")]
//public class Orientation{
//	[XmlAttribute]
//	public float yaw { get; set; }	
//
//	[XmlAttribute]
//	public float pitch { get; set; }
//
//	[XmlAttribute]
//	public float roll { get; set; }	
//}
