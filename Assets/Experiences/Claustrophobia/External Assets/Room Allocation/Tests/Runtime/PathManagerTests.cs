using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RoomAllocation;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    //Tests the path management system, including the tree path structure and manipulating the tree
    //Also tests the components of archetypes, i.e. that they have the archetype component attached and so on
    public class PathManagerTests
    {
        private readonly GameObject[] ArchetypePrefabs = Resources.LoadAll<GameObject>("Prefabs/Room Archetypes");
        private List<GameObject> Archetypes { get; set; }

        private GameObject TestController { get; set; }
        private PathManager TestPathManager { get; set; }
        private int ArchetypeFillCount { get; set; }

        [SetUp]
        public void Setup()
        {
            Assert.NotZero(ArchetypePrefabs.Length);
            TestController = new GameObject();
            Archetypes = new List<GameObject>();
            foreach (GameObject obj in ArchetypePrefabs)
            {
                Archetypes.Add(GameObject.Instantiate(obj));
            }

            Assert.AreEqual(Archetypes.Count, ArchetypePrefabs.Length);
            TestPathManager = TestController.AddComponent<PathManager>();
            TestPathManager.InitialisePath(ChooseRandomArchetype(), null);
            Assert.NotNull(TestPathManager.Path.ArchetypeObject);
            ArchetypeFillCount = Random.Range(1, Archetypes.Count);
        }

        [UnityTest]
        public IEnumerator ArchetypeComponentValidation_Test()
        {
            GameObject[] archetypes = GameObject.FindGameObjectsWithTag("Archetype");
            foreach (GameObject obj in archetypes)
            {
                Assert.NotNull(obj);
                RoomArchetype archetypeComponent = obj.GetComponent<RoomArchetype>();
                //Ensure that all archetypes have the required archetype component
                Assert.NotNull(archetypeComponent);
                foreach (GameObject doorObj in archetypeComponent.Doors)
                {
                    Door doorComponent = doorObj.GetComponent<Door>();
                    Assert.NotNull(doorComponent);
                }
            }
            yield return null;
        }

        //Tests if the AddChild function of the allocation tree works as intended
        [UnityTest]
        public IEnumerator AddChildToTree_Test()
        {
            AllocationTree path = TestPathManager.Path;
            Assert.NotNull(path);
            Assert.GreaterOrEqual(Archetypes.Count, 2);
            //Test adding first child
            AllocationTree childOne = path.AddChild(ChooseRandomArchetype(), null, path);
            Assert.NotNull(childOne);
            Assert.IsTrue(path.Children.Count == 1);

            //Test adding second child
            AllocationTree childTwo = path.AddChild(ChooseRandomArchetype(), null, path);
            Assert.NotNull(childTwo);
            Assert.IsTrue(path.Children.Count == 2);

            yield return null;
        }

        //Tests if the AddNodeToParentArchetype function of the allocation tree works as intended
        [UnityTest]
        public IEnumerator AddNodeToParentArchetype_Test()
        {
            AllocationTree path = TestPathManager.Path;
            Assert.NotNull(path);
            Assert.GreaterOrEqual(Archetypes.Count, 4);

            //Test adding first child
            AllocationTree childOne = path.AddNodeToParentArchetype(path.GetArchetype(), ChooseRandomArchetype(), null);
            Assert.NotNull(childOne);
            Assert.IsTrue(path.Children.Count == 1);
            //Test adding second child
            AllocationTree childTwo = path.AddNodeToParentArchetype(path.GetArchetype(), ChooseRandomArchetype(), null);
            Assert.NotNull(childTwo);
            Assert.IsTrue(path.Children.Count == 2);

            yield return null;
        }

        //Tests if the FindNodeByArchetype function of the allocation tree works as intended
        [UnityTest]
        public IEnumerator FindNodeByArchetype_Test()
        {
            List<AllocationTree> inserted = FillPathWithAllArchetypes();
            foreach (AllocationTree t in inserted)
            {
                Debug.Log("Trying to find archetype: " + t.GetArchetype().gameObject.name);
                Assert.NotNull(TestPathManager.Path.FindNodeByArchetype(t.GetArchetype().gameObject));
            }

            yield return null;
        }

        //Tests if the RemoveNode function of the allocation tree works as intended
        [UnityTest]
        public IEnumerator RemoveNode_Test()
        {
            FillPathWithAllArchetypes();
            TestPathManager.Path.RemoveChildrenFromNode(TestPathManager.Path);
            Assert.AreEqual(1, TestPathManager.Path.GetNumberOfNodes());

            yield return null;
        }

        //Tests if the GetNumberOfNodes function of the allocation tree return the correct number of nodes
        [UnityTest]
        public IEnumerator GetNumberOfNodes_Test()
        {
            FillPathWithAllArchetypes();
            int number = TestPathManager.Path.GetNumberOfNodes();
            TestPathManager.Path.GetPathAsString();
            //Add one for the parent archetype
            Assert.AreEqual(ArchetypeFillCount + 1, number);
            yield return null;
        }

        //Tests if the GetNumberOfNodes function finds the number of nodes between archetypes on different paths
        [UnityTest]
        public IEnumerator GetNumberOfNodesOnDifferentPaths_Test()
        {
            AllocationTree childA = TestPathManager.Path.AddNodeToParentArchetype(TestPathManager.Path.GetArchetype(), ChooseRandomArchetype(), null);
            AllocationTree subChildA = TestPathManager.Path.AddNodeToParentArchetype(childA.GetArchetype(), ChooseRandomArchetype(), null);
            AllocationTree childB = TestPathManager.Path.AddNodeToParentArchetype(TestPathManager.Path.GetArchetype(), ChooseRandomArchetype(), null);
            AllocationTree subChildB = TestPathManager.Path.AddNodeToParentArchetype(childB.GetArchetype(), ChooseRandomArchetype(), null);

            int number = TestPathManager.Path.GetNumberOfNodesBetween(subChildA, subChildB);
            int numberAgain = TestPathManager.Path.GetNumberOfNodesBetween(subChildB, subChildA);

            Assert.AreEqual(number, numberAgain);
            Assert.AreEqual(4, number);
            yield return null;
        }

        private List<AllocationTree> FillPathWithAllArchetypes()
        {
            List<AllocationTree> insertedNodes = new List<AllocationTree>();
            AllocationTree path = TestPathManager.Path;
            Assert.NotNull(path);
            AllocationTree lastInserted = path;
            int insertCount = 0;
            Debug.Log(ArchetypeFillCount);
            while (insertCount < ArchetypeFillCount)
            {
                float random = Random.value;
                //Add node to last inserted
                GameObject randomArchetype = ChooseRandomArchetype();
                if (random < 0.5)
                    lastInserted = path.AddNodeToParentArchetype(lastInserted.GetArchetype(), randomArchetype, null);
                else
                    lastInserted = path.AddNodeToParentArchetype(path.GetArchetype(), randomArchetype, null);
                Assert.NotNull(lastInserted);
                insertedNodes.Add(lastInserted);
                insertCount++;
            }
            return insertedNodes;
        }

        private GameObject ChooseRandomArchetype()
        {
            GameObject random = UtilityHelper.ChooseRandomObject(Archetypes, false);
            Archetypes.Remove(random);
            return random;
        }
    }
}