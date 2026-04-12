using UnityEngine;
using UnityEngine.Playables;
public class GameStarter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayableDirector director;
    public GameObject Map;

    public ApiTextureMapper textureManager;
    public GameObject FirstPOIPhysical;
    public POIManager POIManager;

    public GameObject CinematicCanvas;

    public bool HasCinematic = true;
    void Start()
    {
        if (HasCinematic) 
        {
            director.Play();    
        }
        else 
        {
            CreateFirstScene();
            CinematicCanvas.SetActive(false);
        }

    }

    private void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector){
            CreateFirstScene();
        }
    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTimeLineFinished() 
    {
       
    
    }

    private void CreateFirstScene() 
    {
        textureManager.SetPanorama("cIstdrDyOLnd4iUfgg61QQ");
        if (FirstPOIPhysical != null)
        {
            POIManager.SpawnPhysicalPOI(FirstPOIPhysical);

        }
        UIMAnager.Instance.mapNameText.text = "Ankara Kalesi";
    }
}
