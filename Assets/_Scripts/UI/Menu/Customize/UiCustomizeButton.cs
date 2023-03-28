using System;
using Playstel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UiCustomizeButton : MonoBehaviour
{
    [Header("Setup")]
    public UiFactory.SlotName slotName;
    public ItemInfo.Catalog currentCatalog;
    public ItemInfo.Class currentClass;
    public ItemInfo.Subclass currentSubclass;
    public bool activeOnStart;
    public Image notificationIcon;
    public CacheAudio.MenuSound buttonSound = CacheAudio.MenuSound.OnCatalog;

    [Header("Factory")] 
    public UiCustomize UiCustomize;

    [Inject] private CacheAudio _cacheAudio;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Active);
    }

    private void Start()
    {
        ButtonInteraction(UiCustomize.MaleBeard(this));
        ButtonInteraction(UiCustomize.FoldierNotEmpty(this));
        
        if (activeOnStart) Active();
    }

    private void Active()
    {
        UiCustomize.CreateCustomizeSlots(slotName, currentCatalog, currentClass, currentSubclass);
        SetFocus();
        NewItemInCatalog(false);
        
        _cacheAudio.Play(buttonSound, false);
    }

    private void ButtonInteraction(bool state)
    {
        if(_button.interactable == false) return;
        
        _button.interactable = state;
    }

    private void SetFocus()
    {
        UiCustomize.customizeButtonFocus.SetParent(transform);
        UiCustomize.customizeButtonFocus.localPosition = new Vector3(0, 0, 0);
    }

    public void NewItemInCatalog(bool state)
    {
        notificationIcon.enabled = state;
    }
}
