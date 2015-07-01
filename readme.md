# Fast Runtime Type Helpers

## Description

The library contains set of helper methods that provide fast runtime access to object's properties and fields.
This works up to 10x times faster than the classic reflection using GetField() / GetProperty() and then GetValue / SetValue().

There are also methods that generate .NET types during run time from meta data.
These types implement a IObjectAccessor interface which gives fast access to class members by indexer.

## Installation

The library can be built from source or also is available as a [NuGet package][6e5dff96]:
```
PM> Install-Package FastRTHelpers
```

  [6e5dff96]: https://www.nuget.org/packages/FastRTHelpers/ "FastRTHelpers"

## Usage

### Type-level member accessors

**Generic version**

```
var accessor = TypeAccessor.GetMemberAccessor(x => x.MemberName);
var value = accessor.GetValue(obj);
accessor.SetValue(obj, newValue);
```

**Basic version**
```
var accessor = TypeAccessor.GetMemberAccessor(typeof(T), "MemberName");
var value = accessor.GetValue(obj);
accessor.SetValue(obj, newValue);
```

### Object-level member accessors

*Implemented as set of extension methods*

**Generic version**

```
SomeRealType obj = ...; //SomeRealType has a member (property or field) "MemberName"
var objectAccessor = obj.GetObjectMemberAccessor(x => x.MemberName);
objectAccessor.Value = newValue;
```

**Basic version**

```
object obj = ...;
var objectAccessor = obj.GetObjectMemberAccessor("MemberName");
var value = objectAccessor.GetValue();
objectAccessor.SetValue(newValue);
```

### Type generation from a list of field definitions [string, Type]

**How to generate a new type that implements IObjectAccessor**

```
Dictionary<string, Type> typeDef = new Dictionary<string, Type>
{
	{"StringProp", typeof(String)},
	{"IntProp", typeof(int)},
	{"DateProp", typeof(DateTime)},
	{"DoubleNullableProp", typeof(double?)}
};
Type t = TypeGenerator.MakeType("TestClassA", typeDef);
```

**How to instantiate a new object (type is auto-generated on demand if necessary)**

```
Dictionary<string, object> objDef = new Dictionary<string, object>
{
	{"StringProp", "test data"},
	{"IntProp", 48},
	{"DateProp", DateTime.Now},
	{"DoubleProp", 45.98}
};
var obj = TypeGenerator.MakeObject("NewTypeB", objDef);
obj["StringProp"] = "new data";
```

**How to generate linked types from model definition**

```
var elTypeDef = TypeGenerator.MakeTypeDef("TElement", new Dictionary<string, Type>
{
    {"StringProp", typeof(String)},
});
var pointTypeDef = TypeGenerator.MakeTypeDef("TPoint", new Dictionary<string, Type>
{
    {"X", typeof(double)},
    {"Y", typeof(double)}
});
var objTypeDef = TypeGenerator.MakeTypeDef("TRoot", new Dictionary<string, Type>
{
    {"ListProp", TypeGenerator.MakeListDef(elTypeDef)},
    {"Center", pointTypeDef},
    {"Parent", TypeGenerator.MakeTypeRef("TRoot")},
    {"Nodes", TypeGenerator.MakeListDef(TypeGenerator.MakeTypeRef("TRoot"))}
});

IObjectFactory objFactory = objTypeDef.MakeObjectFactory();
IObjectFactory elFactory = elTypeDef.MakeObjectFactory();
IObjectFactory pointFactory = pointTypeDef.MakeObjectFactory();

var list = elFactory.NewList();
list.Add(elFactory.NewObject());
list.Add(elFactory.NewObject());

var obj = objFactory.NewObjectAccessor();
obj.SetValue("ListProp", list);
foreach (var el in obj.GetValue<IEnumerable<IObjectAccessor>>("ListProp"))
    el[0] = "test";

var st = pointFactory.NewObjectAccessor();
st.SetValue("X", 10.5);
st.SetValue("Y", 48.96);

obj.SetValue("Center", st);

var parent = objFactory.NewObjectAccessor();
obj.SetValue("Parent", parent);

var nodes = objFactory.NewList();
nodes.Add(obj);
parent.SetValue("Nodes", nodes);

```

**Advanced: implementing ITypeDef and IListDef interfaces to generate types**

It is possible to implement special interfaces which describe your model:

* ITypeDef: provides information about type name and properties
* IListDef: definition of generic list for linked objects

In order to reference an already defined type in your model, the member type must be defined with `TypeGenerator.MakeTypeRef("TypeName")`

Then just call TypeGenerator.MakeType() on the root type in the model.

## TODO
1. dynamic objects support
2. build fast delegates for method calls
