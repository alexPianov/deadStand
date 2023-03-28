using Playstel;
using UnityEngine.UI;

public class UiRatingButton : UiCatalogButton
{
    public UiRatingList UiRatingList;
    public UserPayload.Statistics currentFilter;
    public bool useAtStart;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenFriendList);
        if(useAtStart) OpenFriendList();
    }

    public void OpenFriendList()
    {
        UiRatingList.UpdateRatingTable(currentFilter);
    }
}
