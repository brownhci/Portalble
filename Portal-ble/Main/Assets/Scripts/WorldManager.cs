using UnityEngine;

public class WorldManager : MonoBehaviour {

    [SerializeField]
    private GameObject game = null, player_camera = null;

    public void ActiveGame() {
        game.transform.localPosition = player_camera.transform.localPosition - new Vector3(0, 0.1f, 0);
        // game.transform.forward = (new Vector3(player_camera.transform.forward.x, 0, player_camera.transform.forward.z)).normalized;
        game.SetActive(true);
    }

}
