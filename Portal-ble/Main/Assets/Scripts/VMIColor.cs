using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMIColor : VirtualMenuItem {
	public Color m_color;

	protected override void Start ()
	{
		base.Start ();
	}

	protected override void Update() {
		base.Update ();

		GetComponent<Renderer> ().material.color = m_color;

		this.GetComponent<Renderer> ().material.SetFloat ("_UseBodyColor", 1);
		this.GetComponent<Renderer> ().material.SetColor ("_BodyColor", m_color);
	}

	public override void onHandGrab ()
	{
		hand_l.GetComponent<HandManager>().contextSwitch("paint");
        hand_r.GetComponent<HandManager>().contextSwitch("paint");

        GameObject gobj = GameObject.Find("Hand_l");
        if (gobj) {
            gobj.GetComponent<PaintManager>().CurrentColor = m_color;
        }
        gobj = GameObject.Find("Hand_r");
        if (gobj) {
            gobj.GetComponent<PaintManager>().CurrentColor = m_color;
        }

		m_menu.close ();

		Debug.Log("3D Color selected from VMIColor Script");
	}

}
