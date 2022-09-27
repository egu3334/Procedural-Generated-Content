# Procedural-Generated-Content
Showcase of procedurally generated dungeons, trees, and birds using Unity.
Each Unity scene showcases one of the three aforementioned entities randomly generated multiple times with a editable random seed field in the generator object.

# Dungeon
The DungeonGenerator scene creates a dungeon using a cellular automata algorithm. It also uses flood filling to resolve the issue of generating separate disconnected caverns and added a river to the dungeon which replaces walls with water and open cells with lilypad bridges. Random numbers are used for the initial
randomized filling of the cell grid, chest spawn location, and the river's changes in width and trajectory. Settings of the dungeon can be changed in the inspector of the DungeonGenerator game object. Note - the script sets the minimum grid length to 5. Floor and wall textures were found from an outside source. Other textures were made by me.

# Trees
The Trees scene creates a set of trees. Each tree is drawn using smoothly joined cylinders. Trees are created starting with a single starting node in a queue and uses a BFS-esque algorithm to continue generating tree nodes. At each timestep, all of the nodes in the queue at that timestep are dequeued one by one with each popped node generating its subsequent node, drawing the tree segment between the two nodes, and enqueuing the subsequent node. Additionally, each dequeued node has a chance to die (generates a node with a width of 0 that cannot generate subsequent nodes) and branch (generate another node in a different direction than its first generated child node). Random numbers also affect the the direction change between different tree segments, how much shorter a tree segment becomes relative to its parent, and how much each tree segment tapers (becomes thinner).

# Birds
The Birds scene creates a set of birds. Each bird has a body, two legs, two wings and a beak. The birds have two swappable parts with 3 variations each with one being bird beaks (triangular, pelican, parrot) and the other being bird feet (particularly toes, normal 2 toes, clawed 4 toed, and flipper 3 toed). The size of each bird's body parts can have variance that is also determined randomly. The 3 types of bird feet and the triangular type beak are all drawn with a similar method to how the trees are drawn. For the two bird beak types of pelican and parrot, they are both drawn with bezier patches. The body and 2 wings are each created with a Unity 3D shape.
