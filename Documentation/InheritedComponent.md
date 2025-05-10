### Components
# Inherited Component
## Overview

> Note:  This namespace also demonstrates the use of   [`NetworkInheritedComponent<T>`](../Assets/Scripts/components/NetworkInheritedComponent.cs), a specialized version for `NetworkBehaviour` components

`InheritedComponent<T>` is an abstract class that extends `SmartComponent`, enabling components to inherit properties and methods from another component of type `T`. It is designed to work with Unity's MonoBehaviour system, allowing for dynamic inheritance of component functionality.

- **Generic Type Parameter**: `T` - Must be a subclass of `MonoBehaviour`.
- **Properties**:
  - `inherited`: A protected property of type `T` that holds the inherited component. It is set lazily through the `GetInherited` method.
  - `inheritedExists`: A boolean property that checks if the `inherited` component exists. It uses the null-coalescing assignment operator (`??=`) to ensure `inherited` is initialized if it hasn't been set yet.
- **Methods**:
  - `GetInherited()`: A virtual method that attempts to find and return the component of type `T` in the parent hierarchy of the current GameObject. By the default `GetInherited` has this implementation:
```csharp
  protected virtual T GetInherited()
  {
      return gameObject.GetComponentInParent<T>();
  }
```

## Usage

To use these classes, you would typically create a new class that extends `InheritedComponent<T>` or `NetworkInheritedComponent<T>`, depending on your needs. You would then override the `shouldAddMissingComponents` property and implement any additional logic required for your specific component.

```csharp 
public class RigComponent : InheritedComponent<Rig> 
{     
  protected override Rig GetInherited(){
      // write own implementation, if it needs
      return Rig.Singleton;
  }
// Additional implementation
}
```
```csharp 
public class Movement : RigComponent
{     
  public void Move(){
    inherited.anchors.body.position += Vector3.right();
  }
// Additional implementation
}
```
This approach allows for flexible and reusable component design, especially useful in complex Unity projects involving networking and component inheritance.