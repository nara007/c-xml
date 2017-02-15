using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;

using System.Linq;
using System.Text;
using System.Xml.Serialization;

public class XMLProcessor : MonoBehaviour
{

	XmlDocument doc;
	SceneAnalyzerOutput objs;
	Hashtable focusedObstacles = new Hashtable ();
	Hashtable absoluteDirection = new Hashtable ();

	//	AnalyzedFrame[] frameArray;

	// Use this for initialization
	void Start ()
	{
		doc = new XmlDocument ();
		ProcessXML ();	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void ProcessXML ()
	{
		string path = Application.dataPath + "/xml/scene.xml";
//		string path = Application.dataPath + "/xml/cars.xml";

		if (File.Exists (path)) {
			Debug.Log ("file exits...");
//			doc.Load(path); 
//
//			XmlElement root = doc.DocumentElement;
//			XmlNodeList personNodes = root.GetElementsByTagName("AnalyzedFrame");
//
//			Debug.Log (personNodes.Count);
//
//			frameArray = new AnalyzedFrame[personNodes.Count];
//
//			int i = 0;
//			foreach (XmlNode node in personNodes)
//			{
//				frameArray [i] = new AnalyzedFrame ();
//
////				string id = ((XmlElement)node).GetElementsByTagName("Obstacle")[0].Attributes["id"].Value;   //获取Name属性值
//				XmlNode el = ((XmlElement)node).GetElementsByTagName("Obstacle")[0];
//				if (el!=null) {
//					string id = el.Attributes["id"].Value;
//					Debug.Log(id);
//				}
//				i++;
//
//			}

//			SceneAnalyzerOutput objs = null;


			XmlSerializer serializer = new XmlSerializer (typeof(SceneAnalyzerOutput));

			StreamReader reader = new StreamReader (path);
			objs = (SceneAnalyzerOutput)serializer.Deserialize (reader);

			Debug.Log (objs.AnalyzedFrame [5].Obstacle [1].centralPosition.x);

			OutPutSVG (50);
			reader.Close ();

		}
	}


	public void OutPutSVG (float direction)
	{

		focusedObstacles.Clear ();
		absoluteDirection.Clear ();
		string htmlHead = "<!DOCTYPE html>\n<html lang=\"en\">\n\n<head>\n<meta charset=\"UTF-8\">\n<title>Document\n</title>\n<style>\n    .hover_group:hover {\n        /*opacity: 0.5;*/\n        cursor: pointer;\n    }\n    </style>\n</head>\n\n<body>";
		string htmlTail = "</body>\n\n</html>";
		string SVGHeader = "<svg width=\"100%\" height=\"500px\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">";
		string SVGTail = " </svg>";
		float startDegree;
		float endDegree;

		if (direction < 30) {

			startDegree = 330 + direction;
			endDegree = direction + 30;

		} else if (direction > 330) {
			startDegree = direction - 30;
			endDegree = 30 - (360 - direction);
		} else {
			startDegree = direction - 30;
			endDegree = direction + 30;
		}

		if (objs == null) {
			return;
		} else {
			foreach (AnalyzedFrame curAnalyzedFrame in objs.AnalyzedFrame) {
				foreach (Obstacle obstacle in curAnalyzedFrame.Obstacle) {
					float absDirection = CalcuteAbsoluteYaw (curAnalyzedFrame.Orientation.yaw, obstacle.direction);
					if (isLocatedInScope (startDegree, endDegree, absDirection)) {
						focusedObstacles.Add (obstacle.id, obstacle);
						absoluteDirection.Add (obstacle.id, absDirection);
					} else {
					}


				}
			}

//			DrawSVG ();

		}

//		Debug.Log (htmlHead+SVGHeader + DrawSVG (1, 2, 3) + SVGTail+htmlTail);
		Debug.Log (DrawSVG (500, 250, 150, direction));
		File.WriteAllText("./test.html", htmlHead+SVGHeader + DrawSVG (500, 250, 150, direction) + SVGTail+htmlTail);
	}

	public string DrawSVG (float cx, float cy, float radius, float direction)
	{

//		string circle = "<circle cx=\"500\" cy=\"250\" r=\"150\" stroke=\"lightgreen\" stroke-width=\"2\" fill=\"none\" stroke-dasharray=\"10 20\" />";
		string circle = "<circle cx=" + WrapString (cx.ToString ()) + " cy=" + WrapString (cy.ToString ()) +
		                " r=" + WrapString (radius.ToString ()) + " stroke=" + WrapString ("lightgreen") + " stroke-width=" + WrapString ("2") + " fill=" + WrapString ("none") +
		                " stroke-dasharray=" + WrapString ("10 20") + " />";
//		string lineLeft = "<line x1=\"500\" y1=\"250\" x2=\"425\" y2=\"120\" style=\"stroke:rgb(255,0,0);stroke-width:2\" stroke-dasharray=\"10 20\" />";
		float lineLeft_x2 = cx - Mathf.Cos(Mathf.PI/3)*radius;
		float lineLeft_y2 = cy - Mathf.Sin(Mathf.PI/3)*radius;
		string lineLeft = "<line x1="+WrapString(cx.ToString())+" y1="+WrapString(cy.ToString())+" x2="+WrapString(lineLeft_x2.ToString())+" y2="+
			WrapString(lineLeft_y2.ToString())+" style="+WrapString("stroke:rgb(255,0,0);stroke-width:2")+" stroke-dasharray="+
			WrapString("10 20")+" />";

		float lineRight_x2 = cx + Mathf.Cos(Mathf.PI/3)*radius;
		float lineRight_y2 = cy - Mathf.Sin(Mathf.PI/3)*radius;

		string lineRight = "<line x1="+WrapString(cx.ToString())+" y1="+WrapString(cy.ToString())+" x2="+WrapString(lineRight_x2.ToString())+" y2="+
			WrapString(lineRight_y2.ToString())+" style="+WrapString("stroke:rgb(255,0,0);stroke-width:2")+" stroke-dasharray="+
			WrapString("10 20")+" />";

//		string degree = "<text x=\"500\" y=\"80\" fill=\"red\">0°</text>";
		string degree = "<text x="+WrapString(cx.ToString())+" y="+WrapString((cy-radius-20).ToString())+" fill="+WrapString("red")+">"+
			direction+"°"+"</text>";

		string objs=null;
		if (direction > 30 && direction < 330) {

			foreach (DictionaryEntry de in absoluteDirection) { //ht为一个Hashtable实例
				Debug.Log (de.Key);//de.Key对应于key/value键值对key
				Debug.Log (de.Value);//de.Key对应于key/value键值对value
				if (((float)(de.Value) - direction) >= 0) {

					float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
					objs += DrawCircle (x, y);

				} else {
					float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					objs += DrawCircle (x, y);
				}
			}
		} else if (direction >= 330) {
			foreach (DictionaryEntry de in absoluteDirection) { //ht为一个Hashtable实例
				if (((float)(de.Value) - direction) >= 0) {
					float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
					objs += DrawCircle (x, y);
				} else {
					if (Mathf.Abs (((float)(de.Value) - direction)) > 30) {
						float x = cx + Mathf.Cos ((90 - 360 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((90 - 360 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
						objs += DrawCircle (x, y);
					} else {
						float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						objs += DrawCircle (x, y);
					}
				}
			}	
		} else {
			foreach (DictionaryEntry de in absoluteDirection) {
				if (((float)(de.Value) - direction) <= 0) {
					float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
					objs += DrawCircle (x, y);
				} else {
					if (Mathf.Abs (((float)(de.Value) - direction)) > 30) {
						float x = cx - Mathf.Cos ((((float)(de.Value) - direction) + 90 - 360) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((((float)(de.Value) - direction) + 90 - 360) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						objs += DrawCircle (x, y);
					} else {
						float x = cx + Mathf.Cos ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)(focusedObstacles [de.Key])).distance * radius / 2;
						float y = cy - Mathf.Sin ((90 + direction - (float)(de.Value)) * Mathf.PI / 180) * ((Obstacle)focusedObstacles [de.Key]).distance * radius / 2;
						objs += DrawCircle (x, y);	
					}
				}
			}	
		}
		
//		return circle+lineLeft+lineRight+degree+DrawCircle(500, 200)+DrawRect(520, 150, 7, 7)+DrawTriangle(10,20);
		return circle+lineLeft+lineRight+degree+objs;
	}

	public string DrawCircle(float cx, float cy){

//		string circle = "<circle cx=\"490\" cy=\"190\" r=\"5\" fill=\"blue\" />";
		string circle = "<circle cx="+WrapString(cx.ToString())+" cy="+WrapString(cy.ToString())+" r="+WrapString("5")+" fill="+WrapString("blue")+" />";
		return circle;
	}

	public string DrawRect(float x, float y, float w, float h){

//		string rect = "<rect x=\"520\" y=\"150\" width=\"7\" height=\"7\" style=\"fill:green\" />";
		string rect = "<rect x="+WrapString(x.ToString())+" y="+WrapString(y.ToString())+" width="+WrapString(w.ToString())+
			" height="+WrapString(h.ToString())+" style="+WrapString("fill:green")+" />";
		return rect;
	}

	public string DrawTriangle(float x, float y){
//		string triangle = "<polygon points=\"450,130 440,130 445,120 \" style=\"fill:red;stroke:black;stroke-width:1\" />";
		float x1 = x - 4;
		float y1 = y + 3;
		float x2 = x + 4;
		float y2 = y + 3;
		float x3 = x;
		float y3 = y - 3;
		string points = x1+","+y1+" "+x2+","+y2+" "+x3+","+y3;
		string triangle = "<polygon points="+WrapString(points)+" style="+WrapString("fill:red;stroke:black;stroke-width:1")+" />";
		return triangle;
	}

	private string WrapString (string str)
	{

		return "\"" + str + "\"";
	}

	public float CalcuteAbsoluteYaw (float frameYaw, float relativeDirection)
	{
		float direction = frameYaw + relativeDirection;
		if (direction >= 360) {
			direction = direction - 360;
		} else if (direction < 0) {
			direction = 360 + direction;
		}

		return direction;
	}

	public bool isLocatedInScope (float startDegree, float endDegree, float direction)
	{

		if (startDegree > endDegree) {
			if ((direction >= startDegree && direction < 360) || (direction < endDegree)) {
				return true;
			} else {
				return false;
			}

		} else {
			if (direction >= startDegree && direction <= endDegree) {
				return true;
			} else {
				return false;
			}
		}
	}
}


[System.Xml.Serialization.XmlRoot ("SceneAnalyzerOutput", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
public class SceneAnalyzerOutput
{
	[XmlArray ("AnalyzedFrames")]
	[XmlArrayItem ("AnalyzedFrame", typeof(AnalyzedFrame))]
	public AnalyzedFrame[] AnalyzedFrame { get; set; }


}


public class AnalyzedFrame
{
	//
	[XmlAttribute]
	public int id { get; set; }
	//
	[XmlAttribute]
	public string timestamp { get; set; }
	//
	[XmlAttribute]
	public string mode { get; set; }
	//
	[XmlAttribute]
	public float cameraHeight { get; set; }
	//
	[XmlAttribute]
	public bool muteState { get; set; }


	[XmlElement ("Orientation", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Orientation Orientation { get; set; }

	[XmlElement ("Saturation", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Saturation Saturation { get; set; }


	[XmlArray ("Obstacles")]
	[XmlArrayItem ("Obstacle", typeof(Obstacle))]
	public Obstacle[] Obstacle { get; set; }
}


public class Orientation
{
	[XmlAttribute]
	public float yaw { get; set; }

	[XmlAttribute]
	public float pitch { get; set; }

	[XmlAttribute]
	public float roll { get; set; }
}

public class Saturation
{
	[XmlAttribute]
	public int rate { get; set; }

	[XmlAttribute]
	public string area { get; set; }
}

public class Obstacle
{
	[XmlAttribute]
	public int id { get; set; }

	[XmlAttribute]
	public string obstacleType { get; set; }

	[XmlAttribute]
	public float distance { get; set; }

	[XmlAttribute]
	public float direction { get; set; }

	[XmlAttribute]
	public string category { get; set; }

	[XmlElement ("Size", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public Size size { get; set; }

	[XmlElement ("CentralPosition", Namespace = "http://range-it.eu/xmlschemata/sceneanalyzeroutput")]
	public CentralPosition centralPosition { get; set; }
}

public class Size
{

	[XmlAttribute]
	public float width { get; set; }

	[XmlAttribute]
	public float height { get; set; }

	[XmlAttribute]
	public float depth { get; set; }
}

public class CentralPosition
{
	
	[XmlAttribute]
	public float x { get; set; }

	[XmlAttribute]
	public float y { get; set; }

	[XmlAttribute]
	public float z { get; set; }
}

