
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;

using NetFocus.Components.AddIns.Conditions;
using NetFocus.Components.AddIns.Codons;

namespace NetFocus.Components.AddIns
{
    /// <summary>
    /// This class represents a node in the <see cref="IAddInTree"/>
    /// </summary>
    public class DefaultAddInTreeNode : IAddInTreeNode
    {
        Hashtable childNodes = new Hashtable();
        ICodon codon = null;
        ConditionCollection conditionCollection = null;
        string path = string.Empty;
        IAddInTreeNode parent = null;

        public Hashtable ChildNodes
        {
            get
            {
                return childNodes;
            }
        }

        public ICodon Codon
        {
            get
            {
                return codon;
            }
            set
            {
                codon = value;
            }
        }

        public IAddInTreeNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public string GetFullPath()
        {
            string path = string.Empty;
            IAddInTreeNode currentNode = this;
            while (currentNode != null)
            {
                path = currentNode.Path + "/" + path;
                currentNode = currentNode.Parent;
            }
            if (!string.IsNullOrEmpty(path) && path.StartsWith("//"))
            {
                path = path.Substring(1);
            }
            return path;
        }

        public ConditionCollection ConditionCollection
        {
            get
            {
                return conditionCollection;
            }
            set
            {
                conditionCollection = value;
            }
        }


        /// <summary>
        /// Get's the direct child nodes of the TreeNode node as an array. The 
        /// array is sorted acordingly to the insertafter and insertbefore preferences
        /// the node has using topoligical sort.
        /// </summary>
        /// <param name="node">
        /// The TreeNode which childs are given back.
        /// </param>
        /// <returns>
        /// A valid topological sorting of the childs of the TreeNode as an array.
        /// </returns>
        private IAddInTreeNode[] GetSubnodesAsSortedArray()
        {
            IAddInTreeNode node = this;
            int index = node.ChildNodes.Count;
            IAddInTreeNode[] sortedNodes = new IAddInTreeNode[index];
            Hashtable visited = new Hashtable(index);
            Hashtable anchestor = new Hashtable(index);

            foreach (string key in node.ChildNodes.Keys)
            {
                visited[key] = false;
                anchestor[key] = new ArrayList();
            }

            foreach (DictionaryEntry child in node.ChildNodes)
            {
                if (((IAddInTreeNode)child.Value).Codon.InsertAfter != null)
                {
                    for (int i = 0; i < ((IAddInTreeNode)child.Value).Codon.InsertAfter.Length; ++i)
                    {
                        //						Console.WriteLine(((IAddInTreeNode)child.Value).Codon.ID + " " + ((IAddInTreeNode)child.Value).Codon.InsertAfter[i].ToString());
                        if (anchestor.Contains(((IAddInTreeNode)child.Value).Codon.InsertAfter[i].ToString()))
                        {
                            ((ArrayList)anchestor[((IAddInTreeNode)child.Value).Codon.InsertAfter[i].ToString()]).Add(child.Key);
                        }
                    }
                }

                if (((IAddInTreeNode)child.Value).Codon.InsertBefore != null)
                {
                    for (int i = 0; i < ((IAddInTreeNode)child.Value).Codon.InsertBefore.Length; ++i)
                    {
                        if (anchestor.Contains(child.Key))
                        {
                            ((ArrayList)anchestor[child.Key]).Add(((IAddInTreeNode)child.Value).Codon.InsertBefore[i]);
                        }
                    }
                }
            }

            string[] keyarray = new string[visited.Keys.Count];
            visited.Keys.CopyTo(keyarray, 0);

            for (int i = 0; i < keyarray.Length; ++i)
            {
                if ((bool)visited[keyarray[i]] == false)
                {
                    index = Visit(keyarray[i], node.ChildNodes, sortedNodes, visited, anchestor, index);
                }
            }
            return sortedNodes;
        }

        private int Visit(string key, Hashtable nodes, IAddInTreeNode[] sortedNodes, Hashtable visited, Hashtable anchestor, int index)
        {
            visited[key] = true;
            foreach (string anch in (ArrayList)anchestor[key])
            {
                if ((bool)visited[anch] == false)
                {
                    index = Visit(anch, nodes, sortedNodes, visited, anchestor, index);
                }
            }

            sortedNodes[--index] = (IAddInTreeNode)nodes[key];
            return index;
        }


        public object BuildChildItem(string childItemID, object caller)
        {   //这是一个递归函数
            IAddInTreeNode[] sortedNodes = GetSubnodesAsSortedArray();

            foreach (IAddInTreeNode curNode in sortedNodes)
            {
                if (curNode.Codon.ID == childItemID)
                {
                    ArrayList subItems = curNode.BuildChildItems(caller);
                    return curNode.Codon.BuildItem(caller, subItems, null);
                }
                object o = curNode.BuildChildItem(childItemID, caller);
                if (o != null)
                {
                    return o;
                }
            }

            return null;
        }


        public ArrayList BuildChildItems(object caller)
        {   //这是一个递归函数
            ArrayList items = new ArrayList();

            IAddInTreeNode[] sortedNodes = GetSubnodesAsSortedArray();

            foreach (IAddInTreeNode curNode in sortedNodes)
            {
                ArrayList subItems = curNode.BuildChildItems(caller);//先对子结点进行递归调用
                object newItem = null;//有两种情况需要创建该项:1.不需要响应条件的项;2.满足条件的项;
                if (curNode.Codon.HandleConditions || curNode.ConditionCollection.GetCurrentConditionFailedAction(caller) == ConditionFailedAction.Nothing)
                {
                    newItem = curNode.Codon.BuildItem(caller, subItems, curNode.ConditionCollection);
                }

                if (newItem != null)
                {
                    items.Add(newItem);
                }
            }
            return items;
        }
    }
}
