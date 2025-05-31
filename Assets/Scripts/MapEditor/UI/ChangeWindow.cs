using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChangeWindow : MonoBehaviour
{
    public ChangeWindow(){}
    private static ChangeWindow instance;
    public static ChangeWindow Instance => instance;
    
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    
    public MapEditor mapEditor;
    public GameObject TilePanel, Decopanel, WallPanel, FWallPanel, ObjectPanel, FilterPanel;

    public Image tileButton,
        decoButton,
        wallButton,
        fwallButton,
        objectButton,
        filterButton,
        penButton,
        eraserButton,
        selectButton;

    public Sprite[] tile, deco, wall, fwall, obj, filter, pen, eraser, select;
    // Start is called before the first frame update
    void Start()
    {
        OnClickObjectButton();
        OnPenModeClick();
    }

    public void OnClickDecoButton()
    {
        SetAllPanelFalse();
        mapEditor.paletteMode = MapEditor.PaletteMode.Deco;
        Decopanel.SetActive(true);
        decoButton.sprite = deco[1];
    }

    public void OnClickWallButton()
    {
        SetAllPanelFalse();
        mapEditor.paletteMode = MapEditor.PaletteMode.Wall;
        WallPanel.SetActive(true);
        wallButton.sprite = wall[1];
    }

    public void OnClickFWallButton()
    {
        SetAllPanelFalse();
        mapEditor.paletteMode = MapEditor.PaletteMode.FilterWall;
        FWallPanel.SetActive(true);
        fwallButton.sprite = fwall[1];
    }
    public void OnClickObjectButton()
    {
        SetAllPanelFalse();
        mapEditor.paletteMode = MapEditor.PaletteMode.Object;
        ObjectPanel.SetActive(true);
        objectButton.sprite = obj[1];
    }

    public void OnClickFilterButton()
    {
        SetAllPanelFalse();
        mapEditor.paletteMode = MapEditor.PaletteMode.Filter;
        FilterPanel.SetActive(true);
        filterButton.sprite = filter[1];
    }

    public void SetAllPanelFalse()
    {
        TilePanel.SetActive(false);
        Decopanel.SetActive(false);
        WallPanel.SetActive(false);
        FWallPanel.SetActive(false);
        ObjectPanel.SetActive(false);
        FilterPanel.SetActive(false);
        
        tileButton.sprite = tile[0];
        decoButton.sprite = deco[0];
        wallButton.sprite = wall[0];
        fwallButton.sprite = fwall[0];
        objectButton.sprite = obj[0];
        filterButton.sprite = filter[0];
    }
    
    public void OnPenModeClick()
    {
        mapEditor.editMode = MapEditor.EditMode.Pen;
        OffModeButtons();
        penButton.sprite = pen[1];
    }

    public void OnEraserModeClick()
    {
        mapEditor.editMode = MapEditor.EditMode.Eraser;
        OffModeButtons();
        eraserButton.sprite = eraser[1];
    }

    public void OnSelectModeClick()
    {
        mapEditor.editMode = MapEditor.EditMode.Select;
        OffModeButtons();
        selectButton.sprite = select[1];
    }

    public void OffModeButtons()
    {
        penButton.sprite = pen[0];
        eraserButton.sprite = eraser[0];
        selectButton.sprite = select[0];
    }
    
}
