DynamicXml
==========

What's it's all about
---------------------
Control xml serialization with clever attributes.

Note: This is a work in progress, but, as every other serious person on internet,
I intend to develop it. Now the bare minimum seem to be working.

Let's see what it's all about.

History
-------
Many times at my old job I had to deal for various reasons with serialisation, for
a number of reasons, tests being one of them. For example I had a benchmark MVC
model which I wanted to compare with a generated one to make sure important
properties had not changed in the meanwhile, due to me having a serious lack of
attention. So my solution ended up serializing both models and comparing the 
XML output to see if any fields had changed.

Unfortunately some double fields would have their last digit swapped between
each session, due to whatever reason a hardware dude could probably explain. 
For example instead of having 

    <myDouble>2.2000000000001</myDouble>

Maybe it would have turned out as 

    <myDouble>2.2000000000004</myDouble>

As far as I know, that's a processor problem, but yet the tests would here fail because, well
`1` and `4` are not the same byte, when string-compared.

The first solution was to change the type of these properties to something like
`XmlDouble`, which would be a type implicit to `double`s, except it would have had
implemented `IXmlSerializable` and its `WriteXml` would print the double `ToString`
with some argument limiting the number of digits outputted.

At first sight, it might looks good enough, but it's not. I had some extension methods
working on `double` and extension methods don't care if `XmlDouble` is completely
isomorphic to `double` it will still compile-fail.

What to do then ?

The power of `System.Reflection.Emit`
-------------------------------------
I wanted a simple API, something working with attributes, for example with the
following classes

```C#
public class Person
{
    [XmlElement("ageGoesHere")]
    public double Age { get; set; }

    [XmlElement("heightGoesThere")]
    [XmlToString("N2")]
    public double Height { get; set; }

    [XmlAttribute("pleaseGiveMeAName")]
    public string Name { get; set; }

    [XmlToString("yyyy-MM")]
    public DateTime Date { get; set; }

    public SubObject MySubObject { get; set; }

    public List<SubObject> MySubObjects { get; set; }

    public Person()
    {
        Age = 23 + Math.PI;
        Height = 5 + 10.0 / 12.0;
        Name = "Thierry";
        Date = DateTime.Now;
        MySubObject = new SubObject();
        MySubObjects = new List<SubObject>();

        foreach (var _ in Enumerable.Range(0, 3))
        {
            MySubObjects.Add(new SubObject());
        }
    }
}

public class SubObject
{
    [XmlToString("N4")]
    public double A { get; set; }

    public int B { get; set; }

    [XmlToString("yyyy-MM")]
    public DateTime C { get; set; }

    public SubObject()
    {
        A = 56.156465451;
        B = 8;
        C = new DateTime(2001, 09, 11);
    }
}
```

will be deserialize to the following :

```xml
<?xml version="1.0" encoding="ibm850"?>
<Person xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" pleaseGiveMeAName="Thierry">
  <ageGoesHere>26.141592653589793</ageGoesHere>
  <heightGoesThere>5.83</heightGoesThere>
  <Date>2014-08</Date>
  <MySubObject>
    <A>56.1565</A>S
    <B>8</B>
    <C>2001-09</C>
  </MySubObject>
  <MySubObjects>
    <SubObject>
      <A>56.1565</A>
      <B>8</B>
      <C>2001-09</C>
    </SubObject>
    <SubObject>
      <A>56.1565</A>
      <B>8</B>
      <C>2001-09</C>
    </SubObject>
    <SubObject>
      <A>56.1565</A>
      <B>8</B>
      <C>2001-09</C>
    </SubObject>
  </MySubObjects>
</Person>
```

I wanted the `XmlToString` attribute to control how the XML would look like.

And, ladies and gents, that's exactly what the `DynamicXml` class does :

```C#
var dSerializer = new DynamicXmlSerializer<Person>();
var person = new Person();

dSerializer.Serialize(Console.Out, person);
```

Of course, it's a one-way serializer, ie. you *can't deserialize* (yet !)