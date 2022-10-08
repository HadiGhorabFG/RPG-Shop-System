using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Directory = System.IO.Directory;

public class ItemDatabase : EditorWindow
{
    private static List<Item> itemDatabase = new List<Item>();
    private Sprite defaultItemIcon;

    private VisualElement itemsTab;
    private static VisualTreeAsset itemRowTemplate;
    private ListView itemListView;
    private float itemHeight = 40;

    private ScrollView detailSection;
    private VisualElement largeDisplayIcon;
    private Item activeItem;

    [MenuItem("WUG/Item Database")]
    public static void Init()
    {
        ItemDatabase wnd = GetWindow<ItemDatabase>();
        wnd.titleContent = new GUIContent("Item Database");
        
        Vector2 size = new Vector2(800, 475);
        wnd.minSize = size;
        wnd.maxSize = size;
    }
    
    // CreateGUI is called when the EditorWindow’s rootVisualElement is ready to be drawn. 
    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UIBuilder/EditorUI/ItemDatabase.uxml");
        
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootVisualElement.Add(rootFromUXML);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UIBuilder/EditorUI/ItemDatabase.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UIBuilder/EditorUI/ItemRowTemplate.uxml");
        
        defaultItemIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/UnknownIcon.png", typeof(Sprite));
        itemsTab = rootVisualElement.Q<VisualElement>("ItemsTab");

        detailSection = rootVisualElement.Q<ScrollView>("ScrollView_Details");
        detailSection.style.visibility = Visibility.Hidden;
        largeDisplayIcon = detailSection.Q<VisualElement>("Icon");

        rootVisualElement.Q<Button>("Btn_AddItem").clicked += AddItem_OnClick;
        rootVisualElement.Q<Button>("Btn_DeleteItem").clicked += DeleteItem_OnClick;

        LoadAllItems();
        GenerateListView();
        
        detailSection.Q<TextField>("ItemName")
            .RegisterValueChangedCallback(evt =>
            {
                activeItem.name = evt.newValue;
                itemListView.Rebuild();
            });
        detailSection.Q<ObjectField>("IconPicker")
            .RegisterValueChangedCallback(evt =>
            {
                Sprite newSprite = evt.newValue as Sprite;
                activeItem.icon = newSprite == null ? defaultItemIcon : newSprite;
                
                largeDisplayIcon.style.backgroundImage = 
                    newSprite == null ? defaultItemIcon.texture : newSprite.texture;
                
                itemListView.Rebuild();
            });
    }

    private void LoadAllItems()
    {
        itemDatabase.Clear();
        
        string[] allPaths = Directory.GetFiles("Assets/Scripts/ScriptableObjects/Items", "*asset",
            SearchOption.AllDirectories);
        
        foreach (string path in allPaths)
        {
            itemDatabase.Add((Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item)));
        }
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("Icon").style.backgroundImage = 
                itemDatabase[i].icon == null ? defaultItemIcon.texture : itemDatabase[i].icon.texture;

            e.Q<Label>("Name").text = itemDatabase[i].name;
        };

        itemListView = new ListView(itemDatabase, 35, makeItem, bindItem);
        itemListView.selectionType = SelectionType.Single;
        itemListView.style.height = itemDatabase.Count * itemHeight;
        itemsTab.Add(itemListView);

        itemListView.onSelectionChange += ListView_onSelectionChange;
    }
    
    private void AddItem_OnClick()
    {
        //Create an instance of the scriptable object and set the default parameters
        Item newItem = CreateInstance<Item>();
        newItem.name = "New Item";
        newItem.icon = defaultItemIcon;
        
        //Create the asset
        AssetDatabase.CreateAsset(newItem, $"Assets/Scripts/ScriptableObjects/Items/{newItem.id}.asset");
        
        //Add it to the top of the item list
        itemDatabase.Insert(0, newItem);
        
        //Refresh the ListView so everything is redrawn again
        itemListView.style.height = itemDatabase.Count * itemHeight;
        
        ItemsContainer.UpdateItems();
        
        itemListView.Rebuild();
    }
    
    private void DeleteItem_OnClick()
    {
        //Get the path of the fie and delete it through AssetDatabase
        string path = AssetDatabase.GetAssetPath(activeItem);
        AssetDatabase.DeleteAsset(path);
    
        //Purge the reference from the list and refresh the ListView
        itemDatabase.Remove(activeItem);
        
        ItemsContainer.UpdateItems();
        
        itemListView.Rebuild();
        
        //Nothing is selected, so hide the details section
        detailSection.style.visibility = Visibility.Hidden;
    }

    private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
    {
        activeItem = (Item)selectedItems.First();

        SerializedObject so = new SerializedObject(activeItem);
        detailSection.Bind(so);

        if (activeItem.icon != null)
        {
            largeDisplayIcon.style.backgroundImage = activeItem.icon.texture;
        }

        detailSection.style.visibility = Visibility.Visible;
    }
}
