using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMIOpen : VirtualMenuItem {

	public override void onHandGrab ()
	{
		m_menu.open();
	}

}
