using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Tree structure for the allocation path.
    /// Links archetypes to parents and children.
    /// </summary>
    public class AllocationTree
    {
        /// <summary>
        /// The child nodes this node leads to
        /// </summary>
        public List<AllocationTree> Children { get; set; }

        /// <summary>
        /// The object that represents the archetype associated with this node
        /// </summary>
        public GameObject ArchetypeObject { get; set; }

        /// <summary>
        /// The object that represents the room associated with this node
        /// </summary>
        public GameObject RoomObject { get; set; }

        /// <summary>
        /// The parent node of this node - this is null if the node is the root
        /// </summary>
        public AllocationTree Parent { get; set; }

        /// <summary>
        /// Constructor that creates an allocation tree node containing objects referencing archetype and associated room
        /// </summary>
        /// <param name="archetypeObject">The archetype object to be assigned to the node</param>
        /// <param name="roomObject">The room object to be assigned to the node</param>
        /// <param name="parent">The parent of the node</param>
        public AllocationTree(GameObject archetypeObject, GameObject roomObject, AllocationTree parent)
        {
            ArchetypeObject = archetypeObject;
            RoomObject = roomObject;
            Parent = parent;
            Children = new List<AllocationTree>();
        }

        /// <summary>
        /// Adds a child to the end of the Children (linked list) on the parent node
        /// </summary>
        /// <param name="archetypeObject">The archetype object to be assigned to the child node</param>
        /// <param name="roomObject">The room object to be assigned to the child node</param>
        /// <param name="parent">The parent of the child node</param>
        /// <returns>The generated child node</returns>
        public AllocationTree AddChild(GameObject archetypeObject, GameObject roomObject, AllocationTree parent)
        {
            AllocationTree child = new AllocationTree(archetypeObject, roomObject, parent);
            Children.Add(child);
            return child;
        }

        /// <summary>
        /// Finds the node containing the specified archetype and adds a child node to it
        /// </summary>
        /// <param name="parentArchetype">The parent archetype to be found in the tree</param>
        /// <param name="childArchetype">The archetype object to be assigned to the child node </param>
        /// <param name="childRoom">The room object to be assigned to the child node</param>
        /// <returns>The generated child node, or null if none could be created</returns>
        public AllocationTree AddNodeToParentArchetype(RoomArchetype parentArchetype,
            GameObject childArchetype,
            GameObject childRoom)
        {
            AllocationTree found = FindNodeByArchetype(parentArchetype.gameObject);
            AllocationTree child = null;
            if (found != null)
            {
                child = found.AddChild(childArchetype, childRoom, FindNodeByArchetype(parentArchetype.gameObject));
            }

            return child;
        }

        /// <summary>
        /// Updates the room object of a particular node
        /// </summary>
        /// <param name="archetype">The archetype of the node to be found</param>
        /// <param name="roomObj">The room object the node will be updated with</param>
        public void UpdateRoomObject(RoomArchetype archetype, GameObject roomObj)
        {
            AllocationTree node = FindNodeByArchetype(archetype.gameObject);
            if (node != null)
                node.RoomObject = roomObj;
            else
                Debug.LogError("Could not find archetype " + archetype.gameObject.name + " in tree");
        }

        /// <summary>
        /// Removes a node and its children from the tree subject by its archetype data.
        /// Makes all subsequent children unreachable
        /// </summary>
        /// <param name="node">The node whose children are to be removed</param>
        public void RemoveChildrenFromNode(AllocationTree node)
        {
            if (node != null)
            {
                foreach (AllocationTree tree in node.Children)
                {
                    RemoveChildrenFromNode(tree);
                }
                node.Children.Clear();
            }
        }

        /// <summary>
        /// Iterates through all children of the node and removes those that have no archetype assigned
        /// </summary>
        /// <returns>Returns a list of all nodes that need to be cleaned</returns>
        public List<AllocationTree> CleanTree()
        {
            List<AllocationTree> toRemove = new List<AllocationTree>();
            foreach (AllocationTree child in Children)
            {
                toRemove.AddRange(child.CleanTree());
                if (child.ArchetypeObject == null)
                    toRemove.Add(child);
            }

            return toRemove;
        }

        /// <summary>
        /// Converts this node into the path's root node
        /// </summary>
        public void MakeNodeIntoRoot()
        {
            //If null then already at the root
            if (Parent == null)
                return;

            //Establish the path to the root
            Stack<AllocationTree> path = FindPathToRoot();
            //Reverse the path to make the node the root
            if (path.Count >= 2)
            {
                //Root node
                AllocationTree current = path.Pop();
                AllocationTree nextParent = path.Pop();

                //Obtain the inverse path to reverse the path between the current and root node
                while (path.Count > 0)
                {
                    nextParent.Children.Add(current);
                    current.Children.Remove(nextParent);
                    current.Parent = nextParent;
                    //Flip parents
                    current = nextParent;
                    nextParent = path.Pop();
                }

                Parent = null;
                nextParent.Children.Add(current);
                current.Children.Remove(nextParent);
            }
        }

        /// <summary>
        /// Finds the path from this node to the path root (inclusive)
        /// </summary>
        /// <returns>A stack containing the full path from the node to the root</returns>
        private Stack<AllocationTree> FindPathToRoot()
        {
            AllocationTree current = Parent;

            Stack<AllocationTree> pathToRoot = new Stack<AllocationTree>();
            //Push the current
            pathToRoot.Push(this);
            //Find path to the current root node
            //Parent will be null at the root
            while (current != null)
            {
                pathToRoot.Push(current);
                current = current.Parent;
            }

            return pathToRoot;
        }

        /// <summary>
        /// Works out the number of nodes between two nodes in the tree
        /// </summary>
        /// <param name="a">A particular node</param>
        /// <param name="b">Another particular node</param>
        /// <returns>The number of nodes between the two nodes, or -1 if no path is found</returns>
        public int GetNumberOfNodesBetween(AllocationTree a, AllocationTree b)
        {
            int count = 0;
            if (a.FindNodeByArchetype(b.ArchetypeObject) != null) // A is above b
            {
                AllocationTree current = b;
                while (current != a && current.Parent != null)
                {
                    current = current.Parent;
                    count++;
                }
            }
            else if (b.FindNodeByArchetype(a.ArchetypeObject) != null) // B is above a
            {
                AllocationTree current = a;
                while (current != b && current.Parent != null)
                {
                    current = current.Parent;
                    count++;
                }
            }
            else // A and b could be on separate paths but both still in the tree
            {
                AllocationTree current = b;
                while (current.Parent != null)
                {
                    current = current.Parent;
                    count++;
                }

                if (current.FindNodeByArchetype(a.ArchetypeObject) != null)
                {
                    AllocationTree newCurrent = a;
                    while (newCurrent != current && newCurrent.Parent != null)
                    {
                        newCurrent = newCurrent.Parent;
                        count++;
                    }
                }
                else
                {
                    Debug.LogError("Could not establish link between a and b");
                    return -1;
                }
            }

            return count;
        }

        /// <summary>
        /// Finds a node from the current node to any of its subtrees, searching itself and its children
        /// </summary>
        /// <param name="requiredArchetype"></param>
        /// <returns>The node that has the specified room in its data</returns>
        public AllocationTree FindNodeByArchetype(GameObject requiredArchetype)
        {
            //Check if the required archetype is this current node
            if (requiredArchetype == ArchetypeObject)
                return this;

            foreach (AllocationTree tree in Children)
            {
                AllocationTree search = tree.FindNodeByArchetype(requiredArchetype);
                if (search != null)
                    return search;
            }
            return null;
        }

        /// <summary>
        /// Prints to the console all parent nodes and their children
        /// </summary>
        public void GetPathAsString()
        {
            string path;
            if (Parent != null)
                path = "Node Parent: " + Parent.ArchetypeObject.name + " -> " + ArchetypeObject.name + " -> ";
            else
                path = "Tree Root: " + ArchetypeObject.name + " - > ";

            //Print children of current node
            if (Children.Count > 0)
            {
                foreach (AllocationTree tree in Children)
                {
                    path += "Child (" + tree.ArchetypeObject.name + ") ";
                    tree.GetPathAsString();
                }
            }
        }

        /// <summary>
        /// Gets the number of nodes present from the node and its subsequent children
        /// </summary>
        /// <returns>The number of nodes from this node downwards</returns>
        public int GetNumberOfNodes()
        {
            int count = 1;
            foreach (AllocationTree child in Children)
            {
                count += child.GetNumberOfNodes();
            }

            return count;
        }

        /// <summary>
        /// Returns the roomarchetype component of the archetype object
        /// </summary>
        /// <returns></returns>
        public RoomArchetype GetArchetype()
        {
            if (ArchetypeObject != null)
                return ArchetypeObject.GetComponent<RoomArchetype>();
            return null;
        }

        /// <summary>
        /// Returns the room component of the room object
        /// </summary>
        /// <returns></returns>
        public Room GetRoom()
        {
            if (RoomObject != null)
                return RoomObject.GetComponent<Room>();
            return null;
        }
    }
}