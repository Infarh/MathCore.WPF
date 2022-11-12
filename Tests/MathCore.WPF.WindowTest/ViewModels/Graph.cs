using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

class Graph : ViewModel, IEnumerable<Node>, IEnumerable<Link>
{
    private readonly ObservableCollection<Node> _Nodes = new ObservableCollection<Node>();
    private readonly ObservableCollection<Link> _Links = new ObservableCollection<Link>();

    public IEnumerable<Node> Nodes => _Nodes;
    public IEnumerable<Link> Links => _Links;

    public IEnumerable<Link> Add(Node node)
    {
        var links = _Nodes.Select(end => new Link(node, end)).ToArray();
        _Nodes.Add(node);
        _Links.AddItems(links);
        return links;
    }

    public IEnumerable<Link> Remove(Node node)
    {
        var links = _Links.Where(n => n.Start == node || n.End == node).ToArray();

        _Nodes.Remove(node);
        _Links.RemoveItems(links);

        return links;
    }

    IEnumerator<Link> IEnumerable<Link>.GetEnumerator() => _Links.GetEnumerator();

    public IEnumerator<Node> GetEnumerator() => _Nodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Nodes).GetEnumerator();
}