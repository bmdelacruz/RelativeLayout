using Map.Controls.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Map.Controls.Utilities
{
    internal class DependencyGraph
    {
        public Dictionary<string, Node> KeyNodes { get; } = new Dictionary<string, Node>();

        public Queue<Node> Roots { get; } = new Queue<Node>();

        public List<Node> Nodes { get; } = new List<Node>();


        public void Add(FrameworkElement element)
        {
            string name = element.Name;
            Node node = Node.Acquire(element);

            if (name != null)
                KeyNodes.Add(name, node);

            Nodes.Add(node);
        }

        public void Clear()
        {
            foreach (Node node in Nodes)
                node.Release();

            Nodes.Clear();
            KeyNodes.Clear();
            Roots.Clear();
        }

        public void GetSortedViews(FrameworkElement[] sortedChildren, params int[] propertyIds)
        {
            var roots = FindRoots(propertyIds);
            int index = 0;

            Node rootNode;
            while (roots.Count > 0)
            {
                rootNode = roots.Dequeue();

                var rootElement = rootNode.Element;
                var name = rootElement.Name;

                sortedChildren[index++] = rootElement;

                foreach (var dependent in rootNode.Dependents.Keys)
                {
                    var dependencies = dependent.Dependencies;
                    dependencies.Remove(name);

                    if (dependencies.Count == 0)
                        roots.Enqueue(dependent);
                }
            }

            if (index < sortedChildren.Length)
            {
                throw new InvalidOperationException("Circular dependencies cannot exist in RelativeLayout");
            }
        }


        public Queue<Node> FindRoots(int[] propertyIdsFilter)
        {
            foreach (Node node in Nodes)
            {
                node.Dependents.Clear();
                node.Dependencies.Clear();
            }

            foreach (Node node in Nodes)
            {
                var element = node.Element;

                foreach (int propertyId in propertyIdsFilter)
                {
                    string dependencyName = element.GetDependencyName(propertyId);

                    if (dependencyName == null || !KeyNodes.TryGetValue(dependencyName, out Node dependency))
                        continue;

                    if (!dependency.Dependents.ContainsKey(node))
                    {
                        dependency.Dependents.Add(node, this);
                    }
                    if (!node.Dependencies.ContainsKey(dependencyName))
                    {
                        node.Dependencies.Add(dependencyName, dependency);
                    }
                }
            }

            Roots.Clear();

            foreach (Node node in Nodes)
                if (node.Dependencies.Count == 0) Roots.Enqueue(node);

            return Roots;
        }

        public class Node
        {
            private static readonly ObjectPool<Node> pool = new ObjectPool<Node>();

            private readonly Dictionary<Node, DependencyGraph> dependents = new Dictionary<Node, DependencyGraph>();
            private readonly Dictionary<string, Node> dependencies = new Dictionary<string, Node>();

            public FrameworkElement Element { get; set; }

            public Dictionary<string, Node> Dependencies => dependencies;
            public Dictionary<Node, DependencyGraph> Dependents => dependents;


            public void Release()
            {
                Element = null;

                pool.Release(this);
            }

            public static Node Acquire(FrameworkElement element)
            {
                Node node = pool.Acquire();
                if (node == null)
                {
                    node = new Node();
                }

                node.Element = element;
                return node;
            }
        }
    }
}
