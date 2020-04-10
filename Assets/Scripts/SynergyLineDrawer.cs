using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyLineDrawer : Singleton<SynergyLineDrawer>
{
    [SerializeField]
    private GameObject LinePrefab;

    private List<GameObject> lines = new List<GameObject>();

    public void Show(List<LineInfo> linesPairs)
    {
        Hide();
        Dictionary<Vector3, int> linesCount = new Dictionary<Vector3, int>();

        foreach (LineInfo l in linesPairs)
        {
            if (linesCount.ContainsKey(l.From))
            {
                linesCount[l.From]++;
            }
            else
            {
                linesCount.Add(l.From, 0);
            }

            GameObject newLine = Instantiate(LinePrefab);
            newLine.transform.SetParent(transform);
            Gradient g = new Gradient();
            g.mode = GradientMode.Blend;

            List<GradientColorKey> colors = new List<GradientColorKey>();
            List<GradientAlphaKey> alphas = new List<GradientAlphaKey>();

            alphas.Add(new GradientAlphaKey(0,0));
            alphas.Add(new GradientAlphaKey(1,1));

            if (l.lineIfo.combination)
            {
                colors.Add(new GradientColorKey(new Color(1, 1, 0, 1), 0));
            }
            else
            {

                colors.Add(new GradientColorKey(new Color(0, 1, 0, 1), 0));

            }

            g.SetKeys(colors.ToArray(), alphas.ToArray());

            
          
            newLine.GetComponent<LineRenderer>().colorGradient = g;

            Vector3[] pos = new Vector3[0];
            if (!l.lineIfo.emmiting)
            {
                pos = StaticTools.CurveLine(l.From + linesCount[l.From] * Vector3.up * 0.1f, l.To + linesCount[l.From] * Vector3.up * 0.1f);
            }
            else
            {
                pos = StaticTools.CurveLine(l.To + linesCount[l.From] * Vector3.up * 0.1f, l.From + linesCount[l.From] * Vector3.up * 0.1f);
            }
           
            newLine.GetComponent<LineRenderer>().positionCount = pos.Length;
            newLine.GetComponent<LineRenderer>().SetPositions(pos);

            List<Keyframe> frames = new List<Keyframe>();
            frames.Add(new Keyframe(0,0.05f));
            frames.Add(new Keyframe(0.8f, 0.05f));
            frames.Add(new Keyframe(0.85f, 0.2f));
            frames.Add(new Keyframe(1, 0.05f));
            AnimationCurve wCurve = new AnimationCurve(frames.ToArray());

            newLine.GetComponent<LineRenderer>().widthCurve = wCurve;

            lines.Add(newLine);
        }
    }

    public void Hide()
    {
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
    }
}
