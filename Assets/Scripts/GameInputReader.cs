using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameInputReader : MonoBehaviour
{
    public enum SourceType
    {
        LOCAL_GRAPHML,
        CLOUD
    }

    // Maximum number of vertices and edges
    private int maxCount = 1000;

    private static GraphController graphControl;
    private static GameUI gameUI;

    private class nodeListObj
    {
        public string id { get; set; }
    }
    private List<nodeListObj> nodesList = new List<nodeListObj>();

    private class linkListObj
    {
        public string source { get; set; }
        public string target { get; set; }
    }
    private List<linkListObj> linksList = new List<linkListObj>();

    private IEnumerable<bool> ParseGraphML(string xml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        XmlElement root = xmlDoc.FirstChild as XmlElement;
        if (root == null)
        {
            root = xmlDoc.ChildNodes[1] as XmlElement;
            if (root == null)
            {
                throw new System.InvalidOperationException("Graph ML cannot be read");
            }
        }

        for (int i = 0; i < root.ChildNodes.Count; i++)
        {
            XmlElement xmlGraph = root.ChildNodes[i] as XmlElement;

            int nodeCount = 0;
            int linkCount = 0;

            int childCount = xmlGraph.ChildNodes.Count;

            for (int j = 0; j < childCount; j++)
            {

                XmlElement xmlNode = xmlGraph.ChildNodes[j] as XmlElement;

                //create nodes
                if ((xmlNode.Name == "node") && (xmlNode.Attributes != null) && xmlNode.HasAttribute("id") && (nodeCount < maxCount))
                {
                    nodeCount++;
                    nodesList.Add(new nodeListObj
                    {
                        id = xmlNode.Attributes["id"].Value
                    });
                }
                //create links
                else if ((xmlNode.Name == "edge") && (xmlNode.Attributes != null) && xmlNode.HasAttribute("source") &&
                    xmlNode.HasAttribute("target") && (linkCount < maxCount))
                {
                    linkCount++;
                    linksList.Add(new linkListObj
                    {
                        source = xmlNode.Attributes["source"].Value,
                        target = xmlNode.Attributes["target"].Value
                    });
                }

                //every 100 cycles return control to unity
                if (j % 100 == 0)
                    yield return true;
            }
        }
    }

    private void LoadGraph()
    {
        foreach (nodeListObj genNode in nodesList)
        {
            // Create a node on random Coordinates, but with labels
            graphControl.GenerateNode(genNode.id, "");
        }

        foreach (linkListObj genLink in linksList)
        {
            graphControl.GenerateLink("specific_src_tgt", GameObject.Find(genLink.source), GameObject.Find(genLink.target));
        }

    }

    private IEnumerator LoadGraphMLFile(string sourceFile)
    {
        gameUI.SandClockSetActive(true);

        //StreamReader sr = new StreamReader(sourceFile);
        //string xml = sr.ReadToEnd();
        //sr.Close();

        TextAsset xml = Resources.Load<TextAsset>("Models/" + sourceFile);
        foreach (bool val in ParseGraphML(xml.ToString()))
        {
            yield return val;
        }

        gameUI.SandClockSetActive(false);

        LoadGraph();
    }

    public void LoadWolframModel(string source, SourceType type)
    {
        nodesList.Clear();
        linksList.Clear();

        if (type == SourceType.LOCAL_GRAPHML)
        {
            StartCoroutine(LoadGraphMLFile(source));
        }
    }

    void Awake()
    {
        graphControl = GetComponent<GraphController>();
        gameUI = GetComponent<GameUI>();
        //gameCtrlHelper gameCtrlHelper = GetComponent<gameCtrlHelper>();
    }

}
