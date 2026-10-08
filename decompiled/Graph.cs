using System;
using System.Collections.Generic;

[Serializable]
public class Graph
{
	public List<Node> nodes = new List<Node>();

	public List<Link> links = new List<Link>();

	public Node AddNode(string id, string name)
	{
		Node node = new Node
		{
			id = id,
			name = name
		};
		nodes.Add(node);
		return node;
	}

	private bool LinkExists(string source, string target)
	{
		foreach (Link link in links)
		{
			if (link.source == source && link.target == target)
			{
				return true;
			}
		}
		return false;
	}

	public void AddLink(string source, string target)
	{
		if (!LinkExists(source, target))
		{
			Link item = new Link
			{
				source = source,
				target = target
			};
			links.Add(item);
		}
	}
}
