This is a collection of files you can use to easily create your own custom slot in tModLoader.

**This project is a work in progress** and reuses a lot of vanilla code. If you want to help, read Terraria's source code in [ILSpy](https://github.com/icsharpcode/ILSpy) or another decompiler and make a pull request.

## Contents
1. [How to use these files](#how-to-use-these-files)
    1. [Creating a custom slot](#creating-a-custom-slot)
    2. [Slot properties](#slot-properties)
2. [Reporting bugs and making suggestions](#reporting-bugs-and-making-suggestions)
3. [Known issues](#known-issues)

## How to use these files
To add the files to your project, you can either [download this repository](https://github.com/abluescarab/tModLoader-CustomSlot/archive/master.zip) or add it as a submodule in Git using a GUI or the command line. To clone the repository into a subfolder named `CustomSlot`:
```
git submodule add https://github.com/abluescarab/tModLoader-CustomSlot.git CustomSlot
```

To update the files when new commits are made:
```
git submodule update --remote --merge
git commit
```

### Creating a custom slot
A basic inventory slot can be created by creating a new `CustomItemSlot` with no parameters:
```csharp
CustomItemSlot mySlot = new CustomItemSlot();
```
An item slot can also be created with parameters and an initializer:
```csharp
CustomItemSlot mySlot = new CustomItemSlot(ItemSlot.Context.InventoryItem,
                                           1f, CustomItemSlot.ArmorType.Head) {
    ItemVisible = true,
    HoverText = "Item Name",
    IsValidItem = item => item.type > 0
};
```
Check the [slot properties](#slot-properties) section to see how you can customize the slot's appearance and functionality.

The first step to creating a custom slot is adding a new `UIState`.
```csharp
using Terraria.UI;

public class MySlotUI : UIState {
    public CustomItemSlot MyNormalSlot;
    public CustomItemSlot MyAccessorySlot;

    public bool Visible {
        get => Main.playerInventory; // how do you display your slot?
    }

    public override void OnInitialize() {
        // add a texture to display when the accessory slot is empty
        CroppedTexture2D emptyTexture = new CroppedTexture2D(
            ModContent.GetInstance<MyMod>().GetTexture("MyTexture"),
            CustomItemSlot.DefaultColors.EmptyTexture);

        MyNormalSlot = new CustomItemSlot(); // leave blank for a plain inventory space
        MyAccessorySlot = new CustomItemSlot(ItemSlot.Context.EquipAccessory, 0.85f) {
            IsValidItem = item => item.type > 0, // what do you want in the slot?
            EmptyTexture = emptyTexture,
            HoverText = "Accessories" // try to describe what will go into the slot
        };

        // you can set these once or change them in DrawSelf()
        MyNormalSlot.Left.Set(100, 0);
        MyNormalSlot.Top.Set(100, 0);

        MyAccessorySlot.Left.Set(150, 0);
        MyAccessorySlot.Top.Set(100, 0);

        // don't forget to add them to the UIState!
        Append(MyNormalSlot);
        Append(MyAccessorySlot);
    }
}
```
After that, you'll want to activate the UI in your main mod file.
```csharp
using Terraria.UI;

public class MyMod : Mod {
    private UserInterface _myUserInterface;
    public MySlotUI SlotUI;

    public override void Load() {
        // you can only display the ui to the local player -- prevent an error message!
        if(!Main.dedServ) {
            _myUserInterface = new UserInterface();
            SlotUI = new MySlotUI();

            SlotUI.Activate();
            _myUserInterface.SetState(SlotUI);
        }
    }

    // make sure the ui can draw
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        // this will draw on the same layer as the inventory
        int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        if(inventoryLayer != -1) {
            layers.Insert(
                inventoryLayer,
                new LegacyGameInterfaceLayer("My Mod: My Slot UI", () => {
                    if(SlotUI.Visible) {
                        _myUserInterface.Draw(Main.spriteBatch, new GameTime());
                    }

                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
```
Then just customize your item slot and add any additional functionality.

### Slot properties
* `ArmorType` - Used to determine the armor icon when the slot has an armor context and no empty texture.
* `BackgroundTexture` - The background texture of the slot, typically a square.
* `Context` - The context of the slot (`ItemSlot.Context`), used to determine default behavior.
* `DefaultColors` - Use these colors for a `CroppedTexture2D` unless you want to customize the color and opacity.
* `EmptyTexture` - A texture to draw inside the background when the slot is empty.
* `ForceToggleButton` - Whether to force a toggle visibility button even if the context isn't for an equipment slot.
* `HoverText` - The text to display when the mouse is hovered over the empty slot.
* `IsValidItem` - A function to determine which items can be placed in the slot.
* `Item` - The item in the slot.
* `ItemVisible` - Whether the item is visible on the player (usually determined by the toggle visibility button).
* `Partner` - A slot to swap items with when this one is right-clicked.
* `Scale` - The current scale of the slot (0.5 is half-size, 1.0 is full-size, 2.0 is double-size, etc.).

## Reporting bugs and making suggestions
Please use GitHub's [issues section](https://github.com/abluescarab/tModLoader-CustomSlot/issues) on this repository to report bugs, make suggestions, or request new features.

## Known issues
* Right-clicking empty accessory slots makes a sound