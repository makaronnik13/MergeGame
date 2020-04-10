using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineArrow : MonoBehaviour {

	public float curving;

	public Transform tip;

	private LineRenderer lr;



	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ManaPlayer.Instance.PlayingMana.Value != -1) 
		{
			lr.enabled = true;
			Vector3 endPosition = GetAimPosition ();

			List<Vector3> points = new List<Vector3> ();
			points.Add (ManaPlayer.Instance.GetManaTransform().position);
			points.Add (endPosition);
			lr.positionCount = 2;
			lr.SetPositions (points.ToArray());

			tip.transform.position = endPosition;

			Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - endPosition;
			diff.Normalize();
			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;


			tip.transform.localRotation = Quaternion.Euler(endPosition- ManaPlayer.Instance.GetManaTransform().position);
			tip.gameObject.SetActive (true);
		} else 
		{
			lr.enabled = false;
			tip.gameObject.SetActive (false);
		}
	}

	private bool ShowAimForCard(Card c)
	{
		bool v = false;
		if(c == null)
		{
			return v;
		}
		foreach(CardEffect ce in c.CardEffects)
		{
			if(ce.cardAim == CardEffect.CardAim.Cell)
			{
				if(ce.cellAimType != CardEffect.CellAimType.All && ce.cellAimType != CardEffect.CellAimType.Random)
				{
					v = true;	
				}
			}

			if(ce.cardAim == CardEffect.CardAim.Player)
			{
				if(ce.playerAimType != CardEffect.PlayerAimType.All && ce.playerAimType != CardEffect.PlayerAimType.Enemies && ce.playerAimType != CardEffect.PlayerAimType.You)
				{
					v = true;	
				}
			}
		}

		return v;
	}

	private Vector3 GetAimPosition()
	{
        
		if(ManaPlayer.Instance.AimingBlock.Value != null)
		{
			return ManaPlayer.Instance.AimingBlock.Value.transform.position + Vector3.up;
		}



        var v3 = Input.mousePosition;
        v3.z = 10.0f;
        return Camera.main.ScreenToWorldPoint(v3);

    }

	public Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}
}
