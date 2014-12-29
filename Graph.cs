/// <summary>
///	Graph class
/// 
/// 2010/07/23
/// Copyright (c) 2010 - Sebastien PASSIER, All rights reserved
/// </summary>

using System;
using System.Collections.Generic;
using System.Text;


namespace SPL
{
	public struct Connection
	{
		public int NeighborNodeID;
		public float Weight;
	}
	
	class Graph
	{
		#region Structs
		private class Node
		{
			public int ID;
			public List<Connection> NeighborList;
			
			public bool Tagged;
			public int Deep;
		}
		#endregion

		#region Variables
		private Dictionary<int, Node> m_NodeList= null;
		#endregion

		#region Constructors
		public Graph()
		{
			m_NodeList = new Dictionary<int, Node>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add a node into the graph
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public bool AddNode(int ID)
		{
			bool result = false;
			
			// Check node existence in the global list
			if (!m_NodeList.ContainsKey(ID))
			{
				result = true;
				
				Node node = new Node();
				node.ID = ID;
				node.NeighborList = new List<Connection>();
				node.Tagged = false;
				node.Deep = 0;

				m_NodeList.Add(ID, node);
			}

			return result;
		}

		/// <summary>
		/// Link 2 nodes and set a weigth. Warning, the 2 nodes must been previouly added in the graph before connect them
		/// </summary>
		/// <param name="nodeID"></param>
		/// <param name="neightborID"></param>
		/// <param name="weight"></param>
		/// <returns></returns>
		public bool ConnectNode(int nodeID, int neightborID, float weight)
		{
			bool result = false;

			// Check nodes existence in the global list
			if (m_NodeList.ContainsKey(nodeID) && m_NodeList.ContainsKey(neightborID))
			{
				result = true;

				Connection connection;
				connection.NeighborNodeID = neightborID;
				connection.Weight = weight;

				m_NodeList[nodeID].NeighborList.Add(connection);
			}

			return result;
		}

		/// <summary>
		/// BFS (Breadth First Search) is a graph search algorithm that begins at the root node and explores all the neighboring nodes.
		/// </summary>
		/// <param name="nodeID"></param>
		/// <returns></returns>
		public List<int> BreadthFirstSearch(int nodeID)
		{
			this.ResetAllTags();

			List<int> resultList = new List<int>();
			
			Queue<int> queue =  new Queue<int> ();

			this.TagNode(nodeID);
			queue.Enqueue(nodeID);

			while (queue.Count != 0)
			{
				int ID = queue.Peek();
				queue.Dequeue();
				resultList.Add(ID);

				foreach (Connection Item in m_NodeList[ID].NeighborList)
				{
					if (!m_NodeList[Item.NeighborNodeID].Tagged)
					{
						m_NodeList[Item.NeighborNodeID].Tagged = true;
						queue.Enqueue(Item.NeighborNodeID);
					}
				}
			}

			return resultList;
		}

		/// <summary>
		/// BFS (Breadth First Search) is a graph search algorithm that begins at the root node and explores all the neighboring nodes until a threshold is reach.
		/// </summary>
		/// <param name="nodeID"></param>
		/// <param name="threshold"></param>
		/// <returns></returns>
		public List<int> BreadthFirstSearch(int nodeID, uint threshold)
		{
			this.ResetAllNodes();

			List<int> resultList = new List<int>();

			Queue<int> queue = new Queue<int>();

			this.TagNode(nodeID);
			queue.Enqueue(nodeID);

			while (queue.Count != 0)
			{
				int ID = queue.Peek();
				queue.Dequeue();
				resultList.Add(ID);

				if (m_NodeList[ID].Deep < threshold)
				{
					foreach (Connection Item in m_NodeList[ID].NeighborList)
					{
						if (!m_NodeList[Item.NeighborNodeID].Tagged)
						{
							m_NodeList[Item.NeighborNodeID].Tagged = true;
							m_NodeList[Item.NeighborNodeID].Deep = m_NodeList[ID].Deep + 1; // Child deep is +1 from parent
							queue.Enqueue(Item.NeighborNodeID);
						}
					}
				}
				else
				{
					// Transfert the pending value of the queue into the result list
					for (int i = queue.Count; i != 0 ; i--)
					{
						ID = queue.Peek();
						queue.Dequeue();
						resultList.Add(ID);
					}
				}
			}

			return resultList;
		}

		/// <summary>
		/// 
		/// </summary>
		private void ResetAllNodes()
		{
			foreach (KeyValuePair<int, Node> Item in m_NodeList)
			{
				Item.Value.Tagged = false;
				Item.Value.Deep = 0;
			}
		}

		/// <summary>
		/// Untag all nodes of the graph 
		/// </summary>
		private void ResetAllTags()
		{
			foreach(KeyValuePair<int, Node> Item in m_NodeList)
				Item.Value.Tagged = false;
		}

		/// <summary>
		/// Tag a specific node of the graph
		/// </summary>
		/// <param name="nodeID"></param>
		private void TagNode(int nodeID)
		{
			m_NodeList[nodeID].Tagged = true;
		}
		#endregion
	}
}
