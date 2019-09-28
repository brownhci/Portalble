using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMINextPage : VirtualMenuItem {

	public override void onHandGrab ()
	{
		m_menu.nextPage();
	}

}
