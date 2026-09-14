# 2D Asteroids - Production-Ready Unity Architecture

A modernized, high-performance 2D Asteroids game built in **Unity 6.3.8**. This project serves as a technical showcase for clean code principles, decoupling patterns, generic architectural wrappers, and zero-allocation memory management in game development. 

Rather than focusing on visual complexity, this repository demonstrates how to architect a scalable, maintainable, and robust codebase tailored to modern enterprise game development standards.

---

## 🛠️ Tech Stack & Key Architecture Features

*   **Unity Version:** Unity 6 (6.3.8) utilizing the 2D physics engine.
*   **Architectural Pattern:** **MVC (Model-View-Controller)** separation of concerns.
*   **Design Principles:** Comprehensive implementation of **SOLID** principles and Interface Segregation.
*   **Memory Optimization:** Custom **Generic Type-Safe Object Pooling** leveraging Unity's native engine pooling system capabilities.
*   **Decoupling Strategy:** Hybrid event architecture combining standard C# **`Action` events** for the same logical system and **ScriptableObject Event Channels** for global, scene-wide communication.

---

## 🏗️ Architecture & Component Breakdown

The codebase strictly avoids "God Scripts" by breaking down system logic into granular, single-responsibility components. Using the **Asteroid System** as a primary blueprint, the project is structured as follows:

### 🎮 The Controller Layer (System Managers)
*   **`AsteroidsManager`**: Inherits from a reusable generic `PoolManager<T>`. Acts as the central coordinator. It initializes the type-safe pool, syncs with scriptable asset states, triggers screen-visibility checks, and handles the conditional lifecycle of gameplay stages (e.g., broadcasting level progression events when the board is cleared).

### ⚙️ The Sub-Logic Workers (Single Responsibility Principle)
*   **`AsteroidsSpawn`**: Solely responsible for fetching items from the pool, positioning them dynamically based on generation parameters, scaling them based on sub-division tiers (splitting larger asteroids into smaller variants), and calculating life metrics proportionally.
*   **`AsteroidsMovement`**: Interfaces directly with the 2D physics engine (`Rigidbody2D`), applying continuous linear and angular velocities to the pooled objects without mixing deployment or allocation logic.

### 📦 The Entity Composition & MVC Layer (Prefab Architecture)
*   **`Asteroid` (The Local Controller)**: The core entry point for the entity prefab. Implements `IDamageable` and a generic `IPoolable<T>`. It coordinates internal components and relies on local C# actions (`OnDeath`) to immediately notify the master system manager when it gets destroyed, ensuring zero hard dependencies between individual game objects.
*   **`AsteroidData` (The Model)**: Operates strictly as the mutable state container storing real-time variables (speed, directional vectors, current health). Implements `IScoreable` and updates health via deterministic methods while notifying UI/Score tracking globally.
*   **`AsteroidInteractions` (The View/Input Driver)**: Implements `ICollidable` and `IScoreable`. It listens to the Unity Physics engine (`OnTriggerEnter2D`), safely extracts weapon data using `TryGetComponent`, broadcasts visual FX requests via global event channels, and routes damage back to the local controller.
*   **`AsteroidDataSO` (Configuration Model)**: A **ScriptableObject** inheriting from a custom `ScriptableSingleton<T>`. It acts as the single source of truth for immutable configuration setups (prefabs, base scales, speed multipliers) and handles dynamic game-wave progression scales.

---

## 🧠 Technical Highlights & Code Practices Showcase

### 1. Reusable Generic Optimization Framework (`GenericsPools<T>`)
To prevent runtime Garbage Collection spikes (`GC.Alloc`), the project utilizes a highly decoupled, generic pooling wrapper around Unity's native `ObjectPool<T>`. 
*   **Encapsulated Tracking**: It extends native capabilities by offering an encapsulated `IReadOnlyList<T>` to expose active items without allowing external collections manipulation.
*   **Injected Lifetime Cycles**: It leverages C# `Action<T>` delegates passed via the constructor to inject logic dynamically on allocation and release steps.
*   **Safety Cleanup**: Safely routes callback updates to dependencies even during unexpected engine destructions when maximum bounds thresholds are passed.

```csharp
public class GenericsPools<T> where T : Component
{
    private ObjectPool<T> _pool;
    private readonly List<T> _activeItems = new List<T>();
    public IReadOnlyList<T> ActiveItems => _activeItems; // Encapsulated read-only access

    public GenericsPools(T prefab, int poolInicial = 10, int poolMaximo = 50, Transform parent = null,
                         Action<T> onGet = null, Action<T> onRelease = null)
    {
        // Internal initialization mapping custom Action wrappers to native pooled logic lifecycle
    }
}
```

### 2. Robust Interface Segregation (SOLID)
Components are highly decoupled through small, focused interfaces (`IDamageable`, `IPoolable<T>`, `IScoreable`, `ICollidable`). This allows physics interactions to trigger visual events or record score data without tightly coupling the game entity to global gameplay state engines:
```csharp
public class AsteroidInteractions : MonoBehaviour, ICollidable, IScoreable
{
    public void OnCollide(Collider2D other)
    {
        if (other.TryGetComponent<ShootData>(out ShootData shootData))
        {
            _explosionEventChannel.RaiseEvent(other.transform.position);
            _asteroid.TakeDamage(shootData.Damage);
        }
    }
}
```

### 3. Loop Safety & Algorithmic Care
Demonstrates care for low-level collection manipulation. When wiping or shifting active items, loops run inversely to prevent index-shifting bugs common in array manipulation:
```csharp
// Safe item cleanup loop avoiding index-skipping side effects
for (int i = Pool.ActiveItems.Count - 1; i >= 0; i--)
{
    AsteroidsSpawn.DespawnAsteroid(Pool, Pool.ActiveItems[i]);
}
```

### 4. Decoupled Architecture via ScriptableObject Channels
Global gameplay events (like recording a score change, triggering explosions, or initializing a wave) are piped through ScriptableObject Architecture channels (`ScoreEventChannel`, `ExplosionEventChannel`). This ensures systems can communicate globally across scenes without relying on fragile global singletons or messy `GetComponent` hierarchies.

---

## 🚀 Getting Started

1. Clone this repository: `git clone https://github.com`
2. Open the project folder using **Unity Hub** (Version `6.3.8` or newer required).
3. Open the Menu scene located under `Assets/Scenes/`.
4. Press **Play**.
