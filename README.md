# ExtendedEnum
Better (but fake) 'enum' class

## Stop adding enum attributes!

```C#
private class DataAsCode : ExtendedEnumeration
{
    public static readonly DataAsCode One = new DataAsCode(1, "One");
    public static readonly DataAsCode Two = new DataAsCode(2, "Two");

    public readonly string Description;

    private DataAsCode(int value, string description) : base(value)
    {
        Description = description;
    }
}
```

## Get it!
install-package ExtendedEnum

## Use it like this!

```C#
var allEntries = ExtendedEnumeration.GetEntries<DataAsCode>();
foreach (var entry in allEntries) 
{
	var desc = entry.Description;
	//...
}
```

## It's a bit easier than...
```C#
var allEntries = Enum.GetValues(typeof(DataAsCodeEnum));
foreach (var entry in allEntries)
{
	var type = entry.GetType();
	var info = type.GetMember(entry.ToString());
	var attribute = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
	var desc = (attributes.Length > 0) ? (DescriptionAttribute)attributes[0] : null;
	// ...
}
```

## Not an enum
This isn't a real enum. You need to follow the pattern up above - public readonly members, constructor initializer, static readonly members as your values.

## Serializable
Need it to (de)serialize nicely? If you're using a real (de)serializer, you need to add some boilerplate (sorry). Your class needs to be marked serializable, and to add a constructor (yes, it just throws an exception):
```C#
[Serializable]
public class Something : ExtendedEnumeration
{
    public static readonly Something One = new Something(1, "One");
    public static readonly Something Two = new Something(2, "Two");

    public readonly string Description;

    private Something(int value, string description) : base(value)
    {
        Description = description;
    }
    protected Something(SerializationInfo info, StreamingContext context) : base(info, context) // This is new and will never be called.
    {
        throw new NotImplementedException();             
    }
}
```

## WebApi-friendly
Want to pass one in a URL parameter? There's a type converter ready to use! Just add the right attribute
```C#
[Serializable]
[TypeConverter(typeof(ExtendedEnumTypeConverter<Something>))]
public class Something : ExtendedEnumeration
{
    //...
}
```

## Newtonsoft-friendly
Use Newtonsoft.Json? It should just work - but you need to tell it which type to deserialize to.
```C#
var result = JsonConvert.DeserializeObject<Something>(passedInJson);
```

## Preserves strict reference equality
Well, within an app domain, anyway.
```C#
var serialised = JsonConvert.SerializeObject(Something.One);
var deserialised = JsonConvert.DeserializeObject<Something>(serialised);
Assert.ReferenceEquals(Something.One, deserialised);	// Passes assertion
```

# WHY?
Dunno. I sometimes find it useful and I got sick of having twenty different versions with different fixes peppered around different code bases. Don't like it? Don't use it! (But if you find a bug... Log it! Want a new feature? ... Ask!)