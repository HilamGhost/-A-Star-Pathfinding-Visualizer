# \# 🗺️ Unity 2D A\* Pathfinding Visualizer

# 

# An interactive, highly optimized A\* (A-Star) pathfinding algorithm built in Unity. This project demonstrates clean architectural separation between pure mathematical data structures and Unity's visual components, allowing for lightning-fast route calculations on dynamic grids.

# 

# > \*\*\[Insert a high-quality GIF here showing the pathfinding running, dragging the start/end points, and obstacles]\*\*

# 

# \---

# 

# \## ✨ Key Features

# \* \*\*Interactive Grid:\*\* Click and drag the Start and Target nodes in real-time. The algorithm instantly recalculates the optimal path.

# \* \*\*Smart Snapping:\*\* If a user drops a node on an obstacle, a breadth-first search automatically snaps the node to the nearest safe, walkable cell.

# \* \*\*Procedural Obstacles:\*\* Generates random grid layouts based on a customizable obstacle density slider.

# \* \*\*Decoupled Architecture:\*\* The A\* algorithm runs entirely on pure C# classes, completely independent of Unity's heavy `MonoBehaviour` lifecycle, ensuring peak performance.

# 

# \---

# 

# \## 🏗️ Software Architecture

# 

# To ensure scalability and performance, the project follows a strict separation of concerns:

# 

# \* \*\*`PathFinder.cs` (The Brain):\*\* A static, pure C# class. It takes in a 2D array of raw data, calculates the shortest path using the F-cost ($f(n) = g(n) + h(n)$) and a 10/14 diagonal heuristic, and returns an array of nodes. It has zero knowledge of Unity GameObjects.

# \* \*\*`VirtualCell.cs` (The Data):\*\* A lightweight C# class representing a single grid coordinate. It stores its `X/Y` position, walkability state, and A\* costs (G, H, F).

# \* \*\*`GridManager.cs` (The Factory):\*\* Handles the Unity side of things. It instantiates the visual `SpriteRenderers`, builds the `VirtualCell` data grid behind the scenes, and acts as the API bridge between the user's mouse and the math.

# \* \*\*`DraggableEntity.cs` (The Input):\*\* Handles physics-less mouse dragging, translating screen space to world space, and querying the GridManager for valid drop locations.

# 

# \---

# 

# \## 🧮 The Mathematics (Heuristics)

# 

# This implementation avoids expensive `Mathf.Sqrt` floating-point calculations to maximize performance. Instead, it uses an integer-based distance estimation:

# \* Moving horizontally or vertically costs \*\*10\*\*.

# \* Moving diagonally costs \*\*14\*\* (an approximation of the square root of 2).

# 

# ```csharp

# int horizontalMovesRequired = highest - lowest;

# return (lowest \* 14) + (horizontalMovesRequired \* 10);



\## 🚀 Getting Started



\### Prerequisites

\* Unity 2021.3 LTS or higher (2D Core template).



\### Installation \& Usage

1\. Clone the repository and open it in Unity.

2\. Open the `Main` scene located in the `Scenes` folder.

3\. Hit \*\*Play\*\*.

4\. \*\*Left-Click and Drag\*\* the Green square to move the Starting position.

5\. \*\*Right-Click and Drag\*\* the Red square to move the Target position.

6\. Adjust the `Grid Size` and `Obstacle Opacity` sliders on the \*\*GridManager\*\* object in the hierarchy to test different layouts.



\---



\## 🛠️ Future Improvements

\* Add Dijkstra and Breadth-First Search toggles to compare algorithm efficiencies.

\* Implement an animated "Step-by-Step" visualizer to show the Open and Closed sets expanding in real-time.

\* Add terrain weight penalties.

