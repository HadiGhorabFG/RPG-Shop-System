# RPG-Shop-System

Hadi Ghorab FG21FT
RPG Shop System framework for generic rpg games with buying and selling functionality and storing items in an inventory, and ffxiv crafting sytem replica.

Patterns:
- Singletons, in `ShopUI.cs` and `ItemsManager.cs` as Instance.
  Used singleton to make the single instance of the class that could be accessed from anywhere using a static.
  
- Component, in `PlayerController.cs` called agentMovement from the script `AgentMovement.cs`.
  Component used to handle agent movement using NavMeshAgent, the component takes a ray position and an agent object and translates it into movement.
  
- Dependency Injections, in several methods in `Shop.cs` and other classes. like for example the method InitializeShop.
  The methods are dependant on the injected class, in the example I picked its a class of type IShop.
  
- Observatories, in `Shop.cs` and `UIHandler.cs`.
  Events for UI notications are invoked from the Shop class and is subscribed to in the UIHandler class. Events are invoked in OnTriggerEnter and OnTriggerExit methods in the Shop     class when player is in range of the shop interaction or when the player leaves the range.
 
- Dirty Flags, in `UIHandler.cs` and `ShopUI.cs`.
  Dirty flag used in UIHandler to run method only when money is changed (can be seen in the Update function), used for the same purpose in ShopUI to update price only when more or   less items have been selected, if its the same, there is no need to update.
