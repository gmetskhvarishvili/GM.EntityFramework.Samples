using GM.EntityFramework.Domain.Abstractions;
using GM.EntityFramework.Domain.Base;

namespace GM.EntityFramework.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;

public class Sample : SoftDeletableEntity<int>, IAggregateRoot
{
    protected Sample()
    {
        SampleItems = new HashSet<SampleItem>();
    }

    private Sample(string name, string description) : this()
    {
        Name = name;
        Description = description;
    }
    
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public ICollection<SampleItem> SampleItems { get; private set; }
    
    public static Sample Create(
        string name,
        string description)
    {
        return new Sample(
            name,
            description);
    }
    
    public Sample AddSampleItems(params SampleItem[] items)
    {
        foreach (var item in items)
        {
            SampleItems.Add(item);
        }

        return this;
    }

    public void Update(
        string name,
        string description)
    {
        UpdateName(name);
        UpdateDescription(description);
    }

    private void UpdateName(string name)
    {
        Name = name;
    }
    
    private void UpdateDescription(string description)
    {
        Description = description;
    }
}