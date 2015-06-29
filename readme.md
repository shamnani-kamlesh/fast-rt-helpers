# Fast Runtime Type Helpers

## Description

This library contains set of helper methods that provide fast runtime access to object's properties and fields.
Reading/writing works 5-10x times faster than the classic reflection using GetField() / GetProperty() and then GetValue / SetValue().

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
SomeRealType obj = ...; //has a member (property or field) "MemberName"
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
Type t = TypeGenerator.MakeType(typeDef, "TestClassA");
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
var obj = TypeGenerator.MakeObject(objDef, "NewTypeB");
obj["StringProp"] = "new data";
```

## TODO
1. dynamic objects support
2. build fast delegates for method calls
